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
import { InputSwitchModule } from 'primeng/inputswitch';

// Ortak Bileşenler
import { ConfirmDialogComponent } from '../../../core/components/confirm-dialog/confirm-dialog.component';
import { DataGridComponent, ActionButtonConfig } from '../../../core/components/data-grid/data-grid.component';
import { ModalComponent } from '../../../core/components/modal/modal.component';
import { TextInputComponent } from '../../../core/components/text-input/text-input.component';
import { ButtonComponent } from '../../../core/components/button/button.component';
import { SelectComponent } from '../../../core/components/select/select.component';
import { SelectInputModel } from '../../../core/components/select/select-input.model';
import { AutoCompleteComponent } from '../../../core/components/auto-complete';

// Servis ve Modeller
import { FirmaKisiService } from '../../../core/services/modules/firmaKisi.service';
import { FirmaService } from '../../../core/services/modules/firma.service';
import { KisiService } from '../../../core/services/modules/kisi.service';
import { KodService } from '../../../core/services/modules/kod.service';
import { FirmaKisiDto } from '../../../core/models/FirmaKisiDto';
import { FirmaDto } from '../../../core/models/FirmaDto';
import { KisiDto } from '../../../core/models/KisiDto';
import { KodDto } from '../../../core/models/KodDto';

@Component({
  selector: 'app-firmakisi',
  templateUrl: './firmakisi.component.html',
  styleUrls: ['./firmakisi.component.scss'],
  standalone: true,  imports: [
    CommonModule,
    FormsModule,
    ButtonModule,
    InputTextModule,
    InputTextareaModule,
    TooltipModule,
    PaginatorModule,
    DropdownModule,
    InputSwitchModule,
    DataGridComponent,
    ModalComponent,
    ConfirmDialogComponent,
    TextInputComponent,
    ButtonComponent,
    AutoCompleteComponent,
    SelectComponent
  ]
})
export class FirmaKisiComponent implements OnInit {
  // Firma kişi listesi
  firmaKisiList: FirmaKisiDto[] = [];
  loading: boolean = false;
  firmaList: FirmaDto[] = [];
  kisiTipleri: KodDto[] = [];
  
  // Select component için BehaviorSubject'ler
  firmaListDto$ = new BehaviorSubject<SelectInputModel[]>([]);
  kisiTipleriListDto$ = new BehaviorSubject<SelectInputModel[]>([]);

  // Seçilen firma kişi
  selectedFirmaKisi: FirmaKisiDto | null = null;
  firmaKisiModel: FirmaKisiDto = {} as FirmaKisiDto;

  // Seçilen firma
  selectedFirma: FirmaDto | null = null;
  
  // Modal kontrolleri
  firmaKisiModalVisible: boolean = false;
  deletingFirmaKisiId: string = '';
  
  // Yeni kişi eklendiğinde kullanılacak flag
  isNewKisi: boolean = false;

  // Kişi arama için gerekli değişkenler
  isSelectExistingPerson: boolean = false;
  filteredKisiList: KisiDto[] = [];
  selectedSearchKisi: KisiDto | null = null;
    // DataGrid kolonları
  columns: any[] = [
    { field: 'kisiDto.ad', header: 'Ad' },
    { field: 'kisiDto.soyad', header: 'Soyad' },
    { field: 'kisiDto.tc', header: 'TC Kimlik No' },
    { field: 'firmaDto.ad', header: 'Firma Adı' },
    { field: 'firmaKisiTipKodDto.kod', header: 'Kişi Tipi' }
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
    private firmaKisiService: FirmaKisiService,
    private firmaService: FirmaService,
    private kisiService: KisiService,
    private kodService: KodService
  ) { }

  ngOnInit(): void {
    // Önce kişi tiplerini yükleyelim
    this.loadKisiTipleri();
    
    // Sonra diğer verileri yükleyelim
    this.loadFirmaList();
    this.loadFirmaKisiList();
  }

