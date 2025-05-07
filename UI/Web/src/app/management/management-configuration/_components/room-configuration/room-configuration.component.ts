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
import {DialogService} from '../../../../_services/dialog.service';
import {ToastService} from '../../../../_services/toast-service';

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
    private toastR: ToastService,
    private dialogService: DialogService,
  ) {
  }

  ngOnInit(): void {
    this.roomService.all().subscribe(m => {
      this.meetingRooms = m;
      this.loading = false;
    })
  }

  async delete(id: number) {
    if (!await this.dialogService.openDialog("Are you sure you want to delete this room")) {
      return;
    }

    this.roomService.delete(id).subscribe({
      next: _ => {
        const room = this.meetingRooms.find(r => r.id === id);
        this.toastR.successLoco("management.configuration.rooms.delete", {}, {name: room?.displayName});
        this.meetingRooms = this.meetingRooms.filter(r => r.id !== id);
      },
      error: error => {
        this.toastR.errorLoco("shared.generic-error", {}, {err: error.message});
      }
    })
  }

  gotoWizard(id?: number) {
    this.router.navigateByUrl('management/wizard/room' + (id ? `?roomId=${id}` : ''));
  }

  goto(roomId: number) {
    this.router.navigateByUrl(`dashboard?roomId=${roomId}`)
  }

  protected readonly ROOMS = ROOMS;
}
