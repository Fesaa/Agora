import {Component, EventEmitter, Input, Output} from '@angular/core';
import {Facility} from '../../../../../_models/facility';
import {Card} from 'primeng/card';
import {TranslocoDirective} from '@jsverse/transloco';
import {Fieldset} from 'primeng/fieldset';
import {FACILITIES} from '../../../../../_constants/links';

@Component({
  selector: 'app-facility-wizard-general',
  imports: [
    Card,
    TranslocoDirective,
    Fieldset
  ],
  templateUrl: './facility-wizard-general.component.html',
  styleUrl: './facility-wizard-general.component.css'
})
export class FacilityWizardGeneralComponent {

  @Input({required: true}) facility!: Facility;
  @Output() next: EventEmitter<void> = new EventEmitter();

  constructor() {
  }

  protected readonly FACILITIES = FACILITIES;
}
