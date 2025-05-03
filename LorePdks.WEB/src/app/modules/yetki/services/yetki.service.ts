import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from 'src/app/core/services/api.service';
import { EkranDTO } from 'src/app/core/models/EkranDTO';

@Injectable({
  providedIn: 'root'
})
export class YetkiService {

  constructor(private apiService: ApiService) { }

  // Ekran Yönetimi
  
  /**
   * Tüm ekranları listeler
   * @returns Observable<EkranDTO[]>
   */
  getAllEkranList(): Observable<EkranDTO[]> {
    return this.apiService.post<EkranDTO[]>('Yetki/getAllEkranList', {});
  }

  /**
   * Ekran kaydeder (ekleme/güncelleme)
   * @param ekranDto Ekran bilgileri
   * @returns Observable<EkranDTO>
   */
  saveEkran(ekranDto: EkranDTO): Observable<EkranDTO> {
    return this.apiService.post<EkranDTO>('Yetki/saveEkranByEkranDto', ekranDto);
  }

  /**
   * Ekran siler
   * @param id Ekran ID
   * @returns Observable<boolean>
   */
  deleteEkran(id: number): Observable<boolean> {
    return this.apiService.post<boolean>('Yetki/deleteEkranByEIdDto', { id });
  }

  /**
   * ID'ye göre ekran getirir
   * @param id Ekran ID
   * @returns Observable<EkranDTO>
   */
  getEkranById(id: number): Observable<EkranDTO> {
    return this.apiService.post<EkranDTO>('Yetki/getEkranDtoByEIdDto', { id });
  }

  /**
   * Ağaç yapısında menü olarak ekranları getirir
   * @returns Observable<EkranDTO[]>
   */
  getMenuTree(): Observable<EkranDTO[]> {
    return this.apiService.post<EkranDTO[]>('Yetki/getMenu', {});
  }
}