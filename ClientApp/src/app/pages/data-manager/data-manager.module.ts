import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { SharedModule } from '@shared/shared.module';
import { DATA_MANAGER_ROUTES } from './dat-manager.routes';
import { PersonsDataComponent } from './persons-data/persons-data.component';

@NgModule({
  declarations: [
    PersonsDataComponent
  ],
  imports: [
    SharedModule,
    RouterModule.forChild(DATA_MANAGER_ROUTES),
  ]
})
export class DataManagerModule { }
