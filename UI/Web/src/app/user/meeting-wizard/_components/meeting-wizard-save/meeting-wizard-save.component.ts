import {Component, EventEmitter, Input, Output} from '@angular/core';
import {Meeting} from '../../../../_models/meeting';

@Component({
  selector: 'app-meeting-wizard-save',
  imports: [],
  templateUrl: './meeting-wizard-save.component.html',
  styleUrl: './meeting-wizard-save.component.css'
})
export class MeetingWizardSaveComponent {

  @Input({required: true}) meeting!: Meeting;
  @Output() prev: EventEmitter<void> = new EventEmitter();

}
