import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../api.service';
import { PdksHareketDto } from '../../models/PdksHareketDto';

@Injectable({
  providedIn: 'root'
})
export class PdksHareketService {
  private endpoint = 'PdksHareket';

  constructor(private apiService: ApiService) { }

  /**
   * PDKS hareket kaydeder veya günceller
   * @param pdksHareket PDKS hareket modeli
   * @returns Observable<PdksHareketDto>
   */
  savePdksHareket(pdksHareket: PdksHareketDto): Observable<PdksHareketDto> {
    return this.apiService.post<PdksHareketDto>(`${this.endpoint}/savePdksHareket`, pdksHareket);
  }

  /**
   * PDKS hareketini siler
   * @param eid PDKS hareketinin encrypt edilmiş ID'si
   * @returns Observable<boolean>
   */
  deletePdksHareket(eid: string): Observable<boolean> {
    return this.apiService.post<boolean>(`${this.endpoint}/deletePdksHareket`, { eid });
  }

  /**
   * ID'ye göre PDKS hareket bilgisini getirir
   * @param eid PDKS hareketinin encrypt edilmiş ID'si
   * @returns Observable<PdksHareketDto>
   */
  getPdksHareketById(eid: string): Observable<PdksHareketDto> {
    return this.apiService.post<PdksHareketDto>(`${this.endpoint}/getPdksHareketById`, { eid });
  }

  /**
   * Tüm PDKS hareketlerini listeler
   * @returns Observable<PdksHareketDto[]>
   */
  getAllPdksHareketList(): Observable<PdksHareketDto[]> {
    return this.apiService.post<PdksHareketDto[]>(`${this.endpoint}/getAllPdksHareketList`, {});
  }

  /**
   * Firma ID'sine göre PDKS hareketlerini listeler
   * @param firmaEid Firmanın encrypt edilmiş ID'si
   * @returns Observable<PdksHareketDto[]>
   */
  getPdksHareketListByFirmaId(firmaEid: string): Observable<PdksHareketDto[]> {
    return this.apiService.post<PdksHareketDto[]>(`${this.endpoint}/getPdksHareketListByFirmaId`, { firmaEid });
  }

  /**
   * Tarih aralığına göre PDKS hareketlerini listeler
   * @param baslangicTarihi Başlangıç tarihi
   * @param bitisTarihi Bitiş tarihi
   * @returns Observable<PdksHareketDto[]>
   */
  getPdksHareketListByDateRange(baslangicTarihi: Date, bitisTarihi: Date): Observable<PdksHareketDto[]> {
    return this.apiService.post<PdksHareketDto[]>(`${this.endpoint}/getPdksHareketListByDateRange`, {
      baslangicTarihi,
      bitisTarihi
    });
  }
}
