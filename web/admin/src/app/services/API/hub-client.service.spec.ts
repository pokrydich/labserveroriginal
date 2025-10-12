import { TestBed } from '@angular/core/testing';

import { HubClientService } from './hub-client.service';

describe('HubClientService', () => {
  let service: HubClientService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(HubClientService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
