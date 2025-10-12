import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StudentLabPreviewComponent } from './student-lab-preview.component';

describe('StudentLabPreviewComponent', () => {
  let component: StudentLabPreviewComponent;
  let fixture: ComponentFixture<StudentLabPreviewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StudentLabPreviewComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StudentLabPreviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
