import { Component, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { InputNumberModule } from 'primeng/inputnumber';
import { DropdownModule } from 'primeng/dropdown';

import { ConfirmDialogComponent } from '../../../core/components/confirm-dialog/confirm-dialog.component';
import { DataGridComponent, ActionButtonConfig } from '../../../core/components/data-grid/data-grid.component';
import { ModalComponent } from '../../../core/components/modal/modal.component';
import { KodService } from '../../../core/services/modules/kod.service';
import { KodDto } from '../../../core/models/KodDto';
import { finalize } from 'rxjs';

interface KodTip {
  label: string;
  value: number;
}

@Component({
  selector: 'app-kod-list',
  templateUrl: './kod-list.component.html',
  styleUrls: ['./kod-list.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ButtonModule,
    InputTextModule,
    InputTextareaModule,
    InputNumberModule,
    DropdownModule,
    DataGridComponent,
    ModalComponent,
    ConfirmDialogComponent
  ]
})
export class KodListComponent implements OnInit {
  // Kod listesi
  kodList: KodDto[] = [];
  loading: boolean = false;
  
  // İşlem butonları tanımı
  actionButtons: ActionButtonConfig[] = [
    { icon: 'pi pi-pencil', tooltip: 'Düzenle', action: 'edit' },
    { icon: 'pi pi-trash', tooltip: 'Sil', action: 'delete', class: 'p-button-danger' },
  ];
  
  // Data grid sütun tanımları
  columns = [
    { field: 'tipId', header: 'Tip ID' },
    { field: 'kod', header: 'Kod' },
    { field: 'kisaAd', header: 'Kısa Ad' },
    { field: 'sira', header: 'Sıra' }
  ];
  
  // Confirm Dialog referansı
  @ViewChild('confirmDialog') confirmDialog: ConfirmDialogComponent;
  
  // Modal durum bayrakları
  kodModalVisible: boolean = false;
  
  // Seçili kod ve form modeli
  selectedKod: KodDto | null = null;
  kodModel: KodDto = new KodDto();
  
  // Kod tipleri listesi (örnek olarak, gerçek uygulama için düzenlenebilir)
  kodTipleri: KodTip[] = [
    { label: 'Sistem Ayarları', value: 1 },
    { label: 'PDKS Durumları', value: 2 },
    { label: 'Cihaz Tipleri', value: 3 },
    { label: 'Departmanlar', value: 4 },
    { label: 'Unvanlar', value: 5 }
  ];
  
  constructor(private kodService: KodService) { }
  
  ngOnInit(): void {
    this.loadKodList();
  }
  
  /**
   * Kod listesini yükler
   */
  loadKodList(): void {
    if (this.loading) return;
    
    this.loading = true;
    // Burada gerçek veri yerine tipId'yi belirli bir değere sabitledik, kullanıcı seçimi için düzenlenebilir
    this.kodService.getKodListByTipId(1)
      .pipe(finalize(() => this.loading = false))
      .subscribe({
        next: (data) => {
          this.kodList = data;
        },
        error: (error) => {
          console.error('Kod listesi yüklenirken hata oluştu:', error);
        }
      });
  }
  
  /**
   * Kod tipine göre kodları getirir
   */
  loadKodListByTipId(tipId: number): void {
    if (this.loading) return;
    
    this.loading = true;
    this.kodService.getKodListByTipId(tipId)
      .pipe(finalize(() => this.loading = false))
      .subscribe({
        next: (data) => {
          this.kodList = data;
        },
        error: (error) => {
          console.error('Kod listesi yüklenirken hata oluştu:', error);
        }
      });
  }
  
  /**
   * Yeni kod ekleme modalını açar
   */
  openAddKodModal(): void {
    this.kodModel = new KodDto();
    this.kodModalVisible = true;
  }
  
  /**
   * Kod düzenleme modalını açar
   * @param kod Düzenlenecek kod
   */
  openEditKodModal(kod: KodDto): void {
    this.kodModel = { ...kod };
    this.kodModalVisible = true;
  }
  
  /**
   * Data Grid'den gelen işlem butonlarını yönetir
   * @param event İşlem bilgisi
   */
  onRowAction(event: { action: string, data: any }): void {
    switch (event.action) {
      case 'edit':
        this.openEditKodModal(event.data);
        break;
      case 'delete':
        this.confirmDelete(event.data);
        break;
    }
  }
  
  /**
   * Silme işlemi için onay modalını açar
   * @param kod Silinecek kod
   */  confirmDelete(kod: KodDto): void {
    this.selectedKod = kod;
    this.confirmDialog.show();
  }
  
  /**
   * Kod kaydeder
   */
  saveKod(): void {
    this.kodService.saveKod(this.kodModel)
      .subscribe({
        next: (savedKod) => {
          this.kodModalVisible = false;
          this.loadKodList();
        },
        error: (error) => {
          console.error('Kod kaydedilirken hata oluştu:', error);
        }
      });
  }
  
  /**
   * Kod siler (backend'de bir silme metodu olsaydı kullanılırdı,
   * ancak mevcut serviste silme metodu yok. saveKod içinde ISDELETED değeri
   * true yapılarak silme işlemi gerçekleştirilebilir)
   */  deleteKod(): void {
    if (!this.selectedKod) return;
    
    // Silme işlemi için model manipülasyonu (backend'de ISDELETED = true olarak işaretlenir)
    const deleteModel = { ...this.selectedKod };
    
    this.kodService.saveKod(deleteModel)
      .subscribe({
        next: () => {
          this.selectedKod = null;
          this.loadKodList();
        },
        error: (error) => {
          console.error('Kod silinirken hata oluştu:', error);
        }
      });
  }
  
  /**
   * Kod cache'ini yeniler
   */
  refreshCache(): void {
    this.kodService.refreshKodListCache()
      .subscribe({
        next: (message) => {
          console.log('Kod cache yenilendi:', message);
          this.loadKodList();
        },
        error: (error) => {
          console.error('Kod cache yenilenirken hata oluştu:', error);
        }
      });
  }
  
  /**
   * Modal kapatıldığında temizlik işlemleri
   */
  onKodModalClosed(): void {
    this.kodModel = new KodDto();
  }
}
