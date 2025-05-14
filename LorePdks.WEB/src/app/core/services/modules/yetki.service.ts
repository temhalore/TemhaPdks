import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../api.service';
import { RolDto } from '../../models/RolDto';
import { ControllerAndMethodsDTO } from '../../models/ControllerAndMethodsDTO';
import { RolControllerMethodDto } from '../../models/RolControllerMethodDto';
import { RolControllerMethodsRequestDto } from '../../models/RolControllerMethodsRequestDto';
import { EkranDto } from '../../models/EkranDto';
import { KisiRolDto } from '../../models/KisiRolDto';
import { KisiDto } from '../../models/KisiDto';


@Injectable({
  providedIn: 'root'
})
export class YetkiService {
  private endpoint = 'Yetki';

  constructor(private apiService: ApiService) { }

  /**
   * Tüm rolleri listeler
   * @returns Observable<RolModel[]>
   */
  getAllRolList(): Observable<RolDto[]> {
    return this.apiService.post<RolDto[]>(`${this.endpoint}/getAllRolList`, {});
  }

  /**
   * ID'ye göre rol bilgisini getirir
   * @param id Rolün encrypt edilmiş ID'si
   * @returns Observable<RolModel>
   */
  getRolById(eid: string): Observable<RolDto> {
    return this.apiService.post<RolDto>(`${this.endpoint}/getRolDtoByEIdDto`, { eid });
  }
  /**
   * Rol kaydeder/günceller
   * @param rol Rol modeli
   * @returns Observable<RolModel>
   */
  saveRol(rol: RolDto): Observable<RolDto> {
    return this.apiService.post<RolDto>(`${this.endpoint}/saveRolByRolDto`, rol);
  }

  /**
   * Rol siler
   * @param id Rolün encrypt edilmiş ID'si
   * @returns Observable<boolean>
   */
  deleteRol(eid: string): Observable<boolean> {
    return this.apiService.post<boolean>(`${this.endpoint}/deleteRolByEIdDto`, { eid });
  }




  /**
   * Tüm controller ve metotları listeler
   * @returns Observable<ControllerAndMethodsModel[]>
   */
  getControllerAndMethodsList(): Observable<ControllerAndMethodsDTO[]> {
    return this.apiService.post<ControllerAndMethodsDTO[]>(`${this.endpoint}/GetControllerAndMethodsList`, {});
  }

  /**
   * Rol için yetkili controller ve metotları getirir
   * @param rolId Rolün encrypt edilmiş ID'si
   * @returns Observable<RolControllerMethodModel[]>
   */
  getRolControllerMethods(rolId: string): Observable<RolControllerMethodDto[]> {
    return this.apiService.post<RolControllerMethodDto[]>(`${this.endpoint}/getRolControllerMethodsByEIdDto`, { eid: rolId });
  }

  /**
   * Rol için controller ve metot yetkilerini kaydeder
   * @param request Rol ve controller-metot bilgileri
   * @returns Observable<boolean>
   */
  saveRolControllerMethods(request: RolControllerMethodsRequestDto): Observable<boolean> {
    const requestObj = {
      rolEidDto: request.rolEidDto,
      controllerMethods: request.controllerMethods
    };
    return this.apiService.post<boolean>(`${this.endpoint}/saveRolControllerMethodsByRolIdAndControllerMethods`, requestObj);
  }






  

  /**
   * Tüm ekranları listeler
   * @returns Observable<EkranModel[]>
   */
  getAllEkranList(): Observable<EkranDto[]> {
    return this.apiService.post<EkranDto[]>(`${this.endpoint}/getAllEkranList`, {});
  }

  /**
   * Rol için ekranları getirir
   * @param rolId Rolün encrypt edilmiş ID'si
   * @returns Observable<EkranModel[]>
   */
  getEkransByRolId(rolId: string): Observable<EkranDto[]> {
    return this.apiService.post<EkranDto[]>(`${this.endpoint}/getEkransByRolIdDto`, { eid: rolId });
  }

  /**
   * Role ekran ekler
   * @param rolId Rolün encrypt edilmiş ID'si
   * @param ekranId Ekranın encrypt edilmiş ID'si
   * @returns Observable<boolean>
   */
  addEkranToRol(rolId: string, ekranId: string): Observable<boolean> {
    const request = {
      rolEidDto: { eid: rolId },
      ekranEidDto: { eid: ekranId }
    };
    return this.apiService.post<boolean>(`${this.endpoint}/addEkranToRolByRolEkranDto`, request);
  }
  /**
   * Rolden ekran çıkarır
   * @param rolId Rolün encrypt edilmiş ID'si
   * @param ekranId Ekranın encrypt edilmiş ID'si
   * @returns Observable<boolean>
   */
  removeEkranFromRol(rolId: string, ekranId: string): Observable<boolean> {
    const request = {
      rolEidDto: { eid: rolId },
      ekranEidDto: { eid: ekranId }
    };
    return this.apiService.post<boolean>(`${this.endpoint}/removeEkranFromRolByRolEkranDto`, request);
  }

  /**
   * Ekran kaydeder/günceller
   * @param ekran Ekran modeli
   * @returns Observable<EkranDto>
   */
  saveEkran(ekran: EkranDto): Observable<EkranDto> {
    return this.apiService.post<EkranDto>(`${this.endpoint}/saveEkranByEkranDto`, ekran);
  }

  /**
   * Ekran siler
   * @param eid Ekranın encrypt edilmiş ID'si
   * @returns Observable<boolean>
   */
  deleteEkran(eid: string): Observable<boolean> {
    return this.apiService.post<boolean>(`${this.endpoint}/deleteEkranByEIdDto`, { eid });
  }

  /**
   * Kişiye ait rolleri getirir
   * @param kisiId Kişinin encrypt edilmiş ID'si
   * @returns Observable<RolDto[]>
   */
  getRolsByKisiId(kisiId: string): Observable<RolDto[]> {
    return this.apiService.post<RolDto[]>(`${this.endpoint}/getRolDtoListByKisiIdDto`, { eid: kisiId });
  }

  /**
   * Kişiye rol ekler
   * @param kisiId Kişinin encrypt edilmiş ID'si
   * @param rolId Rolün encrypt edilmiş ID'si
   * @returns Observable<boolean>
   */
  addRolToKisi(kisiId: string, rolId: string): Observable<boolean> {
    const request = {
      kisiEidDto: { eid: kisiId },
      rolEidDto: { eid: rolId }
    };
    return this.apiService.post<boolean>(`${this.endpoint}/addRolToKisiByKisiRolDto`, request);
  }

  /**
   * Kişiden rol çıkarır
   * @param kisiId Kişinin encrypt edilmiş ID'si
   * @param rolId Rolün encrypt edilmiş ID'si
   * @returns Observable<boolean>
   */
  removeRolFromKisi(kisiId: string, rolId: string): Observable<boolean> {
    const request = {
      kisiEidDto: { eid: kisiId },
      rolEidDto: { eid: rolId }
    };
    return this.apiService.post<boolean>(`${this.endpoint}/removeRolFromKisiByKisiRolDto`, request);
  }

  /**
   * Role ait kişileri getirir
   * @param rolId Rolün encrypt edilmiş ID'si
   * @returns Observable<KisiDto[]>
   */
  getKisisByRolId(rolId: string): Observable<KisiDto[]> {
    return this.apiService.post<KisiDto[]>(`${this.endpoint}/getKisiDtoListByRolIdDto`, { eid: rolId });
  }
}