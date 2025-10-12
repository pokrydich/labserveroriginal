import { Component, OnInit } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { LocaleService } from '../../../services/locale.service';
import { NgClass, NgFor } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-user-preferences-menu',
  standalone: true,
  imports: [TranslateModule, NgClass, FormsModule, NgFor],
  templateUrl: './user-preferences-menu.component.html',
  styleUrl: './user-preferences-menu.component.css'
})
export class UserPreferencesMenuComponent implements OnInit {
  protected selectedLocale: string = "ru";
  protected supportedLocales: string[] = [];

  constructor(protected localeServie: LocaleService) {}

  async ngOnInit() {
    this.selectedLocale = this.localeServie.getLocale();
    this.supportedLocales = this.getLocalesList();
  }

  protected getLocalesList(): string[] {
    return Object.keys(this.localeServie.cSupportedLocales);
  }

  protected checkSelected(locale: string): boolean {
    return locale === this.localeServie.getLocale();
  }
}
