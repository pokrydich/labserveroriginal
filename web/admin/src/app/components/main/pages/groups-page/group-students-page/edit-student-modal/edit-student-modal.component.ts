import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { PopupNotificationsService } from '../../../../../../popup-notifications.service';
import { GroupStudentsService } from '../../../../../../services/API/group-students.service';
import { IStudent } from '../../../../../../models/entity/student';

@Component({
  selector: 'app-edit-student-modal',
  standalone: true,
  imports: [TranslateModule, ReactiveFormsModule],
  templateUrl: './edit-student-modal.component.html',
  styleUrl: './edit-student-modal.component.css'
})
export class EditStudentModalComponent implements OnInit {
  protected editStudentForm: FormGroup;

  protected groupId?: number;
  protected student?: IStudent;

  constructor(public bsModalRef: BsModalRef,
    private fb: FormBuilder,
    private notificationService: PopupNotificationsService,
    private studentService: GroupStudentsService) {

    this.editStudentForm = this.fb.group({
      "studentname": [""],
      "studentemail" : [""],
      "username": [""]
    })
  }

  async ngOnInit() {
    this.editStudentForm = this.fb.group({
      "studentname": [this.student?.name ?? ""],
      "studentemail" : [this.student?.email ?? ""],
      "username": [this.student?.username ?? ""]
    })
  }

  protected async editStudent() {
    if (this.student) {
      const val = this.editStudentForm.value;
      if (val.studentname && val.studentemail && val.username) {
        await this.studentService.updateStudent({
          id: this.student.id,
          name: val.studentname,
          email: val.studentemail,
          username: val.username,
          groupId: this.student.groupId,
          gitLabUserId: undefined,
          initialPassword: undefined,

          group: undefined
        });
        this.bsModalRef.hide();
      } else {
        this.notificationService.warning("groupStudentsEditModalErrorEnterData");
      }
    }
  }
}
