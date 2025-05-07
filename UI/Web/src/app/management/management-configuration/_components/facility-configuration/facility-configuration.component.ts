import {Component, OnInit} from '@angular/core';
import {FacilityService} from '../../../../_services/facility.service';
import {Facility} from '../../../../_models/facility';
import {TranslocoDirective} from '@jsverse/transloco';
import {Card} from 'primeng/card';
import {TableModule} from 'primeng/table';
import {TitleCasePipe} from '@angular/common';
import {AgoraButtonComponent} from '../../../../shared/components/agora-button/agora-button.component';
import {Router} from '@angular/router';
import {Skeleton} from 'primeng/skeleton';
import {ToggleSwitch} from 'primeng/toggleswitch';
import {FormsModule} from '@angular/forms';
import {Tooltip} from 'primeng/tooltip';
import {Observable} from 'rxjs';
import {ToastService} from '../../../../_services/toast-service';
import {DialogService} from '../../../../_services/dialog.service';

@Component({
  selector: 'app-facility-configuration',
  imports: [
    TranslocoDirective,
    Card,
    TableModule,
    AgoraButtonComponent,
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
    private dialogService: DialogService,
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
      error: error => {
        this.toastR.errorLoco("shared.generic-error", {}, {err: error.message});
      }
    })
  }

  async delete(id: number) {
    if (!await this.dialogService.openDialog("Are you sure you want to delete this facility")) {
      return;
    }

    this.facilityService.delete(id).subscribe({
      next: _ => {
        const facility = this.facilities.find(f => f.id === id);
        this.toastR.successLoco("management.configuration.facilities.delete", {}, {name: facility?.displayName});
        this.facilities = this.facilities.filter(f => f.id !== id);
      },
      error: error => {
        this.toastR.errorLoco("shared.generic-error", {}, {err: error.message});
      }
    });
  }

  gotoWizard(id?: number) {
    this.router.navigateByUrl('management/wizard/facility' + (id ? `?facilityId=${id}` : ''));
  }

}
