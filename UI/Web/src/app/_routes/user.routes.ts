import {Routes} from '@angular/router';
import {UserDashboardComponent} from '../user/user-dashboard/user-dashboard.component';
import { RoomWizardComponent } from '../room-wizard/room-wizard.component';

export const routes: Routes = [
  {
    path: 'dashboard',
    component: UserDashboardComponent,
  },

  {
    path: 'wizard',
    component: RoomWizardComponent,
  }

  
]
