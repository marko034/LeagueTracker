import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SeasonPicker } from './season-picker';

describe('SeasonPicker', () => {
  let component: SeasonPicker;
  let fixture: ComponentFixture<SeasonPicker>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SeasonPicker],
    }).compileComponents();

    fixture = TestBed.createComponent(SeasonPicker);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
