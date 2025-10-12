import { TestBed } from '@angular/core/testing';

import { PopupNotificationsService } from './popup-notifications.service';

describe('PopupNotificationsService', () => {
  let service: PopupNotificationsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PopupNotificationsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
