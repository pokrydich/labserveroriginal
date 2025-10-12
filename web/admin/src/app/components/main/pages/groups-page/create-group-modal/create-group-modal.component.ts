import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { GroupService } from '../../../../../services/API/group.service';
import { PopupNotificationsService } from '../../../../../popup-notifications.service';

@Component({
  selector: 'app-create-group-modal',
  standalone: true,
  imports: [ReactiveFormsModule, TranslateModule],
  templateUrl: './create-group-modal.component.html',
  styleUrl: './create-group-modal.component.css'
})
export class CreateGroupModalComponent {
  protected createGroupForm: FormGroup;

  constructor(public bsModalRef: BsModalRef, 
    private fb: FormBuilder,
    private groupsService: GroupService,
    private notificationService: PopupNotificationsService) {

    this.createGroupForm = fb.group({
      'groupname': ['']
    });
  }

  protected async createGroup() {
    const val = this.createGroupForm.value;
    if (val.groupname) {
      await this.groupsService.create({
        id: undefined,
        name: val.groupname,
        gitLabName: undefined,
        gitLabGroupId: undefined,
        students: undefined
      });
      this.bsModalRef.hide();
    } else {
      this.notificationService.warning("groupsCreateErrorProvideData");
    }
  }
}
