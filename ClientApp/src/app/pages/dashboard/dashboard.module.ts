import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { DASHBOARD_ROUTES } from './dashboard.routes';
import { FamilyTreeComponent } from './components/family-tree/family-tree.component';
import { FamilyTreesListComponent } from './components/family-trees-list/family-trees-list.component';
import { TreeCardComponent } from './components/tree-card/tree-card.component';
import { PersonSideFormComponent } from './components/person-side-form/person-side-form.component';
import { SharedModule } from '@shared/shared.module';
import { FamilyTreeSideFormComponent } from './components/family-tree-side-form/family-tree-side-form.component';

@NgModule({
  declarations: [
    FamilyTreeComponent,
    FamilyTreesListComponent,
    TreeCardComponent,
    PersonSideFormComponent,
    FamilyTreeSideFormComponent
  ],
  imports: [
    SharedModule,
    RouterModule.forChild(DASHBOARD_ROUTES),
  ]
})
export class DashboardModule { }
