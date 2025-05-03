import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ServiceResponse } from '../models/ServiceResponse';
import { catchError, map } from 'rxjs/operators';
import { HttpErrorResponse } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { ApiErrorCodes } from '../constants/api-error-codes';
import { MessageType } from '../constants/message-types';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private apiUrl = environment.apiUrl;
  
  constructor(
    private http: HttpClient,
    private toastr: ToastrService,
    private router: Router
  ) { }
  
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
        catchError(error => {
          // HTTP hatalarını burada işle (interceptor tarafından yakalanacak)
          return throwError(() => error);
        })
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
        catchError(error => {
          // HTTP hatalarını burada işle (interceptor tarafından yakalanacak)
          return throwError(() => error);
        })
      );
  }

  /**
   * API yanıtını işler ve başarılı/başarısız yanıtlarda mesaj tipine göre işlem yapar
   * @param response API yanıtı
   * @returns T - API yanıtının data özelliği
   */
  private handleResponse<T>(response: ServiceResponse<T>): T {
    // Başarısız yanıt kontrolü
    if (!response.isSuccess ) {
      // API hata koduna göre özel işlemler yap
      this.handleApiError(response.errorCode, response.message, response.messageType);
      
      // Hatayı fırlat (subscribe eden komponentte yakalanabilir)
      throw new Error(response.message || 'API isteği başarısız oldu.');
    }
    
    // Başarılı yanıt - eğer bir mesaj varsa göster
    if (response.message && response.message.trim() !== '') {
      // Mesaj tipine göre bildirim göster
      this.showResponseMessage(response.message, response.messageType);
    }
    
    return response.data;
  }

  /**
   * Mesaj tipine göre bildirim gösterir
   * @param message Gösterilecek mesaj
   * @param messageType Mesaj tipi
   */
  private showResponseMessage(message: string, messageType?: string): void {
    if (!message || message.trim() === '') {
      return;
    }
    
    switch (messageType) {
      case MessageType.SUCCESS:
        this.toastr.success(message, 'Başarılı');
        break;
        
      case MessageType.WARNING:
        this.toastr.warning(message, 'Uyarı');
        break;
        
      case MessageType.INFO:
        this.toastr.info(message, 'Bilgi');
        break;
        
      case MessageType.ERROR:
        this.toastr.error(message, 'Hata');
        break;
        
      default:
        // Varsayılan olarak info göster
        this.toastr.info(message, 'Bilgi');
    }
  }

  /**
   * API hata kodlarına göre özel işlemler yapar
   * @param errorCode Hata kodu
   * @param message Hata mesajı
   * @param messageType Mesaj tipi
   */
  private handleApiError(errorCode?: number, message?: string, messageType?: string): void {
    const defaultErrorMessage = message || 'API isteği başarısız oldu.';
    
    // Eğer messageType error değilse ve bir hata kodu yoksa, messageType'a göre göster
    if (messageType && messageType !== MessageType.ERROR && !errorCode) {
      this.showResponseMessage(defaultErrorMessage, messageType);
      return;
    }
    
    // Hata kodu yoksa veya bilinmeyen bir hata ise
    if (!errorCode) {
      this.toastr.error(defaultErrorMessage, 'Hata!');
      return;
    }
    
    // Hata koduna göre işlem yap
    switch (errorCode) {
      case ApiErrorCodes.ERROR_401_YETKISIZ_ERISIM:
        
        this.toastr.error(message , 'Yetkisiz Erişim!');
        break;
        
      case ApiErrorCodes.ERROR_501_YENIDEN_LOGIN_OLMALI:
        this.toastr.warning(message , 'Oturum Sonlandı!');
        // LocalStorage'dan token'ı temizle
        localStorage.removeItem('kisiToken');
        // Login sayfasına yönlendir
        this.router.navigate(['/auth/login']);
        break;
        
      case ApiErrorCodes.ERROR_500_BIR_HATA_OLUSTU:
        this.toastr.error(message ,'Uygulamada bir hata oluştu.');
        break;
        
      case ApiErrorCodes.ERROR_502_EKSIK_VERI_GONDERIMI:
        this.toastr.error(message , 'Eksik veri gönderimi yapıldı.');
        break;
        
      case ApiErrorCodes.ERROR_503_GECERSIZ_VERI_GONDERIMI:
        this.toastr.error(message ,'Geçersiz veri gönderimi yapıldı.');
        break;
        
      case ApiErrorCodes.ERROR_BIR_HATA_OLUSTU_YONLENDIR:
        this.toastr.error('Uygulamada bir hata oluştu. Lütfen yönlendirmeyi bekleyiniz..', 'Sistem Hatası!');
        // 3 saniye sonra ana sayfaya yönlendir
        setTimeout(() => {
          this.router.navigate(['/secure/dashboard']);
        }, 3000);
        break;
        
      default:
        // Bilinmeyen hata kodları için genel mesaj göster
        this.toastr.error(defaultErrorMessage, 'Hata!');
    }
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