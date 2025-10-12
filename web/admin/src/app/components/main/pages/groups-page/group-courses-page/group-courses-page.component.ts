import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { GroupCoursesService } from '../../../../../services/API/group-courses.service';
import { IGroup } from '../../../../../models/entity/group';
import { GroupService } from '../../../../../services/API/group.service';
import { UpdatableMap } from '../../../../../utils/utils';
import { IGroupCourse } from '../../../../../models/entity/group-course';
import { GitLabHelperService } from '../../../../../services/git-lab-helper.service';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { AssignCourseModalComponent } from './assign-course-modal/assign-course-modal.component';
import { ICourse } from '../../../../../models/entity/course';
import { CoursesService } from '../../../../../services/API/courses.service';
import { HubClientService } from '../../../../../services/API/hub-client.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-group-courses-page',
  standalone: true,
  imports: [TranslateModule, RouterLink],
  templateUrl: './group-courses-page.component.html',
  styleUrl: './group-courses-page.component.css'
})
export class GroupCoursesPageComponent implements OnInit, OnDestroy {
  protected groupId?: number;
  protected group?: IGroup;

  protected courses: UpdatableMap<ICourse> = new UpdatableMap<ICourse>(c => String(c.id));
  protected groupCourses: UpdatableMap<IGroupCourse> = new UpdatableMap<IGroupCourse>(gc => String(gc.id));

  protected assignCourseModalRef?: BsModalRef;

  protected courseUpdateSubscription?: Subscription;
  protected groupCourseUpdateSubscription?: Subscription;
  
  constructor(private route: ActivatedRoute,
    private groupCoursesService: GroupCoursesService,
    private groupService: GroupService,
    private coursesService: CoursesService,
    private modalService: BsModalService,
    private hubClient: HubClientService) {}

  async ngOnInit() {
    this.route.paramMap.subscribe(async params => {
      const paramId = params.get("id");
      if (paramId) {
        this.groupId = +paramId;
        this.group = await this.groupService.getById(this.groupId);

        this.groupCourses.setElems(await this.groupCoursesService.getAll(this.groupId));
      }
    });

    this.courses.setElems(await this.coursesService.getAll());

    this.courseUpdateSubscription = this.hubClient.courseDataUpdates.subscribe(course => {
      this.courses.onUpdate(course);
    });
    this.groupCourseUpdateSubscription = this.hubClient.groupCourseDataUpdates.subscribe(groupCourse => {
      this.groupCourses.onUpdate(groupCourse);
    });
  }

  async ngOnDestroy() {
    this.courseUpdateSubscription?.unsubscribe();
  }

  protected getCourseName(courseId: number): string {
    return this.courses.getElement(String(courseId))?.name ?? "";
  }

  protected makeGitLabGroupLink(groupCourse: IGroupCourse): string {
    const course = this.courses.getElement(String(groupCourse.courseId));

    if (course && this.group && this.group.id) {
      return GitLabHelperService.makeGroupLink(`${this.group.gitLabName}/${course.gitLabName}`);
    }
    return "";
  }

  protected openAssignCourseModal() {
    if (this.group) {
      const initialState: ModalOptions = {
        initialState: {
          groupId: this.groupId,
          groupName: this.group.name,
          courses: this.courses
        }
      };
      this.assignCourseModalRef = this.modalService.show(AssignCourseModalComponent, initialState);
    }
    
  }

  protected async gitlabSync() {
    if (this.group && this.group.id) {
      this.groupCoursesService.gilabSyncCourses(this.group.id);
    }
  }
}
