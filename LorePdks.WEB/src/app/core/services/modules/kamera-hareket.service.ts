import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../api.service';
import { KameraHareketDto } from '../../models/KameraHareketDto';

@Injectable({
  providedIn: 'root'
})
export class KameraHareketService {
  private endpoint = 'KameraHareket';

  constructor(private apiService: ApiService) { }

  /**
   * Kamera hareket kaydeder veya günceller
   * @param kameraHareket Kamera hareket modeli
   * @returns Observable<KameraHareketDto>
   */
  saveKameraHareket(kameraHareket: KameraHareketDto): Observable<KameraHareketDto> {
    return this.apiService.post<KameraHareketDto>(`${this.endpoint}/saveKameraHareket`, kameraHareket);
  }

  /**
   * Kamera hareketini siler
   * @param eid Kamera hareketinin encrypt edilmiş ID'si
   * @returns Observable<boolean>
   */
  deleteKameraHareket(eid: string): Observable<boolean> {
    return this.apiService.post<boolean>(`${this.endpoint}/deleteKameraHareket`, { eid });
  }

  /**
   * ID'ye göre kamera hareket bilgisini getirir
   * @param eid Kamera hareketinin encrypt edilmiş ID'si
   * @returns Observable<KameraHareketDto>
   */
  getKameraHareketById(eid: string): Observable<KameraHareketDto> {
    return this.apiService.post<KameraHareketDto>(`${this.endpoint}/getKameraHareketById`, { eid });
  }

  /**
   * Tüm kamera hareketlerini listeler
   * @returns Observable<KameraHareketDto[]>
   */
  getAllKameraHareketList(): Observable<KameraHareketDto[]> {
    return this.apiService.post<KameraHareketDto[]>(`${this.endpoint}/getAllKameraHareketList`, {});
  }

  /**
   * Firma ID'sine göre kamera hareketlerini listeler
   * @param firmaEid Firmanın encrypt edilmiş ID'si
   * @returns Observable<KameraHareketDto[]>
   */
  getKameraHareketListByFirmaId(firmaEid: string): Observable<KameraHareketDto[]> {
    return this.apiService.post<KameraHareketDto[]>(`${this.endpoint}/getKameraHareketListByFirmaId`, { firmaEid });
  }

  /**
   * Tarih aralığına göre kamera hareketlerini listeler
   * @param baslangicTarihi Başlangıç tarihi
   * @param bitisTarihi Bitiş tarihi
   * @returns Observable<KameraHareketDto[]>
   */
  getKameraHareketListByDateRange(baslangicTarihi: Date, bitisTarihi: Date): Observable<KameraHareketDto[]> {
    return this.apiService.post<KameraHareketDto[]>(`${this.endpoint}/getKameraHareketListByDateRange`, {
      baslangicTarihi,
      bitisTarihi
    });
  }
}
