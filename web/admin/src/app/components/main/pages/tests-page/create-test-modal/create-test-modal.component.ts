import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { TestService } from '../../../../../services/API/test.service';
import { PopupNotificationsService } from '../../../../../popup-notifications.service';

@Component({
  selector: 'app-create-test-modal',
  standalone: true,
  imports: [TranslateModule, ReactiveFormsModule],
  templateUrl: './create-test-modal.component.html',
  styleUrl: './create-test-modal.component.css'
})
export class CreateTestModalComponent {
  protected createTestForm: FormGroup;

  constructor(public bsModalRef: BsModalRef, 
    private fb: FormBuilder,
    private testService: TestService,
    private notificationService: PopupNotificationsService) {

    this.createTestForm = fb.group({
      'testname': [''],
      'serverurl': ['']
    });
  }

  protected async createCourse() {
    const val = this.createTestForm.value;
    if (val.testname && val.serverurl) {
      await this.testService.create({
        id: undefined,
        name: val.testname,
        testServerUrl: val.serverurl
      });
      this.bsModalRef.hide();
    } else {
      this.notificationService.warning("courseCreateErrorProvideData");
    }
  }
}
