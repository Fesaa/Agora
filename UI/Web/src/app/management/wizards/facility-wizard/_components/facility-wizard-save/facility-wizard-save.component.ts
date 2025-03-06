import {Component, EventEmitter, Input, Output} from '@angular/core';
import {AllWeekDays, Facility} from '../../../../../_models/facility';
import {Card} from 'primeng/card';
import {TranslocoDirective} from '@jsverse/transloco';
import {Fieldset} from 'primeng/fieldset';
import {NgForOf, NgIf} from '@angular/common';
import {Button} from 'primeng/button';

@Component({
  selector: 'app-facility-wizard-save',
  imports: [
    Card,
    TranslocoDirective,
    Fieldset,
    NgForOf,
    NgIf,
    Button
  ],
  templateUrl: './facility-wizard-save.component.html',
  styleUrl: './facility-wizard-save.component.css'
})
export class FacilityWizardSaveComponent {

  @Input({required: true}) facility!: Facility;
  @Output() prev: EventEmitter<void> = new EventEmitter();
  @Output() save: EventEmitter<void> = new EventEmitter();

  constructor() {
  }

  protected readonly AllWeekDays = AllWeekDays;
}
