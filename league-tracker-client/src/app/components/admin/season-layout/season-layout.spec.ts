import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SeasonLayout } from './season-layout';

describe('SeasonLayout', () => {
  let component: SeasonLayout;
  let fixture: ComponentFixture<SeasonLayout>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SeasonLayout],
    }).compileComponents();

    fixture = TestBed.createComponent(SeasonLayout);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
