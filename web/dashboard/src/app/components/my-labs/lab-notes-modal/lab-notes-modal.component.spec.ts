import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LabNotesModalComponent } from './lab-notes-modal.component';

describe('LabNotesModalComponent', () => {
  let component: LabNotesModalComponent;
  let fixture: ComponentFixture<LabNotesModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LabNotesModalComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LabNotesModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
