import { Routes } from '@angular/router';
import { AuthGuard } from '@core/guards/auth.guard';
import { ScreenComponent } from '@core/components/screen/screen.component';

export const PAGES_ROUTES: Routes = [
    {
      path: '',
      canActivate: [AuthGuard],
      component: ScreenComponent,
      children: [
        {
          path: 'dashboard',
          loadChildren: () => import('./dashboard/dashboard.module').then((m) => m.DashboardModule)
        },
        {
          path: 'user-management',
          loadChildren: () => import('./connections/connections.module').then((m) => m.ConnectionsModule)
        },
        {
          path: 'data-management',
          loadChildren: () => import('./data-manager/data-manager.module').then((m) => m.DataManagerModule)
        },
      ]
    },
  ];