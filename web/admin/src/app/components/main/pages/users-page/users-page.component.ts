import { Component, OnDestroy, OnInit } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { IUser } from '../../../../models/entity/user';
import { UserApiService } from '../../../../services/API/user-api.service';
import { RoleBageComponent } from './role-bage/role-bage.component';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { RoleEditModalComponent } from './role-edit-modal/role-edit-modal.component';
import { UpdatableMap, Utils } from '../../../../utils/utils';
import { HubClientService } from '../../../../services/API/hub-client.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-users-page',
  standalone: true,
  imports: [TranslateModule, RoleBageComponent],
  templateUrl: './users-page.component.html',
  styleUrl: './users-page.component.css'
})
export class UsersPageComponent implements OnInit, OnDestroy {
  private users: UpdatableMap<IUser> = new UpdatableMap(u => String(u.id));

  public sortedUsers(): IUser[] {
    return this.users.getElements();
  }

  protected userUpdateSubscription?: Subscription;

  protected modalRef?: BsModalRef;

  constructor(private usersService: UserApiService,
    private hubClient: HubClientService,
    private modalService: BsModalService) {}

  async ngOnInit() {
    this.users.setElems((await this.usersService.getUsers()));

    this.userUpdateSubscription = this.hubClient.userDataUpdates.subscribe(user => {
      this.users.onUpdate(user);
    });
  }

  async ngOnDestroy() {
    this.userUpdateSubscription?.unsubscribe();
  }

  protected openModal(user: IUser) {
    const initialState: ModalOptions = {
      initialState: {
        id: user.id,
        email: user.email,
        roles: Utils.deepCopy(user.roles),
        initialRoles: Utils.deepCopy(user.roles)
      }
    };
    this.modalRef = this.modalService.show(RoleEditModalComponent, initialState);
  }
}
