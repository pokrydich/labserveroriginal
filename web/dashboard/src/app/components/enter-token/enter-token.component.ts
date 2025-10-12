import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-enter-token',
  standalone: true,
  imports: [TranslateModule, ReactiveFormsModule],
  templateUrl: './enter-token.component.html',
  styleUrl: './enter-token.component.css'
})
export class EnterTokenComponent {
  protected tokenForm: FormGroup;

  constructor(private fb: FormBuilder, private router: Router) {
    this.tokenForm = fb.group({
      'token': ['']
    });
  }

  protected submitToken() {
    const val = this.tokenForm.value;
    if (val.token) {
      this.router.navigate([`/mylabs/${val.token}`])
    }
  }
}
