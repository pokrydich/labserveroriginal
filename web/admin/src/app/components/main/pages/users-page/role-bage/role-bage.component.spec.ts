import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RoleBageComponent } from './role-bage.component';

describe('RoleBageComponent', () => {
  let component: RoleBageComponent;
  let fixture: ComponentFixture<RoleBageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RoleBageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RoleBageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
