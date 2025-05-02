import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { loginReqDto } from '../../../../core/models/loginReqDto';
import { AuthService } from '../../services/auth.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule]
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
    private toastr: ToastrService
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
    
    const loginRequest: loginReqDto = {
      loginName: this.f['loginName'].value,
      sifre: this.f['sifre'].value
    };

    this.authService.login(loginRequest)
      .subscribe({
        next: () => {
          // Başarılı giriş sonrası
          this.toastr.success('Giriş işlemi başarıyla gerçekleştirildi.', 'Başarılı!');
          this.router.navigate([this.returnUrl]);
        },
        error: () => {
          // Hata interceptor tarafından yönetilecek
          this.loading = false;
        }
      });
  }
}