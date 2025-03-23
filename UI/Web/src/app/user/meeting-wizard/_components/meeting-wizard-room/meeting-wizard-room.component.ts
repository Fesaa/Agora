import {Component, EventEmitter, Input, Output} from '@angular/core';
import {Meeting} from '../../../../_models/meeting';
import {Card} from 'primeng/card';
import {Button} from 'primeng/button';
import {TranslocoDirective} from '@jsverse/transloco';

@Component({
  selector: 'app-meeting-wizard-room',
  imports: [
    Card,
    Button,
    TranslocoDirective
  ],
  templateUrl: './meeting-wizard-room.component.html',
  styleUrl: './meeting-wizard-room.component.css'
})
export class MeetingWizardRoomComponent {

  @Input({required: true}) meeting!: Meeting;
  @Output() prev: EventEmitter<void> = new EventEmitter();
  @Output() next: EventEmitter<void> = new EventEmitter();

}
