import { Routes } from "@angular/router";
import { RouterComponent } from "@core/components/router.component";
import { PersonsDataComponent } from "./persons-data/persons-data.component";

export const DATA_MANAGER_ROUTES: Routes = [
    {
      path: '',
      component: RouterComponent,
      children: [
        {
          path: 'persons',
          component: PersonsDataComponent,
        },
      ]
    }
  ];