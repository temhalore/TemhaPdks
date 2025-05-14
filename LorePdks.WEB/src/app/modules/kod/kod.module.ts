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

// Modülün bileşenleri
import { KodRoutingModule } from './kod-routing.module';
import { KodComponent } from './kod.component';
import { KodListComponent } from './kod-list/kod-list.component';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    FormsModule,
    KodRoutingModule,
    ButtonModule,
    InputTextModule,
    InputTextareaModule,
    TableModule,
    TooltipModule,
    InputNumberModule,
    DropdownModule,
    KodComponent, // Standalone component
    KodListComponent // Standalone component
  ]
})
export class KodModule { }
