import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { finalize, BehaviorSubject } from 'rxjs';

// PrimeNG Modülleri
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { TooltipModule } from 'primeng/tooltip';
import { TabViewModule } from 'primeng/tabview';
import { PanelModule } from 'primeng/panel';
import { AccordionModule } from 'primeng/accordion';
import { DividerModule } from 'primeng/divider';
import { MessageService } from 'primeng/api';

// Ortak Bileşenler
import { TextInputComponent } from '../../../core/components/text-input/text-input.component';
import { ButtonComponent } from '../../../core/components/button/button.component';
import { SelectComponent } from '../../../core/components/select/select.component';
import { SelectInputModel } from '../../../core/components/select/select-input.model';

// Servis ve Modeller
import { FirmaCihazService } from '../../../core/services/modules/firmacihaz.service';
import { KodService } from '../../../core/services/modules/kod.service';
import { LogParserService } from '../../../core/services/modules/log-parser.service';
import { FirmaCihazDto } from '../../../core/models/FirmaCihazDto';
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

@Component({
  selector: 'app-log-parser',
  templateUrl: './log-parser.component.html',
  styleUrls: ['./log-parser.component.scss'],
  standalone: true,  imports: [
    CommonModule,
    FormsModule,
    ButtonModule,
    InputTextModule,
    InputTextareaModule,
    TooltipModule,
    TabViewModule,
    PanelModule,
    AccordionModule,
    DividerModule,
    SelectComponent
  ],
  providers: [MessageService]
})
export class LogParserComponent implements OnInit {
  firmaCihaz: FirmaCihazDto | null = null;
  firmaCihazEid: string = '';
  
  logParserSampleData: string = '';
  logParserTestResults: LogParserTestResultDto | null = null;
  
  // Select component için BehaviorSubject'ler
  delimiterOptions$ = new BehaviorSubject<SelectInputModel[]>([]);
  dateFormatOptions$ = new BehaviorSubject<SelectInputModel[]>([]);
  timeFormatOptions$ = new BehaviorSubject<SelectInputModel[]>([]);
  fieldTypes$ = new BehaviorSubject<SelectInputModel[]>([]);
  predefinedFields$ = new BehaviorSubject<SelectInputModel[]>([]);
  
  // Log parser konfigürasyon
  logParserConfig: LogParserConfigDto = {
    delimiter: ',',
    dateFormat: 'yyyy-MM-dd',
    timeFormat: 'HH:mm:ss',
    fieldMapping: [
      { name: 'userId', index: 0, type: 'string' }
    ]
  };
  
  availableTemplates: LogParserTemplateDto[] = [];
  selectedTemplate: LogParserTemplateDto | null = null;
  templateFilter: 'ALL' | 'PDKS' | 'ALARM' | 'KAMERA' = 'ALL';
  
  jsonConfig: string = '';
  testResult: LogParserTestResultDto | null = null;
  
  isLoading = false;
  isSaving = false;
  isTesting = false;
  
  activeTabIndex: number = 0;
  showHelp = false;
  
  constructor(
    private firmaCihazService: FirmaCihazService,
    private logParserService: LogParserService,
    private kodService: KodService,
    private route: ActivatedRoute,
    private router: Router,
    private messageService: MessageService
  ) { }

  ngOnInit(): void {
    // URL'den cihaz ID'sini al
    this.route.params.subscribe(params => {
      this.firmaCihazEid = params['eid'] || '';
      if (this.firmaCihazEid) {
        this.loadFirmaCihazData();
      } else {
        this.messageService.add({
          severity: 'error',
          summary: 'Hata',
          detail: 'Cihaz bilgisi bulunamadı!'
        });
        this.router.navigate(['/firma/firmacihaz']);
      }
    });
    
    // Dropdown değerlerini t_kod tablosundan yükle
    this.loadDropdownOptions();
    
    // Şablonları yükle
    this.loadTemplates();
  }
  
