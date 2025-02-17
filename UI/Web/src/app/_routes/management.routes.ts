import {Routes} from '@angular/router';
import {ManagementDashboardComponent} from '../management/management-dashboard/management-dashboard.component';
import {
  ManagementConfigurationComponent
} from '../management/management-configuration/management-configuration.component';

export const routes: Routes = [
  {
    path: 'dashboard',
    component: ManagementDashboardComponent,
  },
  {
    path: 'configuration',
    component: ManagementConfigurationComponent,
  }
]
