import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AssignCourseModalComponent } from './assign-course-modal.component';

describe('AssignCourseModalComponent', () => {
  let component: AssignCourseModalComponent;
  let fixture: ComponentFixture<AssignCourseModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AssignCourseModalComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AssignCourseModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
