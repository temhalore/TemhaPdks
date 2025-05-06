import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../../modules/auth/services/auth.service';

// PrimeNG imports
import { MenuModule } from 'primeng/menu';
import { ButtonModule } from 'primeng/button';
import { AvatarModule } from 'primeng/avatar';
import { MenuItem } from 'primeng/api';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss'],
  standalone: true,
  imports: [CommonModule, RouterModule, MenuModule, ButtonModule, AvatarModule]
})
export class HeaderComponent implements OnInit {
  
  userMenuItems: MenuItem[] = [];
  
  constructor(public authService: AuthService) { }
  
  ngOnInit(): void {
    this.initUserMenu();
  }
  
  /**
   * Kullanıcı menüsünü başlatır
   */
  initUserMenu(): void {
    this.userMenuItems = [
      {
        label: 'Profil',
        icon: 'pi pi-user',
        command: () => {
          // Profil sayfasına yönlendirme yapılabilir
        }
      },
      {
        label: 'Ayarlar',
        icon: 'pi pi-cog',
        command: () => {
          // Ayarlar sayfasına yönlendirme yapılabilir
        }
      },
      {
        separator: true
      },
      {
        label: 'Çıkış',
        icon: 'pi pi-sign-out',
        command: () => {
          this.logout();
        }
      }
    ];
  }

  logout(): void {
    this.authService.logout();
  }
}