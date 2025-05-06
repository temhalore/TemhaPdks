import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { KisiTokenDto } from '../../models/KisiTokenDto';
import { loginReqDto } from '../../models/loginReqDto';
import { ApiService } from '../api.service';
import { tap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private currentUserSubject: BehaviorSubject<KisiTokenDto | null>;
  public currentUser: Observable<KisiTokenDto | null>;

  constructor(
    private apiService: ApiService,
    private router: Router
  ) {
    // LocalStorage'dan mevcut kullanıcı bilgisini çek
    const storedUser = localStorage.getItem('kisiToken');
    this.currentUserSubject = new BehaviorSubject<KisiTokenDto | null>(
      storedUser ? JSON.parse(storedUser) : null
    );
    this.currentUser = this.currentUserSubject.asObservable();
  }

  /**
   * Mevcut kullanıcıyı döndürür
   */
  public get currentUserValue(): KisiTokenDto | null {
    return this.currentUserSubject.value;
  }

  /**
   * Kullanıcı giriş işlemi
   * @param loginRequest Giriş bilgileri
   * @returns Observable<KisiToken>
   */
  login(loginRequest: loginReqDto): Observable<KisiTokenDto> {
    return this.apiService.post<KisiTokenDto>('Auth/login', loginRequest)
      .pipe(
        tap(userData => {
          // LocalStorage'a kullanıcı bilgisini kaydet
          localStorage.setItem('kisiToken', JSON.stringify(userData));
          // BehaviorSubject'i güncelle
          this.currentUserSubject.next(userData);
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
  getUserFromLocalStorage(): Observable<KisiTokenDto | null> {
    const storedUser = localStorage.getItem('kisiToken');
    const user = storedUser ? JSON.parse(storedUser) : null;
    
    if (user) {
      this.currentUserSubject.next(user);
    }
    
    return of(user);
  }
}