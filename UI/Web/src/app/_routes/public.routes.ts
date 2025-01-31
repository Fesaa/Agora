import {Routes} from '@angular/router';
import {DashboardComponent} from '../dashboard/dashboard.component';
import {CallbackComponent} from '../callback/callback.component';
import {FirstSetupComponent} from '../first-setup/first-setup.component';

export const routes: Routes = [
  {
    path: 'dashboard',
    component: DashboardComponent,
  },
  {
    path: 'callback',
    component: CallbackComponent,
  },
  {
    path: 'first-setup',
    component: FirstSetupComponent,
  }
]
