import { Component, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { finalize } from 'rxjs';

// PrimeNG Modülleri
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { TooltipModule } from 'primeng/tooltip';
import { CalendarModule } from 'primeng/calendar';
import { DropdownModule } from 'primeng/dropdown';
import { CheckboxModule } from 'primeng/checkbox';
import { PanelModule } from 'primeng/panel';

// Ortak Bileşenler
import { DataGridComponent, ActionButtonConfig } from '../../../core/components/data-grid/data-grid.component';
import { ModalComponent } from '../../../core/components/modal/modal.component';
import { TextInputComponent } from '../../../core/components/text-input/text-input.component';
import { ButtonComponent } from '../../../core/components/button/button.component';

// Servis ve Modeller
import { KameraHareketService } from '../../../core/services/modules/kamera-hareket.service';
import { FirmaService } from '../../../core/services/modules/firma.service';
import { KameraHareketDto } from '../../../core/models/KameraHareketDto';
import { FirmaDto } from '../../../core/models/FirmaDto';

@Component({
  selector: 'app-kamera-hareket',
  templateUrl: './kamera-hareket.component.html',
  styleUrls: ['./kamera-hareket.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ButtonModule,
    InputTextModule,
    InputTextareaModule,
    TooltipModule,
    CalendarModule,
    DropdownModule,
    CheckboxModule,
    PanelModule,
    DataGridComponent,
    ModalComponent,
    TextInputComponent,
    ButtonComponent
  ]
})
export class KameraHareketListeComponent implements OnInit {
  // Kamera hareket listesi
  kameraHareketList: KameraHareketDto[] = [];
  
  // Firma listesi (filtreleme için)
  firmaList: FirmaDto[] = [];
  
  // Filtre değişkenleri
  selectedFirma: FirmaDto | null = null;
  baslangicTarihi: Date | null = null;
  bitisTarihi: Date | null = null;
  
  // Loading durumları
  isLoading = false;
  isSaving = false;
  
  // Modal durumları
  isModalVisible = false;
  modalTitle = '';
  
  // Düzenleme için seçili öğe
  selectedKameraHareket: KameraHareketDto = {};
  
  // DataGrid konfigürasyonu
  columns = [
    { field: 'firmaAdi', header: 'Firma', sortable: true },
    { field: 'cihazNumarasi', header: 'Cihaz No', sortable: true },
    { field: 'eventTarihi', header: 'Event Tarihi', sortable: true, type: 'date' },
    { field: 'eventTipiAdi', header: 'Event Tipi', sortable: true },
    { field: 'aciklama', header: 'Açıklama', sortable: true },
    { field: 'lokasyon', header: 'Lokasyon', sortable: true },
    { field: 'kullaniciAdi', header: 'Kullanıcı', sortable: true },
    { field: 'dosyaYolu', header: 'Dosya', sortable: true },
    { field: 'isProcessed', header: 'İşlendi', sortable: true, type: 'boolean' }
  ];
  
  actionButtons: ActionButtonConfig[] = [
    {
      label: 'Düzenle',
      icon: 'pi pi-pencil',
      action: (item: KameraHareketDto) => this.editKameraHareket(item),
      styleClass: 'p-button-sm p-button-text'
    },
    {
      label: 'Sil',
      icon: 'pi pi-trash',
      action: (item: KameraHareketDto) => this.deleteKameraHareket(item),
      styleClass: 'p-button-sm p-button-text p-button-danger'
    }
  ];

  @ViewChild('dataGrid') dataGrid!: DataGridComponent;
  @ViewChild('modal') modal!: ModalComponent;

  constructor(
    private kameraHareketService: KameraHareketService,
    private firmaService: FirmaService
  ) {}

  ngOnInit(): void {
    this.loadInitialData();
  }

  /**
   * İlk yükleme verilerini getirir
   */
  loadInitialData(): void {
    this.loadFirmaList();
    this.loadKameraHareketList();
  }

  /**
   * Firma listesini yükler
   */
  loadFirmaList(): void {
    this.firmaService.getAllFirmaList().subscribe({
      next: (data) => {
        this.firmaList = data || [];
      },
      error: (error) => {
        console.error('Firma listesi yüklenirken hata oluştu:', error);
      }
    });
  }

  /**
   * Kamera hareket listesini yükler
   */
  loadKameraHareketList(): void {
    this.isLoading = true;
    
    let observable;
    
    if (this.baslangicTarihi && this.bitisTarihi) {
      observable = this.kameraHareketService.getKameraHareketListByDateRange(
        this.baslangicTarihi, 
        this.bitisTarihi
      );
    } else if (this.selectedFirma?.eid) {
      observable = this.kameraHareketService.getKameraHareketListByFirmaId(this.selectedFirma.eid);
    } else {
      observable = this.kameraHareketService.getAllKameraHareketList();
    }
    
    observable.pipe(
      finalize(() => this.isLoading = false)
    ).subscribe({
      next: (data) => {
        this.kameraHareketList = data || [];
      },
      error: (error) => {
        console.error('Kamera hareket listesi yüklenirken hata oluştu:', error);
        this.kameraHareketList = [];
      }
    });
  }

  /**
   * Filtreleri uygular
   */
  applyFilters(): void {
    this.loadKameraHareketList();
  }

  /**
   * Filtreleri temizler
   */
  clearFilters(): void {
    this.selectedFirma = null;
    this.baslangicTarihi = null;
    this.bitisTarihi = null;
    this.loadKameraHareketList();
  }

  /**
   * Yeni kamera hareket ekleme modalını açar
   */
  addNewKameraHareket(): void {
    this.selectedKameraHareket = {};
    this.modalTitle = 'Yeni Kamera Hareket';
    this.isModalVisible = true;
  }

  /**
   * Kamera hareket düzenleme modalını açar
   */
  editKameraHareket(kameraHareket: KameraHareketDto): void {
    this.selectedKameraHareket = { ...kameraHareket };
    this.modalTitle = 'Kamera Hareket Düzenle';
    this.isModalVisible = true;
  }

  /**
   * Kamera hareket kaydeder
   */
  saveKameraHareket(): void {
    if (!this.selectedKameraHareket) return;

    this.isSaving = true;
    
    this.kameraHareketService.saveKameraHareket(this.selectedKameraHareket)
      .pipe(finalize(() => this.isSaving = false))
      .subscribe({
        next: (result) => {
          this.isModalVisible = false;
          this.loadKameraHareketList();
        },
        error: (error) => {
          console.error('Kamera hareket kaydedilirken hata oluştu:', error);
        }
      });
  }

  /**
   * Kamera hareket siler
   */
  deleteKameraHareket(kameraHareket: KameraHareketDto): void {
    if (!kameraHareket.eid) return;

    if (confirm('Bu kamera hareketini silmek istediğinizden emin misiniz?')) {
      this.kameraHareketService.deleteKameraHareket(kameraHareket.eid)
        .subscribe({
          next: (success) => {
            if (success) {
              this.loadKameraHareketList();
            }
          },
          error: (error) => {
            console.error('Kamera hareket silinirken hata oluştu:', error);
          }
        });
    }
  }

  /**
   * Modalı kapatır
   */
  closeModal(): void {
    this.isModalVisible = false;
    this.selectedKameraHareket = {};
  }
}
