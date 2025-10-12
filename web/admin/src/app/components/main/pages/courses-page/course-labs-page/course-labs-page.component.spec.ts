import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CourseLabsPageComponent } from './course-labs-page.component';

describe('CourseLabsPageComponent', () => {
  let component: CourseLabsPageComponent;
  let fixture: ComponentFixture<CourseLabsPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CourseLabsPageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CourseLabsPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
