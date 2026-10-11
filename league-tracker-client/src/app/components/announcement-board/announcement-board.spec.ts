import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AnnouncementBoard } from './announcement-board';

describe('AnnouncementBoard', () => {
  let component: AnnouncementBoard;
  let fixture: ComponentFixture<AnnouncementBoard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AnnouncementBoard],
    }).compileComponents();

    fixture = TestBed.createComponent(AnnouncementBoard);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
