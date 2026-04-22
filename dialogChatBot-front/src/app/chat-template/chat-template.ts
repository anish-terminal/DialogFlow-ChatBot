import { HttpClient } from '@angular/common/http';
import { ChangeDetectorRef, Component, NgZone, ElementRef, ViewChild } from '@angular/core';
import { ChatMessage } from '../model/chat-message';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Message } from '../model/message';
import * as signalR from '@microsoft/signalr';

@Component({
  selector: 'app-chat-template',
  imports: [FormsModule, CommonModule],
  templateUrl: './chat-template.html',
  styleUrl: './chat-template.css',
})
export class ChatTemplate {

  baseUrl: string = "http://localhost:5042/api/"

  @ViewChild('scrollContainer') scrollContainer!: ElementRef;

  constructor(private http: HttpClient, private zone: NgZone, private cd: ChangeDetectorRef) { }

  messages: Message[] = [];
  message: string = '';
  sessionId: string = '';
  connection : any ;
  isTyping: boolean = false;
  errorMessage: string = '';

  ngOnInit() {
    let sessionId = localStorage.getItem('sessionId') || crypto.randomUUID();
    localStorage.setItem('sessionId', sessionId);

    this.sessionId = sessionId;

    this.connection = new signalR.HubConnectionBuilder()
    .withUrl(`http://localhost:5042/chatHub?sessionId=${this.sessionId}`)
    .withAutomaticReconnect()
    .build();

    this.connection.start()
      .then(() => console.log('Connected'));

    //  Loading history chats
    this.connection.on('LoadMessages', (msgs: Message[]) => {
      this.messages = msgs;
      this.cd.detectChanges();
      setTimeout(() => {
        this.scrollToBottom();
      }, 100);
    });

    this.connection.on('ReceiveMessage', (msg: Message) => {
        this.messages = [...this.messages, msg];
        if (msg.sender === 'bot') {
          this.isTyping = false;
        }
        this.cd.detectChanges();
        setTimeout(() => {
          this.scrollToBottom();
        }, 100);
    });

    this.connection.on('Error', (err: string) => {
      this.errorMessage = err;

      this.cd.detectChanges();
      setTimeout(() => {
        this.errorMessage = '';
        this.cd.detectChanges();
      }, 3000);
    });
  }

scrollToBottom() {
  // const el = this.scrollContainer.nativeElement;
 const el = document.getElementById("scrollContainer")!;
  el.scrollTo({
    top: el.scrollHeight,
    behavior: 'smooth'
  });
}

  sendMessage() {
  if (!this.message) return;

    

  this.connection.invoke('SendMessage', this.sessionId, this.message);

  this.isTyping = true;
    setTimeout(() => this.scrollToBottom(), 100);

  this.message = '';
}

}
