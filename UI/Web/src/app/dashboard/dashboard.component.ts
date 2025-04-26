import {Component, OnInit, HostListener} from '@angular/core';
import {MeetingService} from '../_services/meeting.service';
import {Meeting} from '../_models/meeting';
import {ToastService} from '../_services/toast-service';
import {MeetingCardComponent} from '../shared/components/meeting-card/meeting-card.component';
import {Carousel} from 'primeng/carousel';
import {PrimeTemplate} from 'primeng/api';
import {TranslocoDirective} from '@jsverse/transloco';
import {ActivatedRoute, Router} from '@angular/router';
import {MeetingRoomService} from '../_services/meeting-room.service';
import {MeetingRoom} from '../_models/room';
import {AgoraButtonComponent} from '../shared/components/agora-button/agora-button.component';

@Component({
  selector: 'app-dashboard',
  imports: [MeetingCardComponent, Carousel, PrimeTemplate, TranslocoDirective, AgoraButtonComponent],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit{

  meetings: Meeting[] = [];
  roomId: number | null = null;
  room: MeetingRoom | null = null;

  window = window;

  carouselOptions = {
    numVisible: 3,
    numScroll: 1,
    circular: true,
    autoplayInterval: 5000,
    showNavigators: false,
    showIndicators: true
  };

  responsiveOptions = [
    {
      breakpoint: '1400px',
      numVisible: 3,
      numScroll: 1
    },
    {
      breakpoint: '1024px',
      numVisible: 2,
      numScroll: 1
    },
    {
      breakpoint: '768px',
      numVisible: 1,
      numScroll: 1
    }
  ];

  constructor(
    private meetingService: MeetingService,
    private toastR: ToastService,
    private route: ActivatedRoute,
    private router: Router,
    private meetingRoomService: MeetingRoomService
  ) {
  }

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      if (params['roomId']) {
        try {
          this.roomId = parseInt(params['roomId']);
        } catch (e) {
          this.roomId = null;
          this.fetchMeetings();
          return;
        }

        this.meetingRoomService.get(this.roomId).subscribe({
          next: room => {
            this.room = room;
            this.fetchMeetings();
          },
          error: err => {
            this.toastR.genericError('Error loading room details');
            this.roomId = null;
            this.fetchMeetings();
          }
        });
      } else {
        this.fetchMeetings();
      }
    });
  }

  fetchMeetings(): void {
    this.meetingService.today(false, this.roomId).subscribe({
      next: m => {
        this.meetings = m;
      },
      error: err => {
        this.toastR.genericError(err.error.message);
      }
    });
  }

  createMeeting(): void {
    const queryParams = this.roomId ? { roomId: this.roomId } : {};
    this.router.navigate(['/user/wizard/meeting'], { queryParams });
  }

}