  /**
   * Firma cihaz bilgilerini yükler
   */
  loadFirmaCihazData(): void {
    this.isLoading = true;
    
    this.firmaCihazService.getFirmaCihazById(this.firmaCihazEid)
      .pipe(finalize(() => this.isLoading = false))
      .subscribe({
        next: (res) => {
          this.firmaCihaz = res;
          this.getLogParserConfig();
        },
        error: (err) => {
          console.error('Firma cihazı yüklenirken hata:', err);
          this.messageService.add({
            severity: 'error',
            summary: 'Hata',
            detail: 'Cihaz bilgisi yüklenirken bir hata oluştu.'
          });
        }
      });
  }
  
  /**
   * Log parser konfigürasyonunu yükler
   */
  getLogParserConfig(): void {
    this.isLoading = true;
    
    this.logParserService.getLogParserConfig(this.firmaCihazEid)
      .pipe(finalize(() => this.isLoading = false))
      .subscribe({
        next: (config) => {
          this.logParserConfig = config;
          this.logParserSampleData = config.sampleLogData || '';
          this.updateJsonDisplay();
          this.updateDropdownSelections();
        },
        error: (err) => {
          console.error('Log parser konfigürasyonu yüklenirken hata:', err);
          this.messageService.add({
            severity: 'error',
            summary: 'Hata',
            detail: 'Log parser ayarları yüklenirken bir hata oluştu.'
          });
        }
      });
  }
    /**
   * t_kod tablosundan dropdown değerlerini yükler
   */  loadDropdownOptions(): void {
    // LogParserService içindeki metotları kullanarak t_kod tablosundan veriler çekilir
    this.logParserService.getDelimiterOptions().subscribe(
      options => this.delimiterOptions$.next(options)
    );
    
    this.logParserService.getDateFormatOptions().subscribe(
      options => this.dateFormatOptions$.next(options)
    );
    
    this.logParserService.getTimeFormatOptions().subscribe(
      options => this.timeFormatOptions$.next(options)
    );
    
    this.logParserService.getFieldTypeOptions().subscribe(
      options => this.fieldTypes$.next(options)
    );
    
    this.logParserService.getPredefinedFieldOptions().subscribe(
      options => this.predefinedFields$.next(options)
    );
  }
  
  /**
   * Dropdown seçimlerini config değerlerine göre günceller
   */  /**
   * Dropdown seçimlerini config değerlerine göre günceller
   * t_kod'dan yüklenen değerlerle mevcut logParserConfig değerlerini eşleştirir
   */
  updateDropdownSelections(): void {
    console.log('Dropdown seçimleri güncelleniyor...', this.logParserConfig);
    
    // Delimiter options ile mevcut delimiter değerini eşleştir
    this.delimiterOptions$.subscribe(options => {
      if (options && options.length > 0 && this.logParserConfig.delimiter) {
        // Tam eşleşme kontrolü
        const match = options.find(opt => 
          opt.key === this.logParserConfig.delimiter || 
          opt.value.includes(this.logParserConfig.delimiter)
        );
        
        if (match) {
          // Eşleşen değeri kullan
          this.logParserConfig.delimiter = match.value;
        }
      }
    });
    
    // Date format options ile mevcut dateFormat değerini eşleştir
    this.dateFormatOptions$.subscribe(options => {
      if (options && options.length > 0 && this.logParserConfig.dateFormat) {
        const match = options.find(opt => 
          opt.key === this.logParserConfig.dateFormat || 
          opt.value.includes(this.logParserConfig.dateFormat)
        );
        
        if (match) {
          this.logParserConfig.dateFormat = match.value;
        }
      }
    });
    
    // Time format options ile mevcut timeFormat değerini eşleştir
    this.timeFormatOptions$.subscribe(options => {
      if (options && options.length > 0 && this.logParserConfig.timeFormat) {
        const match = options.find(opt => 
          opt.key === this.logParserConfig.timeFormat || 
          opt.value.includes(this.logParserConfig.timeFormat)
        );
        
        if (match) {
          this.logParserConfig.timeFormat = match.value;
        }
      }
    });
    
    // Field type options ile mevcut fieldMapping.type değerlerini eşleştir
    this.fieldTypes$.subscribe(options => {
      if (options && options.length > 0 && this.logParserConfig.fieldMapping) {
        this.logParserConfig.fieldMapping.forEach(field => {
          if (field.type) {
            const match = options.find(opt => 
              opt.key === field.type || 
              opt.value.includes(field.type)
            );
            
            if (match) {
              field.type = match.value;
            }
          }
        });
      }
    });
  }
  
