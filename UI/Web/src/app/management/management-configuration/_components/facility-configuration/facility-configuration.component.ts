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
import {ToggleSwitch} from 'primeng/toggleswitch';
import {FormsModule} from '@angular/forms';
import {Tooltip} from 'primeng/tooltip';
import {Observable} from 'rxjs';
import {ToastService} from '../../../../_services/toast-service';

@Component({
  selector: 'app-facility-configuration',
  imports: [
    TranslocoDirective,
    Card,
    TableModule,
    Button,
    TitleCasePipe,
    Skeleton,
    ToggleSwitch,
    FormsModule,
    Tooltip
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
    private toastR: ToastService,
  ) {
  }

  ngOnInit(): void {
    this.facilityService.all().subscribe(facilities => {
      this.facilities = facilities;
      this.loading = false;
    })
  }

  updateActive(facility: Facility, active: boolean): void {
    let obs: Observable<any>
    if (active) {
      obs = this.facilityService.activate(facility.id);
    } else {
      obs = this.facilityService.deactivate(facility.id);
    }

    obs.subscribe({
      next: result => {

      },
      error: error => {
        this.toastR.errorLoco("shared.generic-error", {}, {err: error.message});
      }
    })
  }

  delete(id: number) {}

  gotoWizard(id?: number) {
    this.router.navigateByUrl('management/wizard/facility' + (id ? `?facilityId=${id}` : ''));
  }

}
