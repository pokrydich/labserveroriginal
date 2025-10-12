import { Component, OnDestroy, OnInit } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { GroupService } from '../../../../services/API/group.service';
import { UpdatableMap } from '../../../../utils/utils';
import { IGroup } from '../../../../models/entity/group';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { CreateGroupModalComponent } from './create-group-modal/create-group-modal.component';
import { HubClientService } from '../../../../services/API/hub-client.service';
import { Subscription } from 'rxjs';
import { GitLabHelperService } from '../../../../services/git-lab-helper.service';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-groups-page',
  standalone: true,
  imports: [TranslateModule, RouterLink],
  templateUrl: './groups-page.component.html',
  styleUrl: './groups-page.component.css'
})
export class GroupsPageComponent implements OnInit, OnDestroy {
  protected groups: UpdatableMap<IGroup> = new UpdatableMap(g => String(g.id));

  protected createGroupModalRef?: BsModalRef;

  protected groupUpdateSubscription?: Subscription;

  constructor(private groupService: GroupService,
    private hubClient: HubClientService,
    private modalService: BsModalService) {}

  async ngOnInit() {
    this.groups.setElems(await this.groupService.getAll());

    this.groupUpdateSubscription = this.hubClient.groupDataUpdates.subscribe(group => {
      this.groups.onUpdate(group);
    });
  }

  async ngOnDestroy() {
    this.groupUpdateSubscription?.unsubscribe();
  }

  protected openCreateGroupModal() {
    this.createGroupModalRef = this.modalService.show(CreateGroupModalComponent);
  }

  protected async gitlabSync() {
    await this.groupService.gitlabSync();
  }

  protected makeGroupLink(group: IGroup): string {
    if (group.gitLabName) {
      return GitLabHelperService.makeGroupLink(group.gitLabName);
    }
    return "";
  }
}
