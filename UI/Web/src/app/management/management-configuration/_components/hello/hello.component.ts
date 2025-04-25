import {Component, EventEmitter, OnInit, Output} from '@angular/core';
import {TranslocoDirective} from "@jsverse/transloco";
import {Card} from 'primeng/card';
import {MeetingRoomService} from '../../../../_services/meeting-room.service';
import {FacilityService} from '../../../../_services/facility.service';
import {forkJoin} from 'rxjs';
import {ConfigurationId} from '../../management-configuration.component';
import {RouterLink} from '@angular/router';
import {AgoraButtonComponent} from '../../../../shared/components/agora-button/agora-button.component';

@Component({
  selector: 'app-hello',
  imports: [
    TranslocoDirective,
    Card,
    AgoraButtonComponent,
    RouterLink
  ],
  templateUrl: './hello.component.html',
  styleUrl: './hello.component.css'
})
export class HelloComponent implements OnInit {

  @Output() changeMenuTab: EventEmitter<ConfigurationId> = new EventEmitter();

  // Assume true to ensure the recommendations don't flicker
  hasAnyRooms: boolean = true;
  hasAnyFacilities: boolean = true;

  constructor(
    private roomService: MeetingRoomService,
    private facilityService: FacilityService,
  ) {
  }

  ngOnInit(): void {
    forkJoin({
      rooms: this.roomService.all(),
      facilities: this.facilityService.all()
      }).subscribe(({rooms, facilities}) => {
      this.hasAnyRooms = rooms.length > 0;
      this.hasAnyFacilities = facilities.length > 0;
    });
  }

  protected readonly ConfigurationId = ConfigurationId;
}
