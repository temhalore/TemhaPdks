import { Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';
import { ApiService } from '../api.service';
import { KodService } from './kod.service';
import { LogParserConfigDto, LogParserTestResultDto } from '../../models/LogParserDto';
import { SelectInputModel } from '../../components/select/select-input.model';
import { KodDto } from '../../models/KodDto';

@Injectable({
  providedIn: 'root'
})
export class LogParserService {
  private endpoint = 'FirmaCihaz';

  // Kod tipleri için sabit değerler (t_kod_tip_tanimlari tablosundaki tip ID'ler)
  readonly KOD_TIP_LOG_DELIMITER = 215; // Log delimiter için kod tip ID
  readonly KOD_TIP_LOG_DATE_FORMAT = 216; // Log tarih formatı için kod tip ID
  readonly KOD_TIP_LOG_TIME_FORMAT = 217; // Log saat formatı için kod tip ID
  readonly KOD_TIP_LOG_FIELD_TYPE = 218; // Log alan tipleri için kod tip ID
  readonly KOD_TIP_LOG_PREDEFINED_FIELD = 219; // Log önceden tanımlı alanlar için kod tip ID

  constructor(
    private apiService: ApiService, 
    private kodService: KodService
  ) {}

  /**
   * Firma cihazının log parser konfigürasyonunu günceller
   * @param firmaCihazEid Firma cihazının encrypt edilmiş ID'si
   * @param config Log parser konfigürasyonu
   * @returns Observable<boolean>
   */
  updateLogParserConfig(firmaCihazEid: string, config: LogParserConfigDto): Observable<boolean> {
    // FieldMappings boş olmamalı
    if (!config.fieldMapping || config.fieldMapping.length === 0) {
      throw new Error('En az bir alan eşleşmesi gereklidir');
    }

    // Backend'in beklediği formata dönüştür
    const backendConfig = {
      Type: "standard",
      Delimiter: config.delimiter,
      DateFormat: config.dateFormat,
      TimeFormat: config.timeFormat,
      RegexPattern: config.regexPattern || "",
      FieldMappings: config.fieldMapping.map(fm => ({
        Field: fm.name,   // Backend'de Field olarak bekleniyor
        Name: fm.name,    // Tanımlayıcı isim 
        Position: fm.index,
        Index: fm.index,  // Backend'de hem Position hem Index kullanılıyor
        Type: fm.type,
        Format: fm.format || ""
      }))
    };

    console.log('Backend\'e gönderilecek config:', backendConfig);

    // KodDto ile çalışırken kod değerlerini gönderiyoruz (display değil)
    // Burada dropdown seçim değerlerinin kod alanları gönderilmeli
    return this.apiService.post<boolean>(`${this.endpoint}/updateLogConfig`, {
      eid: firmaCihazEid,  // Backend'de 'eid' bekleniyor
      logParserConfig: JSON.stringify(backendConfig),
      logDelimiter: this.extractKodValue(config.delimiter), // KodDto kod değeri
      logDateFormat: this.extractKodValue(config.dateFormat), // KodDto kod değeri
      logTimeFormat: this.extractKodValue(config.timeFormat), // KodDto kod değeri
      logFieldMapping: JSON.stringify(config.fieldMapping.map(fm => ({
        ...fm,
        type: this.extractKodValue(fm.type) // Alan tipi için KodDto kod değeri
      }))),
      sampleLogData: config.sampleLogData || ""
    });
  }

  /**
   * Log parser konfigürasyonunu test eder
   * @param config Log parser konfigürasyonu
   * @param sampleLogData Örnek log verisi
   * @returns Observable<LogParserTestResultDto>
   */
  testLogParserConfig(config: LogParserConfigDto, sampleLogData: string): Observable<LogParserTestResultDto> {
    return this.apiService.post<LogParserTestResultDto>(`${this.endpoint}/testLogConfig`, {
      logParserConfig: JSON.stringify(config),
      sampleLogData: sampleLogData  // Backend'de 'sampleLogData' bekleniyor
    });
  }

  /**
   * Firma cihazının log parser konfigürasyonunu getirir
   * @param firmaCihazEid Firma cihazının encrypt edilmiş ID'si
   * @returns Observable<LogParserConfigDto>
   */  
  getLogParserConfig(firmaCihazEid: string): Observable<LogParserConfigDto> {
    return new Observable<LogParserConfigDto>(observer => {
      this.apiService.post<any>(`${this.endpoint}/getLogConfig`, { eid: firmaCihazEid }).subscribe({
        next: (response) => {
          console.log('Backend\'den getLogConfig yanıtı:', response);
          
          try {
            // Frontend'deki LogParserConfigDto formatına dönüştür
            const frontendConfig: LogParserConfigDto = {
              // t_kod tablosundaki kod değerlerini kullanıyoruz
              delimiter: this.formatKodDisplay(response.logDelimiter, this.KOD_TIP_LOG_DELIMITER),
              dateFormat: this.formatKodDisplay(response.logDateFormat, this.KOD_TIP_LOG_DATE_FORMAT),
              timeFormat: this.formatKodDisplay(response.logTimeFormat, this.KOD_TIP_LOG_TIME_FORMAT),
              fieldMapping: [],
              sampleLogData: response.logSample || ''
            };
            
            // Field Mapping'i parse et
            if (response.logFieldMapping) {
              try {
                const fieldMappings = JSON.parse(response.logFieldMapping);
                console.log('Parsed fieldMappings:', fieldMappings);
                
                if (Array.isArray(fieldMappings)) {
                  frontendConfig.fieldMapping = fieldMappings.map((fm: any) => {
                    // Her field için detaylı log
                    console.log('Processing field:', fm);
                    
                    return {
                      name: fm.name || fm.Field || '',  
                      index: typeof fm.index === 'number' ? fm.index : 
                             typeof fm.Position === 'number' ? fm.Position : 0,
                      // Alan tipini t_kod formatına dönüştür
                      type: this.formatKodDisplay(fm.type || fm.Type || 'string', this.KOD_TIP_LOG_FIELD_TYPE),  
                      format: fm.format || fm.Format || ''  
                    };
                  });
                } else {
                  console.error('fieldMappings bir array değil:', fieldMappings);
                  frontendConfig.fieldMapping = [];
                }
              } catch (parseErr) {
                console.error('Field mapping parse hatası:', parseErr);
                frontendConfig.fieldMapping = [];
              }
            }
            
            // LogParserConfig alanındaki JSON'ı kontrol et ve varsa değerleri kullan
            if (response.logParserConfig) {
              try {
                const parsedConfig = JSON.parse(response.logParserConfig);
                console.log('Parsed logParserConfig:', parsedConfig);
                
                // Eksik alanları tamamla
                if (parsedConfig.Delimiter) {
                  frontendConfig.delimiter = this.formatKodDisplay(parsedConfig.Delimiter, this.KOD_TIP_LOG_DELIMITER);
                }
                if (parsedConfig.DateFormat) {
                  frontendConfig.dateFormat = this.formatKodDisplay(parsedConfig.DateFormat, this.KOD_TIP_LOG_DATE_FORMAT);
                }
                if (parsedConfig.TimeFormat) {
                  frontendConfig.timeFormat = this.formatKodDisplay(parsedConfig.TimeFormat, this.KOD_TIP_LOG_TIME_FORMAT);
                }
                if (parsedConfig.RegexPattern) {
                  frontendConfig.regexPattern = parsedConfig.RegexPattern;
                }
                
                // Field Mapping'leri kontrol et ve gerekiyorsa güncelle
                if (parsedConfig.FieldMappings && Array.isArray(parsedConfig.FieldMappings) && 
                   parsedConfig.FieldMappings.length > 0) {
                  
                  // Eğer frontend mapping boşsa veya backend'den daha detaylı bilgi geliyorsa güncelle
                  if (frontendConfig.fieldMapping.length === 0 || 
                      (frontendConfig.fieldMapping.length > 0 && 
                       frontendConfig.fieldMapping[0].name === '')) {
                       
                    frontendConfig.fieldMapping = parsedConfig.FieldMappings.map((fm: any) => ({
                      name: fm.Field || fm.Name || '',
                      index: typeof fm.Index === 'number' ? fm.Index : 
                             typeof fm.Position === 'number' ? fm.Position : 0,
                      type: this.formatKodDisplay(fm.Type || fm.type || 'string', this.KOD_TIP_LOG_FIELD_TYPE),
                      format: fm.Format || fm.format || ''
                    }));
                  }
                }
              } catch (parseErr) {
                console.error('LogParserConfig parse hatası:', parseErr);
              }
            }
            
            // Eğer mapping boşsa, en az bir mapping ekle
            if (!frontendConfig.fieldMapping || frontendConfig.fieldMapping.length === 0) {
              frontendConfig.fieldMapping = [{
                name: 'userId',
                index: 0,
                type: this.formatKodDisplay('string', this.KOD_TIP_LOG_FIELD_TYPE),
                format: ''
              }];
            }
            
            console.log('Frontend için dönüştürülmüş config:', frontendConfig);
            observer.next(frontendConfig);
            observer.complete();
            
          } catch (err) {
            console.error('Log parser config dönüşüm hatası:', err);
            observer.error('Log parser config dönüşüm hatası: ' + err.message);
          }
        },
        error: (err) => {
          console.error('getLogConfig API hatası:', err);
          observer.error(err);
        }
      });
    });
  }

  /**
   * Sistem şablonlarını getirir
   * @returns Observable<any[]>
   */
  getSystemTemplates(): Observable<any[]> {
    return this.apiService.get<any[]>(`${this.endpoint}/getSystemTemplates`);
  }

  /**
   * Cihaz tipine göre şablonları getirir
   * @param deviceType Cihaz tipi (PDKS, ALARM, KAMERA)
   * @returns Observable<any[]>
   */
  getTemplatesByDeviceType(deviceType: string): Observable<any[]> {
    return this.apiService.get<any[]>(`${this.endpoint}/getTemplatesByDeviceType/${deviceType}`);
  }

  /**
   * Şablondan konfigürasyon oluşturur
   * @param templateId Şablon ID'si
   * @returns Observable<string>
   */
  createConfigFromTemplate(templateId: number): Observable<string> {
    return this.apiService.get<string>(`${this.endpoint}/createConfigFromTemplate/${templateId}`);
  }

  /**
   * Akıllı örnek veri analizi yapar
   * @param sampleData Örnek log verisi
   * @returns Observable<any>
   */
  analyzeSampleData(sampleData: string): Observable<any> {
    return this.apiService.post<any>(`${this.endpoint}/analyzeSampleData`, { sampleData });
  }

  /**
   * T_kod tablosundan Delimiter seçeneklerini SelectInputModel[] formatında getirir
   */
  getDelimiterOptions(): Observable<SelectInputModel[]> {
    return this.kodService.getKodListByTipId(this.KOD_TIP_LOG_DELIMITER).pipe(
      map((items: KodDto[]) => {
        return items.map(item => new SelectInputModel(item.kisaAd, `${item.kod} (${item.kisaAd})`));
      })
    );
  }

  /**
   * T_kod tablosundan Tarih Format seçeneklerini SelectInputModel[] formatında getirir
   */
  getDateFormatOptions(): Observable<SelectInputModel[]> {
    return this.kodService.getKodListByTipId(this.KOD_TIP_LOG_DATE_FORMAT).pipe(
      map((items: KodDto[]) => {
        return items.map(item => new SelectInputModel(item.kisaAd, `${item.kod} (${item.kisaAd})`));
      })
    );
  }

  /**
   * T_kod tablosundan Saat Format seçeneklerini SelectInputModel[] formatında getirir
   */
  getTimeFormatOptions(): Observable<SelectInputModel[]> {
    return this.kodService.getKodListByTipId(this.KOD_TIP_LOG_TIME_FORMAT).pipe(
      map((items: KodDto[]) => {
        return items.map(item => new SelectInputModel(item.kisaAd, `${item.kod} (${item.kisaAd})`));
      })
    );
  }

  /**
   * T_kod tablosundan Alan Tipi seçeneklerini SelectInputModel[] formatında getirir
   */
  getFieldTypeOptions(): Observable<SelectInputModel[]> {
    return this.kodService.getKodListByTipId(this.KOD_TIP_LOG_FIELD_TYPE).pipe(
      map((items: KodDto[]) => {
        return items.map(item => new SelectInputModel(item.kisaAd, `${item.kod} (${item.kisaAd})`));
      })
    );
  }

  /**
   * T_kod tablosundan Tanımlı Alan seçeneklerini SelectInputModel[] formatında getirir
   */
  getPredefinedFieldOptions(): Observable<SelectInputModel[]> {
    return this.kodService.getKodListByTipId(this.KOD_TIP_LOG_PREDEFINED_FIELD).pipe(
      map((items: KodDto[]) => {
        return items.map(item => new SelectInputModel(item.kisaAd, `${item.kod} (${item.kisaAd})`));
      })
    );
  }
  /**
   * Dropdown seçeneklerinden kod değerini ayıklar
   * @param selectedValue Seçilen değer (örn: "," (Virgül))
   * @returns Kod değeri
   * @private
   */
  private extractKodValue(selectedValue: string): string {
    if (!selectedValue) return '';
    
    // Format: "kod (kısaAd)" veya düz string olabilir
    if (selectedValue && selectedValue.includes('(') && selectedValue.includes(')')) {
      // "kod (kısaAd)" formatından sadece kod kısmını al
      return selectedValue.split('(')[0].trim();
    }
    
    // KodDto ile çalışırken key-value kombinasyonlarını işle
    const possibleJsonFormat = selectedValue.trim();
    if (possibleJsonFormat.startsWith('{') && possibleJsonFormat.endsWith('}')) {
      try {
        const parsed = JSON.parse(possibleJsonFormat);
        if (parsed && parsed.kod) {
          return parsed.kod;
        }
      } catch (e) {
        console.error('KodDto parse hatası:', e);
      }
    }
    
    return selectedValue; // Zaten kod formatındaysa aynı değeri döndür
  }

  /**
   * Kod değerini ve tip ID'sini kullanarak, t_kod tablosundaki karşılık gelen değeri döndürür
   * @param kod Kod değeri
   * @param tipId Kod tipi ID'si
   * @returns Kodun display formatı (örn: "kod (kısaAd)")
   */  /**
   * Kod değerini ve tip ID'sini kullanarak, t_kod tablosundaki karşılık gelen değeri döndürür
   * @param kod Kod değeri
   * @param tipId Kod tipi ID'si
   * @returns Kodun display formatı (örn: "kod (kısaAd)")
   * @private
   */
  private formatKodDisplay(kod: string, tipId: number): string {
    // Eğer kod değeri boşsa, boş string döndür
    if (!kod) {
      return '';
    }
    
    // Eğer zaten formatlanmış değer ise (örn: "kod (kısaAd)") direkt döndür
    if (kod.includes('(') && kod.includes(')')) {
      return kod;
    }
    
    // Birden fazla değer gelirse, ilkini al
    const kodValues = kod.split(',');
    const firstKodValue = kodValues[0].trim();
    
    // Özel durum kontrolleri - tipine göre önceden tanımlanmış değerleri dönebilir
    
    // Tekil bir değer olarak döndür - backend ve UI arasındaki kod-kısaAd eşleşmesi 
    // component seviyesinde dropdown'larla ve seçim anında yapılacak
    return firstKodValue; 
  }
}
