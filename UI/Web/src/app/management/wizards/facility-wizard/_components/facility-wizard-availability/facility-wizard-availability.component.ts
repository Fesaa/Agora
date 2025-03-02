import {Component, EventEmitter, Input, Output} from '@angular/core';
import {AllWeekDays, DayOfWeek, Facility} from '../../../../../_models/facility';
import {Button} from 'primeng/button';
import {Card} from 'primeng/card';
import {Checkbox} from 'primeng/checkbox';
import {Fieldset} from 'primeng/fieldset';
import {InputNumber} from 'primeng/inputnumber';
import {InputText} from 'primeng/inputtext';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {Textarea} from 'primeng/textarea';
import {TranslocoDirective} from '@jsverse/transloco';
import {Slider} from 'primeng/slider';
import {Select} from 'primeng/select';
import {MultiSelect} from 'primeng/multiselect';

@Component({
  selector: 'app-facility-wizard-availability',
  imports: [
    Button,
    Card,
    Checkbox,
    Fieldset,
    InputNumber,
    InputText,
    ReactiveFormsModule,
    Textarea,
    TranslocoDirective,
    Slider,
    FormsModule,
    Select,
    MultiSelect
  ],
  templateUrl: './facility-wizard-availability.component.html',
  styleUrl: './facility-wizard-availability.component.css'
})
export class FacilityWizardAvailabilityComponent {

  @Input({required: true}) facility!: Facility;
  @Output() next: EventEmitter<void> = new EventEmitter();
  @Output() prev: EventEmitter<void> = new EventEmitter();

  constructor() {
  }

  updateString(index: number, start: number, end: number) {

  }

  addNewAvailability() {
    this.facility.availability.push({
      id: 0,
      dayOfWeek: [],
      timeRange: "",
    })
  }

  protected readonly Object = Object;
  protected readonly AllWeekDays = AllWeekDays;
}
