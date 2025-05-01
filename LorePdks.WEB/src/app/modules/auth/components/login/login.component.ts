import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../../../core/services/auth.service';
import { LoginRequest } from '../../../../core/models/login-request.model';
import Swal from 'sweetalert2';
import { CommonModule } from '@angular/common';

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
    private authService: AuthService
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
    
    const loginRequest: LoginRequest = {
      loginName: this.f['loginName'].value,
      sifre: this.f['sifre'].value
    };

    this.authService.login(loginRequest)
      .subscribe({
        next: () => {
          // Başarılı giriş sonrası SweetAlert göster
          Swal.fire({
            icon: 'success',
            title: 'Başarılı!',
            text: 'Giriş işlemi başarıyla gerçekleştirildi.',
            timer: 1500,
            showConfirmButton: false
          }).then(() => {
            // Başarılı giriş sonrası return url'e yönlendir
            this.router.navigate([this.returnUrl]);
          });
        },
        error: (error: unknown) => {
          console.error('Login error:', error);
          this.loading = false;
        }
      });
  }
}