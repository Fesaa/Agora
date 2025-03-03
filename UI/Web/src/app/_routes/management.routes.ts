import {Routes} from '@angular/router';
import {ManagementDashboardComponent} from '../management/management-dashboard/management-dashboard.component';
import {
  ManagementConfigurationComponent
} from '../management/management-configuration/management-configuration.component';
import {FacilityWizardComponent} from '../management/wizards/facility-wizard/facility-wizard.component';

export const routes: Routes = [
  {
    path: 'dashboard',
    component: ManagementDashboardComponent,
  },
  {
    path: 'configuration',
    component: ManagementConfigurationComponent,
  },
  {
    path: 'wizard',
    children: [
      {
        path: 'facility',
        component: FacilityWizardComponent,
      }
    ]
  },
]
