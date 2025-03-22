import {Component, EventEmitter, Input, Output} from '@angular/core';
import {Meeting} from '../../../../_models/meeting';
import {Card} from 'primeng/card';
import {TranslocoDirective} from '@jsverse/transloco';
import {FormsModule} from '@angular/forms';
import {InputText} from 'primeng/inputtext';
import {Textarea} from 'primeng/textarea';
import {DatePicker} from 'primeng/datepicker';
import {Button} from 'primeng/button';
import {ToastService} from '../../../../_services/toast-service';

@Component({
  selector: 'app-meeting-wizard-general',
  imports: [
    Card,
    TranslocoDirective,
    FormsModule,
    InputText,
    Textarea,
    Button
  ],
  templateUrl: './meeting-wizard-general.component.html',
  styleUrl: './meeting-wizard-general.component.css'
})
export class MeetingWizardGeneralComponent {

  @Input({required: true}) meeting!: Meeting;
  @Output() next: EventEmitter<void> = new EventEmitter();

  constructor(private toastR: ToastService) {
  }

  continue() {
    if (this.meeting.title.length === 0) {
      this.toastR.errorLoco("user.wizard.meeting.general.require-title");
      return;
    }

    this.next.emit();
  }

}
