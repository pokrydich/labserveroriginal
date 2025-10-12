import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { UpdatableMap } from '../../../../../utils/utils';
import { IStudent } from '../../../../../models/entity/student';
import { GroupStudentsService } from '../../../../../services/API/group-students.service';
import { GroupService } from '../../../../../services/API/group.service';
import { IGroup } from '../../../../../models/entity/group';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { AddStudentModalComponent } from './add-student-modal/add-student-modal.component';
import { HubClientService } from '../../../../../services/API/hub-client.service';
import { Subscription } from 'rxjs';
import { GitLabHelperService } from '../../../../../services/git-lab-helper.service';
import { ImportStudentsModalComponent } from './import-students-modal/import-students-modal.component';
import { EditStudentModalComponent } from './edit-student-modal/edit-student-modal.component';
import { PopoverModule } from 'ngx-bootstrap/popover';

@Component({
  selector: 'app-group-students-page',
  standalone: true,
  imports: [TranslateModule, RouterLink, PopoverModule],
  templateUrl: './group-students-page.component.html',
  styleUrl: './group-students-page.component.css'
})
export class GroupStudentsPageComponent implements OnInit, OnDestroy {
  protected groupId: number | null = null;
  protected group: IGroup | null = null;
  protected students: UpdatableMap<IStudent> = new UpdatableMap(s => String(s.id));

  protected studentUpdateSubscription?: Subscription;

  protected addStudentModalRef?: BsModalRef;
  protected editStudentModalRef?: BsModalRef;
  protected importStudentsModalRef?: BsModalRef;

  constructor(private route: ActivatedRoute,
    private groupService: GroupService,
    private studentService: GroupStudentsService,
    private modalService: BsModalService,
    private hubClient: HubClientService) {}

  async ngOnInit() {
    this.route.paramMap.subscribe(async params => {
      this.groupId = +params.get("id")!;
      this.group = await this.groupService.getById(this.groupId);
      this.students.setElems(await this.studentService.getStudents(this.groupId));
    });

    this.studentUpdateSubscription = this.hubClient.studentDataUpdates.subscribe(student => {
      this.students.onUpdate(student);
    });
  }

  async ngOnDestroy() {
    this.studentUpdateSubscription?.unsubscribe();
  }

  protected openAddStudentModal() {
    if (this.group) {
      const initialState: ModalOptions = {
        initialState: {
          groupId: this.groupId,
          groupName: this.group.name
        }
      };
      this.addStudentModalRef = this.modalService.show(AddStudentModalComponent, initialState);
    }
  }

  protected openEditStudentModal(student: IStudent) {
    if (this.group) {
      const initialState: ModalOptions = {
        initialState: {
          groupId: this.groupId,
          student: student
        }
      };
      this.editStudentModalRef = this.modalService.show(EditStudentModalComponent, initialState);
    }
  }

  protected openImportStudentsModal() {
    if (this.group) {
      const initialState: ModalOptions = {
        initialState: {
          groupId: this.groupId,
          groupName: this.group.name
        }
      };
      this.importStudentsModalRef = this.modalService.show(ImportStudentsModalComponent, initialState);
    }
  }

  protected async gitlabSync() {
    if (this.groupId) {
      await this.studentService.gitlabSync(this.groupId);
    }
  }

  protected makeGitLabUserLink(student: IStudent) {
    if (student.username) {
      return GitLabHelperService.makeUserLink(student.username);
    }
    return "";
  }

  protected async sendEmails() {
    if (this.groupId) {
      await this.studentService.sendCredentialsAll(this.groupId);
    }
  }

  protected async sendEmail(student: IStudent) {
    if (this.groupId && student.id) {
      await this.studentService.sendCredentialStudent(this.groupId, student.id);
    }
  }
}