  /**
   * JSON gösterimini günceller
   */
  updateJsonDisplay(): void {
    try {
      const configCopy = { ...this.logParserConfig };
      this.jsonConfig = JSON.stringify(configCopy, null, 2);
    } catch (e) {
      console.error('JSON oluşturma hatası:', e);
      this.jsonConfig = '{}';
    }
  }
  
  /**
   * Şablonları yükler
   */
  loadTemplates(): void {
    const deviceType = this.firmaCihaz?.firmaCihazTipKodDto?.kod || '';
    
    if (deviceType) {
      this.logParserService.getTemplatesByDeviceType(deviceType)
        .subscribe({
          next: (templates) => {
            this.availableTemplates = templates;
          },
          error: (err) => {
            console.error('Şablonlar yüklenirken hata:', err);
            this.messageService.add({
              severity: 'error',
              summary: 'Hata',
              detail: 'Şablonlar yüklenirken bir hata oluştu.'
            });
          }
        });
    } else {
      this.logParserService.getSystemTemplates()
        .subscribe({
          next: (templates) => {
            this.availableTemplates = templates;
          },
          error: (err) => {
            console.error('Şablonlar yüklenirken hata:', err);
          }
        });
    }
  }
  
  /**
   * Önerilen şablonları getirir
   */
  getRecommendedTemplates(): LogParserTemplateDto[] {
    if (!this.firmaCihaz?.firmaCihazTipKodDto?.kod) return [];
    
    return this.availableTemplates.filter(t => 
      t.deviceType === this.firmaCihaz?.firmaCihazTipKodDto?.kod
    );
  }
  
  /**
   * Filtrelenmiş şablonları getirir
   */
  getFilteredTemplates(): LogParserTemplateDto[] {
    if (this.templateFilter === 'ALL') {
      return this.availableTemplates;
    }
    return this.availableTemplates.filter(t => t.deviceType === this.templateFilter);
  }
  
  /**
   * Şablon seçer
   */
  selectTemplate(template: LogParserTemplateDto): void {
    this.selectedTemplate = template;
  }
  
  /**
   * Seçilen şablonu uygular
   */
  applyTemplate(template: LogParserTemplateDto): void {
    try {
      const fieldMapping = JSON.parse(template.logFieldMapping);
      
      this.logParserConfig = {
        delimiter: template.logDelimiter,
        dateFormat: template.logDateFormat,
        timeFormat: template.logTimeFormat,
        fieldMapping: fieldMapping,
        sampleLogData: template.sampleLogData
      };
      
      this.logParserSampleData = template.sampleLogData;
      this.updateJsonDisplay();
      this.updateDropdownSelections();
      
      this.messageService.add({
        severity: 'success',
        summary: 'Başarılı',
        detail: `"${template.templateName}" şablonu uygulandı.`
      });
      
    } catch (err) {
      console.error('Şablon uygulama hatası:', err);
      this.messageService.add({
        severity: 'error',
        summary: 'Hata',
        detail: 'Şablon uygulanırken bir hata oluştu.'
      });
    }
  }
  
  /**
   * Yeni bir alan eşleştirmesi ekler
   */
  addFieldMapping(): void {
    this.logParserConfig.fieldMapping.push({
      name: '',
      index: this.logParserConfig.fieldMapping.length,
      type: 'string'
    });
  }
  
  /**
   * Alan eşleştirmesini siler
   */
  removeFieldMapping(index: number): void {
    if (this.logParserConfig.fieldMapping.length > 1) {
      this.logParserConfig.fieldMapping.splice(index, 1);
      
      // Yeniden indeks ataması
      this.logParserConfig.fieldMapping.forEach((field, i) => {
        field.index = i;
      });
    } else {
      this.messageService.add({
        severity: 'warn',
        summary: 'Uyarı',
        detail: 'En az bir alan eşleştirmesi olmalıdır.'
      });
    }
  }
  
