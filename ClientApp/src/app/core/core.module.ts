import { NgModule } from "@angular/core";
import { RouterModule } from "@angular/router";
import { DefaultSetModule } from "@shared/default.module";
import { RouterComponent } from "./components/router.component";
import { ScreenComponent } from "./components/screen/screen.component";
import { PageNotFoundComponent } from './components/page-not-found/page-not-found.component';
import { MenuComponent } from './components/menu/menu.component';
import { HeaderComponent } from './components/header/header.component';

@NgModule({
    declarations: [
      ScreenComponent,
      RouterComponent,
      PageNotFoundComponent,
      MenuComponent,
      HeaderComponent,
    ],
    imports: [
      RouterModule,
      DefaultSetModule,
    ],
    exports: [
      RouterComponent,
      PageNotFoundComponent,
    ]
  })
export class CoreModule { }
  