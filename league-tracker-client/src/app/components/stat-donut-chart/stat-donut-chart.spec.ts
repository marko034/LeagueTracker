import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StatDonutChart } from './stat-donut-chart';

describe('StatDonutChart', () => {
  let component: StatDonutChart;
  let fixture: ComponentFixture<StatDonutChart>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StatDonutChart],
    }).compileComponents();

    fixture = TestBed.createComponent(StatDonutChart);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
