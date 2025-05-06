import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { loginReqDto } from '../../../../core/models/loginReqDto';
import { AuthService } from '../../../../core/services/modules/auth.service';
import { ToastrService } from 'ngx-toastr';
import { BehaviorSubject } from 'rxjs';

// PrimeNG imports
import { InputTextModule } from 'primeng/inputtext';
import { CardModule } from 'primeng/card';
import { PasswordModule } from 'primeng/password';
import { DividerModule } from 'primeng/divider';

// NGX-Spinner import
import { NgxSpinnerModule, NgxSpinnerService } from 'ngx-spinner';

// Özel bileşen importları
import { ButtonComponent } from '../../../../core/components/button/button.component';
import { TextInputComponent } from '../../../../core/components/text-input/text-input.component';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
  standalone: true,
  imports: [
    CommonModule, 
    ReactiveFormsModule, 
    InputTextModule, 
    CardModule, 
    PasswordModule,
    DividerModule,
    NgxSpinnerModule,
    ButtonComponent,
    TextInputComponent
  ]
})
export class LoginComponent implements OnInit {
  // Form geçerliliğini kontrol etmek için formGroup
  loginForm!: FormGroup;
  loading = false;
  submitted = false;
  returnUrl: string = '/secure/dashboard';
  
  // Yükleme durumu için observable
  loadingState$ = new BehaviorSubject<boolean>(false);
  
  // Form verilerini tutacak model - doğrudan loginReqDto kullanıyoruz
  loginModel: loginReqDto = {
    loginName: '',
    sifre: ''
  };
  
  constructor(
    private formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private authService: AuthService,
    private toastr: ToastrService,
    private spinner: NgxSpinnerService
  ) {
    // Eğer kullanıcı zaten giriş yapmışsa, dashboard'a yönlendir
    if (this.authService.isLoggedIn()) {
      this.router.navigate(['/secure/dashboard']);
    }
  }

  ngOnInit(): void {
    // Login formunu model üzerinden oluştur
    this.loginForm = this.formBuilder.group({
      loginName: [this.loginModel.loginName, [Validators.required]],
      sifre: [this.loginModel.sifre, [Validators.required]]
    });

    // return url'i al (eğer query param olarak gelmişse)
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/secure/dashboard';
  }

  // Form kontrollerine kolay erişim için bir getter
  get f() { return this.loginForm.controls; }
  
  // Form değerlerini doğrulamak için helper metot
  validateForm(): boolean {
    this.submitted = true;
    
    // Modeldeki değerleri form'a aktar
    this.loginForm.patchValue({
      loginName: this.loginModel.loginName,
      sifre: this.loginModel.sifre
    });
    
    return this.loginForm.valid;
  }

  // Özel buton bileşeni için click handler
  onLoginClick(): void {
    // Form geçerliliğini kontrol et
    if (!this.validateForm()) {
      return;
    }

    this.loading = true;
    this.loadingState$.next(true);
    
    // Spinner'ı göster
    this.spinner.show();

    // Doğrudan loginModel'i service'e gönder
    this.authService.login(this.loginModel)
      .subscribe({
        next: () => {
          // Başarılı giriş sonrası
          this.spinner.hide();
          this.loadingState$.next(false);
          this.toastr.success('Giriş işlemi başarıyla gerçekleştirildi.', 'Başarılı!');
          this.router.navigate([this.returnUrl]);
        },
        error: () => {
          // Hata durumunda spinner'ı gizle
          this.spinner.hide();
          this.loadingState$.next(false);
          this.loading = false;
        }
      });
  }
}