import { Component, OnInit } from '@angular/core';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { AuthService } from '../../../../services/auth.service';
import { IUser } from '../../../../models/entity/user';
import { TranslateModule } from '@ngx-translate/core';
import { Router, RouterLink } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-account-menu',
  standalone: true,
  imports: [BsDropdownModule, TranslateModule, RouterLink],
  templateUrl: './account-menu.component.html',
  styleUrl: './account-menu.component.css'
})
export class AccountMenuComponent implements OnInit {
  protected user: IUser | null = null;

  constructor(private router: Router,
    protected authService: AuthService) {}
  
  async ngOnInit() {
    try {
      this.user = await this.authService.getLoggedInUser();
    } catch (error)  {
      if (error instanceof HttpErrorResponse) {
        if (error.status === 401) {
          this.router.navigate(["/login"]);
        } else {
          console.log(`[!] unexpected auth heartbeat response error: ${error.message}`);
        }
      } else {
        throw error;
      }
    }
  }
}
