import { NgModule, APP_INITIALIZER } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule } from '@angular/common/http';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { CoreModule } from './core/core.module';
import { SharedModule } from './shared/shared.module';
import { AuthService } from './core/services/auth.service';
import { LayoutsModule } from './layouts/layouts.module';

// Auth service initialize fonksiyonu
function appInitializer(authService: AuthService) {
  return () => {
    return new Promise((resolve) => {
      authService.getUserFromLocalStorage().subscribe().add(resolve);
    });
  };
}

@NgModule({
  declarations: [
    AppComponent,
    // Diğer global bileşenler buraya eklenebilir
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    HttpClientModule,
    AppRoutingModule,
    CoreModule,
    SharedModule,
    LayoutsModule
    // Diğer modüller burada import edilebilir
  ],
  providers: [
    {
      provide: APP_INITIALIZER,
      useFactory: appInitializer,
      multi: true,
      deps: [AuthService],
    },
    // Diğer global servisler burada tanımlanabilir
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }