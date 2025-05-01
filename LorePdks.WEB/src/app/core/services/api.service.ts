import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ServiceResponse } from '../models/service-response.model';

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
   * @returns Observable<ServiceResponse<T>>
   */
  get<T>(endpoint: string, params?: any): Observable<ServiceResponse<T>> {
    const url = `${this.apiUrl}/${endpoint}`;
    const headers = this.getHeaders();
    
    return this.http.get<ServiceResponse<T>>(url, { headers, params });
  }
  
  /**
   * POST isteği göndermek için kullanılır
   * @param endpoint API endpoint'i
   * @param body İstek gövdesi
   * @returns Observable<ServiceResponse<T>>
   */
  post<T>(endpoint: string, body: any): Observable<ServiceResponse<T>> {
    const url = `${this.apiUrl}/${endpoint}`;
    const headers = this.getHeaders();
    
    return this.http.post<ServiceResponse<T>>(url, body, { headers });
  }
  
  /**
   * PUT isteği göndermek için kullanılır
   * @param endpoint API endpoint'i
   * @param body İstek gövdesi
   * @returns Observable<ServiceResponse<T>>
   */
  put<T>(endpoint: string, body: any): Observable<ServiceResponse<T>> {
    const url = `${this.apiUrl}/${endpoint}`;
    const headers = this.getHeaders();
    
    return this.http.put<ServiceResponse<T>>(url, body, { headers });
  }
  
  /**
   * DELETE isteği göndermek için kullanılır
   * @param endpoint API endpoint'i
   * @returns Observable<ServiceResponse<T>>
   */
  delete<T>(endpoint: string): Observable<ServiceResponse<T>> {
    const url = `${this.apiUrl}/${endpoint}`;
    const headers = this.getHeaders();
    
    return this.http.delete<ServiceResponse<T>>(url, { headers });
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