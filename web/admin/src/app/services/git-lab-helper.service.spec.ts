import { TestBed } from '@angular/core/testing';

import { GitLabHelperService } from './git-lab-helper.service';

describe('GitLabHelperService', () => {
  let service: GitLabHelperService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(GitLabHelperService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
