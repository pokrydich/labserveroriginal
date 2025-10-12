import { Component, OnInit } from '@angular/core';
import { GroupService } from '../../services/API/group.service';
import { CollapseModule } from 'ngx-bootstrap/collapse';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { AccountMenuComponent } from './sidebar/account-menu/account-menu.component';
import { RouterLink, RouterOutlet } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { AuthorizedComponent } from '../framework/authorized/authorized.component';

@Component({
  selector: 'app-main',
  standalone: true,
  imports: [CollapseModule, BsDropdownModule, RouterOutlet, RouterLink, TranslateModule, AccountMenuComponent, AuthorizedComponent],
  templateUrl: './main.component.html',
  styleUrl: './main.component.css'
})
export class MainComponent implements OnInit {
  async ngOnInit() {
  }
}
