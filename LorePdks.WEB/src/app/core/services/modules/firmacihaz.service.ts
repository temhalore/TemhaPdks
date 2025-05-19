import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../api.service';
import { FirmaCihazDto } from '../../models/FirmaCihazDto';
import { FirmaDto } from '../../models/FirmaDto';

@Injectable({
  providedIn: 'root'
})
export class FirmaCihazService {
  private endpoint = 'FirmaCihaz';

  constructor(private apiService: ApiService) { }

  /**
   * Firma Cihaz kaydeder veya günceller
   * @param firmaCihaz Firma Cihaz modeli
   * @returns Observable<FirmaCihazDto>
   */
  saveFirmaCihaz(firmaCihaz: FirmaCihazDto): Observable<FirmaCihazDto> {
    return this.apiService.post<FirmaCihazDto>(`${this.endpoint}/saveFirmaCihazByFirmaCihazDto`, firmaCihaz);
  }

  /**
   * Firma Cihazı siler
   * @param eid Firma Cihazın encrypt edilmiş ID'si
   * @returns Observable<boolean>
   */
  deleteFirmaCihaz(eid: string): Observable<boolean> {
    return this.apiService.post<boolean>(`${this.endpoint}/deleteFirmaCihazByEIdDto`, { eid });
  }

  /**
   * ID'ye göre firma cihaz bilgisini getirir
   * @param eid Firma Cihazın encrypt edilmiş ID'si
   * @returns Observable<FirmaCihazDto>
   */
  getFirmaCihazById(eid: string): Observable<FirmaCihazDto> {
    return this.apiService.post<FirmaCihazDto>(`${this.endpoint}/getFirmaCihazDtoByEIdDto`, { eid });
  }

  /**
   * Firma ID'ye göre firma cihazlarını listeler
   * @param firmaEid Firmanın encrypt edilmiş ID'si
   * @returns Observable<FirmaCihazDto[]>
   */
  getFirmaCihazListByFirmaId(firmaEid: string): Observable<FirmaCihazDto[]> {
    const firma: FirmaDto = { eid: firmaEid } as FirmaDto;
    return this.apiService.post<FirmaCihazDto[]>(`${this.endpoint}/getFirmaCihazDtoListByFirmaDto`, firma);
  }

  /**
   * Firma ID'ye göre firma cihazlarını listeler (alternatif yöntem)
   * @param firmaId Firmanın ID değeri
   * @returns Observable<FirmaCihazDto[]>
   */
  getFirmaCihazListByFirmaIdValue(firmaId: number): Observable<FirmaCihazDto[]> {
    return this.apiService.post<FirmaCihazDto[]>(`${this.endpoint}/getFirmaCihazDtoListByFirmaId`, { Value: firmaId });
  }
}
