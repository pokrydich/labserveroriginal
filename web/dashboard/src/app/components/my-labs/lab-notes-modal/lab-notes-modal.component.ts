import { Component } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-lab-notes-modal',
  standalone: true,
  imports: [TranslateModule],
  templateUrl: './lab-notes-modal.component.html',
  styleUrl: './lab-notes-modal.component.css'
})
export class LabNotesModalComponent {
  notes?: string;

  constructor(public bsModalRef: BsModalRef) {}
}
