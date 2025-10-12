import { Component } from '@angular/core';
import { UpdatableMap } from '../../../../../../utils/utils';
import { TranslateModule } from '@ngx-translate/core';
import { ICourse } from '../../../../../../models/entity/course';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { PopupNotificationsService } from '../../../../../../popup-notifications.service';
import { GroupCoursesService } from '../../../../../../services/API/group-courses.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-assign-course-modal',
  standalone: true,
  imports: [TranslateModule, FormsModule],
  templateUrl: './assign-course-modal.component.html',
  styleUrl: './assign-course-modal.component.css'
})
export class AssignCourseModalComponent {
  protected groupId?: number;
  protected groupName?: string;
  protected courses?: UpdatableMap<ICourse>;

  protected courseSelector: string = "";
  protected selectedCoure?: ICourse;

  constructor(public bsModalRef: BsModalRef,
    private notificationService: PopupNotificationsService,
    private groupCoursesService: GroupCoursesService) {
  }

  public filterCourses(): ICourse[] {
    if (!this.courses) {
      return [];
    }
    
    if (this.courseSelector.length > 0) {
      return this.courses.getElements().filter(c => c.name.toLocaleLowerCase().includes(this.courseSelector)).slice(0, 5);
    } else {
      return this.courses?.getElements().slice(0, 5);
    }
  }

  public selectCourse(course: ICourse) {
    this.selectedCoure = course;
  }

  public async assignCourse() {
    if (this.groupId && this.selectedCoure && this.selectedCoure.id) {
      this.groupCoursesService.assignCourse(this.groupId, this.selectedCoure.id);
      this.bsModalRef.hide();
    } else {
      this.notificationService.warning("groupCoursesAsignModalErrorCourseNotSelected");
    }
  }
}
