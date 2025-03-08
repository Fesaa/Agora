import {Component, OnInit} from '@angular/core';
import {MeetingRoomService} from '../../_services/meeting-room.service';
import {FacilityService} from '../../_services/facility.service';
import {Splitter} from 'primeng/splitter';
import {Button} from 'primeng/button';
import {RouterLink} from '@angular/router';
import {Facility} from '../../_models/facility';
import {CdkFixedSizeVirtualScroll, CdkVirtualForOf, CdkVirtualScrollViewport} from '@angular/cdk/scrolling';

@Component({
  selector: 'app-management-dashboard',
  imports: [
    Splitter,
    Button,
    RouterLink,
    CdkVirtualScrollViewport,
    CdkVirtualForOf,
    CdkFixedSizeVirtualScroll
  ],
  templateUrl: './management-dashboard.component.html',
  styleUrl: './management-dashboard.component.css'
})
export class ManagementDashboardComponent implements OnInit {

  mostUsedFacilities: Facility[] = [];


  constructor(
    private facilityService: FacilityService,
    private roomService: MeetingRoomService,
  ) {
  }

  ngOnInit(): void {
    // TODO: Change to MostUsed after endpoint is implemented
    this.facilityService.all().subscribe(f => {
      this.mostUsedFacilities = f;
    })
  }

}
