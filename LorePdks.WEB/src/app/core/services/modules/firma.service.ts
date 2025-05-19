import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../api.service';
import { FirmaDto } from '../../models/FirmaDto';

@Injectable({
  providedIn: 'root'
})
export class FirmaService {
  private endpoint = 'Firma';

  constructor(private apiService: ApiService) { }

  /**
   * Firma kaydeder veya günceller
   * @param firma Firma modeli
   * @returns Observable<FirmaDto>
   */
  saveFirma(firma: FirmaDto): Observable<FirmaDto> {
    return this.apiService.post<FirmaDto>(`${this.endpoint}/saveFirmaByFirmaDto`, firma);
  }

  /**
   * Firmayı siler
   * @param eid Firmanın encrypt edilmiş ID'si
   * @returns Observable<boolean>
   */
  deleteFirma(eid: string): Observable<boolean> {
    return this.apiService.post<boolean>(`${this.endpoint}/deleteFirmaByEIdDto`, { eid });
  }

  /**
   * ID'ye göre firma bilgisini getirir
   * @param eid Firmanın encrypt edilmiş ID'si
   * @returns Observable<FirmaDto>
   */
  getFirmaById(eid: string): Observable<FirmaDto> {
    return this.apiService.post<FirmaDto>(`${this.endpoint}/getFirmaDtoByEIdDto`, { eid });
  }

  /**
   * Tüm firmaları listeler
   * @returns Observable<FirmaDto[]>
   */
  getAllFirmaList(): Observable<FirmaDto[]> {
    return this.apiService.post<FirmaDto[]>(`${this.endpoint}/getAllFirmaListDto`, {});
  }
}
