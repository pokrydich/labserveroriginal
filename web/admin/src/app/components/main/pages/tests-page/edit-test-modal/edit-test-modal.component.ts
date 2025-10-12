import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { TranslateModule } from '@ngx-translate/core';
import { TestService } from '../../../../../services/API/test.service';
import { PopupNotificationsService } from '../../../../../popup-notifications.service';
import { ITest } from '../../../../../models/entity/test';
import { NgClass } from '@angular/common';

@Component({
  selector: 'app-edit-test-modal',
  standalone: true,
  imports: [NgClass, ReactiveFormsModule, TranslateModule],
  templateUrl: './edit-test-modal.component.html',
  styleUrl: './edit-test-modal.component.css'
})
export class EditTestModalComponent implements OnInit {
  test?: ITest;

  protected editTestForm: FormGroup;

  constructor(public bsModalRef: BsModalRef, 
    private fb: FormBuilder,
    private testService: TestService,
    private notificationService: PopupNotificationsService) {

    this.editTestForm = fb.group({
      'testname': [''],
      'serverurl': ['']
    });
  }

  async ngOnInit() {
    if (this.test) {
      this.editTestForm = this.fb.group({
        'testname': [this.test.name],
        'serverurl': [this.test.testServerUrl]
      });
    }
  }

  protected saveButtonActive() {
    const val = this.editTestForm.value;
    return this.test && val.testname 
                      && val.serverurl && this.test?.name != val.testname 
                      && this.test?.testServerUrl !== val.serverurl;
  }

  protected async saveChanges() {
    if (this.test) {
      const val = this.editTestForm.value;
      if (val.testname && val.serverurl) {
        await this.testService.update(this.test.id!, {
          id: this.test.id,
          name: val.testname,
          testServerUrl: val.serverurl
        });
        this.bsModalRef.hide();
      } else {
        this.notificationService.warning("courseCreateErrorProvideData");
      }
    }
  }
}