  /**
   * Kişi tiplerini veritabanından yükler (tipId = 102 olan kodları getirir - ÖRNEK ID!)
   */
  loadKisiTipleri(): void {
    const KISI_TIP_ID = 102; // Örnek ID, gerçek ID'yi kontrol edin
    this.kodService.getKodListByTipId(KISI_TIP_ID)
      .subscribe({
        next: (data) => {
          this.kisiTipleri = data;
          // Kişi tipleri yüklendikten sonra SelectInputModel'leri güncelle
          this.prepareSelectData();
        },
        error: (err) => {
          console.error('Kişi tipleri yüklenirken hata oluştu', err);
          // Hata durumunda varsayılan değerleri kullan
          this.kisiTipleri = [
            { id: 1, tipId: KISI_TIP_ID, kod: 'PERSONEL', kisaAd: 'Personel', sira: 1, digerUygEnumAd: '', digerUygEnumDeger: 0 },
            { id: 2, tipId: KISI_TIP_ID, kod: 'YONETICI', kisaAd: 'Yönetici', sira: 2, digerUygEnumAd: '', digerUygEnumDeger: 0 },
            { id: 3, tipId: KISI_TIP_ID, kod: 'DIGER', kisaAd: 'Diğer', sira: 3, digerUygEnumAd: '', digerUygEnumDeger: 0 }
          ];
          this.prepareSelectData();
        }
      });
  }

