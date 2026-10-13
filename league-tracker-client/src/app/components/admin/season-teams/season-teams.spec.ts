import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SeasonTeams } from './season-teams';

describe('SeasonTeams', () => {
  let component: SeasonTeams;
  let fixture: ComponentFixture<SeasonTeams>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SeasonTeams],
    }).compileComponents();

    fixture = TestBed.createComponent(SeasonTeams);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
