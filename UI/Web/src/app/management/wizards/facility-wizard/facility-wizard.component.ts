import {Component, OnInit} from '@angular/core';
import {Facility} from '../../../_models/facility';
import {FacilityService} from '../../../_services/facility.service';
import {ActivatedRoute, NavigationExtras, Router} from '@angular/router';
import {ToastService} from '../../../_services/toast-service';
import {Steps} from 'primeng/steps';
import {FacilityWizardGeneralComponent} from './_components/facility-wizard-general/facility-wizard-general.component';
import {
  FacilityWizardAvailabilityComponent
} from './_components/facility-wizard-availability/facility-wizard-availability.component';
import {FacilityWizardSaveComponent} from './_components/facility-wizard-save/facility-wizard-save.component';
import {Observable} from 'rxjs';

export enum FacilityWizardID {
  General = 'General',
  Availability = 'Availability',

  Save = 'Save',
}

@Component({
  selector: 'app-facility-wizard',
  imports: [
    Steps,
    FacilityWizardGeneralComponent,
    FacilityWizardAvailabilityComponent,
    FacilityWizardSaveComponent
  ],
  templateUrl: './facility-wizard.component.html',
  styleUrl: './facility-wizard.component.css'
})
export class FacilityWizardComponent implements OnInit{

  facility: Facility | undefined;
  index: number = 0;
  // TODO: Transloco
  sections: {id: FacilityWizardID, label: string}[] = [
    {id: FacilityWizardID.General, label: 'General'},
    {id: FacilityWizardID.Availability, label: 'Availability'},
    {id: FacilityWizardID.Save, label: 'Save'},
  ]

  private readonly defaultFacility: Facility = {
    id: 0,
    displayName: '',
    description: '',
    availability: [],
    alertManagement: false,
    active: true,
    cost: 0,
  }

  constructor(
    private facilityService: FacilityService,
    private route: ActivatedRoute,
    private toastService: ToastService,
    private router: Router,
  ) {
  }

  ngOnInit(): void {
    this.route.queryParams.subscribe((params) => {
      const facilityIdParam = params['facilityId'];
      if (!facilityIdParam) {
        this.facility = this.defaultFacility;
        return;
      }

      let facilityId;
      try {
        facilityId = parseInt(facilityIdParam);
      } catch (e) {
        console.error(e);
        this.router.navigateByUrl("/management/configuration#facilities")
        return
      }

      this.facilityService.get(facilityId).subscribe({
        next: (facility) => {
          this.facility = facility;
          this.loadStage();
        },
        error: (error) => {
          console.error(error);
          this.toastService.errorLoco("shared.generic-error", {}, {err: error.message});
          this.router.navigateByUrl("/management/configuration#facilities")
        }
      })
    })
  }

  private loadStage() {
    this.route.fragment.subscribe(fragment => {
      const section = this.sections.find(section => section.id == fragment)
      if (section) {
        this.navigateToPage(this.sections.indexOf(section), true)
      } else {
        this.navigateToPage(0, true)
      }
    })
  }

  navigateToPage(index: number, skipLocation: boolean = false) {
    this.index = index;

    const sectionId = this.sections[this.index].id;

    const extras: NavigationExtras = {
      fragment: sectionId,
      skipLocationChange: skipLocation,
    };

    if (this.facility && this.facility.id !== 0) {
      extras.queryParams = {
        facilityId: this.facility.id
      }
    }

    this.router.navigate([], extras)
  }

  save(): void {
    if (!this.facility) {
      this.toastService.errorLoco("shared.generic-error", {err: "No facility?"})
      return;
    }

    let obs: Observable<Facility>
    if (this.facility.id > 0) {
      obs = this.facilityService.update(this.facility)
    } else {
      obs = this.facilityService.create(this.facility)
    }

    obs.subscribe({
      next: (facility) => {
        this.toastService.success("Saved facility");
        this.router.navigateByUrl("/management/configuration#facilities")
      },
      error: (error) => {
        this.toastService.errorLoco("shared.generic-error", {err: error.message});
      }
    })
  }

  protected readonly FacilityWizardID = FacilityWizardID;
}
