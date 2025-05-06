import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { loginReqDto } from '../../../../core/models/loginReqDto';
import { AuthService } from '../../../../core/services/modules/auth.service';
import { ToastrService } from 'ngx-toastr';

// PrimeNG imports
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { PasswordModule } from 'primeng/password';
import { DividerModule } from 'primeng/divider';

// NGX-Spinner import
import { NgxSpinnerModule, NgxSpinnerService } from 'ngx-spinner';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
  standalone: true,
  imports: [
    CommonModule, 
    ReactiveFormsModule, 
    InputTextModule, 
    ButtonModule, 
    CardModule, 
    PasswordModule,
    DividerModule,
    NgxSpinnerModule
  ]
})
export class LoginComponent implements OnInit {
  loginForm!: FormGroup;
  loading = false;
  submitted = false;
  returnUrl: string = '/secure/dashboard';
  
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
    // Login formu oluştur
    this.loginForm = this.formBuilder.group({
      loginName: ['', [Validators.required]],
      sifre: ['', [Validators.required]]
    });

    // return url'i al (eğer query param olarak gelmişse)
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/secure/dashboard';
  }

  // Form kontrollerine kolay erişim için bir getter
  get f() { return this.loginForm.controls; }

  onSubmit(): void {
    this.submitted = true;

    // Form geçersizse işlemi durdur
    if (this.loginForm.invalid) {
      return;
    }

    this.loading = true;
    // Spinner'ı göster
    this.spinner.show();
    
    const loginRequest: loginReqDto = {
      loginName: this.f['loginName'].value,
      sifre: this.f['sifre'].value
    };

    this.authService.login(loginRequest)
      .subscribe({
        next: () => {
          // Başarılı giriş sonrası
          this.spinner.hide();
          this.toastr.success('Giriş işlemi başarıyla gerçekleştirildi.', 'Başarılı!');
          this.router.navigate([this.returnUrl]);
        },
        error: () => {
          // Hata durumunda spinner'ı gizle
          this.spinner.hide();
          this.loading = false;
        }
      });
  }
}