  /**
   * Firma listesini yükler
   */
  loadFirmaList(): void {
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
   * Firma kişi listesini yükler
   */
  loadFirmaKisiList(): void {
    this.loading = true;
    
    // Spesifik firma seçilmişse sadece o firmaya ait kişileri getir
    if (this.selectedFirma && this.selectedFirma.eid) {
      this.firmaKisiService.getFirmaKisiListByFirmaId(this.selectedFirma.eid)
        .pipe(finalize(() => this.loading = false))
        .subscribe({
          next: (data) => {
            console.log('API\'den dönen firma kişi verileri:', data);
            console.log('İlk veri örneği:', data?.[0]);
            this.firmaKisiList = data;
          },
          error: (err) => {
            console.error('Firma kişi listesi yüklenirken hata oluştu', err);
          }
        });
    } else {
      // Tüm firmaların kişilerini getir
      this.loading = false;
      this.firmaKisiList = [];
      
      // Gerçek implementasyonda firmaları dönüp her biri için kişileri çekecek
      if (this.firmaList.length > 0) {
        this.firmaList.forEach(firma => {
          if (firma.eid) {
            this.firmaKisiService.getFirmaKisiListByFirmaId(firma.eid)
              .subscribe({
                next: (data) => {
                  console.log(`${firma.ad} firması için API'den dönen veriler:`, data);
                  this.firmaKisiList = [...this.firmaKisiList, ...data];
                },
                error: (err) => {
                  console.error(`${firma.ad} firması için kişiler yüklenirken hata oluştu`, err);
                }
              });
          }
        });
      }
    }
  }

  /**
   * Yeni firma kişi eklemek için modal açar
   */  openAddFirmaKisiModal(): void {
    this.isNewKisi = true;    this.firmaKisiModel = {
      kisiDto: {
        ad: '',
        soyad: '',
        tc: '',
        loginName: '',
        cepTel: '',
        email: '',
        sifre: '' // Şifre alanını boş başlat
      } as KisiDto,
      firmaDto: null,
      firmaKisiTipKodDto: null
    } as FirmaKisiDto;
    
    // Eğer bir firma seçiliyse, yeni kişiyi o firmaya atamak için
    if (this.selectedFirma) {
      this.firmaKisiModel.firmaDto = this.selectedFirma;
    }
    
    // Eğer kişi tipleri henüz yüklenmemişse, tekrar yükle
    if (!this.kisiTipleri || this.kisiTipleri.length === 0) {
      this.loadKisiTipleri();
    }
    
    this.firmaKisiModalVisible = true;
  }

  /**
   * Firma kişi düzenlemek için modal açar
   * @param firmaKisi Düzenlenecek firma kişi
   */
  openEditFirmaKisiModal(firmaKisi: FirmaKisiDto): void {
    this.isNewKisi = false;
    this.firmaKisiModel = { ...firmaKisi };
    this.firmaKisiModalVisible = true;
  }

  /**
   * DataGrid satır aksiyonlarını yönetir
   * @param event Aksiyon olayı
   */
  onRowAction(event: { action: string; data: any }): void {
    switch (event.action) {
      case 'edit':
        this.openEditFirmaKisiModal(event.data);
        break;
      case 'delete':
        this.confirmDeleteFirmaKisi(event.data);
        break;
    }
  }

  /**
   * Firma kişi silme işlemini onaylatmak için dialog açar
   * @param firmaKisi Silinecek firma kişi
   */
  confirmDeleteFirmaKisi(firmaKisi: FirmaKisiDto): void {
    this.deletingFirmaKisiId = firmaKisi.eid;
    
    // Onay dialogunu göster
    this.confirmDialog.header = 'Kişi Sil';
    this.confirmDialog.message = 'Bu kişiyi silmek istediğinize emin misiniz?';
    this.confirmDialog.show();
  }

  /**
   * Firma kişi siler
   */
  deleteFirmaKisi(): void {
    if (!this.deletingFirmaKisiId) return;

    this.loading = true;
    this.firmaKisiService.deleteFirmaKisi(this.deletingFirmaKisiId)
      .pipe(finalize(() => {
        this.loading = false;
      }))
      .subscribe({
        next: () => {
          this.loadFirmaKisiList();
        },
        error: (err) => {
          console.error('Firma kişi silinirken hata oluştu', err);
        }
      });
  }
  /**
   * Firma kişi kaydeder veya günceller
   */
  saveFirmaKisi(): void {
    this.loading = true;
    this.firmaKisiService.saveFirmaKisi(this.firmaKisiModel)
      .pipe(finalize(() => {
        this.loading = false;
      }))
      .subscribe({
        next: () => {
          this.loadFirmaKisiList();
          // Sadece başarılı olduğunda modalı kapat
          this.firmaKisiModalVisible = false;
        },
        error: (err) => {
          console.error('Firma kişi kaydedilirken hata oluştu', err);
          // Hata durumunda modal açık kalır
        }
      });
  }

  /**
   * Firma değiştiğinde çağrılır
   */
  onFirmaChange(firmaId: string): void {
    // firmaId kullanarak firma nesnesini bul
    this.selectedFirma = this.firmaList.find(firma => firma.eid === firmaId) || null;
    this.loadFirmaKisiList();
  }

  /**
   * Modal içindeki firma seçimi değiştiğinde çağrılır
   */
  onModalFirmaChange(firmaId: string): void {
    const selectedFirma = this.firmaList.find(firma => firma.eid === firmaId);
    if (selectedFirma) {
      this.firmaKisiModel.firmaDto = selectedFirma;
    }
  }

  /**
   * Modal içindeki kişi tipi seçimi değiştiğinde çağrılır
   */
  onModalKisiTipChange(kisiTipId: number): void {
    const selectedTip = this.kisiTipleri.find(tip => tip.id === kisiTipId);
    if (selectedTip) {
      this.firmaKisiModel.firmaKisiTipKodDto = selectedTip;
    }
  }
  /**
   * Form validasyonunu kontrol eder
   * @returns Boolean - Form geçerli mi?
   */  isFormValid(): boolean {
    if (!this.firmaKisiModel) return false;
    
    const kisi = this.firmaKisiModel.kisiDto;
    const firma = this.firmaKisiModel.firmaDto;
    const kisiTip = this.firmaKisiModel.firmaKisiTipKodDto;
    
    // Temel doğrulama
    const baseValidation = !!(
      kisi && 
      kisi.ad && 
      kisi.soyad && 
      firma && 
      firma.eid && 
      kisiTip && 
      kisiTip.id > 0
    );
    
    // Eğer yeni kişi ekleniyorsa ve var olan bir kişi seçilmiyorsa şifre zorunlu
    if (this.isNewKisi && !this.isSelectExistingPerson) {
      return baseValidation && !!kisi.sifre;
    }
    
    return baseValidation;
  }
  /**
   * Firma kişi modalı kapandığında çağrılır
   */
  onFirmaKisiModalClosed(): void {
    this.firmaKisiModel = {} as FirmaKisiDto;
    this.isNewKisi = false;
    this.isSelectExistingPerson = false;  // Toggle varsayılan duruma çevir
    this.selectedSearchKisi = null;
    this.filteredKisiList = [];
  }
  
  /**
   * Kişi araması gerçekleştirir
   * @param event Arama olayı
   */  searchKisi(event: any): void {
    const query = typeof event === 'string' ? event : (event.query || event.target?.value);
    
    if (!query || query.length < 2) {
      this.filteredKisiList = [];
      return;
    }
    
    this.kisiService.getKisiListByAramaText(query)
      .subscribe({
        next: (data) => {
          this.filteredKisiList = data;
        },
        error: (err) => {
          console.error('Kişi araması yapılırken hata oluştu:', err);
          this.filteredKisiList = [];
        }
      });
  }
    /**
   * Var olan bir kişinin seçilmesi durumunda çağrılır
   */
  onSelectExistingPerson(): void {
    if (!this.selectedSearchKisi) return;
    
    // Seçilen kişiyi model'e ata
    this.firmaKisiModel.kisiDto = { ...this.selectedSearchKisi };
    
    // Arama sonuçlarını temizle
    this.filteredKisiList = [];
  }
    /**
   * Ekleme/düzenleme modunu değiştirir
   * @param event Toggle olayı
   */  togglePersonSelectionMode(event: any): void {
    // Event değerini doğru şekilde kontrol et 
    let value: boolean;
    
    if (typeof event === 'boolean') {
      value = event;
    } else if (event && typeof event.checked !== 'undefined') {
      value = event.checked;
    } else {
      // Toggle değerini tersine çevir
      value = !this.isSelectExistingPerson;
    }
    
    this.isSelectExistingPerson = value;
    
    // Seçilen kişiyi ve kişi arama sonuçlarını temizle
    this.selectedSearchKisi = null;
    this.filteredKisiList = [];
      // Eğer var olan kişi seçme modundan çıkılıyorsa, yeni bir kişi nesnesi oluştur
    if (!value) {
      // Yeni bir kişi nesnesi oluştur
      this.firmaKisiModel.kisiDto = {
        ad: '',
        soyad: '',
        tc: '',
        loginName: '',
        cepTel: '',
        email: '',
        sifre: '' // Şifre alanını boş başlat
      } as KisiDto;
    }
  }

  /**
   * Select bileşenleri için veri hazırlar
   */
  prepareSelectData(): void {
    // Firma listesi için SelectInputModel nesneleri oluştur
    if (this.firmaList && this.firmaList.length > 0) {
      const firmaSelectItems = this.firmaList.map(firma => 
        new SelectInputModel(firma.eid, firma.ad)
      );
      this.firmaListDto$.next(firmaSelectItems);
    }
    
    // Kişi tipleri için SelectInputModel nesneleri oluştur
    if (this.kisiTipleri && this.kisiTipleri.length > 0) {
      const kisiTipSelectItems = this.kisiTipleri.map(tip => 
        new SelectInputModel(tip.id, `${tip.kisaAd} (${tip.kod || ''})`)
      );
      this.kisiTipleriListDto$.next(kisiTipSelectItems);
    }
  }
  // Kişi araması gerçekleştirir fonksiyonu yukarıda tanımlandı
  /**
   * Seçilen mevcut kişiyi ayarlar
   * @param kisi Seçilen kişi
   */
  selectExistingKisi(kisi: KisiDto): void {
    this.selectedSearchKisi = kisi;
    
    // Seçilen kişiyi firmaKisiModel'e ata
    this.firmaKisiModel.kisiDto = { ...kisi };
    // Kullanıcının kendisinin seçmesi için otomatik değer atama işlemleri kaldırıldı
  }

  /**
   * Mevcut kişi seçimini temizler
   */
  clearSelectedKisi(): void {
    this.selectedSearchKisi = null;
    this.firmaKisiModel.kisiDto = {} as KisiDto;
  }
}
