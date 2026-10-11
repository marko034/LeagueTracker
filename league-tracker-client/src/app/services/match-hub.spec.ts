import { TestBed } from '@angular/core/testing';

import { MatchHub } from './match-hub';

describe('MatchHub', () => {
  let service: MatchHub;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(MatchHub);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
