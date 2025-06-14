import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { FirmaComponent } from './firma.component';
import { FirmaListeComponent } from './firma/firma.component';
import { FirmaCihazComponent } from './firmacihaz/firmacihaz.component';
import { FirmaKisiComponent } from './firma-kisi/firmakisi.component';
import { LogParserComponent } from './log-parser/log-parser.component';

const routes: Routes = [
  {
    path: '',
    component: FirmaComponent,
    children: [
      {
        path: '',
        redirectTo: 'liste',
        pathMatch: 'full'
      },
      {
        path: 'liste',
        component: FirmaListeComponent,
        data: { title: 'Firma Yönetimi' }
      },      {
        path: 'cihaz',
        component: FirmaCihazComponent,
        data: { title: 'Firma Cihaz Yönetimi' }
      },      {
        path: 'kisi',
        component: FirmaKisiComponent,
        data: { title: 'Firma Kişi Yönetimi' }
      },
      {
        path: 'log-parser/:eid',
        component: LogParserComponent,
        data: { title: 'Log Parser Ayarları' }
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class FirmaRoutingModule { }
