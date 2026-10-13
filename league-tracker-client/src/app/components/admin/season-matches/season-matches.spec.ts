import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SeasonMatches } from './season-matches';

describe('SeasonMatches', () => {
  let component: SeasonMatches;
  let fixture: ComponentFixture<SeasonMatches>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SeasonMatches],
    }).compileComponents();

    fixture = TestBed.createComponent(SeasonMatches);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
