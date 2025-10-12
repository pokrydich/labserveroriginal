import { NgClass } from '@angular/common';
import { Component, Input } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-role-bage',
  standalone: true,
  imports: [NgClass, TranslateModule],
  templateUrl: './role-bage.component.html',
  styleUrl: './role-bage.component.css'
})
export class RoleBageComponent {
  @Input() role?: string;

  private readonly cRoleColorClassMap: { [key: string]: string } = {
    "Administrator": "text-bg-primary",
    "Professor": "text-bg-success",
    "Assistant" : "text-bg-info"
  }

  roleKey(): string {
    if (this.role && this.role in this.cRoleColorClassMap) {
      return `usersRoleBadge${this.role}`;
    } else {
      return "usersRoleBadgeUnkownRole";
    }
  }

  getColorClass() {
    if (this.role && this.role in this.cRoleColorClassMap) {
      return this.cRoleColorClassMap[this.role];
    } else {
      return "badge-danger";
    }
  }
}
