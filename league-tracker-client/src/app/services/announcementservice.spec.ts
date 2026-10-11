import { TestBed } from '@angular/core/testing';

import { Announcementservice } from './announcementservice';

describe('Announcementservice', () => {
  let service: Announcementservice;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Announcementservice);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
