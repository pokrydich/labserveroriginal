import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditTestModalComponent } from './edit-test-modal.component';

describe('EditTestModalComponent', () => {
  let component: EditTestModalComponent;
  let fixture: ComponentFixture<EditTestModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EditTestModalComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EditTestModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
