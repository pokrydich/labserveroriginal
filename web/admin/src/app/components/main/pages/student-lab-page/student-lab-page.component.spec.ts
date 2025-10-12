import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StudentLabPageComponent } from './student-lab-page.component';

describe('StudentLabPageComponent', () => {
  let component: StudentLabPageComponent;
  let fixture: ComponentFixture<StudentLabPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StudentLabPageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StudentLabPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
