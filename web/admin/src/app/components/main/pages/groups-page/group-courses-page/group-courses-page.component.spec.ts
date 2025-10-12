import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GroupCoursesPageComponent } from './group-courses-page.component';

describe('GroupCoursesPageComponent', () => {
  let component: GroupCoursesPageComponent;
  let fixture: ComponentFixture<GroupCoursesPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GroupCoursesPageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GroupCoursesPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
