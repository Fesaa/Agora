import {Component, OnInit} from '@angular/core';
import {MeetingRoomService} from '../../_services/meeting-room.service';
import {FacilityService} from '../../_services/facility.service';
import {Splitter} from 'primeng/splitter';
import {Button} from 'primeng/button';
import {RouterLink} from '@angular/router';
import {Facility} from '../../_models/facility';
import {Meeting} from '../../_models/meeting';
import {MeetingService} from '../../_services/meeting.service';
import {UtcToLocalTimePipe} from '../../_pipes/utc-to-local.pipe';
import {CdkFixedSizeVirtualScroll, CdkVirtualForOf, CdkVirtualScrollViewport} from '@angular/cdk/scrolling';
import {TranslocoDirective} from '@jsverse/transloco';

@Component({
  selector: 'app-management-dashboard',
  imports: [
    Splitter,
    Button,
    RouterLink,
    UtcToLocalTimePipe,
    CdkVirtualScrollViewport,
    CdkFixedSizeVirtualScroll,
    CdkVirtualForOf,
    TranslocoDirective
  ],
  templateUrl: './management-dashboard.component.html',
  styleUrl: './management-dashboard.component.css'
})
export class ManagementDashboardComponent implements OnInit {

  mostUsedFacilities: Facility[] = [];
  upcomingMeetings: Meeting[] = [];


  constructor(
    private facilityService: FacilityService,
    private roomService: MeetingRoomService,
    private meetingService: MeetingService,
  ) {
  }

  ngOnInit(): void {
    // TODO: Change to MostUsed after endpoint is implemented
    this.facilityService.all().subscribe(f => {
      this.mostUsedFacilities = f;
    });

    this.meetingService.upcoming().subscribe(m => {
      this.upcomingMeetings = m;
    });
  }

}
