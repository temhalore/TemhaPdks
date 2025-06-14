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
import { MessageService } from 'primeng/api';

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

// Template sistemi için interface'ler
interface LogParserTemplateDto {
  id?: number;
  templateName: string;
  description: string;
  deviceType: string;
  deviceBrand: string;
  logDelimiter: string;
  logDateFormat: string;
  logTimeFormat: string;
  logFieldMapping: string;
  sampleLogData: string;
  isSystemTemplate: boolean;
  isActive: boolean;
}

interface FieldMappingDetail {
  fieldName: string;
  displayName: string;
  mappingType: FieldMappingType;
  position: number;
  keyword: string;
  pattern: string;
  dataType: string;
  format: string;
  isRequired: boolean;
}

enum FieldMappingType {
  Position = 1,
  Keyword = 2,
  Regex = 3
}

@Component({
  selector: 'app-firmacihaz',
  templateUrl: './firmacihaz.component.html',
  styleUrls: ['./firmacihaz.component.scss'],  standalone: true,
  imports: [
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
  ],
  providers: [MessageService]
})
export class FirmaCihazComponent implements OnInit {  // Firma cihaz listesi
  firmaCihazList: FirmaCihazDto[] = [];
  loading: boolean = false;
  firmaList: FirmaDto[] = [];
  cihazTipleri: KodDto[] = []; // Cihaz tipleri listesi
    // Log parser için gerekli değişkenler
  logParserSampleData: string = '';
  logParserTestResults: LogParserTestResultDto | null = null;
  
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
  
