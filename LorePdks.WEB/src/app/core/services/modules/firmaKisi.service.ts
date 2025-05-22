import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../api.service';
import { FirmaKisiDto } from '../../models/FirmaKisiDto';

@Injectable({
  providedIn: 'root'
})
export class FirmaKisiService {
  private endpoint = 'FirmaKisi';

  constructor(private apiService: ApiService) { }

  /**
   * Firma kişi kaydeder veya günceller
   * @param firmaKisi FirmaKisi modeli
   * @returns Observable<FirmaKisiDto>
   */
  saveFirmaKisi(firmaKisi: FirmaKisiDto): Observable<FirmaKisiDto> {
    return this.apiService.post<FirmaKisiDto>(`${this.endpoint}/saveFirmaKisiByFirmaKisiDto`, firmaKisi);
  }

  /**
   * Firma kişiyi siler
   * @param eid FirmaKişinin encrypt edilmiş ID'si
   * @returns Observable<boolean>
   */
  deleteFirmaKisi(eid: string): Observable<boolean> {
    return this.apiService.post<boolean>(`${this.endpoint}/deleteFirmaKisiByEIdDto`, { eid });
  }

  /**
   * ID'ye göre firma kişi bilgisini getirir
   * @param eid FirmaKişinin encrypt edilmiş ID'si
   * @returns Observable<FirmaKisiDto>
   */
  getFirmaKisiById(eid: string): Observable<FirmaKisiDto> {
    return this.apiService.post<FirmaKisiDto>(`${this.endpoint}/getFirmaKisiDtoByEIdDto`, { eid });
  }

  /**
   * Firma ID'ye göre firma kişi listesini getirir
   * @param firmaEid Firmanın encrypt edilmiş ID'si
   * @returns Observable<FirmaKisiDto[]>
   */
  getFirmaKisiListByFirmaId(firmaEid: string): Observable<FirmaKisiDto[]> {
    return this.apiService.post<FirmaKisiDto[]>(`${this.endpoint}/getFirmaKisiDtoListByFirmaIdDto`, { eid: firmaEid });
  }
}
