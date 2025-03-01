import {Component, EventEmitter, Input, Output} from '@angular/core';
import {Facility} from '../../../../../_models/facility';

@Component({
  selector: 'app-facility-wizard-availability',
  imports: [],
  templateUrl: './facility-wizard-availability.component.html',
  styleUrl: './facility-wizard-availability.component.css'
})
export class FacilityWizardAvailabilityComponent {

  @Input({required: true}) facility!: Facility;
  @Output() next: EventEmitter<void> = new EventEmitter();
  @Output() prev: EventEmitter<void> = new EventEmitter();

  constructor() {
  }

}
