import { NgModule } from '@angular/core';
import { DefaultSetModule } from './default.module';
import { FormComponent } from './components/form/form.component';
import { LoadingPanelComponent } from './components/loading-panel/loading-panel.component';
import { DataTableComponent } from './components/data-table/data-table.component';

@NgModule({
  declarations: [
    FormComponent,
    DataTableComponent,
    LoadingPanelComponent,
  ],
  imports: [
    DefaultSetModule,
  ],
  exports: [
    DefaultSetModule,
    FormComponent,
    DataTableComponent,
    LoadingPanelComponent,
  ],
})
export class SharedModule { }
