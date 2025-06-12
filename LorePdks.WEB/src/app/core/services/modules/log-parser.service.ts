import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../api.service';
import { LogParserConfigDto, LogParserTestResultDto } from '../../models/LogParserDto';

@Injectable({
  providedIn: 'root'
})
export class LogParserService {
  private endpoint = 'FirmaCihaz';

  constructor(private apiService: ApiService) { }
  /**
   * Firma cihazının log parser konfigürasyonunu günceller
   * @param firmaCihazEid Firma cihazının encrypt edilmiş ID'si
   * @param config Log parser konfigürasyonu
   * @returns Observable<boolean>
   */
  updateLogParserConfig(firmaCihazEid: string, config: LogParserConfigDto): Observable<boolean> {
    return this.apiService.post<boolean>(`${this.endpoint}/updateLogConfig`, {
      eid: firmaCihazEid,  // Backend'de 'eid' bekleniyor
      logParserConfig: JSON.stringify(config),
      logDelimiter: config.delimiter,
      logDateFormat: config.dateFormat,
      logTimeFormat: config.timeFormat,
      logFieldMapping: JSON.stringify(config.fieldMapping)
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
    return this.apiService.post<LogParserConfigDto>(`${this.endpoint}/getLogConfig`, { eid: firmaCihazEid });  // Backend'de 'eid' bekleniyor
  }
}
