import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { CourseLabsService } from '../../../../../../services/API/course-labs.service';
import { PopupNotificationsService } from '../../../../../../popup-notifications.service';

@Component({
  selector: 'app-create-course-lab-modal',
  standalone: true,
  imports: [TranslateModule, ReactiveFormsModule],
  templateUrl: './create-course-lab-modal.component.html',
  styleUrl: './create-course-lab-modal.component.css'
})
export class CreateCourseLabModalComponent {
  protected createCourseLabForm: FormGroup;

  protected courseId?: number;

  constructor(public bsModalRef: BsModalRef, 
    private fb: FormBuilder,
    private labsService: CourseLabsService,
    private notificationService: PopupNotificationsService) {

    this.createCourseLabForm = fb.group({
      'labname': ['']
    });
  }

  protected async createCourseLab() {
    const val = this.createCourseLabForm.value;
    if (this.courseId) {
      if (val.labname) {
        await this.labsService.createLab(this.courseId, {
          id: undefined,
          name: val.labname,
          courseId: undefined,
          gitLabName: undefined,

          course: undefined
        });
        this.bsModalRef.hide();
      } else {
        this.notificationService.warning("courseLabsCreateModalErrorEnterData");
      }
    }
  }
}
