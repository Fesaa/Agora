import {Component, EventEmitter, Input, Output} from '@angular/core';
import {Facility} from '../../../../../_models/facility';

@Component({
  selector: 'app-facility-wizard-save',
  imports: [],
  templateUrl: './facility-wizard-save.component.html',
  styleUrl: './facility-wizard-save.component.css'
})
export class FacilityWizardSaveComponent {

  @Input({required: true}) facility!: Facility;
  @Output() prev: EventEmitter<void> = new EventEmitter();

  constructor() {
  }

}
