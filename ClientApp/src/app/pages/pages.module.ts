import { NgModule } from "@angular/core";
import { RouterModule } from "@angular/router";
import { PAGES_ROUTES } from "./pages.routes";

@NgModule({
    imports: [
      RouterModule.forChild(PAGES_ROUTES)
    ]
  })
  export class PagesModule { }  