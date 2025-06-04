import { Component, Input, Output, EventEmitter } from '@angular/core';
import { Meeting } from '../../../_models/meeting';
import { TranslocoDirective } from '@jsverse/transloco';
import { Card } from 'primeng/card';
import { RouterLink } from '@angular/router';
import { UtcToLocalTimePipe } from '../../../_pipes/utc-to-local.pipe';
import { NgIf, NgClass } from '@angular/common';
import {Tooltip} from 'primeng/tooltip';

@Component({
  selector: 'app-meeting-card',
  imports: [
    TranslocoDirective,
    UtcToLocalTimePipe,
    Card,
    NgIf,
    NgClass,
    Tooltip
  ],
  templateUrl: './meeting-card.component.html',
  styleUrl: './meeting-card.component.css',
  standalone: true
})
export class MeetingCardComponent {
  @Input() meeting!: Meeting;
  @Input() displayMode: 'detailed' | 'compact' = 'detailed';
  @Input() displayAck: boolean = false;
  @Output() meetingClick = new EventEmitter<Meeting>();

  isMeetingRunning(): boolean {
    const now = new Date();
    return new Date(this.meeting.startTime) <= now;
  }

  onMeetingClick(): void {
    this.meetingClick.emit(this.meeting);
  }
}
