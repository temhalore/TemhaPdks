import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

// PrimeNG Modülleri
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { TableModule } from 'primeng/table';
import { TooltipModule } from 'primeng/tooltip';
import { InputNumberModule } from 'primeng/inputnumber';
import { DropdownModule } from 'primeng/dropdown';
import { CalendarModule } from 'primeng/calendar';
import { CheckboxModule } from 'primeng/checkbox';
import { AccordionModule } from 'primeng/accordion';
import { PanelModule } from 'primeng/panel';
import { DividerModule } from 'primeng/divider';

// Ortak Bileşenler ve Routing
import { LogRoutingModule } from './log-routing.module';
import { LogComponent } from './log.component';
import { PdksHareketListeComponent } from './pdks-hareket/pdks-hareket.component';
import { AlarmHareketListeComponent } from './alarm-hareket/alarm-hareket.component';
import { KameraHareketListeComponent } from './kamera-hareket/kamera-hareket.component';
import { LogParserComponent } from './log-parser/log-parser.component';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    FormsModule,
    LogRoutingModule,
    ButtonModule,
    InputTextModule,
    InputTextareaModule,
    TableModule,
    TooltipModule,
    InputNumberModule,
    DropdownModule,
    CalendarModule,
    CheckboxModule,
    AccordionModule,
    PanelModule,
    DividerModule,
    LogComponent,
    PdksHareketListeComponent,
    AlarmHareketListeComponent,
    KameraHareketListeComponent,
    LogParserComponent
  ]
})
export class LogModule { }
