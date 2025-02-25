import {Component, OnInit} from '@angular/core';
import {FacilityService} from '../../../../_services/facility.service';
import {Facility} from '../../../../_models/facility';

@Component({
  selector: 'app-facility-configuration',
  imports: [],
  templateUrl: './facility-configuration.component.html',
  styleUrl: './facility-configuration.component.css'
})
export class FacilityConfigurationComponent implements OnInit{

  facilities: Facility[] = [];

  constructor(
    private facilityService: FacilityService,
  ) {
  }

  ngOnInit(): void {
    this.facilityService.all().subscribe(facilities => {
      this.facilities = facilities;
    })
  }

}
