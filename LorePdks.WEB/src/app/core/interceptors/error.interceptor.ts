import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, catchError, throwError } from 'rxjs';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

//burası http isteklerin http status kodu olan ok 200 bad request 400 gibi durumları yakalamak için kullanılır
// ve hata mesajlarını kullanıcıya göstermek için kullanılır
//apinin mantıksal herşeyi ok dönüp ISucces kımındaki hata fırlatma durumları buradan yakalanamaz.Çünkü hepsi ok ani 200 dür.

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

  constructor(
    private router: Router,
    private toastr: ToastrService
  ) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        let errorMessage = 'Beklenmeyen bir hata oluştu.';
        
        if (error.error instanceof ErrorEvent) {
          // İstemci taraflı hata
          errorMessage = `Hata: ${error.error.message}`;
        } else {
          // Sunucu taraflı hata
          switch (error.status) {
            case 400:
              errorMessage = error.error?.message || 'Geçersiz istek.';
              break;
            case 401:
              errorMessage = 'Oturumunuz sonlanmış veya yetkiniz bulunmamaktadır.';
              localStorage.removeItem('kisiToken');
              this.router.navigate(['/auth/login']);
              break;
            case 403:
              errorMessage = 'Bu işlem için yetkiniz bulunmamaktadır.';
              break;
            case 404:
              errorMessage = 'İstenilen kaynak bulunamadı.';
              break;
            case 500:
              errorMessage = 'Sunucu hatası. Lütfen daha sonra tekrar deneyiniz.';
              break;
            default:
              if (error.error?.message) {
                errorMessage = error.error.message;
              } else {
                errorMessage = `Hata Kodu: ${error.status}\nDetay: ${error.statusText}`;
              }
          }
        }

        // Hata mesajını göster (401 hariç - zaten yönlendirme yapılıyor)
        if (error.status !== 401) {
          this.toastr.error(errorMessage, 'Hata!');
        }
        
        return throwError(() => new Error(errorMessage));
      })
    );
  }
}