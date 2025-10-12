import { Component, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { LocaleService } from './services/locale.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  title = 'Lab Server';

  constructor(private localeService: LocaleService) {}

  async ngOnInit() {
    this.localeService.applyLocale();
  }
}
