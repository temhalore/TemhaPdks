import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/modules/auth.service';

// PrimeNG imports
import { MenuModule } from 'primeng/menu';
import { ButtonModule } from 'primeng/button';
import { AvatarModule } from 'primeng/avatar';
import { OverlayPanelModule } from 'primeng/overlaypanel';
import { MenuItem } from 'primeng/api';

// Bildirim türü için arayüz
interface Notification {
  id: number;
  title: string;
  message: string;
  time: Date;
  type: 'info' | 'success' | 'warning' | 'error';
  read: boolean;
}

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss'],
  standalone: true,
  imports: [CommonModule, RouterModule, MenuModule, ButtonModule, AvatarModule, OverlayPanelModule]
})
export class HeaderComponent implements OnInit {
  
  userMenuItems: MenuItem[] = [];
  notifications: Notification[] = [];
  notificationCount = 0;
  
  constructor(public authService: AuthService) { }
  
  ngOnInit(): void {
    this.initUserMenu();
    this.loadDummyNotifications();
  }
  
  /**
   * Test için örnek bildirimler yükler
   */
  loadDummyNotifications(): void {
    this.notifications = [
      {
        id: 1,
        title: 'Onay Bekleyen İzin',
        message: 'Ahmet Yılmaz\'ın izin talebi onay bekliyor.',
        time: new Date(),
        type: 'info',
        read: false
      },
      {
        id: 2,
        title: 'Başarılı İşlem',
        message: 'Personel kartı başarıyla güncellendi.',
        time: new Date(Date.now() - 3600000), // 1 saat önce
        type: 'success',
        read: false
      }
    ];
    
    // Okunmamış bildirimleri say
    this.notificationCount = this.notifications.filter(n => !n.read).length;
  }
  
  /**
   * Bildirim tipine göre ikon sınıfı döndürür
   */
  getNotificationIcon(type: string): string {
    switch(type) {
      case 'info': return 'pi-info-circle';
      case 'success': return 'pi-check-circle';
      case 'warning': return 'pi-exclamation-triangle';
      case 'error': return 'pi-times-circle';
      default: return 'pi-info-circle';
    }
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