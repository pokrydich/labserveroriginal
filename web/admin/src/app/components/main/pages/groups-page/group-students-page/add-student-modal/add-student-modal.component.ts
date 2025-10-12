import { Component } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { TranslateModule } from '@ngx-translate/core';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { PopupNotificationsService } from '../../../../../../popup-notifications.service';
import { GroupStudentsService } from '../../../../../../services/API/group-students.service';

@Component({
  selector: 'app-add-student-modal',
  standalone: true,
  imports: [TranslateModule, ReactiveFormsModule],
  templateUrl: './add-student-modal.component.html',
  styleUrl: './add-student-modal.component.css'
})
export class AddStudentModalComponent {
  protected addStudentForm: FormGroup;

  protected groupId?: number;
  protected groupName?: string;

  constructor(public bsModalRef: BsModalRef,
    private fb: FormBuilder,
    private notificationService: PopupNotificationsService,
    private studentService: GroupStudentsService) {

    this.addStudentForm = this.fb.group({
      "studentname": [''],
      "studentemail" : ['']
    })
  }

  public async addStudent() {
    const val = this.addStudentForm.value;
    if (this.groupId, val.studentname && val.studentemail) {
      await this.studentService.addStudnet({
        name: val.studentname,
        email: val.studentemail,
        groupId: this.groupId!,
        id: undefined,
        username: undefined,
        gitLabUserId: undefined,
        initialPassword: undefined,

        group: undefined
      });
      this.bsModalRef.hide();
    } else {
      this.notificationService.warning("groupStudentModalErrorEnterData");
    }
  }
}
