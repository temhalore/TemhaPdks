import { Component, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { finalize, BehaviorSubject } from 'rxjs';

// PrimeNG Modülleri
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { TooltipModule } from 'primeng/tooltip';
import { PaginatorModule } from 'primeng/paginator';
import { DropdownModule } from 'primeng/dropdown';

// Ortak Bileşenler
import { ConfirmDialogComponent } from '../../../core/components/confirm-dialog/confirm-dialog.component';
import { DataGridComponent, ActionButtonConfig } from '../../../core/components/data-grid/data-grid.component';
import { ModalComponent } from '../../../core/components/modal/modal.component';
import { TextInputComponent } from '../../../core/components/text-input/text-input.component';
import { ButtonComponent } from '../../../core/components/button/button.component';
import { SelectComponent } from '../../../core/components/select/select.component';
import { SelectInputModel } from '../../../core/components/select/select-input.model';

// Servis ve Modeller
import { FirmaCihazService } from '../../../core/services/modules/firmacihaz.service';
import { FirmaService } from '../../../core/services/modules/firma.service';
import { KodService } from '../../../core/services/modules/kod.service';
import { FirmaCihazDto } from '../../../core/models/FirmaCihazDto';
import { FirmaDto } from '../../../core/models/FirmaDto';
import { KodDto } from '../../../core/models/KodDto';

@Component({
  selector: 'app-firmacihaz',
  templateUrl: './firmacihaz.component.html',
  styleUrls: ['./firmacihaz.component.scss'],
  standalone: true,  imports: [
    CommonModule,
    FormsModule,
    ButtonModule,
    InputTextModule,
    InputTextareaModule,
    TooltipModule,
    PaginatorModule,
    DropdownModule,
    DataGridComponent,
    ModalComponent,
    ConfirmDialogComponent,
    TextInputComponent,
    ButtonComponent,
    SelectComponent
  ]
})
export class FirmaCihazComponent implements OnInit {  // Firma cihaz listesi
  firmaCihazList: FirmaCihazDto[] = [];
  loading: boolean = false;
  firmaList: FirmaDto[] = [];
  cihazTipleri: KodDto[] = [];
  
  // Select component için BehaviorSubject'ler
  firmaListDto$ = new BehaviorSubject<SelectInputModel[]>([]);
  cihazTipleriListDto$ = new BehaviorSubject<SelectInputModel[]>([]);

  // Seçilen firma cihaz
  selectedFirmaCihaz: FirmaCihazDto | null = null;
  firmaCihazModel: FirmaCihazDto = {} as FirmaCihazDto;

  // Seçilen firma
  selectedFirma: FirmaDto | null = null;
  // Modal kontrolleri
  firmaCihazModalVisible: boolean = false;
  deletingFirmaCihazId: string = '';

  // DataGrid kolonları
  columns: any[] = [
    { field: 'ad', header: 'Cihaz Adı' },
    { field: 'firmaDto.ad', header: 'Firma Adı' },
    { field: 'firmaCihazTipKodDto.kisaAd', header: 'Cihaz Tipi' },
    { field: 'cihazMakineGercekId', header: 'Cihaz ID' },
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

  // ViewChild referansı
  @ViewChild(ConfirmDialogComponent) confirmDialog!: ConfirmDialogComponent;
  constructor(
    private firmaCihazService: FirmaCihazService,
    private firmaService: FirmaService,
    private kodService: KodService
  ) { }  ngOnInit(): void {
    // Önce cihaz tiplerini yükleyelim
    this.loadCihazTipleri();
    
    // Sonra diğer verileri yükleyelim
    this.loadFirmaList();
    this.loadFirmaCihazList();
  }
  /**
   * Cihaz tiplerini veritabanından yükler (tipId = 101 olan kodları getirir)
   */
  loadCihazTipleri(): void {
    const CIHAZ_TIP_ID = 101;
    console.log('Cihaz tipleri yükleniyor...');
    
    this.kodService.getKodListByTipId(CIHAZ_TIP_ID)
      .subscribe({
        next: (data) => {
          console.log('Cihaz tipleri başarıyla yüklendi, eleman sayısı:', data.length);
          this.cihazTipleri = data;
          
          // Yüklenen verileri kontrol et
          if (data && data.length > 0) {
            console.log('Örnek cihaz tipi veri yapısı:', JSON.stringify(data[0]));
          }
          
          // Cihaz tipleri yüklendikten sonra SelectInputModel'leri güncelle
          setTimeout(() => {
            // Verilerin Angular değişim döngüsünde işlenmesi için setTimeout kullanıyoruz
            this.prepareSelectData();
          });
        },
        error: (err) => {
          console.error('Cihaz tipleri yüklenirken hata oluştu', err);
          // Hata durumunda varsayılan değerleri kullan
          this.cihazTipleri = [
            { id: 1, tipId: CIHAZ_TIP_ID, kod: 'KARTLI', kisaAd: 'Kartlı Geçiş', sira: 1, digerUygEnumAd: '', digerUygEnumDeger: 0 },
            { id: 2, tipId: CIHAZ_TIP_ID, kod: 'PARMAK', kisaAd: 'Parmak İzi', sira: 2, digerUygEnumAd: '', digerUygEnumDeger: 0 },
            { id: 3, tipId: CIHAZ_TIP_ID, kod: 'YUZ', kisaAd: 'Yüz Tanıma', sira: 3, digerUygEnumAd: '', digerUygEnumDeger: 0 }
          ];
          this.prepareSelectData();
        }
      });
  }

  /**
   * Firma listesini yükler
   */  loadFirmaList(): void {
    this.firmaService.getAllFirmaList()
      .subscribe({
        next: (data) => {
          this.firmaList = data;
          // Firma listesi yüklendikten sonra SelectInputModel'leri güncelle
          this.prepareSelectData();
        },
        error: (err) => {
          console.error('Firma listesi yüklenirken hata oluştu', err);
        }
      });
  }

  /**
   * Firma cihaz listesini yükler
   */
  loadFirmaCihazList(): void {
    this.loading = true;
    
    // Spesifik firma seçilmişse sadece o firmaya ait cihazları getir
    if (this.selectedFirma && this.selectedFirma.eid) {
      this.firmaCihazService.getFirmaCihazListByFirmaId(this.selectedFirma.eid)
        .pipe(finalize(() => this.loading = false))
        .subscribe({
          next: (data) => {
            this.firmaCihazList = data;
          },
          error: (err) => {
            console.error('Firma cihaz listesi yüklenirken hata oluştu', err);
          }
        });
    } else {
      // Tüm firmaların cihazlarını getir (burada tüm firmalar için tekrar tekrar istek yapılmalı)
      // Şimdilik sadece örnek olması için boş liste dönüyoruz
      this.loading = false;
      this.firmaCihazList = [];
      
      // Burası gerçek implementasyonda firmaları dönüp her biri için cihazları çekecektir
      if (this.firmaList.length > 0) {
        this.firmaList.forEach(firma => {
          if (firma.eid) {
            this.firmaCihazService.getFirmaCihazListByFirmaId(firma.eid)
              .subscribe({
                next: (data) => {
                  this.firmaCihazList = [...this.firmaCihazList, ...data];
                },
                error: (err) => {
                  console.error(`${firma.ad} firması için cihazlar yüklenirken hata oluştu`, err);
                }
              });
          }
        });
      }
    }
  }  /**
   * Yeni firma cihaz eklemek için modal açar
   */
  openAddFirmaCihazModal(): void {
    this.firmaCihazModel = {
      ad: '',
      aciklama: '',
      cihazMakineGercekId: null,
      firmaDto: null,
      firmaCihazTipKodDto: null
    } as FirmaCihazDto;
    
    // Eğer bir firma seçiliyse, yeni cihazı o firmaya atamak için
    if (this.selectedFirma) {
      this.firmaCihazModel.firmaDto = this.selectedFirma;
    }
    
    // Eğer cihaz tipleri henüz yüklenmemişse, tekrar yükle
    if (!this.cihazTipleri || this.cihazTipleri.length === 0) {
      this.loadCihazTipleri();
    } else {
      // Select için veri hazırlığını garanti edelim
      this.prepareSelectData();
    }
    
    this.firmaCihazModalVisible = true;
  }

  /**
   * Firma cihaz düzenlemek için modal açar
   * @param firmaCihaz Düzenlenecek firma cihaz
   */
  openEditFirmaCihazModal(firmaCihaz: FirmaCihazDto): void {
    this.firmaCihazModel = { ...firmaCihaz };
    this.firmaCihazModalVisible = true;
  }

  /**
   * DataGrid satır aksiyonlarını yönetir
   * @param event Aksiyon olayı
   */
  onRowAction(event: { action: string; data: any }): void {
    switch (event.action) {
      case 'edit':
        this.openEditFirmaCihazModal(event.data);
        break;
      case 'delete':
        this.confirmDeleteFirmaCihaz(event.data);
        break;
    }
  }
  /**
   * Firma cihaz silme işlemini onaylatmak için dialog açar
   * @param firmaCihaz Silinecek firma cihaz
   */
  confirmDeleteFirmaCihaz(firmaCihaz: FirmaCihazDto): void {
    this.deletingFirmaCihazId = firmaCihaz.eid;
    
    // Onay dialogunu göster
    this.confirmDialog.header = 'Cihaz Sil';
    this.confirmDialog.message = 'Bu cihazı silmek istediğinize emin misiniz?';
    this.confirmDialog.show();
  }

  /**
   * Firma cihaz siler
   */
  deleteFirmaCihaz(): void {
    if (!this.deletingFirmaCihazId) return;

    this.loading = true;
    this.firmaCihazService.deleteFirmaCihaz(this.deletingFirmaCihazId)
      .pipe(finalize(() => {
        this.loading = false;
      }))
      .subscribe({
        next: () => {
          this.loadFirmaCihazList();
        },
        error: (err) => {
          console.error('Firma cihaz silinirken hata oluştu', err);
        }
      });
  }

  /**
   * Firma cihaz kaydeder veya günceller
   */
  saveFirmaCihaz(): void {
    this.loading = true;
    this.firmaCihazService.saveFirmaCihaz(this.firmaCihazModel)
      .pipe(finalize(() => {
        this.loading = false;
        this.firmaCihazModalVisible = false;
      }))
      .subscribe({
        next: () => {
          this.loadFirmaCihazList();
        },
        error: (err) => {
          console.error('Firma cihaz kaydedilirken hata oluştu', err);
        }
      });
  }

  /**
   * Firma değiştiğinde çağrılır
   */  onFirmaChange(firmaId: string): void {
    // firmaId kullanarak firma nesnesini bul
    this.selectedFirma = this.firmaList.find(firma => firma.eid === firmaId) || null;
    this.loadFirmaCihazList();
  }

  /**
   * Modal içindeki firma seçimi değiştiğinde çağrılır
   */
  onModalFirmaChange(firmaId: string): void {
    const selectedFirma = this.firmaList.find(firma => firma.eid === firmaId);
    if (selectedFirma) {
      this.firmaCihazModel.firmaDto = selectedFirma;
    }
  }  /**
   * Modal içindeki cihaz tipi seçimi değiştiğinde çağrılır
   */
  onModalCihazTipChange(cihazTipId: number | null): void {
    console.log('onModalCihazTipChange çağrıldı, cihazTipId:', cihazTipId);
    if (cihazTipId === null) return;
    
    // Cihaz tiplerinin dolu olduğunu kontrol et
    if (!this.cihazTipleri || this.cihazTipleri.length === 0) {
      console.warn('Cihaz tipleri henüz yüklenmedi!');
      return;
    }
    
    // Mevcut tüm tip ID'lerini logla
    console.log('Mevcut cihaz tipleri ID listesi:', this.cihazTipleri.map(tip => tip.id));
    
    // Seçilen tip nesnesini bul
    const selectedTip = this.cihazTipleri.find(tip => tip.id === cihazTipId);
    if (selectedTip) {
      this.firmaCihazModel.firmaCihazTipKodDto = selectedTip;
      console.log('Seçilen cihaz tipi:', selectedTip);
    } else {
      console.warn(`ID: ${cihazTipId} olan cihaz tipi bulunamadı`);
      // Eşleştirme olmadığı için tam tipleri göster
      console.log('Tüm cihaz tipleri:', JSON.stringify(this.cihazTipleri));
    }
  }

  /**
   * Sayısal değerleri işlemek için yardımcı metot
   */
  parseFloat(value: string): number {
    return value ? parseFloat(value) : 0;
  }

  /**
   * Firma cihaz modalı kapandığında çağrılır
   */
  onFirmaCihazModalClosed(): void {
    this.firmaCihazModel = {} as FirmaCihazDto;
  }  /**
   * Select bileşenleri için veri hazırlar
   */  prepareSelectData(): void {
    // Firma listesi için SelectInputModel nesneleri oluştur
    if (this.firmaList && this.firmaList.length > 0) {
      const firmaSelectItems = this.firmaList.map(firma => 
        new SelectInputModel(firma.eid, firma.ad)
      );
      this.firmaListDto$.next(firmaSelectItems);
    }
    
    // Cihaz tipleri için SelectInputModel nesneleri oluştur
    if (this.cihazTipleri && this.cihazTipleri.length > 0) {
      // KodListComponent'teki yapı gibi daha açıklayıcı bir şekilde göster
      const cihazTipSelectItems = this.cihazTipleri.map(tip => 
        new SelectInputModel(tip.id, `${tip.kisaAd} (${tip.kod || ''})`));
        
      console.log('Cihaz tipleri SelectInputModel:', cihazTipSelectItems);
      // BehaviorSubject güncelleniyor
      this.cihazTipleriListDto$.next(cihazTipSelectItems);
      
      // Değişiklikleri hemen görebilmek için log
      console.log('Cihaz tipleri listesi güncellendi, eleman sayısı:', cihazTipSelectItems.length);
    } else {
      console.warn('Cihaz tipleri henüz yüklenmedi veya boş!');
    }
  }
}
