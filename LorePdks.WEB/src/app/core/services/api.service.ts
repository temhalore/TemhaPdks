import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ServiceResponse } from '../models/ServiceResponse';
import { catchError, map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private apiUrl = environment.apiUrl;
  
  constructor(private http: HttpClient) { }
  
  /**
   * GET isteği göndermek için kullanılır
   * @param endpoint API endpoint'i
   * @param params Query parametreleri
   * @returns Observable<T>
   */
  get<T>(endpoint: string, params?: any): Observable<T> {
    const url = `${this.apiUrl}/${endpoint}`;
    const headers = this.getHeaders();
    
    return this.http.get<ServiceResponse<T>>(url, { headers, params })
      .pipe(
        map(response => this.handleResponse<T>(response)),
        catchError(error => throwError(() => error))
      );
  }
  
  /**
   * POST isteği göndermek için kullanılır
   * @param endpoint API endpoint'i
   * @param body İstek gövdesi
   * @returns Observable<T>
   */
  post<T>(endpoint: string, body: any): Observable<T> {
    const url = `${this.apiUrl}/${endpoint}`;
    const headers = this.getHeaders();
    
    return this.http.post<ServiceResponse<T>>(url, body, { headers })
      .pipe(
        map(response => this.handleResponse<T>(response)),
        catchError(error => throwError(() => error))
      );
  }

  /**
   * API yanıtını işler ve başarısız yanıtlarda hata fırlatır
   * @param response API yanıtı
   * @returns T - API yanıtının data özelliği
   */
  private handleResponse<T>(response: ServiceResponse<T>): T {
    if (!response.IsSuccess || !response.data) {
      throw new Error(response.message || 'API isteği başarısız oldu.');
    }
    return response.data;
  }
  
  /**
   * HTTP istekleri için gerekli başlıkları oluşturur
   * @returns HttpHeaders
   */
  private getHeaders(): HttpHeaders {
    let headers = new HttpHeaders({
      'Content-Type': 'application/json'
    });
    
    // localStorage'dan token'ı al
    const kisiTokenJson = localStorage.getItem('kisiToken');
    if (kisiTokenJson) {
      try {
        const kisiToken = JSON.parse(kisiTokenJson);
        if (kisiToken && kisiToken.token) {
          headers = headers.set('appToken', kisiToken.token);
        }
      } catch (error) {
        console.error('Token parse hatası:', error);
      }
    }
    
    return headers;
  }
}