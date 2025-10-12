import { TestBed } from '@angular/core/testing';

import { CourseLabsService } from './course-labs.service';

describe('CourseLabsService', () => {
  let service: CourseLabsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CourseLabsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
