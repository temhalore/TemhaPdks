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
  standalone: true,  
  imports: [
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
   */  
  loadDropdownOptions(): void {
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
   * t_kod'dan yüklenen değerlerle mevcut logParserConfig değerlerini eşleştirir
   */
  updateDropdownSelections(): void {
    console.log('Dropdown seçimleri güncelleniyor...', this.logParserConfig);
    
    // Delimiter options ile mevcut delimiter değerini eşleştir
    this.delimiterOptions$.subscribe(options => {
      if (options && options.length > 0 && this.logParserConfig.delimiter) {
        // Tam eşleşme kontrolü - kod veya kisaAd değerine göre
        const originalValue = this.logParserConfig.delimiter;
        const match = options.find(opt => 
          opt.key === originalValue || 
          opt.value.includes(originalValue) ||
          // Kod kısmı eşleşiyor mu?
          (originalValue.includes('(') && opt.value.split('(')[0].trim() === originalValue.split('(')[0].trim()) || 
          // Kısa kod veya isim eşleşiyor mu?
          originalValue === ',' && opt.value.includes('Virgül') ||
          originalValue === ';' && opt.value.includes('Noktalı Virgül') ||
          originalValue === '\t' && opt.value.includes('Tab') ||
          originalValue === ' ' && opt.value.includes('Boşluk')
        );
        
        if (match) {
          // Eşleşen değeri kullan
          console.log(`Delimiter eşleşti: ${originalValue} -> ${match.value}`);
          this.logParserConfig.delimiter = match.value;
        } else {
          console.warn(`Delimiter eşleşmedi: ${originalValue}`, options);
        }
      }
    });
    
    // Date format options ile mevcut dateFormat değerini eşleştir
    this.dateFormatOptions$.subscribe(options => {
      if (options && options.length > 0 && this.logParserConfig.dateFormat) {
        const originalValue = this.logParserConfig.dateFormat;
        const match = options.find(opt => 
          opt.key === originalValue || 
          opt.value.includes(originalValue) ||
          // Kod kısmı eşleşiyor mu?
          (originalValue.includes('(') && opt.value.split('(')[0].trim() === originalValue.split('(')[0].trim()) ||
          // Format değerleri karşılaştırması
          originalValue.toLowerCase() === 'yyyy-mm-dd' && opt.value.toLowerCase().includes('yyyy-mm-dd') ||
          originalValue.toLowerCase() === 'dd.mm.yyyy' && opt.value.toLowerCase().includes('dd.mm.yyyy') ||
          originalValue.toLowerCase() === 'dd/mm/yyyy' && opt.value.toLowerCase().includes('dd/mm/yyyy')
        );
        
        if (match) {
          console.log(`Date format eşleşti: ${originalValue} -> ${match.value}`);
          this.logParserConfig.dateFormat = match.value;
        } else {
          console.warn(`Date format eşleşmedi: ${originalValue}`, options);
        }
      }
    });
    
    // Time format options ile mevcut timeFormat değerini eşleştir
    this.timeFormatOptions$.subscribe(options => {
      if (options && options.length > 0 && this.logParserConfig.timeFormat) {
        const originalValue = this.logParserConfig.timeFormat;
        const match = options.find(opt => 
          opt.key === originalValue || 
          opt.value.includes(originalValue) ||
          // Kod kısmı eşleşiyor mu?
          (originalValue.includes('(') && opt.value.split('(')[0].trim() === originalValue.split('(')[0].trim()) ||
          // Format değerleri karşılaştırması
          originalValue.toLowerCase() === 'hh:mm:ss' && opt.value.toLowerCase().includes('hh:mm:ss') ||
          originalValue.toLowerCase() === 'hh:mm' && opt.value.toLowerCase().includes('hh:mm')
        );
        
        if (match) {
          console.log(`Time format eşleşti: ${originalValue} -> ${match.value}`);
          this.logParserConfig.timeFormat = match.value;
        } else {
          console.warn(`Time format eşleşmedi: ${originalValue}`, options);
        }
      }
    });
    
    // Field type options ile mevcut fieldMapping.type değerlerini eşleştir
    this.fieldTypes$.subscribe(options => {
      if (options && options.length > 0 && this.logParserConfig.fieldMapping) {
        this.logParserConfig.fieldMapping.forEach(field => {
          if (field.type) {
            const originalType = field.type;
            const match = options.find(opt => 
              opt.key === originalType || 
              opt.value.includes(originalType) ||
              // Tür karşılaştırması (kod adına göre)
              originalType.toLowerCase() === 'string' && opt.value.toLowerCase().includes('string') ||
              originalType.toLowerCase() === 'number' && opt.value.toLowerCase().includes('number') ||
              originalType.toLowerCase() === 'date' && opt.value.toLowerCase().includes('date') ||
              originalType.toLowerCase() === 'time' && opt.value.toLowerCase().includes('time') ||
              originalType.toLowerCase() === 'boolean' && opt.value.toLowerCase().includes('boolean')
            );
            
            if (match) {
              console.log(`Field type eşleşti: ${field.name} - ${originalType} -> ${match.value}`);
              field.type = match.value;
            } else {
              console.warn(`Field type eşleşmedi: ${originalType}`, options);
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
      console.log('Şablon uygulanıyor:', template);
      console.log('Alan eşlemeleri:', fieldMapping);
      
      // t_kod tablosundan KodDto değerlerini almak için
      const delimiterPromise = this.logParserService.getDelimiterOptions().toPromise();
      const dateFormatPromise = this.logParserService.getDateFormatOptions().toPromise();
      const timeFormatPromise = this.logParserService.getTimeFormatOptions().toPromise();
      const fieldTypesPromise = this.logParserService.getFieldTypeOptions().toPromise();
      
      // Tüm sorgular tamamlandığında
      Promise.all([delimiterPromise, dateFormatPromise, timeFormatPromise, fieldTypesPromise])
        .then(([delimiterOptions, dateFormatOptions, timeFormatOptions, fieldTypes]) => {
          // Gelen şablon değerlerini t_kod formatına dönüştür
          console.log('T_kod değerleri alındı:', { delimiterOptions, dateFormatOptions, timeFormatOptions, fieldTypes });
          
          // Delimiter bulma
          const matchDelimiter = delimiterOptions?.find(item => 
            item.value.toLowerCase().includes(template.logDelimiter.toLowerCase()) || 
            item.key.toLowerCase() === template.logDelimiter.toLowerCase() ||
            // Özel karakterler için kontrol
            (template.logDelimiter === ',' && item.value.includes('Virgül')) ||
            (template.logDelimiter === ';' && item.value.includes('Noktalı Virgül')) ||
            (template.logDelimiter === '\t' && item.value.includes('Tab')) ||
            (template.logDelimiter === ' ' && item.value.includes('Boşluk'))
          );
          
          console.log('Delimiter eşleşmesi:', matchDelimiter);
          
          // Tarih format bulma
          const matchDateFormat = dateFormatOptions?.find(item => 
            item.value.toLowerCase().includes(template.logDateFormat.toLowerCase()) || 
            item.key.toLowerCase() === template.logDateFormat.toLowerCase() ||
            // Format eşleşmeleri
            template.logDateFormat.toLowerCase() === 'yyyy-mm-dd' && item.value.toLowerCase().includes('yyyy-mm-dd') ||
            template.logDateFormat.toLowerCase() === 'dd.mm.yyyy' && item.value.toLowerCase().includes('dd.mm.yyyy') ||
            template.logDateFormat.toLowerCase() === 'dd/mm/yyyy' && item.value.toLowerCase().includes('dd/mm/yyyy')
          );
          
          console.log('Tarih format eşleşmesi:', matchDateFormat);
          
          // Saat format bulma
          const matchTimeFormat = timeFormatOptions?.find(item => 
            item.value.toLowerCase().includes(template.logTimeFormat.toLowerCase()) || 
            item.key.toLowerCase() === template.logTimeFormat.toLowerCase() ||
            // Format eşleşmeleri
            template.logTimeFormat.toLowerCase() === 'hh:mm:ss' && item.value.toLowerCase().includes('hh:mm:ss') ||
            template.logTimeFormat.toLowerCase() === 'hh:mm' && item.value.toLowerCase().includes('hh:mm')
          );
          
          console.log('Saat format eşleşmesi:', matchTimeFormat);
          
          // Fonksiyonel bir t_kod uyumlu config oluştur
          this.logParserConfig = {
            delimiter: matchDelimiter ? matchDelimiter.value : template.logDelimiter,
            dateFormat: matchDateFormat ? matchDateFormat.value : template.logDateFormat,
            timeFormat: matchTimeFormat ? matchTimeFormat.value : template.logTimeFormat,
            fieldMapping: fieldMapping.map((field: any) => {
              let dataType = field.dataType || field.type || 'string';
              console.log(`Alan tipi dönüşümü: ${field.fieldName} - ${dataType}`);
              
              // Field tiplerini t_kod formatına dönüştür
              const matchType = fieldTypes?.find(item => 
                item.value.toLowerCase().includes(dataType.toLowerCase()) || 
                item.key.toLowerCase() === dataType.toLowerCase() ||
                // Tür eşleşmeleri
                dataType.toLowerCase() === 'string' && item.value.toLowerCase().includes('string') ||
                dataType.toLowerCase() === 'number' && item.value.toLowerCase().includes('number') ||
                dataType.toLowerCase() === 'date' && item.value.toLowerCase().includes('date') ||
                dataType.toLowerCase() === 'time' && item.value.toLowerCase().includes('time') ||
                dataType.toLowerCase() === 'boolean' && item.value.toLowerCase().includes('boolean')
              );
              
              console.log(`Alan tipi eşleşmesi (${field.fieldName}):`, matchType);
              
              return {
                name: field.fieldName || field.name || '',
                index: field.position || field.index || 0,
                type: matchType ? matchType.value : dataType,
                format: field.format || ''
              };
            }),
            sampleLogData: template.sampleLogData
          };
          
          console.log('Oluşturulan konfigürasyon:', this.logParserConfig);
          
          // Örnek veriyi temizle - LOG_SAMPLE verisinde Virgül (,) gibi metin içeriyor
          if (this.logParserConfig.sampleLogData && this.logParserConfig.sampleLogData.includes('Virgül (,)')) {
            const delimiter = ','; // Gerçek delimiter karakteri
            // Her satır için düzeltme yap
            const lines = this.logParserConfig.sampleLogData.split('\n');
            const correctedLines = lines.map(line => {
              return line.replace(/Virgül \(\,\)/g, delimiter);
            });
            this.logParserSampleData = correctedLines.join('\n');
          } else {
            this.logParserSampleData = template.sampleLogData;
          }
          
          this.updateJsonDisplay();
          
          // Değerler seçilir seçilmez comboları güncelle
          setTimeout(() => {
            // Dropdown değerlerinin Angular tarafından güncellenmesi için
            this.updateDropdownSelections();
            console.log('Son konfigürasyon:', this.logParserConfig);
          }, 100);
          
          this.messageService.add({
            severity: 'success',
            summary: 'Başarılı',
            detail: `"${template.templateName}" şablonu uygulandı.`
          });
        })
        .catch(error => {
          console.error('Dropdown değerleri alma hatası:', error);
          // Hata durumunda yine de temel değerleri ayarla
          this.logParserConfig = {
            delimiter: template.logDelimiter,
            dateFormat: template.logDateFormat,
            timeFormat: template.logTimeFormat,
            fieldMapping: fieldMapping.map((field: any) => ({
              name: field.fieldName || field.name || '',
              index: field.position || field.index || 0,
              type: field.dataType || field.type || 'string',
              format: field.format || ''
            })),
            sampleLogData: template.sampleLogData
          };
          this.logParserSampleData = template.sampleLogData;
          this.updateJsonDisplay();
          this.messageService.add({
            severity: 'warning',
            summary: 'Uyarı',
            detail: `"${template.templateName}" şablonu uygulandı fakat dropdown değerleri yüklenemedi.`
          });
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
    // Akıllı örnek veri yükleme fonksiyonu kaldırılmıştır - artık hazır şablonlar kullanıyoruz
  /**
   * Ana sayfaya geri döner
   */
  goBack(): void {
    this.router.navigate(['/firma/firmacihaz']);
  }
  
  /**
   * Toggle yardım görünümü
   */
  toggleHelp(): void {
    this.showHelp = !this.showHelp;
  }
}
