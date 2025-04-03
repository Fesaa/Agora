import {Component, EventEmitter, Input, Output} from '@angular/core';
import {Meeting} from '../../../../_models/meeting';

@Component({
  selector: 'app-meeting-wizard-facitlity',
  imports: [],
  templateUrl: './meeting-wizard-facitlity.component.html',
  styleUrl: './meeting-wizard-facitlity.component.css'
})
export class MeetingWizardFacilityComponent {

  @Input({required: true}) meeting!: Meeting;
  @Output() prev: EventEmitter<void> = new EventEmitter();
  @Output() next: EventEmitter<void> = new EventEmitter();

}
