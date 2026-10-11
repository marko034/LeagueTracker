import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RecordGame } from './record-game';

describe('RecordGame', () => {
  let component: RecordGame;
  let fixture: ComponentFixture<RecordGame>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RecordGame],
    }).compileComponents();

    fixture = TestBed.createComponent(RecordGame);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
