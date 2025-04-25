import {Component, OnInit} from '@angular/core';
import {MeetingRoomService} from '../../_services/meeting-room.service';
import {FacilityService} from '../../_services/facility.service';
import {Splitter} from 'primeng/splitter';
import {Router, RouterLink} from '@angular/router';
import {AgoraButtonComponent} from '../../shared/components/agora-button/agora-button.component';
import {Facility} from '../../_models/facility';
import {Meeting} from '../../_models/meeting';
import {MeetingService} from '../../_services/meeting.service';
import {UtcToLocalTimePipe} from '../../_pipes/utc-to-local.pipe';
import {CdkFixedSizeVirtualScroll, CdkVirtualForOf, CdkVirtualScrollViewport} from '@angular/cdk/scrolling';
import {TranslocoDirective} from '@jsverse/transloco';
import {StatsService} from '../../_services/stats.service';
import {StatsRecord} from '../../_models/stats';
import {PieChartModule} from '@swimlane/ngx-charts';
import {Tooltip} from 'primeng/tooltip';

@Component({
  selector: 'app-management-dashboard',
  imports: [
    Splitter,
    AgoraButtonComponent,
    RouterLink,
    UtcToLocalTimePipe,
    CdkVirtualScrollViewport,
    CdkFixedSizeVirtualScroll,
    CdkVirtualForOf,
    TranslocoDirective,
    PieChartModule,
    Tooltip
  ],
  templateUrl: './management-dashboard.component.html',
  styleUrl: './management-dashboard.component.css'
})
export class ManagementDashboardComponent implements OnInit {

  mostUsedFacilities: StatsRecord[] = [];
  mostUsedRooms: StatsRecord[] = [];

  upcomingMeetings: Meeting[] = [];


  constructor(
    private facilityService: FacilityService,
    private roomService: MeetingRoomService,
    private meetingService: MeetingService,
    private statsService: StatsService,
    private router: Router,
  ) {
  }

  ngOnInit(): void {
    // TODO: Change to MostUsed after endpoint is implemented
    this.statsService.facilities().subscribe(f => {
      this.mostUsedFacilities = f;
    });

    this.statsService.rooms().subscribe(r => {
      this.mostUsedRooms = r;
    })

    this.meetingService.upcoming().subscribe(m => {
      this.upcomingMeetings = m;
    });
  }

  navigateToMeeting(m: Meeting) {
    this.router.navigateByUrl(`/user/wizard/meeting?meetingId=${m.id}`);
  }

}
