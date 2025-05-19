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
import { TextInputComponent } from '../../../core/components/text-input/text-input.component';
import { SelectComponent } from '../../../core/components/select/select.component';
import { SelectInputModel } from '../../../core/components/select/select-input.model';
import { BehaviorSubject } from 'rxjs';

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
    ConfirmDialogComponent,
    TextInputComponent,
    SelectComponent
  ]
})
export class KodListComponent implements OnInit {
  // Kod listesi
  kodList: KodDto[] = [];
  filteredKodList: KodDto[] = []; // Filtrelenmiş kod listesi
  loading: boolean = false;
  
  // İşlem butonları tanımı
  actionButtons: ActionButtonConfig[] = [
    { icon: 'pi pi-pencil', tooltip: 'Düzenle', action: 'edit' },
    { icon: 'pi pi-trash', tooltip: 'Sil', action: 'delete', class: 'p-button-danger' },
  ];
  // Data grid sütun tanımları
  columns = [
    { field: 'id', header: 'ID' },
    { field: 'tipId', header: 'Tip ID' },
    { field: 'kod', header: 'Kod' },
    { field: 'kisaAd', header: 'Kısa Ad' },
    { field: 'sira', header: 'Sıra' },
    { field: 'digerUygEnumAd', header: 'Enum Adı' },
    { field: 'digerUygEnumDeger', header: 'Enum Değeri' }
  ];
  
  // Confirm Dialog referansı
  @ViewChild('confirmDialog') confirmDialog: ConfirmDialogComponent;
  
  // Modal durum bayrakları
  kodModalVisible: boolean = false;
  tipModalVisible: boolean = false; // Tip ekleme modalı
  
  // Seçili kod ve form modeli
  selectedKod: KodDto | null = null;
  kodModel: KodDto = new KodDto();
  tipModel: KodDto = new KodDto();
  
  // Tip ve Kod listeleri (filtrelenmiş)
  kodTipleri: KodDto[] = []; // Tip ID'si 0 olanlar
  tipDropdownOptions: SelectInputModel[] = []; // Dropdown için format
  tipListDto$ = new BehaviorSubject<SelectInputModel[]>([]); // SelectComponent için BehaviorSubject
  
  // Seçili tip ID
  selectedTipId: number | null = null;
  
  constructor(private kodService: KodService) { }
  
  ngOnInit(): void {
    this.loadKodList();
  }
  
  /**
   * Kod listesini yükler (tüm kodları getirir)
   */  loadKodList(): void {
    if (this.loading) return;
    
    this.loading = true;
    this.kodService.getKodDtoListAll()
      .pipe(finalize(() => this.loading = false))
      .subscribe({
        next: (data) => {
          this.kodList = data;
          this.filteredKodList = data;
          this.filtreleVeHazirla(data);
          
          // Eğer daha önce bir tip filtresi seçilmişse, tekrar uygula
          if (this.selectedTipId !== null) {
            this.filterByTipId();
          }
        },
        error: (error) => {
          console.error('Kod listesi yüklenirken hata oluştu:', error);
        }
      });
  }
    /**
   * Kodları filtreler ve hazırlar
   * @param data Tüm kodlar
   */
  filtreleVeHazirla(data: KodDto[]): void {
    // Tip ID'si 0 olanları kodTipleri listesine ekle
    this.kodTipleri = data.filter(item => item.tipId === 0);
    
    // Dropdown için tipDropdownOptions listesini oluştur
    this.tipDropdownOptions = this.kodTipleri.map(tip => 
      new SelectInputModel(tip.id, `${tip.kisaAd} (${tip.id})`)
    );
    
    // Tüm seçeneği ekle
    const tumunuGosterOption = new SelectInputModel(null, 'Tümünü Göster');
    this.tipDropdownOptions.unshift(tumunuGosterOption);
    
    // BehaviorSubject'i güncelle
    this.tipListDto$.next(this.tipDropdownOptions);
    
    // Filtrelenmiş listeyi güncelle
    this.filteredKodList = [...this.kodList];
    
    console.log('Tip sayısı:', this.kodTipleri.length);
    console.log('Toplam kod sayısı:', data.length);
  }
    /**
   * Tip filtresi değiştiğinde çalışır
   */
  onTipFilterChange(tipId: any): void {
    console.log('Tip filtresi değişti:', tipId);
    this.selectedTipId = tipId;
    this.filterByTipId();
  }
  
