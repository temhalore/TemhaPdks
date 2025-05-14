import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { KodComponent } from './kod.component';
import { KodListComponent } from './kod-list/kod-list.component';

const routes: Routes = [
  {
    path: '',
    component: KodComponent,
    children: [
      {
        path: '',
        redirectTo: 'liste',
        pathMatch: 'full'
      },
      {
        path: 'liste',
        component: KodListComponent,
        data: { title: 'Kod Yönetimi' }
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class KodRoutingModule { }
