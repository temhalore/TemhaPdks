import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RolComponent } from './rol/rol.component';
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
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class YetkiRoutingModule { }