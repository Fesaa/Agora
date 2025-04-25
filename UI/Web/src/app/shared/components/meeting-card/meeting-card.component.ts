import { Component, Input, Output, EventEmitter } from '@angular/core';
import { Meeting } from '../../../_models/meeting';
import { TranslocoDirective } from '@jsverse/transloco';
import { Card } from 'primeng/card';
import { RouterLink } from '@angular/router';
import { UtcToLocalTimePipe } from '../../../_pipes/utc-to-local.pipe';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-meeting-card',
  imports: [
    TranslocoDirective,
    UtcToLocalTimePipe,
    Card,
    NgIf
  ],
  templateUrl: './meeting-card.component.html',
  styleUrl: './meeting-card.component.css',
  standalone: true
})
export class MeetingCardComponent {
  @Input() meeting!: Meeting;
  @Input() displayMode: 'detailed' | 'compact' = 'detailed';
  @Output() meetingClick = new EventEmitter<Meeting>();

  onMeetingClick(): void {
    this.meetingClick.emit(this.meeting);
  }
}
