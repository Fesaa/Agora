import {Component, EventEmitter, Input, Output} from '@angular/core';
import {Facility} from '../../../../../_models/facility';
import {Card} from 'primeng/card';
import {TranslocoDirective} from '@jsverse/transloco';
import {Fieldset} from 'primeng/fieldset';
import {FACILITIES} from '../../../../../_constants/links';
import {Button} from 'primeng/button';
import {InputTextarea} from 'primeng/inputtextarea';
import {Textarea} from 'primeng/textarea';
import {InputText} from 'primeng/inputtext';
import {FormsModule} from '@angular/forms';
import {Checkbox} from 'primeng/checkbox';
import {InputNumber} from 'primeng/inputnumber';
import {Tooltip} from 'primeng/tooltip';

@Component({
  selector: 'app-facility-wizard-general',
  imports: [
    Card,
    TranslocoDirective,
    Fieldset,
    Button,
    InputTextarea,
    Textarea,
    InputText,
    FormsModule,
    Checkbox,
    InputNumber,
    Tooltip
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