  // Template sistemi
  availableTemplates: LogParserTemplateDto[] = [];
  selectedTemplate: LogParserTemplateDto | null = null;
  showTemplateSelector = false;
  templateFilter = 'ALL'; // ALL, PDKS, ALARM, KAMERA
  
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
    private logParserService: LogParserService,
    private messageService: MessageService,
    private router: Router
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
    this.loadAvailableTemplates(); // Template'leri yükle
    this.logParserModalVisible = true;
  }

  /**
   * Mevcut şablonları yükler
   */
  loadAvailableTemplates(): void {
    this.logParserService.getSystemTemplates()
      .subscribe({
        next: (templates) => {
          this.availableTemplates = templates;
          console.log('Templates yüklendi:', templates.length);
        },
        error: (error) => {
          console.error('Template\'ler yüklenirken hata oluştu:', error);
        }
      });
  }

  /**
   * Cihaz tipine göre önerilen şablonları filtreler
   */
  getRecommendedTemplates(): LogParserTemplateDto[] {
    if (!this.selectedFirmaCihazForLogParser?.firmaCihazTipKodDto?.kod) {
      return [];
    }

    const deviceTypeMap: { [key: string]: string } = {
      'KARTLI': 'PDKS',
      'PARMAK': 'PDKS', 
      'YUZ': 'PDKS',
      'KAMERA': 'KAMERA',
      'ALARM': 'ALARM'
    };

    const deviceType = deviceTypeMap[this.selectedFirmaCihazForLogParser.firmaCihazTipKodDto.kod];
    
    return this.availableTemplates.filter(template => 
      template.deviceType === deviceType
    );
  }

  /**
   * Filtre durumuna göre şablonları getirir
   */
  getFilteredTemplates(): LogParserTemplateDto[] {
    if (this.templateFilter === 'ALL') {
      return this.availableTemplates;
    }
    
    return this.availableTemplates.filter(template => 
      template.deviceType === this.templateFilter
    );
  }

  /**
   * Şablon seçici modal'ını göster/gizle
   */
  toggleTemplateSelector(): void {
    this.showTemplateSelector = !this.showTemplateSelector;
  }

  /**
   * Şablon seçildiğinde çağrılır
   */
  selectTemplate(template: LogParserTemplateDto): void {
    this.selectedTemplate = template;
    this.applyTemplate(template);
    this.showTemplateSelector = false;
  }
  /**
   * Seçilen şablonu konfigürasyona uygular
   */  applyTemplate(template: LogParserTemplateDto): void {
    try {
      console.log('Şablon verileri:', template);

      // Temel ayarları uygula - combobox'larda uygun değerleri bulmak için
      // Delimiter için uyumlu seçeneği bul
      const delimiterOption = this.delimiterOptions.find(opt => opt.value === template.logDelimiter);
      if (delimiterOption) {
        this.logParserConfig.delimiter = delimiterOption.value;
      } else {
        this.logParserConfig.delimiter = template.logDelimiter || ',';
      }

      // Tarih formatı için uyumlu seçeneği bul
      const dateFormatOption = this.dateFormatOptions.find(opt => opt.value === template.logDateFormat);
      if (dateFormatOption) {
        this.logParserConfig.dateFormat = dateFormatOption.value;
      } else {
        this.logParserConfig.dateFormat = template.logDateFormat || 'yyyy-MM-dd';
      }

      // Saat formatı için uyumlu seçeneği bul
      const timeFormatOption = this.timeFormatOptions.find(opt => opt.value === template.logTimeFormat);
      if (timeFormatOption) {
        this.logParserConfig.timeFormat = timeFormatOption.value;
      } else {
        this.logParserConfig.timeFormat = template.logTimeFormat || 'HH:mm:ss';
      }
      
      // Örnek veriyi yükle
      this.logParserSampleData = template.sampleLogData || '';
      this.sampleLogData = template.sampleLogData || '';
      this.logParserConfig.sampleLogData = template.sampleLogData || '';
      
      // Field mapping'leri uygula
      if (template.logFieldMapping) {
        try {
          const fieldMappings: FieldMappingDetail[] = JSON.parse(template.logFieldMapping);
          console.log('Şablondan gelen alan eşleşmeleri:', fieldMappings);
          
          // Mevcut field mapping'leri temizle
          this.logParserConfig.fieldMapping = [];
          
          // Yeni field mapping'leri ekle
          fieldMappings.forEach(detail => {
            // Her alan için uygun tip seçeneğini bul
            const fieldTypeOption = this.fieldTypes.find(opt => opt.value === detail.dataType);
            
            this.logParserConfig.fieldMapping.push({
              name: detail.fieldName,
              index: detail.position, // Doğrudan position'ı kullanalım
              type: fieldTypeOption ? fieldTypeOption.value : detail.dataType, // Uyumlu tip seçeneği varsa kullan
              format: detail.format || ''
            });
          });
          
          // Eğer mapping boşsa, en az bir alan ekleyelim
          if (this.logParserConfig.fieldMapping.length === 0) {
            this.addLogParserFieldMapping();
          }
        } catch (err) {
          console.error('Field mapping JSON parse hatası:', err);
          // Parse hatası durumunda boş bir mapping ekle
          this.logParserConfig.fieldMapping = [];
          this.addLogParserFieldMapping();
        }
      } else {
        // Field mapping yoksa yeni bir tane ekle
        this.logParserConfig.fieldMapping = [];
        this.addLogParserFieldMapping();
      }
      
      // Dropdown seçimlerini güncelle
      this.updateDropdownSelections();
      
      // Change detection'ı zorlamak için kopya oluştur
      this.logParserConfig = {...this.logParserConfig};
      
      console.log('Şablon uygulandı:', template.templateName);
      console.log('Güncellenmiş konfigürasyon:', JSON.stringify(this.logParserConfig));
    } catch (error) {
      console.error('Şablon uygulanırken hata oluştu:', error);
    }
  }

  /**
   * Akıllı kelime bazlı alan tanımlama (Alarm sistemleri için)
   */
  generateKeywordBasedMapping(sampleData: string): void {
    const keywordPatterns = [
      { keyword: 'ALARM:', fieldName: 'alarmType', displayName: 'Alarm Tipi' },
      { keyword: 'USER:', fieldName: 'userId', displayName: 'Kullanıcı ID' },
      { keyword: 'ZONE:', fieldName: 'zone', displayName: 'Bölge' },
      { keyword: 'STATUS:', fieldName: 'status', displayName: 'Durum' },
      { keyword: 'EVENT:', fieldName: 'eventType', displayName: 'Olay Tipi' }
    ];

    // Mevcut field mapping'leri temizle
    this.logParserConfig.fieldMapping = [];

    // Tarih ve saat alanlarını ekle (genellikle başta olur)
    this.logParserConfig.fieldMapping.push({
      name: 'date',
      index: 0,
      type: 'datetime',
      format: this.logParserConfig.dateFormat
    });

    this.logParserConfig.fieldMapping.push({
      name: 'time', 
      index: 1,
      type: 'datetime',
      format: this.logParserConfig.timeFormat
    });

    // Kelime bazlı alanları tespit et ve ekle
    let currentIndex = 2;
    keywordPatterns.forEach(pattern => {
      if (sampleData.includes(pattern.keyword)) {
        this.logParserConfig.fieldMapping.push({
          name: pattern.fieldName,
          index: currentIndex++,
          type: 'string',
          format: ''
        });
      }
    });

    console.log('Kelime bazlı alan eşlemesi oluşturuldu');
  }

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
   * Alan mapping'i siler
   */
  removeLogParserFieldMapping(index: number): void {
    if (this.logParserConfig.fieldMapping && index >= 0 && index < this.logParserConfig.fieldMapping.length) {
      this.logParserConfig.fieldMapping.splice(index, 1);
      
      // Index'leri yeniden düzenle
      this.logParserConfig.fieldMapping.forEach((mapping, i) => {
        mapping.index = i;
      });
    }
  }

  /**
   * Log parser konfigürasyonunu kaydet
   */
  saveLogParserConfig(): void {
    if (!this.selectedFirmaCihazForLogParser) {
      this.messageService.add({
        severity: 'error',
        summary: 'Hata',
        detail: 'Seçili firma cihazı bulunamadı'
      });
      return;
    }

    // Alan eşleşmelerini kontrol et ve boş alanları filtrele
    if (this.logParserConfig.fieldMapping && this.logParserConfig.fieldMapping.length > 0) {
      // Boş alan adlarını filtrele
      this.logParserConfig.fieldMapping = this.logParserConfig.fieldMapping.filter(mapping => 
        mapping.name && mapping.name.trim() !== ''
      );
      
      // Field adlarını ve indexleri düzelt
      this.logParserConfig.fieldMapping.forEach((mapping, index) => {
        mapping.index = index; // Indexleri düzelt
        // Tip kontrolü
        if (!mapping.type || mapping.type.trim() === '') {
          mapping.type = 'string'; // Default tip
        }
      });
    }

    // En az bir field mapping kontrolü
    if (!this.logParserConfig.fieldMapping || this.logParserConfig.fieldMapping.length === 0) {
      this.messageService.add({
        severity: 'error',
        summary: 'Hata',
        detail: 'En az bir alan eşleşmesi tanımlamalısınız'
      });
      return;
    }

    // Örnek veriyi ekleyelim (backend'de gerekirse kullanılabilir)
    this.logParserConfig.sampleLogData = this.sampleLogData;

    console.log('Kaydediliyor...', this.logParserConfig);
    
    this.isLogParserSaving = true;
    this.logParserService.updateLogParserConfig(this.selectedFirmaCihazForLogParser.eid, this.logParserConfig)
      .pipe(finalize(() => this.isLogParserSaving = false))
      .subscribe({
        next: (result) => {
          if (result) {
            console.log('Log parser konfigürasyonu başarıyla kaydedildi');
            // Başarı mesajı göster
            this.messageService.add({
              severity: 'success',
              summary: 'Başarılı',
              detail: 'Log parser konfigürasyonu başarıyla kaydedildi'
            });
            
            // Firma cihaz nesnesini de güncelle
            if (this.selectedFirmaCihazForLogParser) {
              this.selectedFirmaCihazForLogParser.logDelimiter = this.logParserConfig.delimiter;
              this.selectedFirmaCihazForLogParser.logDateFormat = this.logParserConfig.dateFormat;
              this.selectedFirmaCihazForLogParser.logTimeFormat = this.logParserConfig.timeFormat;
              this.selectedFirmaCihazForLogParser.logFieldMapping = JSON.stringify(this.logParserConfig.fieldMapping);
              this.selectedFirmaCihazForLogParser.logSample = this.sampleLogData;
            }
          }
        },
        error: (error) => {
          console.error('Log parser konfigürasyonu kaydedilirken hata oluştu:', error);
          this.messageService.add({
            severity: 'error',
            summary: 'Hata',
            detail: 'Log parser konfigürasyonu kaydedilirken hata oluştu: ' + (error.message || 'Bilinmeyen hata')
          });
        }
      });
  }

  /**
   * Log parser konfigürasyonunu test et
   */
  testLogParserConfig(): void {
    if (!this.sampleLogData.trim()) {
      console.warn('Test için örnek log verisi gerekli');
      return;
    }

    this.isLogParserTesting = true;
    this.logParserService.testLogParserConfig(this.logParserConfig, this.sampleLogData)
      .pipe(finalize(() => this.isLogParserTesting = false))
      .subscribe({
        next: (result) => {
          this.testResult = result;
          console.log('Test sonucu:', result);
        },
        error: (error) => {
          console.error('Log parser test edilirken hata oluştu:', error);
          this.testResult = {
            success: false,
            message: 'Test sırasında hata oluştu: ' + error.message
          };
        }
      });
  }

  /**
   * Log parser konfigürasyonunu sıfırla
   */
  resetLogParserConfig(): void {
    this.logParserConfig = {
      delimiter: ',',
      dateFormat: 'dd.MM.yyyy HH:mm:ss',
      timeFormat: 'HH:mm:ss',
      fieldMapping: []
    };
    this.sampleLogData = '';
    this.testResult = null;
    this.selectedTemplate = null;
    this.showTemplateSelector = false;
    this.addLogParserFieldMapping();
  }

  /**
   * Akıllı örnek veri yükleme
   */
  loadSmartSampleData(): void {
    if (!this.sampleLogData.trim()) {
      console.warn('Analiz için örnek log verisi gerekli');
      return;
    }

    // Mevcut akıllı analiz metodunu kullan
    this.generateSmartFieldMapping(this.sampleLogData);
    
    // Delimiter'ı da akıllıca belirle
    this.detectDelimiter(this.sampleLogData);
    
    console.log('Akıllı örnek veri yüklendi');
  }

  /**
   * Delimiter'ı otomatik tespit et
   */
  private detectDelimiter(sampleData: string): void {
    const delimiters = [',', ';', '\t', '|', ' '];
    let bestDelimiter = ',';
    let maxFields = 0;

    delimiters.forEach(delimiter => {
      const lines = sampleData.split('\n').filter(line => line.trim());
      if (lines.length > 0) {
        const fieldCount = lines[0].split(delimiter).length;
        if (fieldCount > maxFields) {
          maxFields = fieldCount;
          bestDelimiter = delimiter;
        }
      }
    });

    this.logParserConfig.delimiter = bestDelimiter === '\t' ? '\\t' : bestDelimiter;
  }

  /**
   * Akıllı alan eşlemesi oluştur
   */
  private generateSmartFieldMapping(sampleData: string): void {
    // Mevcut alan eşlemelerini temizle
    this.logParserConfig.fieldMapping = [];
    
    // Veriyi böl
    let fields: string[] = [];
    const delimiter = this.logParserConfig.delimiter;
    
    if (delimiter === '\\t' || delimiter === '\t') {
      fields = sampleData.split('\t');
    } else if (delimiter === '\\n') {
      fields = sampleData.split('\n');
    } else {
      fields = sampleData.split(delimiter);
    }
    
    // Her alan için akıllı tahmin yap
    fields.forEach((field, index) => {
      const trimmedField = field.trim();
      let fieldName = 'alan' + (index + 1);
      let fieldType = 'string';
      
      // Akıllı alan ismi ve tip tahmini
      if (this.isNumericId(trimmedField)) {
        fieldName = 'userId';
        fieldType = 'string';
      } else if (this.isTime(trimmedField)) {
        fieldName = 'time';
        fieldType = 'datetime';
      } else if (this.isDate(trimmedField)) {
        fieldName = 'date';
        fieldType = 'datetime';
      } else if (this.isDirection(trimmedField)) {
        fieldName = 'direction';
        fieldType = 'string';
      }
      
      this.logParserConfig.fieldMapping.push({
        name: fieldName,
        index: index,
        type: fieldType,
        format: fieldType === 'datetime' ? this.logParserConfig.dateFormat : ''
      });
    });
    
    // Temel ayarları da güncelle
    this.updateBasicConfigFromSample(sampleData);
  }

  /**
   * Sayısal ID kontrolü
   */
  private isNumericId(value: string): boolean {
    return /^\d{4,}$/.test(value);
  }

  /**
   * Saat formatı kontrolü
   */
  private isTime(value: string): boolean {
    return /^\d{1,2}:\d{2}(:\d{2})?$/.test(value) || /^\d{4,6}$/.test(value);
  }

  /**
   * Tarih formatı kontrolü
   */
  private isDate(value: string): boolean {
    return /^\d{1,2}[./-]\d{1,2}[./-]\d{2,4}$/.test(value) || /^\d{6,8}$/.test(value);
  }

  /**
   * Yön kontrolü (1, 2, IN, OUT, vs)
   */
  private isDirection(value: string): boolean {
    return /^[12]$/.test(value) || /^(IN|OUT|ENTER|EXIT)$/i.test(value);
  }

  /**
   * Örnek veriye göre temel konfigürasyonu güncelle
   */
  private updateBasicConfigFromSample(sampleData: string): void {
    // Tarih formatını tespit et
    const dateFormats = [
      { pattern: /\d{2}\.\d{2}\.\d{4}/, format: 'dd.MM.yyyy' },
      { pattern: /\d{2}\/\d{2}\/\d{4}/, format: 'dd/MM/yyyy' },
      { pattern: /\d{4}-\d{2}-\d{2}/, format: 'yyyy-MM-dd' },
      { pattern: /\d{6}/, format: 'ddMMyy' }
    ];

    dateFormats.forEach(df => {
      if (df.pattern.test(sampleData)) {
        this.logParserConfig.dateFormat = df.format;
      }
    });

    // Saat formatını tespit et
    const timeFormats = [
      { pattern: /\d{2}:\d{2}:\d{2}/, format: 'HH:mm:ss' },
      { pattern: /\d{2}:\d{2}/, format: 'HH:mm' },
      { pattern: /\d{6}/, format: 'HHmmss' },
      { pattern: /\d{4}/, format: 'HHmm' }
    ];

    timeFormats.forEach(tf => {
      if (tf.pattern.test(sampleData)) {
        this.logParserConfig.timeFormat = tf.format;
      }
    });
  }

  /**
   * Log parser konfigürasyonunu kaydet (Modal'dan çağrılır)
   */
  saveLogParserConfiguration(): void {
    this.saveLogParserConfig();
  }
  // Log parser için gerekli özellikler
  delimiterOptions: SelectInputModel[] = [
    new SelectInputModel('Boşluk', ' '),
    new SelectInputModel('Virgül', ','),
    new SelectInputModel('Tab', '\t'),
    new SelectInputModel('Noktalı Virgül', ';'),
    new SelectInputModel('Dikey Çizgi', '|'),
  ];
  
  dateFormatOptions: SelectInputModel[] = [
    new SelectInputModel('YYYY-MM-DD', 'yyyy-MM-dd'),
    new SelectInputModel('DD/MM/YYYY', 'dd/MM/yyyy'),
    new SelectInputModel('MM/DD/YYYY', 'MM/dd/yyyy'),
    new SelectInputModel('DD.MM.YYYY', 'dd.MM.yyyy'),
  ];
  
  timeFormatOptions: SelectInputModel[] = [
    new SelectInputModel('HH:mm:ss', 'HH:mm:ss'),
    new SelectInputModel('HH:mm', 'HH:mm'),
    new SelectInputModel('hh:mm:ss a', 'hh:mm:ss a'),
    new SelectInputModel('hh:mm a', 'hh:mm a'),
  ];
  
  predefinedFields: SelectInputModel[] = [
    new SelectInputModel('Kullanıcı ID', 'userId'),
    new SelectInputModel('Tarih', 'date'),
    new SelectInputModel('Saat', 'time'),
    new SelectInputModel('Yön (Giriş/Çıkış)', 'direction'),
    new SelectInputModel('Cihaz ID', 'deviceId'),
    new SelectInputModel('Alarm Tipi', 'alarmType'),
    new SelectInputModel('Alarm Seviyesi', 'alarmLevel'),
    new SelectInputModel('Kamera ID', 'cameraId'),
    new SelectInputModel('Olay Tipi', 'eventType'),
  ];
  
  fieldTypes: SelectInputModel[] = [
    new SelectInputModel('Metin', 'string'),
    new SelectInputModel('Sayı', 'number'),
    new SelectInputModel('Tarih', 'date'),
    new SelectInputModel('Saat', 'time'),
    new SelectInputModel('Tarih ve Saat', 'datetime'),
    new SelectInputModel('Boolean', 'boolean'),
  ];
  
  // Log parser yardım paneli için
  logParserHelpVisible = false;
  
  closeLogParserModal() {
    this.logParserModalVisible = false;
  }
  
  toggleLogParserHelp() {
    this.logParserHelpVisible = !this.logParserHelpVisible;
  }
    loadLogParserSampleData() {
    // Örnek veri yükle
    if (this.selectedFirmaCihazForLogParser && this.selectedFirmaCihazForLogParser.logSampleData) {
      this.logParserSampleData = this.selectedFirmaCihazForLogParser.logSampleData;
    } else {
      this.logParserSampleData = 'Örnek log satırları buraya yüklenecek...';
    }
  }
    testLogParserConfiguration() {
    if (!this.logParserSampleData) {
      this.messageService.add({
        severity: 'error',
        summary: 'Test Edilemedi',
        detail: 'Test için örnek log verisi gereklidir.'
      });
      return;
    }
    
    const config = this.getLogParserConfig();
    this.logParserService.testLogParserConfig(config, this.logParserSampleData)
      .subscribe({
        next: (result) => {
          this.logParserTestResults = result;
          this.messageService.add({
            severity: 'success', 
            summary: 'Test Başarılı',
            detail: `Test başarılı: ${result.message}`
          });
        },
        error: (err) => {
          this.messageService.add({
            severity: 'error',
            summary: 'Test Başarısız',
            detail: err.message || 'Log ayrıştırma yapılandırması test edilirken hata oluştu.'
          });
        }
      });
  }
  
  getLogParserConfigAsJson(): string {
    const config = this.getLogParserConfig();
    return JSON.stringify(config, null, 2);
  }

  // Log parser konfigurasyon getter
  getLogParserConfig(): LogParserConfigDto {
    // Burada formdan alınan değerlerle konfigürasyonu oluşturuyoruz
    // Gerçek implementasyon için formdaki değerler kullanılmalı
    return {
      delimiter: this.selectedFirmaCihazForLogParser?.logDelimiter || ',',
      dateFormat: this.selectedFirmaCihazForLogParser?.logDateFormat || 'yyyy-MM-dd',
      timeFormat: this.selectedFirmaCihazForLogParser?.logTimeFormat || 'HH:mm:ss',
      fieldMapping: this.logParserConfig?.fieldMapping || []
    };
  }
  /**
   * Şablon uygulandıktan sonra dropdown eşleşmelerini günceller
   * Şablon uygulandıktan sonra zaman zaman dropdown değerleri görünmeyebilir,
   * bu fonksiyon dropdown değerlerinin doğru şekilde gösterilmesini sağlar
   */
  updateDropdownSelections(): void {
    // timeout ile Angular change detection'ı zorlamak için
    setTimeout(() => {
      // Delimiter güncellemesi
      if (this.logParserConfig.delimiter) {
        const delimiterMatch = this.delimiterOptions.find(o => o.value === this.logParserConfig.delimiter);
        if (!delimiterMatch) {
          // Eşleşme yoksa yeni bir seçenek ekle
          this.delimiterOptions.push(
            new SelectInputModel(`Özel (${this.logParserConfig.delimiter})`, this.logParserConfig.delimiter)
          );
        }
      }

      // Tarih formatı güncellemesi
      if (this.logParserConfig.dateFormat) {
        const dateFormatMatch = this.dateFormatOptions.find(o => o.value === this.logParserConfig.dateFormat);
        if (!dateFormatMatch) {
          // Eşleşme yoksa yeni bir seçenek ekle
          this.dateFormatOptions.push(
            new SelectInputModel(`Özel (${this.logParserConfig.dateFormat})`, this.logParserConfig.dateFormat)
          );
        }
      }

      // Saat formatı güncellemesi
      if (this.logParserConfig.timeFormat) {
        const timeFormatMatch = this.timeFormatOptions.find(o => o.value === this.logParserConfig.timeFormat);
        if (!timeFormatMatch) {
          // Eşleşme yoksa yeni bir seçenek ekle
          this.timeFormatOptions.push(
            new SelectInputModel(`Özel (${this.logParserConfig.timeFormat})`, this.logParserConfig.timeFormat)
          );
        }
      }

      // Alan eşleşmelerindeki tip güncellemeleri
      if (this.logParserConfig.fieldMapping && this.logParserConfig.fieldMapping.length > 0) {
        this.logParserConfig.fieldMapping.forEach(mapping => {
          // Boş tip kontrolü
          if (!mapping.type || mapping.type.trim() === '') {
            mapping.type = 'string'; // Default tip ata
          } else {
            // Tip eşleşmesi kontrolü
            const typeMatch = this.fieldTypes.find(o => o.value === mapping.type);
            if (!typeMatch) {
              // Eşleşme yoksa yeni bir seçenek ekle
              this.fieldTypes.push(
                new SelectInputModel(`Özel (${mapping.type})`, mapping.type)
              );
            }
          }

          // Alan adı boş kontrolü
          if (!mapping.name || mapping.name.trim() === '') {
            const predefinedOption = this.predefinedFields[0]; // İlk öntanımlı alanı seç
            if (predefinedOption) {
              mapping.name = predefinedOption.value;
            } else {
              mapping.name = 'field_' + mapping.index;
            }
          }
        });
      }

      // ConfigRef'i güncelle
      this.logParserConfig = {...this.logParserConfig};
    }, 100);
  }
}