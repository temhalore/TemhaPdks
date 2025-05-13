import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../api.service';
import { KisiDto } from '../../models/KisiDto';
import { EIdDto } from '../../models/EIdDto';
import { SingleValueDTO } from '../../models/SingleValueDTO';
import { loginReqDto } from '../../models/loginReqDto';

@Injectable({
  providedIn: 'root'
})
export class KisiService {
  private endpoint = 'Kisi';

  constructor(private apiService: ApiService) { }

  /**
   * Kişi kaydeder veya günceller
   * @param kisi Kişi modeli
   * @returns Observable<KisiDto>
   */
  saveKisi(kisi: KisiDto): Observable<KisiDto> {
    return this.apiService.post<KisiDto>(`${this.endpoint}/saveKisiByKisiDto`, kisi);
  }

  /**
   * Kişiyi siler
   * @param eid Kişinin encrypt edilmiş ID'si
   * @returns Observable<boolean>
   */
  deleteKisi(eid: string): Observable<boolean> {
    return this.apiService.post<boolean>(`${this.endpoint}/deleteKisiByEIdDto`, { eid });
  }

  /**
   * ID'ye göre kişi bilgisini getirir
   * @param eid Kişinin encrypt edilmiş ID'si
   * @returns Observable<KisiDto>
   */
  getKisiById(eid: string): Observable<KisiDto> {
    return this.apiService.post<KisiDto>(`${this.endpoint}/getKisiDtoByEIdDto`, { eid });
  }

  /**
   * Tüm kişileri listeler
   * @returns Observable<KisiDto[]>
   */
  getAllKisiList(): Observable<KisiDto[]> {
    return this.apiService.post<KisiDto[]>(`${this.endpoint}/getAllKisiList`, {});
  }

  /**
   * Kullanıcı adı ve şifreye göre kişi bilgisini getirir
   * @param loginReq Login bilgileri
   * @returns Observable<KisiDto>
   */
  getKisiByLoginNameAndSifre(loginReq: loginReqDto): Observable<KisiDto> {
    return this.apiService.post<KisiDto>(`${this.endpoint}/getKisiByLoginNameAndSifre`, loginReq);
  }

  /**
   * Kullanıcı adına göre kişi bilgisini getirir
   * @param loginName Kullanıcı adı
   * @returns Observable<KisiDto>
   */
  getKisiByLoginName(loginName: string): Observable<KisiDto> {
    const request = new SingleValueDTO<string>(loginName);
    return this.apiService.post<KisiDto>(`${this.endpoint}/getKisiByLoginName`, request);
  }

  /**
   * TC'ye göre kişi bilgisini getirir
   * @param tc TC kimlik numarası
   * @returns Observable<KisiDto>
   */
  getKisiByTc(tc: string): Observable<KisiDto> {
    const request = new SingleValueDTO<string>(tc);
    return this.apiService.post<KisiDto>(`${this.endpoint}/getKisiByTc`, request);
  }

  /**
   * Arama metni ile kişileri arar
   * @param aramaText Arama metni
   * @returns Observable<KisiDto[]>
   */
  getKisiListByAramaText(aramaText: string): Observable<KisiDto[]> {
    const request = new SingleValueDTO<string>(aramaText);
    return this.apiService.post<KisiDto[]>(`${this.endpoint}/getKisiListByAramaText`, request);
  }
}
