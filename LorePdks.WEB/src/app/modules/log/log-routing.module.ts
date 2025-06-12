import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LogComponent } from './log.component';
import { PdksHareketListeComponent } from './pdks-hareket/pdks-hareket.component';
import { AlarmHareketListeComponent } from './alarm-hareket/alarm-hareket.component';
import { KameraHareketListeComponent } from './kamera-hareket/kamera-hareket.component';
import { LogParserComponent } from './log-parser/log-parser.component';

const routes: Routes = [
  {
    path: '',
    component: LogComponent,
    children: [
      {
        path: '',
        redirectTo: 'pdks',
        pathMatch: 'full'
      },
      {
        path: 'pdks',
        component: PdksHareketListeComponent,
        data: { title: 'PDKS Hareketleri' }
      },
      {
        path: 'alarm',
        component: AlarmHareketListeComponent,
        data: { title: 'Alarm Hareketleri' }
      },
      {
        path: 'kamera',
        component: KameraHareketListeComponent,
        data: { title: 'Kamera Hareketleri' }
      },
      {
        path: 'parser',
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
export class LogRoutingModule { }
