import {Component, EventEmitter, Input, Output} from '@angular/core';
import {Meeting} from '../../../../_models/meeting';

@Component({
  selector: 'app-meeting-wizard-general',
  imports: [],
  templateUrl: './meeting-wizard-general.component.html',
  styleUrl: './meeting-wizard-general.component.css'
})
export class MeetingWizardGeneralComponent {

  @Input({required: true}) meeting!: Meeting;
  @Output() next: EventEmitter<void> = new EventEmitter();

}
