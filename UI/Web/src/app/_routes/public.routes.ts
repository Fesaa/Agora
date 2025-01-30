import {Routes} from '@angular/router';
import {DashboardComponent} from '../dashboard/dashboard.component';
import {CallbackComponent} from '../callback/callback.component';

export const routes: Routes = [
  {
    path: 'dashboard',
    component: DashboardComponent,
  },
  {
    path: 'callback',
    component: CallbackComponent,
  }
]
