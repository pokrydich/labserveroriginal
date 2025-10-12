import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserPreferencesMenuComponent } from './user-preferences-menu.component';

describe('UserPreferencesMenuComponent', () => {
  let component: UserPreferencesMenuComponent;
  let fixture: ComponentFixture<UserPreferencesMenuComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UserPreferencesMenuComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UserPreferencesMenuComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
