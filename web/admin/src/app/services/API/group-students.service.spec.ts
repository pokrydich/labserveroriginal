import { TestBed } from '@angular/core/testing';

import { GroupStudentsService } from './group-students.service';

describe('GroupStudentsService', () => {
  let service: GroupStudentsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(GroupStudentsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
