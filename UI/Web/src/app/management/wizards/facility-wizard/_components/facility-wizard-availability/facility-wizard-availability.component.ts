import {Component, EventEmitter, Input, Output} from '@angular/core';
import {AllWeekDays, Availability, DayOfWeek, Facility} from '../../../../../_models/facility';
import {Button} from 'primeng/button';
import {Card} from 'primeng/card';
import {Fieldset} from 'primeng/fieldset';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {TranslocoDirective} from '@jsverse/transloco';
import {Slider} from 'primeng/slider';

@Component({
  selector: 'app-facility-wizard-availability',
  imports: [
    Button,
    Card,
    Fieldset,
    ReactiveFormsModule,
    TranslocoDirective,
    Slider,
    FormsModule,
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

  hasDayChecked(availability: Availability, day: DayOfWeek): boolean {
    return availability.dayOfWeek.includes(day);
  }

  updateDayChecked(availability: Availability, day: DayOfWeek) {
    if (this.hasDayChecked(availability, day)) {
      availability.dayOfWeek = availability.dayOfWeek.filter(d => d !== day)
    } else {
      availability.dayOfWeek.push(day);
    }
  }

  updateString(availability: Availability, start: number, end: number) {
    availability.timeRange = `${start}h-${end}h`;
  }

  addNewAvailability() {
    this.facility.availability.push({
      id: 0,
      dayOfWeek: [],
      timeRange: "8h-17h",
    })
  }

  remove(idx: number) {
    this.facility.availability = this.facility.availability.filter((_, i) => {
      return i !== idx;
    });
  }

  protected readonly Object = Object;
  protected readonly AllWeekDays = AllWeekDays;
}
