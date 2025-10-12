import { Component, Input } from '@angular/core';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: '[authorized]',
  standalone: true,
  imports: [],
  templateUrl: './authorized.component.html',
  styleUrl: './authorized.component.css'
})
export class AuthorizedComponent {
  /**
   * authorized roles (render ng-content for users with this role)
   */
  @Input() roles!: string[];

  constructor(private authService: AuthService) {}

  protected isAuthorized(): boolean {
    return this.authService.getUserRoles().filter(r => this.roles.includes(r)).length > 0;
  }
}
