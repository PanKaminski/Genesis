import { Routes } from "@angular/router";
import { RouterComponent } from "@core/components/router.component";
import { FamilyTreeComponent } from "./components/family-tree/family-tree.component";
import { FamilyTreesListComponent } from "./components/family-trees-list/family-trees-list.component";
import { TreeAccessGuard } from "./guards/tree-access.guard";

export const DASHBOARD_ROUTES: Routes = [
  {
    path: '',
    component: RouterComponent,
    children: [
      {
        path: 'trees',
        component: FamilyTreesListComponent,
      },
      {
        path: 'tree/:id',
        component: FamilyTreeComponent,
        canActivate: [TreeAccessGuard],
      },    
    ]
  }
];