import { Component, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { finalize } from 'rxjs';

// PrimeNG Modülleri
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { TooltipModule } from 'primeng/tooltip';
import { PaginatorModule } from 'primeng/paginator';
import { InputNumberModule } from 'primeng/inputnumber';

// Ortak Bileşenler
import { ConfirmDialogComponent } from '../../../core/components/confirm-dialog/confirm-dialog.component';
import { DataGridComponent, ActionButtonConfig } from '../../../core/components/data-grid/data-grid.component';
import { ModalComponent } from '../../../core/components/modal/modal.component';

// Servis ve Modeller
import { FirmaService } from '../../../core/services/modules/firma.service';
import { FirmaDto } from '../../../core/models/FirmaDto';

@Component({
  selector: 'app-firma',
  templateUrl: './firma.component.html',
  styleUrls: ['./firma.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ButtonModule,
    InputTextModule,
    InputTextareaModule,
    TooltipModule,
    PaginatorModule,
    InputNumberModule,
    DataGridComponent,
    ModalComponent,
    ConfirmDialogComponent
  ]
})
export class FirmaListeComponent implements OnInit {
  // Firma listesi
  firmaList: FirmaDto[] = [];
  loading: boolean = false;

  // Seçilen firma
  selectedFirma: FirmaDto | null = null;
  firmaModel: FirmaDto = {} as FirmaDto;

  // Modal kontrolleri
  firmaModalVisible: boolean = false;
  deletingFirmaId: string = '';

  // ViewChild referansı
  @ViewChild(ConfirmDialogComponent) confirmDialog!: ConfirmDialogComponent;

  // DataGrid kolonları
  columns: any[] = [
    { field: 'ad', header: 'Firma Adı' },
    { field: 'kod', header: 'Firma Kodu' },
    { field: 'adres', header: 'Adres' },
    { field: 'aciklama', header: 'Açıklama' }
  ];
  // DataGrid aksiyon butonları
  actionButtons: ActionButtonConfig[] = [
    {
      icon: 'pi pi-pencil',
      tooltip: 'Düzenle',
      action: 'edit'
    },
    {
      icon: 'pi pi-trash',
      tooltip: 'Sil',
      action: 'delete',
      class: 'p-button-danger'
    }
  ];

  constructor(private firmaService: FirmaService) { }

  ngOnInit(): void {
    this.loadFirmaList();
  }

  /**
   * Firma listesini yükler
   */
  loadFirmaList(): void {
    this.loading = true;
    this.firmaService.getAllFirmaList()
      .pipe(finalize(() => this.loading = false))
      .subscribe({
        next: (data) => {
          this.firmaList = data;
        },
        error: (err) => {
          console.error('Firma listesi yüklenirken hata oluştu', err);
        }
      });
  }

  /**
   * Yeni firma eklemek için modal açar
   */
  openAddFirmaModal(): void {
    this.firmaModel = {} as FirmaDto;
    this.firmaModalVisible = true;
  }

  /**
   * Firma düzenlemek için modal açar
   * @param firma Düzenlenecek firma
   */
  openEditFirmaModal(firma: FirmaDto): void {
    this.firmaModel = { ...firma };
    this.firmaModalVisible = true;
  }

  /**
   * DataGrid satır aksiyonlarını yönetir
   * @param event Aksiyon olayı
   */
  onRowAction(event: { action: string; data: any }): void {
    switch (event.action) {
      case 'edit':
        this.openEditFirmaModal(event.data);
        break;
      case 'delete':
        this.confirmDeleteFirma(event.data);
        break;
    }
  }
  /**
   * Firma silme işlemini onaylatmak için dialog açar
   * @param firma Silinecek firma
   */
  confirmDeleteFirma(firma: FirmaDto): void {
    this.deletingFirmaId = firma.eid;
    
    // Onay dialogunu göster
    this.confirmDialog.header = 'Firma Sil';
    this.confirmDialog.message = 'Bu firmayı silmek istediğinize emin misiniz?';
    this.confirmDialog.show();
  }

  /**
   * Firma siler
   */
  deleteFirma(): void {
    if (!this.deletingFirmaId) return;

    this.loading = true;
    this.firmaService.deleteFirma(this.deletingFirmaId)
      .pipe(finalize(() => {
        this.loading = false;
      }))
      .subscribe({
        next: () => {
          this.loadFirmaList();
        },
        error: (err) => {
          console.error('Firma silinirken hata oluştu', err);
        }
      });
  }

  /**
   * Firma kaydeder veya günceller
   */
  saveFirma(): void {
    this.loading = true;
    this.firmaService.saveFirma(this.firmaModel)
      .pipe(finalize(() => {
        this.loading = false;
        this.firmaModalVisible = false;
      }))
      .subscribe({
        next: () => {
          this.loadFirmaList();
        },
        error: (err) => {
          console.error('Firma kaydedilirken hata oluştu', err);
        }
      });
  }

  /**
   * Firma modalı kapandığında çağrılır
   */
  onFirmaModalClosed(): void {
    this.firmaModel = {} as FirmaDto;
  }
}
