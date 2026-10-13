import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SeasonRosters } from './season-rosters';

describe('SeasonRosters', () => {
  let component: SeasonRosters;
  let fixture: ComponentFixture<SeasonRosters>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SeasonRosters],
    }).compileComponents();

    fixture = TestBed.createComponent(SeasonRosters);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
