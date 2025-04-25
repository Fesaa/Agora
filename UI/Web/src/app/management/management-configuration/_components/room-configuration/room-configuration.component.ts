import {Component, OnInit} from '@angular/core';
import {Router} from '@angular/router';
import {MeetingRoomService} from '../../../../_services/meeting-room.service';
import {MeetingRoom} from '../../../../_models/room';
import {Card} from 'primeng/card';
import {TableModule} from 'primeng/table';
import {Skeleton} from 'primeng/skeleton';
import {AgoraButtonComponent} from '../../../../shared/components/agora-button/agora-button.component';
import {TranslocoDirective} from '@jsverse/transloco';
import {ROOMS} from '../../../../_constants/links';

@Component({
  selector: 'app-room-configuration',
  imports: [
    Card,
    TableModule,
    AgoraButtonComponent,
    Skeleton,
    TranslocoDirective
  ],
  templateUrl: './room-configuration.component.html',
  styleUrl: './room-configuration.component.css'
})
export class RoomConfigurationComponent implements OnInit{

  meetingRooms: MeetingRoom[] = [];
  loading: boolean = true;

  size = 10;

  constructor(
    private roomService: MeetingRoomService,
    private router: Router,
  ) {
  }

  ngOnInit(): void {
    this.roomService.all().subscribe(m => {
      this.meetingRooms = m;
      this.loading = false;
    })
  }

  delete(id: number) {}

  gotoWizard(id?: number) {
    this.router.navigateByUrl('management/wizard/room' + (id ? `?roomId=${id}` : ''));
  }

  protected readonly ROOMS = ROOMS;
}
