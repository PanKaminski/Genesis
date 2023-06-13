import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HeaderComponent } from './core/header/header.component';
import { CoreModule } from '@core/core.module';
import { RouterModule } from '@angular/router';
import { APP_ROUTES } from './app-routes';
import { JwtModule } from '@auth0/angular-jwt';
import { tokenGet } from '@shared/services/token-helper';
import { DefaultSetModule } from '@shared/default.module';
import { ErrorInterceptor } from '@shared/helpers/error-interseptor';

@NgModule({
    declarations: [
        AppComponent,
        HeaderComponent,
    ],
    providers: [        
      { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true },
    ],
    bootstrap: [AppComponent],
    imports: [
        CoreModule,
        HttpClientModule,
        DefaultSetModule,
        BrowserModule,
        AppRoutingModule,
        RouterModule.forRoot(APP_ROUTES),
        BrowserAnimationsModule,
        JwtModule.forRoot({
            config: {
              tokenGetter: tokenGet,
              allowedDomains: ["localhost:8181"],
              disallowedRoutes: [],
            }
          })      
    ]
})
export class AppModule { }
