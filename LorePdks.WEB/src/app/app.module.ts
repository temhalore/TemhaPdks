import { NgModule, APP_INITIALIZER } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule } from '@angular/common/http';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { CoreModule } from './core/core.module';
import { SharedModule } from './shared/shared.module';
import { LayoutsModule } from './layouts/layouts.module';
import { AuthService } from './modules/auth/services/auth.service';

// Auth service initialize fonksiyonu
function appInitializer(authService: AuthService) {
  return () => {
    return new Promise<void>((resolve) => {
      authService.getUserFromLocalStorage().subscribe({
        next: () => {
          resolve();
        },
        error: () => {
          resolve();
        }
      });
    });
  };
}

@NgModule({
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    HttpClientModule,
    AppRoutingModule,
    CoreModule,
    SharedModule,
    LayoutsModule,
    AppComponent
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