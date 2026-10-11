import { TestBed } from '@angular/core/testing';

import { Sport } from './sport';

describe('Sport', () => {
  let service: Sport;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Sport);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
