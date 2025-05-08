import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { YetkiRoutingModule } from './yetki-routing.module';
import { YetkiComponent } from './yetki.component';
import { RolComponent } from './rol/rol.component';

// PrimeNG Modülleri
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { TableModule } from 'primeng/table';
import { TooltipModule } from 'primeng/tooltip';
import { TreeModule } from 'primeng/tree';
import { CheckboxModule } from 'primeng/checkbox';

// Ortak Bileşenler
import { DataGridComponent } from '../../core/components/data-grid/data-grid.component';
import { ModalComponent } from '../../core/components/modal/modal.component';
import { ConfirmDialogComponent } from '../../core/components/confirm-dialog/confirm-dialog.component';

@NgModule({
  declarations: [
    YetkiComponent,
    RolComponent,
    DataGridComponent,
    ModalComponent,
    ConfirmDialogComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    YetkiRoutingModule,
    ButtonModule,
    InputTextModule,
    InputTextareaModule,
    TableModule,
    TooltipModule,
    TreeModule,
    CheckboxModule
  ]
})
export class YetkiModule { }