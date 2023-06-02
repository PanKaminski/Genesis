import { NgModule } from '@angular/core';
import { SharedModule } from '@shared/shared.module';
import { RouterModule } from '@angular/router';
import { CONNECTIONS_ROUTES } from './connections.routes';
import { ContactCardComponent } from './components/contact-card/contact-card.component';
import { UserManagementComponent } from './components/user-management/user-management.component';
import { UsersComponent } from './components/users/users.component';

@NgModule({
  declarations: [
    ContactCardComponent,
    UserManagementComponent,
    UsersComponent
  ],
  imports: [
    SharedModule,
    RouterModule.forChild(CONNECTIONS_ROUTES),
  ]
})
export class ConnectionsModule { }
