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

// Ortak Bileşenler ve Routing
import { FirmaRoutingModule } from './firma-routing.module';
import { FirmaComponent } from './firma.component';
import { FirmaListeComponent } from './firma/firma.component';
import { FirmaCihazComponent } from './firmacihaz/firmacihaz.component';
import { FirmaKisiComponent } from './firma-kisi/firmakisi.component';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    FormsModule,
    FirmaRoutingModule,
    ButtonModule,
    InputTextModule,
    InputTextareaModule,
    TableModule,
    TooltipModule,
    InputNumberModule,
    DropdownModule,    FirmaComponent,
    FirmaListeComponent,
    FirmaCihazComponent,
    FirmaKisiComponent
  ]
})
export class FirmaModule { }
