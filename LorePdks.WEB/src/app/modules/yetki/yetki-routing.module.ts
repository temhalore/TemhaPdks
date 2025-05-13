import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RolComponent } from './rol/rol.component';
import { EkranComponent } from './ekran/ekran.component';
import { YetkiComponent } from './yetki.component';

const routes: Routes = [
  {
    path: '',
    component: YetkiComponent,
    children: [
      {
        path: '',
        redirectTo: 'rol',
        pathMatch: 'full'
      },
      {
        path: 'rol',
        component: RolComponent,
        data: { title: 'Rol Yönetimi' }
      },
      {
        path: 'ekran',
        component: EkranComponent,
        data: { title: 'Ekran Yönetimi' }
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class YetkiRoutingModule { }