import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminPlayers } from './admin-players';

describe('AdminPlayers', () => {
  let component: AdminPlayers;
  let fixture: ComponentFixture<AdminPlayers>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminPlayers],
    }).compileComponents();

    fixture = TestBed.createComponent(AdminPlayers);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
