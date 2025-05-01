import { Component } from '@angular/core';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-main-layout',
  templateUrl: './main-layout.component.html',
  styleUrls: ['./main-layout.component.scss']
})
export class MainLayoutComponent {
  
  constructor(private authService: AuthService) { }
  
  /**
   * Kullanıcı oturumunu kapatır
   */
  logout(): void {
    this.authService.logout();
  }
}