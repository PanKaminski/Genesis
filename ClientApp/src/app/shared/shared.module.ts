import { NgModule } from '@angular/core';
import { DefaultSetModule } from './default.module';
import { FormComponent } from './components/form/form.component';
import { LoadingPanelComponent } from './components/loading-panel/loading-panel.component';

@NgModule({
  declarations: [
    FormComponent,
    LoadingPanelComponent,
  ],
  imports: [
    DefaultSetModule,
  ],
  exports: [
    DefaultSetModule,
    FormComponent,
    LoadingPanelComponent,
  ],
})
export class SharedModule { }
