import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {Meeting} from '../../../../_models/meeting';
import {Facility} from '../../../../_models/facility';
import {FacilityService} from '../../../../_services/facility.service';
import {ToastService} from '../../../../_services/toast-service';
import {Card} from 'primeng/card';
import {FormsModule} from '@angular/forms';
import {TranslocoDirective} from '@jsverse/transloco';
import {CdkFixedSizeVirtualScroll, CdkVirtualForOf, CdkVirtualScrollViewport} from '@angular/cdk/scrolling';
import {NgClass, NgForOf, NgIf} from '@angular/common';
import {Tooltip} from 'primeng/tooltip';
import {AgoraButtonComponent} from '../../../../shared/components/agora-button/agora-button.component';

@Component({
  selector: 'app-meeting-wizard-facility',
  imports: [
    AgoraButtonComponent,
    Card,
    FormsModule,
    TranslocoDirective,
    NgIf,
    NgClass,
    Tooltip,
    NgForOf
  ],
  templateUrl: './meeting-wizard-facility.component.html',
  styleUrl: './meeting-wizard-facility.component.css'
})
export class MeetingWizardFacilityComponent implements OnInit {

  @Input({required: true}) meeting!: Meeting;
  @Output() prev: EventEmitter<void> = new EventEmitter();
  @Output() next: EventEmitter<void> = new EventEmitter();

  facilities: Facility[] = []

  constructor(
    private facilityService: FacilityService,
    private toastR: ToastService,
  ) {
  }

  ngOnInit(): void {
    this.facilityService.getForRoom(this.meeting.room.id).subscribe({
      next: (facilities) => {
        this.facilities = facilities;
      },
      error: (error) => {
        console.error(error);
        this.toastR.genericError(error.error.message);
      }
    })
  }

  handleFacilityClick(facility: Facility) {
    if (this.meeting.facilities.includes(facility)) {
      this.meeting.facilities = this.meeting.facilities.filter(f => f.id !== facility.id);
      return;
    }

    this.meeting.facilities.push(facility);
  }

}
