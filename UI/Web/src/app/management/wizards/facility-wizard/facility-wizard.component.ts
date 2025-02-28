import {Component, OnInit} from '@angular/core';
import {Facility} from '../../../_models/facility';
import {FacilityService} from '../../../_services/facility.service';
import {ActivatedRoute, NavigationExtras, Router} from '@angular/router';
import {ToastService} from '../../../_services/toast-service';
import {Steps} from 'primeng/steps';

export enum FacilityWizardID {
  General = 'General',
  Availability = 'Availability',

  Save = 'Save',
}

@Component({
  selector: 'app-facility-wizard',
  imports: [
    Steps
  ],
  templateUrl: './facility-wizard.component.html',
  styleUrl: './facility-wizard.component.css'
})
export class FacilityWizardComponent implements OnInit{

  facility: Facility | undefined;
  index: number = 0;
  sections: {id: FacilityWizardID, label: string}[] = [
    {id: FacilityWizardID.General, label: 'general'},
    {id: FacilityWizardID.Availability, label: 'availability'},
    {id: FacilityWizardID.Save, label: 'save'},
  ]

  private readonly defaultFacility: Facility = {
    id: 0,
    displayName: '',
    description: '',
    availability: [],
    alertManagement: false,
    cost: 0,
  }

  constructor(
    private facilityService: FacilityService,
    private route: ActivatedRoute,
    private activatedRoute: ActivatedRoute,
    private toastService: ToastService,
    private router: Router,
  ) {
  }

  ngOnInit(): void {
    this.activatedRoute.queryParams.subscribe((params) => {
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
        },
        error: (error) => {
          console.error(error);
          this.toastService.errorLoco("shared.generic-error", {}, {err: error.message});
          this.router.navigateByUrl("/management/configuration#facilities")
        }
      })

      this.route.fragment.subscribe(fragment => {
        const section = this.sections.filter(section => section.id == fragment)
        if (section && section.length > 0) {
          this.navigateToPage(this.sections.indexOf(section[0]))
        } else {
          this.navigateToPage(0)
        }
      })
    })
  }

  navigateToPage(index: number) {
    this.index = index;

    const sectionId = this.sections[this.index].id;

    const extras: NavigationExtras = {
      fragment: sectionId
    };

    if (this.facility && this.facility.id !== 0) {
      extras.queryParams = {
        pageId: this.facility.id
      }
    }

    this.router.navigate([], extras)
  }

}
