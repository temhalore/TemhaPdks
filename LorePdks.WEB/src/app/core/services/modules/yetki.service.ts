import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../api.service';
import { RolDto } from '../../models/RolDto';
import { ControllerAndMethodsDTO } from '../../models/ControllerAndMethodsDTO';
import { RolControllerMethodDto } from '../../models/RolControllerMethodDto';
import { RolControllerMethodsRequestDto } from '../../models/RolControllerMethodsRequestDto';
import { EkranDto } from '../../models/EkranDto';


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
  getRolById(id: string): Observable<RolDto> {
    return this.apiService.post<RolDto>(`${this.endpoint}/getRolDtoByEIdDto`, { id });
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
  deleteRol(id: string): Observable<boolean> {
    return this.apiService.post<boolean>(`${this.endpoint}/deleteRolByEIdDto`, { id });
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
}