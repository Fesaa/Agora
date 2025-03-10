import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {MeetingRoom} from '../../../../../_models/room';
import {FacilityService} from '../../../../../_services/facility.service';
import {Facility} from '../../../../../_models/facility';
import {Card} from 'primeng/card';
import {TranslocoDirective} from '@jsverse/transloco';
import {Fieldset} from 'primeng/fieldset';
import {FACILITIES} from '../../../../../_constants/links';
import {CdkFixedSizeVirtualScroll, CdkVirtualForOf, CdkVirtualScrollViewport} from '@angular/cdk/scrolling';
import {Checkbox} from 'primeng/checkbox';
import {Button} from 'primeng/button';
import {FormsModule} from '@angular/forms';

@Component({
  selector: 'app-room-wizard-facility',
  imports: [
    Card,
    TranslocoDirective,
    Fieldset,
    CdkVirtualScrollViewport,
    CdkFixedSizeVirtualScroll,
    CdkVirtualForOf,
    Checkbox,
    Button,
    FormsModule
  ],
  templateUrl: './room-wizard-facility.component.html',
  styleUrl: './room-wizard-facility.component.css'
})
export class RoomWizardFacilityComponent implements OnInit {

  @Input({required: true}) room!: MeetingRoom;
  @Output() roomChange = new EventEmitter<MeetingRoom>();

  @Output() next: EventEmitter<void> = new EventEmitter();
  @Output() prev: EventEmitter<void> = new EventEmitter();

  facilities: Facility[] = [];
  activeMap: { [name: number]: boolean } = {};

  constructor(
    private facilityService: FacilityService,
  ) {
  }

  ngOnInit(): void {
    this.facilityService.all().subscribe({
      next: (result) => {
        this.facilities = result.filter(f => {
          if (f.active) {
            return true;
          }

          const inc = this.room.facilities.find(f2 => f2.id === f.id)
          return inc !== undefined;
        });

        for (let f of this.room.facilities) {
          this.activeMap[f.id] = true;
        }
      },
      error: (error) => {
        console.error(error);
      }
    })
  }

  onCheckBoxChange(facility: Facility, include: boolean) {
    if (include) {
      this.room.facilities.push(facility);
      this.activeMap[facility.id] = true;
      return;
    }

    this.room.facilities = this.room.facilities.filter(f => f.id !== facility.id);
    this.activeMap[facility.id] = false;
  }


  protected readonly FACILITIES = FACILITIES;
}
