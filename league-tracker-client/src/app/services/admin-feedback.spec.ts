import { TestBed } from '@angular/core/testing';

import { AdminFeedback } from './admin-feedback';

describe('AdminFeedback', () => {
  let service: AdminFeedback;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AdminFeedback);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
