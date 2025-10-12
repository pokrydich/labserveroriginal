import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateCourseLabModalComponent } from './create-course-lab-modal.component';

describe('CreateCourseLabModalComponent', () => {
  let component: CreateCourseLabModalComponent;
  let fixture: ComponentFixture<CreateCourseLabModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateCourseLabModalComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreateCourseLabModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
