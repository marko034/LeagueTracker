import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditableAvatar } from './editable-avatar';

describe('EditableAvatar', () => {
  let component: EditableAvatar;
  let fixture: ComponentFixture<EditableAvatar>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EditableAvatar],
    }).compileComponents();

    fixture = TestBed.createComponent(EditableAvatar);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
