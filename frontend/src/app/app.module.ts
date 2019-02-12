import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgModule } from '@angular/core';
import { LocationStrategy, HashLocationStrategy, APP_BASE_HREF } from '@angular/common';
import { ManagerAuthGuard } from './shared/guards/manager-guard.service';
import { ConstructorAuthGuard } from './shared/guards/constructor-guard.service';
import { TestExpertAuthGuard } from './shared/guards/testexpert-guard.service';
import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.module';
import { JwtModule } from '@auth0/angular-jwt';
import { TranslateModule, TranslateLoader } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { SharedModule } from './shared/shared.module';
import { LearningObjectiveModule } from './constructor/learningobjective/learningobjective.page.module';
import { ReviewModule } from './testexpert/reviewitem/reviewitem.page.module';
import { EditModule } from './constructor/edititem/edititem.page.module';
import { ViewModule } from './constructor/viewitem/viewitem.page.module';
import { DashboardCOModule } from './constructor/dashboard/dashboard.co.page.module';
import { DashboardTEModule } from './testexpert/dashboard/dashboard.te.page.module';
import { DashboardMAModule } from './manager/dashboard/dashboard.ma.page.module';
import { ItemsummaryModule } from './constructor/dashboard/itemsummary.module';
import { ToastrModule, ToastrService } from 'ngx-toastr';
import { LoginModule } from './sharedcomponents/login/login.page.module';
import { NavbarModule } from './sharedcomponents/navbar/navbar.module';
import { HttpClientModule, HttpClient, HTTP_INTERCEPTORS } from '@angular/common/http';
import { BackendUrlInterceptor } from './shared/interceptor/backed-url.interceptor';
import { ErrorInterceptor } from './shared/interceptor/error.interceptor';
import { registerLocaleData } from '@angular/common';
import localeNl from '@angular/common/locales/nl';
import localeEn from '@angular/common/locales/en';
import { environment } from '../environments/environment';

registerLocaleData(localeNl, localeEn);
// AoT requires an exported function for factories
export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(http);
}
export function GetToken() {
  return localStorage.getItem((environment.production + '_id_token_qconst'));
}

@NgModule({
  imports: [BrowserModule, AppRoutingModule, LoginModule, ReviewModule, EditModule,
    BrowserAnimationsModule, ViewModule, DashboardCOModule, DashboardTEModule, DashboardMAModule, ItemsummaryModule,
    LearningObjectiveModule, NavbarModule, ToastrModule.forRoot(),
    HttpClientModule,
    SharedModule.forRoot(),
    JwtModule.forRoot({
      config: {
        tokenGetter: GetToken,
        whitelistedDomains: ['localhost:5000']
      }
    }),
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: HttpLoaderFactory,
        deps: [HttpClient]
      }
    })],
  providers: [ManagerAuthGuard, ConstructorAuthGuard, TestExpertAuthGuard,
    { provide: APP_BASE_HREF, useValue: '/' },
    { provide: LocationStrategy, useClass: HashLocationStrategy },
    { provide: HTTP_INTERCEPTORS, useClass: BackendUrlInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true }
  ],
  declarations: [AppComponent],
  bootstrap: [AppComponent]
})

export class AppModule { }
