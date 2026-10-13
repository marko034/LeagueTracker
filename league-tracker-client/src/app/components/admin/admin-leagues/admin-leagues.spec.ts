import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminLeagues } from './admin-leagues';

describe('AdminLeagues', () => {
  let component: AdminLeagues;
  let fixture: ComponentFixture<AdminLeagues>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminLeagues],
    }).compileComponents();

    fixture = TestBed.createComponent(AdminLeagues);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
