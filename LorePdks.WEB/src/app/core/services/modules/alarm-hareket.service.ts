import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../api.service';
import { AlarmHareketDto } from '../../models/AlarmHareketDto';

@Injectable({
  providedIn: 'root'
})
export class AlarmHareketService {
  private endpoint = 'AlarmHareket';

  constructor(private apiService: ApiService) { }
  /**
   * Alarm hareket kaydeder veya günceller
   * @param alarmHareket Alarm hareket modeli
   * @returns Observable<AlarmHareketDto>
   */
  saveAlarmHareket(alarmHareket: AlarmHareketDto): Observable<AlarmHareketDto> {
    return this.apiService.post<AlarmHareketDto>(`${this.endpoint}/save`, alarmHareket);
  }  /**
   * Alarm hareketini siler
   * @param id Alarm hareketinin ID'si
   * @returns Observable<boolean>
   */
  deleteAlarmHareket(id: number): Observable<boolean> {
    return this.apiService.delete<boolean>(`${this.endpoint}/${id}`);
  }

  /**
   * ID'ye göre alarm hareket bilgisini getirir
   * @param eid Alarm hareketinin encrypt edilmiş ID'si
   * @returns Observable<AlarmHareketDto>
   */
  getAlarmHareketById(eid: string): Observable<AlarmHareketDto> {
    return this.apiService.post<AlarmHareketDto>(`${this.endpoint}/getAlarmHareketById`, { eid });
  }

  /**
   * Tüm alarm hareketlerini listeler
   * @returns Observable<AlarmHareketDto[]>
   */
  getAllAlarmHareketList(): Observable<AlarmHareketDto[]> {
    return this.apiService.post<AlarmHareketDto[]>(`${this.endpoint}/getAllAlarmHareketList`, {});
  }

  /**
   * Firma ID'sine göre alarm hareketlerini listeler
   * @param firmaEid Firmanın encrypt edilmiş ID'si
   * @returns Observable<AlarmHareketDto[]>
   */
  getAlarmHareketListByFirmaId(firmaEid: string): Observable<AlarmHareketDto[]> {
    return this.apiService.post<AlarmHareketDto[]>(`${this.endpoint}/getAlarmHareketListByFirmaId`, { firmaEid });
  }

  /**
   * Tarih aralığına göre alarm hareketlerini listeler
   * @param baslangicTarihi Başlangıç tarihi
   * @param bitisTarihi Bitiş tarihi
   * @returns Observable<AlarmHareketDto[]>
   */
  getAlarmHareketListByDateRange(baslangicTarihi: Date, bitisTarihi: Date): Observable<AlarmHareketDto[]> {
    return this.apiService.post<AlarmHareketDto[]>(`${this.endpoint}/getAlarmHareketListByDateRange`, {
      baslangicTarihi,
      bitisTarihi
    });
  }
}
