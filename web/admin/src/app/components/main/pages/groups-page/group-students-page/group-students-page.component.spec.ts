import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GroupStudentsPageComponent } from './group-students-page.component';

describe('GroupStudentsPageComponent', () => {
  let component: GroupStudentsPageComponent;
  let fixture: ComponentFixture<GroupStudentsPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GroupStudentsPageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GroupStudentsPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
