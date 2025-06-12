import { Component, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { finalize, BehaviorSubject } from 'rxjs';

// PrimeNG Modülleri
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { TooltipModule } from 'primeng/tooltip';
import { PaginatorModule } from 'primeng/paginator';
import { DropdownModule } from 'primeng/dropdown';
import { TabViewModule } from 'primeng/tabview';
import { PanelModule } from 'primeng/panel';
import { AccordionModule } from 'primeng/accordion';
import { DividerModule } from 'primeng/divider';

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
import { LogParserService } from '../../../core/services/modules/log-parser.service';
import { FirmaCihazDto } from '../../../core/models/FirmaCihazDto';
import { FirmaDto } from '../../../core/models/FirmaDto';
import { KodDto } from '../../../core/models/KodDto';
import { LogParserConfigDto, FieldMappingDto, LogParserTestResultDto } from '../../../core/models/LogParserDto';

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
    TabViewModule,
    PanelModule,
    AccordionModule,
    DividerModule,
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
  selectedFirma: FirmaDto | null = null;  // Modal kontrolleri
  firmaCihazModalVisible: boolean = false;
  deletingFirmaCihazId: string = '';
  
  // Log Parser Modal kontrolleri
  logParserModalVisible: boolean = false;
  selectedFirmaCihazForLogParser: FirmaCihazDto | null = null;
  
  // Log parser konfigürasyonu
  logParserConfig: LogParserConfigDto = {
    delimiter: ',',
    dateFormat: 'dd.MM.yyyy HH:mm:ss',
    timeFormat: 'HH:mm:ss',
    fieldMapping: []
  };
  
  // Test verileri
  sampleLogData = '';
  testResult: LogParserTestResultDto | null = null;
    // Loading durumları
  isLogParserLoading = false;
  isLogParserSaving = false;
  isLogParserTesting = false;
  
  // Kullanım kılavuzu
  showLogParserHelp = false;

  // DataGrid kolonları
  columns: any[] = [
    { field: 'ad', header: 'Cihaz Adı' },
    { field: 'firmaDto.ad', header: 'Firma Adı' },
    { field: 'firmaCihazTipKodDto.kisaAd', header: 'Cihaz Tipi' },
    { field: 'cihazMakineGercekId', header: 'Cihaz ID' },
    { field: 'aciklama', header: 'Açıklama' }
  ];  // DataGrid aksiyon butonları
  actionButtons: ActionButtonConfig[] = [
    {
      icon: 'pi pi-pencil',
      tooltip: 'Düzenle',
      action: 'edit'
    },
    {
      icon: 'pi pi-cog',
      tooltip: 'Log Parser Ayarları',
      action: 'logparser',
      class: 'p-button-info'
    },
    {
      icon: 'pi pi-trash',
      tooltip: 'Sil',
      action: 'delete',
      class: 'p-button-danger'
    }
  ];

  // ViewChild referansı
  @ViewChild(ConfirmDialogComponent) confirmDialog!: ConfirmDialogComponent;  constructor(
    private firmaCihazService: FirmaCihazService,
    private firmaService: FirmaService,
    private kodService: KodService,
    private router: Router,
    private logParserService: LogParserService
  ) { }  ngOnInit(): void {
    // Önce cihaz tiplerini yükleyelim
    this.loadCihazTipleri();
    
    // Sonra diğer verileri yükleyelim
    this.loadFirmaList();
    this.loadFirmaCihazList();
    
    // Log Parser için ilk field mapping ekle
    this.addLogParserFieldMapping();
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
   */  onRowAction(event: { action: string; data: any }): void {
    switch (event.action) {
      case 'edit':
        this.openEditFirmaCihazModal(event.data);
        break;
      case 'logparser':
        this.openLogParserPage(event.data);
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
  /**
   * Log Parser sayfasını açar
   */
  openLogParserPage(firmaCihaz: FirmaCihazDto): void {
    // Artık modal açacağız sayfa yönlendirmesi yerine
    this.openLogParserModal(firmaCihaz);
  }

  /**
   * Log Parser modal'ını açar
   */
  openLogParserModal(firmaCihaz: FirmaCihazDto): void {
    this.selectedFirmaCihazForLogParser = firmaCihaz;
    this.loadLogParserConfig(firmaCihaz.eid);
    this.logParserModalVisible = true;
  }
  /**
   * Log Parser modal'ını kapatır
   */
  closeLogParserModal(): void {
    this.logParserModalVisible = false;
    this.selectedFirmaCihazForLogParser = null;
    this.showLogParserHelp = false; // Kılavuzu da kapat
    this.resetLogParserConfig();
  }

  /**
   * Kullanım kılavuzunu aç/kapat
   */
  toggleLogParserHelp(): void {
    this.showLogParserHelp = !this.showLogParserHelp;
  }

  // Delimiter seçenekleri
  delimiterOptions = [
    { label: 'Virgül (,)', value: ',' },
    { label: 'Noktalı virgül (;)', value: ';' },
    { label: 'Tab (\\t)', value: '\\t' },
    { label: 'Pipe (|)', value: '|' },
    { label: 'Boşluk ( )', value: ' ' },
    { label: 'Yeni satır (\\n)', value: '\\n' },
    { label: 'Regex Pattern', value: 'regex' }
  ];
  
  // Alan tipleri
  fieldTypes = [
    { label: 'Metin', value: 'string' },
    { label: 'Sayı', value: 'int' },
    { label: 'Ondalık', value: 'double' },
    { label: 'Tarih', value: 'datetime' },
    { label: 'Boolean', value: 'boolean' }
  ];
    // Önceden tanımlı alan isimleri
  predefinedFields = [
    { label: 'Kullanıcı ID', value: 'userId' },
    { label: 'Zaman Damgası', value: 'timestamp' },
    { label: 'Cihaz ID', value: 'deviceId' },
    { label: 'Kart Numarası', value: 'cardNumber' },
    { label: 'Hareket Tipi', value: 'actionType' },
    { label: 'Kapı Numarası', value: 'doorNumber' },
    { label: 'Alarm Tipi', value: 'alarmType' },
    { label: 'Event Tipi', value: 'eventType' },
    { label: 'Lokasyon', value: 'location' },
    { label: 'Açıklama', value: 'description' }
  ];

  // Tarih formatı seçenekleri
  dateFormatOptions = [
    { label: 'dd.MM.yyyy HH:mm:ss (06.12.2025 14:30:15)', value: 'dd.MM.yyyy HH:mm:ss' },
    { label: 'dd/MM/yyyy HH:mm:ss (06/12/2025 14:30:15)', value: 'dd/MM/yyyy HH:mm:ss' },
    { label: 'yyyy-MM-dd HH:mm:ss (2025-12-06 14:30:15)', value: 'yyyy-MM-dd HH:mm:ss' },
    { label: 'ddMMyy HHmm (061225 1430)', value: 'ddMMyy HHmm' },
    { label: 'ddMMyyyy HHmmss (06122025 143015)', value: 'ddMMyyyy HHmmss' },
    { label: 'yyMMdd HHmm (251206 1430)', value: 'yyMMdd HHmm' },
    { label: 'dd-MM-yyyy HH:mm (06-12-2025 14:30)', value: 'dd-MM-yyyy HH:mm' },
    { label: 'MM/dd/yyyy HH:mm:ss (12/06/2025 14:30:15)', value: 'MM/dd/yyyy HH:mm:ss' },
    { label: 'yyyy/MM/dd HH:mm (2025/12/06 14:30)', value: 'yyyy/MM/dd HH:mm' },
    { label: 'ddMMyy (061225)', value: 'ddMMyy' },
    { label: 'yyMMdd (251206)', value: 'yyMMdd' },
    { label: 'dd.MM.yyyy (06.12.2025)', value: 'dd.MM.yyyy' }
  ];

  // Saat formatı seçenekleri
  timeFormatOptions = [
    { label: 'HH:mm:ss (14:30:15)', value: 'HH:mm:ss' },
    { label: 'HH:mm (14:30)', value: 'HH:mm' },
    { label: 'HHmmss (143015)', value: 'HHmmss' },
    { label: 'HHmm (1430)', value: 'HHmm' },
    { label: 'H:mm:ss (14:30:15)', value: 'H:mm:ss' },
    { label: 'H:mm (14:30)', value: 'H:mm' },
    { label: 'hh:mm:ss tt (02:30:15 PM)', value: 'hh:mm:ss tt' },
    { label: 'hh:mm tt (02:30 PM)', value: 'hh:mm tt' }
  ];

  /**
   * Log Parser konfigürasyonunu yükler
   */
  loadLogParserConfig(firmaCihazEid: string): void {
    this.isLogParserLoading = true;
    this.logParserService.getLogParserConfig(firmaCihazEid)
      .pipe(finalize(() => this.isLogParserLoading = false))
      .subscribe({
        next: (config) => {
          if (config) {
            this.logParserConfig = config;
            // Eğer field mapping yoksa en az bir tane ekle
            if (!this.logParserConfig.fieldMapping || this.logParserConfig.fieldMapping.length === 0) {
              this.addLogParserFieldMapping();
            }
          }
        },
        error: (error) => {
          console.error('Log parser konfigürasyonu yüklenirken hata oluştu:', error);
        }
      });
  }

  /**
   * Yeni alan mapping'i ekler
   */
  addLogParserFieldMapping(): void {
    if (!this.logParserConfig.fieldMapping) {
      this.logParserConfig.fieldMapping = [];
    }
    
    this.logParserConfig.fieldMapping.push({
      name: '',
      index: this.logParserConfig.fieldMapping.length,
      type: 'string',
      format: ''
    });
  }

  /**
   * Alan mapping'ini siler
   */
  removeLogParserFieldMapping(index: number): void {
    if (this.logParserConfig.fieldMapping && index >= 0 && index < this.logParserConfig.fieldMapping.length) {
      this.logParserConfig.fieldMapping.splice(index, 1);
      
      // Index'leri yeniden düzenle
      this.logParserConfig.fieldMapping.forEach((field, i) => {
        field.index = i;
      });
    }
  }

  /**
   * Log Parser konfigürasyonunu test eder
   */
  testLogParserConfiguration(): void {
    if (!this.sampleLogData.trim()) {
      alert('Lütfen önce örnek log verisi girin.');
      return;
    }

    this.isLogParserTesting = true;
    this.testResult = null;
    
    this.logParserService.testLogParserConfig(this.logParserConfig, this.sampleLogData)
      .pipe(finalize(() => this.isLogParserTesting = false))
      .subscribe({
        next: (result) => {
          this.testResult = result;
        },
        error: (error) => {
          console.error('Test sırasında hata oluştu:', error);
          this.testResult = {
            success: false,
            message: 'Test sırasında bir hata oluştu.',
            parsedData: {}
          };
        }
      });
  }

  /**
   * Log Parser konfigürasyonunu kaydeder
   */
  saveLogParserConfiguration(): void {
    if (!this.selectedFirmaCihazForLogParser?.eid) {
      alert('Bir hata oluştu. Lütfen modal\'ı kapatıp tekrar açın.');
      return;
    }

    this.isLogParserSaving = true;
    
    this.logParserService.updateLogParserConfig(this.selectedFirmaCihazForLogParser.eid, this.logParserConfig)
      .pipe(finalize(() => this.isLogParserSaving = false))
      .subscribe({
        next: (success) => {
          if (success) {
            alert('Log Parser konfigürasyonu başarıyla kaydedildi.');
            this.closeLogParserModal();
          } else {
            alert('Konfigürasyon kaydedilirken bir hata oluştu.');
          }
        },
        error: (error) => {
          console.error('Konfigürasyon kaydedilirken hata oluştu:', error);
          alert('Konfigürasyon kaydedilirken bir hata oluştu.');
        }
      });
  }

  /**
   * Log Parser örnek veri yükler
   */
  loadLogParserSampleData(): void {
    const samples = {
      csv: '1234,2025-06-12 10:30:15,GİRİŞ,1,192.168.1.100',
      pipe: '1234|2025-06-12 10:30:15|GİRİŞ|1|192.168.1.100',
      tab: '1234\t2025-06-12 10:30:15\tGİRİŞ\t1\t192.168.1.100'
    };
    
    const delimiter = this.logParserConfig.delimiter;
    if (delimiter === ',') {
      this.sampleLogData = samples.csv;
    } else if (delimiter === '|') {
      this.sampleLogData = samples.pipe;
    } else if (delimiter === '\\t') {
      this.sampleLogData = samples.tab;
    } else {
      this.sampleLogData = samples.csv;
    }
  }

  /**
   * Log Parser konfigürasyonunu sıfırlar
   */
  resetLogParserConfig(): void {
    this.logParserConfig = {
      delimiter: ',',
      dateFormat: 'dd.MM.yyyy HH:mm:ss',
      timeFormat: 'HH:mm:ss',
      fieldMapping: []
    };
    this.addLogParserFieldMapping();
    this.sampleLogData = '';
    this.testResult = null;
  }

  /**
   * Log Parser JSON formatında göster
   */
  getLogParserConfigAsJson(): string {
    return JSON.stringify(this.logParserConfig, null, 2);
  }
}
