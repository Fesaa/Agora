import {Routes} from '@angular/router';
import {UserDashboardComponent} from '../user/user-dashboard/user-dashboard.component';
import {MeetingWizardComponent} from '../user/meeting-wizard/meeting-wizard.component';

export const routes: Routes = [
  {
    path: 'dashboard',
    component: UserDashboardComponent,
  },
  {
    path: 'wizard',
    children: [
      {
        path: 'meeting',
        component: MeetingWizardComponent,
      },
    ]
  }
]
