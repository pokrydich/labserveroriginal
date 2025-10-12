import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateCourseModalComponent } from './create-course-modal.component';

describe('CreateCourseModalComponent', () => {
  let component: CreateCourseModalComponent;
  let fixture: ComponentFixture<CreateCourseModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateCourseModalComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreateCourseModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