  /**
   * Tip ID'sine göre kodları filtreler
   */
  filterByTipId(): void {
    console.log('Filtreleme yapılıyor, seçili tip:', this.selectedTipId);
    
    if (this.selectedTipId === null) {
      // Tümünü göster
      this.filteredKodList = [...this.kodList];
      console.log('Tüm kayıtlar gösteriliyor:', this.filteredKodList.length);
    } else {
      // Seçili tip ID'sine göre filtrele
      // Hem tipin kendisini hem de o tipe ait kodları göster
      this.filteredKodList = this.kodList.filter(item => 
        item.tipId === this.selectedTipId || item.id === this.selectedTipId
      );
      console.log('Filtrelenmiş kayıt sayısı:', this.filteredKodList.length);
    }
  }
    /**
   * Yeni kod ekleme modalını açar
   */
  openAddKodModal(): void {
    this.kodModel = new KodDto();
    
    // Boş değerler için varsayılan değerler ata
    this.kodModel.kod = '';
    this.kodModel.kisaAd = '';
    this.kodModel.digerUygEnumAd = '';
    this.kodModel.digerUygEnumDeger = 0;
    
    // Son eklenen kodun sırasını bul ve bir artır
    const existingCodes = this.kodList.filter(k => k.tipId !== 0);
    let maxSira = 0;
    if (existingCodes.length > 0) {
      maxSira = Math.max(...existingCodes.map(k => k.sira || 0));
    }
    
    // Yeni kodun sırasını ayarla
    this.kodModel.sira = maxSira + 1;
    
    // İlk tipi varsayılan olarak seç (eğer varsa)
    if (this.tipDropdownOptions.length > 1) { // İlk eleman "Tümünü Göster" olduğu için > 1
      this.kodModel.tipId = this.tipDropdownOptions[1].key;
    }
    
    this.kodModalVisible = true;
  }
    /**
   * Yeni tip ekleme modalını açar
   */
  openAddTipModal(): void {
    this.tipModel = new KodDto();
    
    // Boş değerler için varsayılan değerler ata
    this.tipModel.kod = '';
    this.tipModel.kisaAd = '';
    this.tipModel.digerUygEnumAd = '';
    this.tipModel.digerUygEnumDeger = 0;
    
    // Tip ID'si her zaman 0 olacak
    this.tipModel.tipId = 0;
    
    // Son eklenen tipin sırasını bul ve bir artır
    let maxSira = 0;
    if (this.kodTipleri.length > 0) {
      maxSira = Math.max(...this.kodTipleri.map(k => k.sira || 0));
    }
    
    // Yeni tipin sırasını ayarla
    this.tipModel.sira = maxSira + 1;
    this.tipModalVisible = true;
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
   */
  confirmDelete(kod: KodDto): void {
    this.selectedKod = kod;
    this.confirmDialog.show();
  }
  
  /**
   * Tip kaydeder
   */
  saveTip(): void {
    // Tip ID'si her zaman 0 olmalı
    this.tipModel.tipId = 0;
    
    this.kodService.saveKod(this.tipModel)
      .subscribe({
        next: (savedTip) => {
          this.tipModalVisible = false;
          this.loadKodList();
        },
        error: (error) => {
          console.error('Tip kaydedilirken hata oluştu:', error);
        }
      });
  }
  
  /**
   * Kod kaydeder
   */
  saveKod(): void {
    // ID formatını oluştur: TipId + Sıra (4 basamaklı)
    if (this.kodModel.tipId && this.kodModel.sira !== undefined && this.kodModel.sira !== null) {
      // Sıra numarasını 4 basamaklı formata dönüştür (ör: 5 -> 0005)
      const formattedSira = this.kodModel.sira.toString().padStart(4, '0');
      // ID oluştur: TipId + formatlanmış sıra
      this.kodModel.id = parseInt(`${this.kodModel.tipId}${formattedSira}`);
    }
    
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
   * Kod siler
   */
  deleteKod(): void {
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
  
  /**
   * Tip modalı kapatıldığında temizlik işlemleri
   */
  onTipModalClosed(): void {
    this.tipModel = new KodDto();
  }
}
