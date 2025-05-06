import { Component, OnInit } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ApiService } from '../../../core/services/api.service';
import { AuthService } from '../../../core/services/modules/auth.service';
import { EkranDto } from '../../../core/models/EkranDto';

// PrimeNG imports
import { PanelMenuModule } from 'primeng/panelmenu';
import { MenuItem } from 'primeng/api';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.scss'],
  standalone: true,
  imports: [CommonModule, RouterModule, PanelMenuModule, ButtonModule]
})
export class SidebarComponent implements OnInit {
  menuItems: EkranDto[] = [];
  panelMenuItems: MenuItem[] = [];
  isCollapsed = false;

  constructor(
    private apiService: ApiService,
    private authService: AuthService,
    private router: Router
  ) { }

  ngOnInit(): void {
    // Kullanıcı giriş yapmışsa menü öğelerini getir
    if (this.authService.isLoggedIn()) {
      this.loadMenuItems();
    }
  }

  /**
   * Kullanıcıya ait menü öğelerini yükler
   */
  loadMenuItems(): void {
    this.menuItems = this.authService.currentUserValue?.rolDtoList?.ekranlar? [] : [];
    //const kisiId = this.authService.currentUserValue?.eid;
    // if (kisiId) {
    //   // API'den menü öğelerini getir
    //   this.apiService.post<any[]>('Yetki/getMenuByKisiId', { id: kisiId })
    //     .subscribe({
    //       next: (response: ServiceResponse<any[]>) => {
    //         if (response.isSuccess && response.data) {
    //           this.menuItems = response.data;
    //           this.convertToMenuItems(); // Menü öğelerini PrimeNG formatına dönüştür
    //         }
    //       },
    //       error: (error: unknown) => {
    //         console.error('Menü yüklenirken hata oluştu:', error);
    //       }
    //     });
    // }
    
    // Geçici olarak sabit menü öğeleri oluşturalım
    this.createDummyMenuItems();
  }
  
  /**
   * Geçici menü öğeleri oluşturur (API bağlantısı yokken test için)
   */
  createDummyMenuItems(): void {
    this.panelMenuItems = [
      {
        label: 'Gösterge Paneli',
        icon: 'pi pi-home',
        routerLink: ['/secure/dashboard']
      },
      {
        label: 'Personel Yönetimi',
        icon: 'pi pi-users',
        items: [
          {
            label: 'Personel Listesi',
            icon: 'pi pi-list',
            routerLink: ['/secure/personel/liste']
          },
          {
            label: 'Yeni Personel',
            icon: 'pi pi-user-plus',
            routerLink: ['/secure/personel/yeni']
          }
        ]
      },
      {
        label: 'PDKS İşlemleri',
        icon: 'pi pi-clock',
        items: [
          {
            label: 'Giriş-Çıkış Kayıtları',
            icon: 'pi pi-calendar',
            routerLink: ['/secure/pdks/kayitlar']
          },
          {
            label: 'İzin İşlemleri',
            icon: 'pi pi-calendar-plus',
            routerLink: ['/secure/pdks/izinler']
          }
        ]
      },
      {
        label: 'Raporlar',
        icon: 'pi pi-chart-bar',
        items: [
          {
            label: 'Aylık Rapor',
            icon: 'pi pi-file',
            routerLink: ['/secure/rapor/aylik']
          },
          {
            label: 'Yıllık Rapor',
            icon: 'pi pi-file',
            routerLink: ['/secure/rapor/yillik']
          }
        ]
      },
      {
        label: 'Ayarlar',
        icon: 'pi pi-cog',
        routerLink: ['/secure/ayarlar']
      }
    ];
  }

  /**
   * EkranDto türündeki menü öğelerini PrimeNG MenuItem formatına dönüştürür
   */
  convertToMenuItems(): void {
    this.panelMenuItems = this.menuItems.map(item => this.mapToMenuItem(item));
  }
  
  /**
   * Bir EkranDto nesnesini MenuItem nesnesine dönüştürür
   */
  mapToMenuItem(ekran: EkranDto): MenuItem {
    const menuItem: MenuItem = {
      label: ekran.ekranAdi,
      icon: this.getIconClass(ekran.ikon),
      routerLink: ekran.ekranYolu ? [ekran.ekranYolu] : undefined
    };
    
    // Alt ekranlar varsa dönüştür
    if (ekran.altEkranlar && ekran.altEkranlar.length > 0) {
      menuItem.items = ekran.altEkranlar.map(subItem => this.mapToMenuItem(subItem));
    }
    
    return menuItem;
  }
  
  /**
   * Font Awesome ikon sınıfını PrimeNG ikon sınıfına dönüştürür
   */
  getIconClass(iconClass?: string): string {
    if (!iconClass) return 'pi pi-circle';
    
    // Font Awesome ikonlarını PrimeNG ikonlarına dönüştür
    // Bu basit bir dönüşüm, gerçek projenizde daha kapsamlı olabilir
    const iconMap: { [key: string]: string } = {
      'fa-home': 'pi pi-home',
      'fa-users': 'pi pi-users',
      'fa-user': 'pi pi-user',
      'fa-clock': 'pi pi-clock',
      'fa-calendar': 'pi pi-calendar',
      'fa-chart-bar': 'pi pi-chart-bar',
      'fa-cog': 'pi pi-cog',
      'fa-circle': 'pi pi-circle'
    };
    
    // Map'te varsa dönüştür, yoksa varsayılan ikon kullan
    return iconMap[iconClass] || 'pi pi-circle';
  }

  /**
   * Sidebar'ı daraltır veya genişletir
   */
  toggleSidebar(): void {
    this.isCollapsed = !this.isCollapsed;
  }
}