import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { IGroup } from '../../../../../../models/entity/group';
import { IGroupCourse } from '../../../../../../models/entity/group-course';
import { GroupCoursesService } from '../../../../../../services/API/group-courses.service';
import { ICourse } from '../../../../../../models/entity/course';
import { cDataConversionOption } from '../../../../../../services/API/api-base.service';
import { UpdatableMap } from '../../../../../../utils/utils';
import { IGroupCourseLab } from '../../../../../../models/entity/group-course-lab';
import { IStudentLab } from '../../../../../../models/entity/student-lab';
import { HubClientService } from '../../../../../../services/API/hub-client.service';
import { Subscription } from 'rxjs';
import { ICourseLab } from '../../../../../../models/entity/course-lab';
import { IStudent } from '../../../../../../models/entity/student';
import { GroupStudentsService } from '../../../../../../services/API/group-students.service';
import { StudentLabPreviewComponent } from './student-lab-preview/student-lab-preview.component';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-group-course-details-page',
  standalone: true,
  imports: [TranslateModule, RouterLink, StudentLabPreviewComponent, FormsModule],
  templateUrl: './group-course-details-page.component.html',
  styleUrl: './group-course-details-page.component.css'
})
export class GroupCourseDetailsPageComponent implements OnInit, OnDestroy {
  protected groupId?: number;
  protected group?: IGroup;

  protected courseId?: number;
  protected course?: ICourse;

  protected groupCourse?: IGroupCourse;

  protected groupCourseLabs: UpdatableMap<IGroupCourseLab> = new UpdatableMap<IGroupCourseLab>(gc => String(gc.courseLabId));
  protected studentLabs: UpdatableMap<IStudentLab> = new UpdatableMap<IStudentLab>(sl => `${sl.groupCourseLabId},${sl.studentId}`);

  protected groupCourseLabsSubscription?: Subscription;
  protected studentLabsSubscription?: Subscription;

  constructor(private route: ActivatedRoute,
    private groupCourseService: GroupCoursesService,
    private groupStudentService: GroupStudentsService,
    private hubClient: HubClientService) {}

  async ngOnInit() {
    this.route.paramMap.subscribe(async params => {
      const groupId = params.get("groupId");
      const courseId = params.get("courseId");

      if (groupId && courseId) {
        this.groupId = +groupId;
        this.courseId = +courseId;

        this.groupCourse = await this.groupCourseService.getById(this.groupId, this.courseId, cDataConversionOption.Full);
        
        this.group = this.groupCourse.group;
        this.course = this.groupCourse.course;

        this.groupCourseLabs.setElems(this.groupCourse.groupLabs ?? []);

        this.studentLabs.setElems([]);
        for (let course of this.groupCourseLabs.getElements()) {
          this.studentLabs.addElems(course.studentLabs ?? []);
        }
      }
    });

    this.groupCourseLabsSubscription = this.hubClient.groupCourseLabDataUpdates.subscribe(groupCourseLab => {
      this.groupCourseLabs.onUpdate(groupCourseLab);
    });
    this.studentLabsSubscription = this.hubClient.studentLabDataUpdates.subscribe(studentLab => {
      this.studentLabs.onUpdate(studentLab);
    })
  }

  async ngOnDestroy() {
    this.groupCourseLabsSubscription?.unsubscribe();
    this.studentLabsSubscription?.unsubscribe();
  }

  protected studentSelector: string = "";

  protected getSelectedStudents(): IStudent[] {
    if (this.groupCourse && this.groupCourse.group && this.groupCourse.group.students) {
      if (this.studentSelector.length > 0) {
        return this.groupCourse.group.students.filter(s => s.name.toLocaleLowerCase().includes(this.studentSelector.toLocaleLowerCase()));
      } else {
        return this.groupCourse.group.students;
      }
    }
    return [];
  }

  protected getGroupCourseLab(courseId: number): IGroupCourseLab | undefined {
    return this.groupCourseLabs.getElement(String(courseId));
  }

  protected getStudentLab(groupCourseLabId: number, studentId: number): IStudentLab | undefined {
    return this.studentLabs.getElement(`${groupCourseLabId},${studentId}`);
  }

  public async startLab(courseLab: ICourseLab) {
    if (this.group && this.course) {
      await this.groupCourseService.startCourseLab(this.group.id!, this.course.id!, courseLab.id!)
    }
  }

  public async openStudnetLab(student: IStudent, groupCourseLab: IGroupCourseLab) {
    if (this.group && this.group.id && student.id && groupCourseLab.id) {
      await this.groupStudentService.openStudentLab(this.group?.id, student.id, groupCourseLab.id)
    }
  }

  public async gitlabSync() {
    if (this.group && this.group.id && this.course && this.course.id) {
      await this.groupCourseService.gitlabSyncGroupCourse(this.group.id, this.course.id);
    }
  }
}