  /**
   * Log parser konfigürasyonunu test eder
   */
  testLogParserConfig(): void {
    if (!this.logParserSampleData) {
      this.messageService.add({
        severity: 'warn',
        summary: 'Uyarı',
        detail: 'Lütfen test için örnek log verisi girin.'
      });
      return;
    }
    
    this.isTesting = true;
    
    this.logParserService.testLogParserConfig(this.logParserConfig, this.logParserSampleData)
      .pipe(finalize(() => this.isTesting = false))
      .subscribe({
        next: (result) => {
          this.testResult = result;
          
          if (result.success) {
            this.messageService.add({
              severity: 'success',
              summary: 'Başarılı',
              detail: 'Log parser testi başarılı!'
            });
          } else {
            this.messageService.add({
              severity: 'error',
              summary: 'Hata',
              detail: `Test başarısız: ${result.message}`
            });
          }
        },
        error: (err) => {
          console.error('Log parser testi yapılırken hata:', err);
          this.messageService.add({
            severity: 'error',
            summary: 'Hata',
            detail: 'Test sırasında bir hata oluştu.'
          });
        }
      });
  }
  
  /**
   * Log parser konfigürasyonunu kaydeder
   */
  saveLogParserConfig(): void {
    if (!this.firmaCihazEid) {
      this.messageService.add({
        severity: 'error',
        summary: 'Hata',
        detail: 'Cihaz bilgisi eksik!'
      });
      return;
    }
    
    if (this.logParserConfig.fieldMapping.length === 0) {
      this.messageService.add({
        severity: 'warn',
        summary: 'Uyarı',
        detail: 'En az bir alan eşleştirmesi olmalıdır.'
      });
      return;
    }
    
    this.isSaving = true;
    
    // Örnek veriyi güncelle
    this.logParserConfig.sampleLogData = this.logParserSampleData;
    
    this.logParserService.updateLogParserConfig(this.firmaCihazEid, this.logParserConfig)
      .pipe(finalize(() => this.isSaving = false))
      .subscribe({
        next: (result) => {
          if (result) {
            this.messageService.add({
              severity: 'success',
              summary: 'Başarılı',
              detail: 'Log parser ayarları kaydedildi.'
            });
          } else {
            this.messageService.add({
              severity: 'error',
              summary: 'Hata',
              detail: 'Log parser ayarları kaydedilemedi.'
            });
          }
        },
        error: (err) => {
          console.error('Log parser ayarları kaydedilirken hata:', err);
          this.messageService.add({
            severity: 'error',
            summary: 'Hata',
            detail: 'Kayıt sırasında bir hata oluştu.'
          });
        }
      });
  }
  
  /**
   * Akıllı örnek veri yükler
   */
  loadSampleData(): void {
    if (!this.logParserConfig.delimiter) {
      this.messageService.add({
        severity: 'warn',
        summary: 'Uyarı', 
        detail: 'Lütfen önce bir delimiter seçin.'
      });
      return;
    }
    
    // Delimiter'a göre örnek veri oluştur
    const delimiter = this.logParserConfig.delimiter;
    const sampleData = `userId${delimiter}date${delimiter}time${delimiter}status${delimiter}action\n` +
                      `user123${delimiter}2025-06-15${delimiter}10:30:45${delimiter}success${delimiter}login\n` +
                      `admin456${delimiter}2025-06-15${delimiter}11:45:22${delimiter}success${delimiter}logout`;
    
    this.logParserSampleData = sampleData;
    
    // Örnek veriye göre alan eşlemeleri oluştur
    const headers = sampleData.split('\n')[0].split(delimiter);
    
    this.logParserConfig.fieldMapping = headers.map((header, index) => ({
      name: header.trim(),
      index: index,
      type: (header === 'date') ? 'date' : 
            (header === 'time') ? 'time' : 'string'
    }));
    
    this.updateJsonDisplay();
    this.updateDropdownSelections();
  }
  
  /**
   * Ana sayfaya geri döner
   */
  goBack(): void {
    this.router.navigate(['/firma/firmacihaz']);
  }
  
  /**
   * Toogle yardım görünümü
   */
  toggleHelp(): void {
    this.showHelp = !this.showHelp;
  }
}
