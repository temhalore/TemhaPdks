import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../api.service';
import { KodDto } from '../../models/KodDto';
import { SingleValueDTO } from '../../models/SingleValueDTO';

@Injectable({
  providedIn: 'root'
})
export class KodService {
  private endpoint = 'Kod';

  constructor(private apiService: ApiService) { }

  /**
   * Kod kaydeder veya günceller
   * @param kod Kod modeli
   * @returns Observable<KodDto>
   */
  saveKod(kod: KodDto): Observable<KodDto> {
    return this.apiService.post<KodDto>(`${this.endpoint}/saveKodByKodDto`, kod);
  }

  /**
   * ID'ye göre kod bilgisini getirir
   * @param eid Kodun encrypt edilmiş ID'si
   * @returns Observable<KodDto>
   */
  getKodById(eid: string): Observable<KodDto> {
    return this.apiService.post<KodDto>(`${this.endpoint}/getKodDtoByEIdDto`, { eid });
  }

  /**
   * Tip ID'ye göre kod listesini getirir
   * @param tipId Kod tipi ID'si
   * @returns Observable<KodDto[]>
   */
  getKodListByTipId(tipId: number): Observable<KodDto[]> {
    const request = new SingleValueDTO<number>(tipId);
    return this.apiService.post<KodDto[]>(`${this.endpoint}/getKodDtoListByKodTipId`, request);
  }

    getKodDtoListAll(): Observable<KodDto[]> {
    return this.apiService.post<KodDto[]>(`${this.endpoint}/getKodDtoListAll`, {});
  }

  

  /**
   * Kod cache'ini yeniler
   * @returns Observable<string>
   */
  refreshKodListCache(): Observable<string> {
    return this.apiService.post<string>(`${this.endpoint}/refreshKodListCache`, {});
  }
}
