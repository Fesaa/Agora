import {Component, OnInit} from '@angular/core';
import {FacilityService} from '../../../../_services/facility.service';
import {Facility} from '../../../../_models/facility';
import {TranslocoDirective} from '@jsverse/transloco';
import {Card} from 'primeng/card';
import {TableModule} from 'primeng/table';
import {Button} from 'primeng/button';
import {TitleCasePipe} from '@angular/common';
import {Router} from '@angular/router';
import {Skeleton} from 'primeng/skeleton';

@Component({
  selector: 'app-facility-configuration',
  imports: [
    TranslocoDirective,
    Card,
    TableModule,
    Button,
    TitleCasePipe,
    Skeleton
  ],
  templateUrl: './facility-configuration.component.html',
  styleUrl: './facility-configuration.component.css'
})
export class FacilityConfigurationComponent implements OnInit{

  facilities: Facility[] = [];
  loading: boolean = true;

  size = 10;

  constructor(
    private facilityService: FacilityService,
    private router: Router,
  ) {
  }

  ngOnInit(): void {
    this.facilityService.all().subscribe(facilities => {
      this.facilities = facilities;
      this.loading = false;
    })
  }

  delete(id: number) {}

  gotoWizard(id?: number) {
    this.router.navigateByUrl('management/wizard/facility' + (id ? `?facilityId=${id}` : ''));
  }

}
