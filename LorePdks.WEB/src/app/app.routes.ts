import { Routes } from '@angular/router';
import { MainLayoutComponent } from './layouts/main-layout/main-layout.component';
import { AuthGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  // Ana yönlendirme - kök URL'i doğrudan login sayfasına yönlendir
  {
    path: '',
    redirectTo: 'auth/login',
    pathMatch: 'full'
  },
  
  // Auth modülü (login sayfası)
  {
    path: 'auth',
    loadChildren: () => import('./modules/auth/auth.module').then(m => m.AuthModule)
  },
  
  // Ana layout ve korumalı sayfalar (path boş yerine özel bir path kullanıyoruz)
  {
    path: 'secure',
    component: MainLayoutComponent,
    canActivate: [AuthGuard],
    children: [
      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full'
      },
      {
        path: 'dashboard',
        loadChildren: () => import('./modules/dashboard/dashboard.module').then(m => m.DashboardModule)
      }
      // Diğer korumalı sayfalar buraya eklenebilir
    ]
  },
  
  // Hata sayfaları - tanımlanmayan tüm yolları login sayfasına yönlendir
  {
    path: '**',
    redirectTo: 'auth/login'
  }
];
