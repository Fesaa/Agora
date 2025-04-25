import {Component, EventEmitter, Input, Output} from '@angular/core';
import {Meeting} from '../../../../_models/meeting';
import {Card} from 'primeng/card';
import {TranslocoDirective} from '@jsverse/transloco';
import {AutoComplete, AutoCompleteCompleteEvent} from 'primeng/autocomplete';
import {FormsModule} from '@angular/forms';
import {MeetingService} from '../../../../_services/meeting.service';
import {ToastService} from '../../../../_services/toast-service';
import {AgoraButtonComponent} from '../../../../shared/components/agora-button/agora-button.component';
import {NgForOf, NgIf} from '@angular/common';

@Component({
  selector: 'app-meeting-wizard-attendees',
  imports: [
    Card,
    AgoraButtonComponent,
    TranslocoDirective,
    AutoComplete,
    FormsModule,
    NgForOf,
    NgIf
  ],
  templateUrl: './meeting-wizard-attendees.component.html',
  styleUrl: './meeting-wizard-attendees.component.css'
})
export class MeetingWizardAttendeesComponent {

  @Input({required: true}) meeting!: Meeting;
  @Output() prev: EventEmitter<void> = new EventEmitter();
  @Output() next: EventEmitter<void> = new EventEmitter();

  suggestions: string[] = []

  constructor(
    private meetingService: MeetingService,
    private toastR: ToastService,
  ) {
  }

  // Attempted fix for not being able to add manual entries
  // Asked about it in dc here: https://discord.com/channels/787967399105134612/787967806367989790/1353304808769060875
  // Is this still needed? I just added the search term in suggestions
  manualEnter() {
    const autoCompleteInput = document.getElementsByClassName('p-autocomplete-dd-input')
      .item(0) as HTMLInputElement | null;
    if (!autoCompleteInput) {
      return;
    }

    const value = autoCompleteInput.value.trim();
    if (value.length === 0) {
      return;
    }

    this.meeting.attendees.push(value);
    autoCompleteInput.value = '';
  }

  autoComplete(e: AutoCompleteCompleteEvent) {
    const q = e.query.trim();
    if (q.length === 0) {
      return;
    }

    this.meetingService.attendees(q).subscribe({
      next: (attendees) => {
        this.suggestions = [q, ...attendees.map(a => a.email)];
      },
      error: (err) => {
        console.error(err);
        this.toastR.genericError(err.error?.message || 'unknown error')
      }
    })
  }

  removeAttendee(attendee: string) {
    const index = this.meeting.attendees.indexOf(attendee);
    if (index !== -1) {
      this.meeting.attendees.splice(index, 1);
    }
  }

}
