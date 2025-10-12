import { Component } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { TranslateModule } from '@ngx-translate/core';
import { PopupNotificationsService } from '../../../../../../popup-notifications.service';
import { GroupStudentsService } from '../../../../../../services/API/group-students.service';

@Component({
  selector: 'app-import-students-modal',
  standalone: true,
  imports: [TranslateModule],
  templateUrl: './import-students-modal.component.html',
  styleUrl: './import-students-modal.component.css'
})
export class ImportStudentsModalComponent {
  protected groupId?: number;
  protected groupName?: string;

  protected fileText?: string;

  constructor(public bsModalRef: BsModalRef,
    private notificationService: PopupNotificationsService,
    private studentService: GroupStudentsService) {
  }

  protected async onFileChanged(event: any) {
    const file: File = event.target.files[0];

    if (file) {
      console.log(file.name);
      this.fileText = await file.text();
    } else {
      this.fileText = undefined;
    }
  }

  protected async importStudents() {
    if (this.groupId && this.fileText) {
      await this.studentService.importFromCsv(this.groupId, this.fileText);
      this.bsModalRef.hide();
    } else {
      this.notificationService.warning("groupStudentsImportModalErrorSelectOneFile");
    }
  }
}
