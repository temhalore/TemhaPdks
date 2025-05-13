import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

// PrimeNG Modülleri
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { TableModule } from 'primeng/table';
import { TooltipModule } from 'primeng/tooltip';
import { CheckboxModule } from 'primeng/checkbox';

// Routing ve Bileşenler
import { KisiRoutingModule } from './kisi-routing.module';
import { KisiComponent } from './kisi.component';
import { KisiListComponent } from './kisi-list/kisi-list.component';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    FormsModule,
    KisiRoutingModule,
    ButtonModule,
    InputTextModule,
    InputTextareaModule,
    TableModule,
    TooltipModule,
    CheckboxModule,
    KisiComponent,
    KisiListComponent // Standalone bileşeni imports içine ekledik
  ]
})
export class KisiModule { }
