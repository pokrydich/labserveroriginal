import { TestBed } from '@angular/core/testing';

import { GroupCoursesService } from './group-courses.service';

describe('GroupCoursesService', () => {
  let service: GroupCoursesService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(GroupCoursesService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
