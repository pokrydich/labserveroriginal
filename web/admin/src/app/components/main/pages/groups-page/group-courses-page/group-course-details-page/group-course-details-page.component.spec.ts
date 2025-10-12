import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GroupCourseDetailsPageComponent } from './group-course-details-page.component';

describe('GroupCourseDetailsPageComponent', () => {
  let component: GroupCourseDetailsPageComponent;
  let fixture: ComponentFixture<GroupCourseDetailsPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GroupCourseDetailsPageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GroupCourseDetailsPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
