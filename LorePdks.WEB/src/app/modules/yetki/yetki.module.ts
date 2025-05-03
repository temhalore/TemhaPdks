import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';

import { YetkiRoutingModule } from './yetki-routing.module';
import { EkranListComponent } from './components/ekran/ekran-list/ekran-list.component';
import { EkranModalComponent } from './components/ekran/ekran-modal/ekran-modal.component';

// PrimeNG Modülleri
import { TableModule } from 'primeng/table';
import { TreeTableModule } from 'primeng/treetable';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { CheckboxModule } from 'primeng/checkbox';
import { DropdownModule } from 'primeng/dropdown';
import { TooltipModule } from 'primeng/tooltip';
import { TreeModule } from 'primeng/tree';
import { CardModule } from 'primeng/card';

@NgModule({
  declarations: [
    EkranListComponent,
    EkranModalComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    YetkiRoutingModule,
    // PrimeNG Modülleri
    TableModule,
    TreeTableModule,
    ButtonModule,
    DialogModule,
    InputTextModule,
    CheckboxModule,
    DropdownModule,
    TooltipModule,
    TreeModule,
    CardModule
  ]
})
export class YetkiModule { }