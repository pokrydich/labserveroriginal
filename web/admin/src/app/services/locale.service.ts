import { Injectable } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';

@Injectable({
  providedIn: 'root'
})
export class LocaleService {
  private cLocaleStorageKey: string = "locale";
  protected cDefaultLocale: string = "ru";

  public readonly cSupportedLocales: { [key: string]: string } = {
    "ru": "Russian",
    "en": "English"
  }

  constructor(private translateService: TranslateService) { }

  getLocale() {
    let locale = localStorage.getItem(this.cLocaleStorageKey);
    if (locale === null) {
      locale = this.cDefaultLocale;
      this.saveLocale(locale);
    }
    return locale;
  }

  saveLocale(locale: string) {
    localStorage.setItem(this.cLocaleStorageKey, locale);
  }

  applyLocale() {
    const locale = this.getLocale();
    this.translateService.setDefaultLang(locale);
    this.translateService.use(locale)
  }

  setLocale(locale: string) {
    this.saveLocale(locale);
    this.applyLocale();
  }
}
