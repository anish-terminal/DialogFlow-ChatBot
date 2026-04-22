import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChatTemplate } from './chat-template';

describe('ChatTemplate', () => {
  let component: ChatTemplate;
  let fixture: ComponentFixture<ChatTemplate>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ChatTemplate],
    }).compileComponents();

    fixture = TestBed.createComponent(ChatTemplate);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
