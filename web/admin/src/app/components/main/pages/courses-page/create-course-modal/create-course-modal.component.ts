import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { CoursesService } from '../../../../../services/API/courses.service';
import { PopupNotificationsService } from '../../../../../popup-notifications.service';

@Component({
  selector: 'app-create-course-modal',
  standalone: true,
  imports: [ReactiveFormsModule, TranslateModule],
  templateUrl: './create-course-modal.component.html',
  styleUrl: './create-course-modal.component.css'
})
export class CreateCourseModalComponent {
  protected createCourseForm: FormGroup;

  constructor(public bsModalRef: BsModalRef, 
    private fb: FormBuilder,
    private coursesService: CoursesService,
    private notificationService: PopupNotificationsService) {

    this.createCourseForm = fb.group({
      'coursename': ['']
    });
  }

  protected async createCourse() {
    const val = this.createCourseForm.value;
    if (val.coursename) {
      await this.coursesService.create({
        id: undefined,
        name: val.coursename,
        gitLabName: undefined,
        courseLabs: undefined
      });
      this.bsModalRef.hide();
    } else {
      this.notificationService.warning("courseCreateErrorProvideData");
    }
  }
}
