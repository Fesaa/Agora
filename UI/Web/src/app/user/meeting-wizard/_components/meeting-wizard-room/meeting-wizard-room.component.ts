import {Component, EventEmitter, Input, Output} from '@angular/core';
import {Meeting} from '../../../../_models/meeting';

@Component({
  selector: 'app-meeting-wizard-room',
  imports: [],
  templateUrl: './meeting-wizard-room.component.html',
  styleUrl: './meeting-wizard-room.component.css'
})
export class MeetingWizardRoomComponent {

  @Input({required: true}) meeting!: Meeting;
  @Output() prev: EventEmitter<void> = new EventEmitter();
  @Output() next: EventEmitter<void> = new EventEmitter();

}
