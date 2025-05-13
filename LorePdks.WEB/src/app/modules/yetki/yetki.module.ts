import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

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
import { YetkiRoutingModule } from './yetki-routing.module';
import { YetkiComponent } from './yetki.component';
import { EkranComponent } from './ekran/ekran.component';
import { RolComponent } from './rol/rol.component';

@NgModule({
  declarations: [],
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
    CheckboxModule,
    YetkiComponent,
    EkranComponent // Standalone bileşeni imports içine ekledik
  ]
})
export class YetkiModule { }