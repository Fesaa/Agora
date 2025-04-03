import {Component, EventEmitter, Input, Output} from '@angular/core';
import {Meeting} from '../../../../_models/meeting';
import {Button} from "primeng/button";
import {Card} from "primeng/card";
import {FormsModule} from "@angular/forms";
import {TranslocoDirective} from "@jsverse/transloco";
import {DatePipe} from '@angular/common';
import {Facility} from '../../../../_models/facility';
import {MeetingService} from '../../../../_services/meeting.service';
import {ToastService} from '../../../../_services/toast-service';
import {Router} from '@angular/router';
import {UtcToLocalTimePipe} from "../../../../_pipes/utc-to-local.pipe";

@Component({
  selector: 'app-meeting-wizard-save',
  imports: [
    Button,
    Card,
    FormsModule,
    TranslocoDirective,
    DatePipe,
    UtcToLocalTimePipe
  ],
  templateUrl: './meeting-wizard-save.component.html',
  styleUrl: './meeting-wizard-save.component.css'
})
export class MeetingWizardSaveComponent {

  @Input({required: true}) meeting!: Meeting;
  @Output() prev: EventEmitter<void> = new EventEmitter();

  constructor(
    private meetingService: MeetingService,
    private toastR: ToastService,
    private router: Router,
  ) {
  }

  save() {
    this.meetingService.create(this.meeting).subscribe({
      next: () => {
        this.toastR.success('Meeting was saved');
      },
      error: (err) => {
        console.error(err);
        this.toastR.genericError(err.error.message);
      }
    })
  }

  getFacilityDisplayNames(facilities: Facility[]): string {
    if (!facilities || facilities.length === 0) {
      return 'None';
    }
    return facilities.map(facility => facility.displayName).join(', ');
  }

}
