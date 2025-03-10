import {Routes} from '@angular/router';
import {AuthGuard} from './_guard/auth.guard';

export const routes: Routes = [
  {
    path: 'user',
    canActivate: [AuthGuard],
    runGuardsAndResolvers: "always",
    loadChildren: () => import('./_routes/user.routes').then(m => m.routes)
  },
  {
    path: 'management',
    canActivate: [AuthGuard],
    runGuardsAndResolvers: "always",
    loadChildren: () => import('./_routes/management.routes').then(m => m.routes)
  },
  {
    path: '',
    loadChildren: () => import('./_routes/public.routes').then(m => m.routes),
  }
];
