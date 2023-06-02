import { Routes } from "@angular/router";
import { UserManagementComponent } from "./components/user-management/user-management.component";
import { UsersComponent } from "./components/users/users.component";
import { UsersToolCode } from "./models/users-tool-code";

export const CONNECTIONS_ROUTES: Routes = [
    {
      path: '',
      component: UserManagementComponent,
      children: [
        {
          path: 'connections',
          component: UsersComponent,
          data: { toolCode: UsersToolCode.Connections }
        },
        {
          path: 'invites',
          component: UsersComponent,
          data: { toolCode: UsersToolCode.Invites }
        },    
        {
            path: 'pendings',
            component: UsersComponent,
            data: { toolCode: UsersToolCode.Pendings }
        },    
        {
            path: 'search',
            component: UsersComponent,
            data: { toolCode: UsersToolCode.Search }
        },
      ]
    }
  ];