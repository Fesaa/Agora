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
import {MeetingCardComponent} from '../../shared/components/meeting-card/meeting-card.component';
import {forkJoin} from 'rxjs';
import {ToastService} from '../../_services/toast-service';

@Component({
  selector: 'app-management-dashboard',
  imports: [
    Splitter,
    AgoraButtonComponent,
    RouterLink,
    CdkVirtualScrollViewport,
    CdkFixedSizeVirtualScroll,
    CdkVirtualForOf,
    TranslocoDirective,
    PieChartModule,
    Tooltip,
    MeetingCardComponent
  ],
  templateUrl: './management-dashboard.component.html',
  styleUrl: './management-dashboard.component.css'
})
export class ManagementDashboardComponent implements OnInit {

  mostUsedFacilities: StatsRecord[] = [];
  mostUsedRooms: StatsRecord[] = [];

  upcomingMeetings: Meeting[] = [];
  requireAckMeetings: Meeting[] = [];


  constructor(
    private facilityService: FacilityService,
    private roomService: MeetingRoomService,
    private meetingService: MeetingService,
    private statsService: StatsService,
    private router: Router,
    private toastR: ToastService,
  ) {
  }

  ngOnInit(): void {
    forkJoin([
      this.statsService.facilities(),
      this.statsService.rooms(),
      this.meetingService.upcoming(),
      this.meetingService.needAck(),
    ]).subscribe(([f, r, m, na]) => {
      this.mostUsedFacilities = f;
      this.mostUsedRooms = r;
      this.upcomingMeetings = m;
      this.requireAckMeetings = na;
    });
  }

  acknowledge(meeting: Meeting) {
    this.meetingService.ack(meeting.id, true).subscribe({
      next: () => {
        this.toastR.successLoco("management.dashboard.toast.ack", {title: meeting.title});
        this.requireAckMeetings = this.requireAckMeetings.filter(m => m.id !== meeting.id);
        this.upcomingMeetings = this.upcomingMeetings.map(m => {
          if (m.id !== meeting.id) return m;

          m.acknowledged = true;
          return m;
        })
      },
      error: err => {
        this.toastR.genericError(err);
      },
    });
  }

  navigateToMeeting(m: Meeting) {
    this.router.navigateByUrl(`/user/wizard/meeting?meetingId=${m.id}`);
  }

}
