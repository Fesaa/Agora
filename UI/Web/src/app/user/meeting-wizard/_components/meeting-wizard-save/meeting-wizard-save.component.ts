import {Component, EventEmitter, Input, Output} from '@angular/core';
import {Meeting} from '../../../../_models/meeting';
import {Card} from "primeng/card";
import {FormsModule} from "@angular/forms";
import {TranslocoDirective} from "@jsverse/transloco";
import {Facility} from '../../../../_models/facility';
import {MeetingService} from '../../../../_services/meeting.service';
import {ToastService} from '../../../../_services/toast-service';
import {Router} from '@angular/router';
import {UtcToLocalTimePipe} from "../../../../_pipes/utc-to-local.pipe";
import {AgoraButtonComponent} from '../../../../shared/components/agora-button/agora-button.component';
import {NgIf} from '@angular/common';

@Component({
  selector: 'app-meeting-wizard-save',
  imports: [
    AgoraButtonComponent,
    Card,
    FormsModule,
    TranslocoDirective,
    UtcToLocalTimePipe,
    NgIf
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
    let obs;

    if (this.meeting.id === 0) {
      obs = this.meetingService.create(this.meeting);
    } else {
      obs = this.meetingService.update(this.meeting);
    }

    obs?.subscribe({
      next: () => {
        this.toastR.success('Meeting was saved');
        this.router.navigateByUrl('user/dashboard');
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
