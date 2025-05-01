import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable, map, tap, of } from 'rxjs';
import { ApiService } from './api.service';
import { KisiToken } from '../models/user.model';
import { LoginRequest } from '../models/login-request.model';
import Swal from 'sweetalert2';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private currentUserSubject: BehaviorSubject<KisiToken | null>;
  public currentUser: Observable<KisiToken | null>;

  constructor(
    private apiService: ApiService,
    private router: Router
  ) {
    // LocalStorage'dan mevcut kullanıcı bilgisini çek
    const storedUser = localStorage.getItem('kisiToken');
    this.currentUserSubject = new BehaviorSubject<KisiToken | null>(
      storedUser ? JSON.parse(storedUser) : null
    );
    this.currentUser = this.currentUserSubject.asObservable();
  }

  /**
   * Mevcut kullanıcıyı döndürür
   */
  public get currentUserValue(): KisiToken | null {
    return this.currentUserSubject.value;
  }

  /**
   * Kullanıcı giriş işlemi
   * @param loginRequest Giriş bilgileri
   * @returns Observable<KisiToken>
   */
  login(loginRequest: LoginRequest): Observable<KisiToken> {
    return this.apiService.post<KisiToken>('Auth/login', loginRequest)
      .pipe(
        map(response => {
          // API yanıtı başarılı ve data varsa
          if (response.isSuccess && response.data) {
            // LocalStorage'a kullanıcı bilgisini kaydet
            localStorage.setItem('kisiToken', JSON.stringify(response.data));
            // BehaviorSubject'i güncelle
            this.currentUserSubject.next(response.data);
            return response.data;
          } else {
            // Hata durumunda SweetAlert ile mesaj göster
            Swal.fire({
              title: 'Hata!',
              text: response.message || 'Giriş sırasında bir hata oluştu.',
              icon: 'error',
              confirmButtonText: 'Tamam'
            });
            throw new Error(response.message || 'Giriş sırasında bir hata oluştu.');
          }
        })
      );
  }

  /**
   * Kullanıcı çıkış işlemi
   */
  logout(): void {
    // Kullanıcı giriş yapmış durumdaysa API'ye logout isteği gönder
    if (this.currentUserValue) {
      this.apiService.post('Auth/logout', {}).subscribe({
        next: () => {
          this.clearUserData();
        },
        error: () => {
          this.clearUserData();
        }
      });
    } else {
      this.clearUserData();
    }
  }

  /**
   * Kullanıcı verilerini temizler ve giriş sayfasına yönlendirir
   */
  private clearUserData(): void {
    // LocalStorage'dan kullanıcı bilgisini temizle
    localStorage.removeItem('kisiToken');
    // BehaviorSubject'i null olarak güncelle
    this.currentUserSubject.next(null);
    // Login sayfasına yönlendir
    this.router.navigate(['/auth/login']);
  }

  /**
   * Kullanıcının oturumunun açık olup olmadığını kontrol eder
   */
  isLoggedIn(): boolean {
    return !!this.currentUserValue;
  }

  /**
   * Local Storage'dan kullanıcı bilgilerini alır ve Observable olarak döndürür
   * @returns Observable<KisiToken | null>
   */
  getUserFromLocalStorage(): Observable<KisiToken | null> {
    const storedUser = localStorage.getItem('kisiToken');
    const user = storedUser ? JSON.parse(storedUser) : null;
    
    if (user) {
      this.currentUserSubject.next(user);
    }
    
    return of(user);
  }
}