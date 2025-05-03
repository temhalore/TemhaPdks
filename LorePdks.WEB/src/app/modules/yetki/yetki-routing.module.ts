import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { EkranListComponent } from './components/ekran/ekran-list/ekran-list.component';

const routes: Routes = [
  {
    path: '',
    redirectTo: 'ekran',
    pathMatch: 'full'
  },
  {
    path: 'ekran',
    component: EkranListComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class YetkiRoutingModule { }