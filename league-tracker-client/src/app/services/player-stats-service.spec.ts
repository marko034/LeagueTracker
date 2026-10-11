import { TestBed } from '@angular/core/testing';

import { PlayerStatsService } from './player-stats-service';

describe('PlayerStatsService', () => {
  let service: PlayerStatsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PlayerStatsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
