import { Component, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { finalize } from 'rxjs';

// PrimeNG Modülleri
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { TooltipModule } from 'primeng/tooltip';
import { DropdownModule } from 'primeng/dropdown';
import { PanelModule } from 'primeng/panel';
import { AccordionModule } from 'primeng/accordion';
import { DividerModule } from 'primeng/divider';
import { MessageModule } from 'primeng/message';
import { MessagesModule } from 'primeng/messages';

// Ortak Bileşenler
import { TextInputComponent } from '../../../core/components/text-input/text-input.component';
import { ButtonComponent } from '../../../core/components/button/button.component';

// Servis ve Modeller
import { LogParserService } from '../../../core/services/modules/log-parser.service';
import { FirmaCihazService } from '../../../core/services/modules/firmacihaz.service';
import { LogParserConfigDto, FieldMappingDto, LogParserTestResultDto } from '../../../core/models/LogParserDto';
import { FirmaCihazDto } from '../../../core/models/FirmaCihazDto';

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
    DropdownModule,
    PanelModule,
    AccordionModule,
    DividerModule,
    MessageModule,
    MessagesModule,
    TextInputComponent,
    ButtonComponent
  ]
})
export class LogParserComponent implements OnInit {
  // Firma cihaz listesi
  firmaCihazList: FirmaCihazDto[] = [];
  selectedFirmaCihaz: FirmaCihazDto | null = null;
  
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
  isLoading = false;
  isSaving = false;
  isTesting = false;
  
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

  constructor(
    private logParserService: LogParserService,
    private firmaCihazService: FirmaCihazService
  ) {}

  ngOnInit(): void {
    this.loadFirmaCihazList();
    this.addFieldMapping(); // İlk alan mapping'ini ekle
  }

  /**
   * Firma cihaz listesini yükler
   */
  loadFirmaCihazList(): void {
    this.firmaCihazService.getAllFirmaCihazList().subscribe({
      next: (data) => {
        this.firmaCihazList = data || [];
      },
      error: (error) => {
        console.error('Firma cihaz listesi yüklenirken hata oluştu:', error);
      }
    });
  }

  /**
   * Seçili cihazın konfigürasyonunu yükler
   */
  onFirmaCihazSelect(): void {
    if (!this.selectedFirmaCihaz?.eid) return;
    
    this.isLoading = true;
    this.logParserService.getLogParserConfig(this.selectedFirmaCihaz.eid)
      .pipe(finalize(() => this.isLoading = false))
      .subscribe({
        next: (config) => {
          if (config) {
            this.logParserConfig = config;
            // Eğer field mapping yoksa en az bir tane ekle
            if (!this.logParserConfig.fieldMapping || this.logParserConfig.fieldMapping.length === 0) {
              this.addFieldMapping();
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
  addFieldMapping(): void {
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
  removeFieldMapping(index: number): void {
    if (this.logParserConfig.fieldMapping && index >= 0 && index < this.logParserConfig.fieldMapping.length) {
      this.logParserConfig.fieldMapping.splice(index, 1);
      
      // Index'leri yeniden düzenle
      this.logParserConfig.fieldMapping.forEach((field, i) => {
        field.index = i;
      });
    }
  }

  /**
   * Konfigürasyonu test eder
   */
  testConfiguration(): void {
    if (!this.sampleLogData.trim()) {
      alert('Lütfen önce örnek log verisi girin.');
      return;
    }

    this.isTesting = true;
    this.testResult = null;
    
    this.logParserService.testLogParserConfig(this.logParserConfig, this.sampleLogData)
      .pipe(finalize(() => this.isTesting = false))
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
   * Konfigürasyonu kaydeder
   */
  saveConfiguration(): void {
    if (!this.selectedFirmaCihaz?.eid) {
      alert('Lütfen önce bir firma cihazı seçin.');
      return;
    }

    this.isSaving = true;
    
    this.logParserService.updateLogParserConfig(this.selectedFirmaCihaz.eid, this.logParserConfig)
      .pipe(finalize(() => this.isSaving = false))
      .subscribe({
        next: (success) => {
          if (success) {
            alert('Konfigürasyon başarıyla kaydedildi.');
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
   * Örnek veri yükler
   */
  loadSampleData(): void {
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
   * Tüm alanları temizler
   */
  clearAllFields(): void {
    this.logParserConfig.fieldMapping = [];
    this.addFieldMapping();
  }

  /**
   * Konfigürasyonu sıfırlar
   */
  resetConfiguration(): void {
    this.logParserConfig = {
      delimiter: ',',
      dateFormat: 'dd.MM.yyyy HH:mm:ss',
      timeFormat: 'HH:mm:ss',
      fieldMapping: []
    };
    this.addFieldMapping();
    this.sampleLogData = '';
    this.testResult = null;
  }

  /**
   * JSON formatında göster
   */
  getConfigAsJson(): string {
    return JSON.stringify(this.logParserConfig, null, 2);
  }

  /**
   * Test sonucunu JSON formatında göster
   */
  getTestResultAsJson(): string {
    return this.testResult ? JSON.stringify(this.testResult.parsedData, null, 2) : '';
  }
}
