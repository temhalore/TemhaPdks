import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { KisiComponent } from './kisi.component';
import { KisiListComponent } from './kisi-list/kisi-list.component';

const routes: Routes = [
  {
    path: '',
    component: KisiComponent,
    children: [
      {
        path: '',
        redirectTo: 'liste',
        pathMatch: 'full'
      },
      {
        path: 'liste',
        component: KisiListComponent,
        data: { title: 'Kişi Yönetimi' }
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class KisiRoutingModule { }
