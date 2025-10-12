import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ImportStudentsModalComponent } from './import-students-modal.component';

describe('ImportStudentsModalComponent', () => {
  let component: ImportStudentsModalComponent;
  let fixture: ComponentFixture<ImportStudentsModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ImportStudentsModalComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ImportStudentsModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
