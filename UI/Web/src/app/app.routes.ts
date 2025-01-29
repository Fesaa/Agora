import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'user', // TODO: Write guard
    runGuardsAndResolvers: "always",
    loadChildren: () => import('./_routes/user.routes').then(m => m.routes)
  },
  {
    path: 'management', // TODO: Write guard
    runGuardsAndResolvers: "always",
    children: [],
  },
  {
    path: '',
    loadChildren: () => import('./_routes/public.routes').then(m => m.routes),
  }
];
