import {Component, EventEmitter, Input, Output} from '@angular/core';
import {Meeting} from '../../../../_models/meeting';

@Component({
  selector: 'app-meeting-wizard-attendees',
  imports: [],
  templateUrl: './meeting-wizard-attendees.component.html',
  styleUrl: './meeting-wizard-attendees.component.css'
})
export class MeetingWizardAttendeesComponent {

  @Input({required: true}) meeting!: Meeting;
  @Output() prev: EventEmitter<void> = new EventEmitter();
  @Output() next: EventEmitter<void> = new EventEmitter();

}
