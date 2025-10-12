import { Component, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { TranslateModule } from '@ngx-translate/core';
import { UserApiService } from '../../../../../services/API/user-api.service';
import { RoleBageComponent } from '../role-bage/role-bage.component';
import { NgClass } from '@angular/common';

@Component({
  selector: 'app-role-edit-modal',
  standalone: true,
  imports: [TranslateModule, NgClass, RoleBageComponent],
  templateUrl: './role-edit-modal.component.html',
  styleUrl: './role-edit-modal.component.css'
})
export class RoleEditModalComponent implements OnInit {
  email?: string;
  id?: number;
  roles?: string[];
  initialRoles?: string[];

  constructor(public bsModalRef: BsModalRef,
    protected userService: UserApiService) {}

  async ngOnInit() {
  }

  protected rolesChanged(): boolean {
    if (this.roles && this.initialRoles) {
      if (this.initialRoles.length !== this.roles.length) {
        return true;
      }
      return this.roles.filter(r => this.initialRoles!.includes(r)).length !== this.roles.length;
    } else {
      return false;
    }
  }

  protected toggleRole(role: string) {
    if (this.roles && this.roles.includes(role)) {
      this.roles = this.roles.filter(r => r !== role);
    } else if (this.roles) {
      this.roles.push(role);
    }
    console.log("current roles: " + this.roles);
  }

  protected async apply() {
    if (this.id && this.roles && this.initialRoles) {
      const addedRoles = this.roles.filter(r => !this.initialRoles!.includes(r));
      const removedRoles = this.initialRoles.filter(r => !this.roles!.includes(r));

      console.log("added: " + addedRoles);
      console.log("removed: " + removedRoles);
      for (let addedRole of addedRoles) {
        await this.userService.addUserRole(this.id, this.userService.cSupportedRoles[addedRole]);
      }
      for (let removedRole of removedRoles) {
        await this.userService.deleteUserRole(this.id, this.userService.cSupportedRoles[removedRole]);
      }
    }
    this.bsModalRef.hide();
  }
}
