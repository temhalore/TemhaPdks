import { Component, OnInit, HostListener, Output, EventEmitter } from '@angular/core';
import { Router, RouterModule, NavigationEnd } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ApiService } from '../../../core/services/api.service';
import { AuthService } from '../../../core/services/modules/auth.service';
import { EkranDto } from '../../../core/models/EkranDto';
import { filter } from 'rxjs/operators';

// PrimeNG imports
import { ButtonModule } from 'primeng/button';

// Menü öğesi arayüzü
interface MenuItemModel {
  label: string;
  icon: string;
  route?: string;
  children?: MenuItemModel[];
  expanded?: boolean;
  routerLink?: string[];
}

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.scss'],
  standalone: true,
  imports: [CommonModule, RouterModule, ButtonModule]
})
export class SidebarComponent implements OnInit {
  menuItems: MenuItemModel[] = [];
  isCollapsed = false;
  isHoverExpanded = false;
  isMobileView = false;
  currentUrl: string = '';
  
  @Output() sidebarToggled = new EventEmitter<boolean>();

  @HostListener('window:resize', ['$event'])
  onResize() {
    this.checkScreenSize();
  }

  constructor(
    private apiService: ApiService,
    private authService: AuthService,
    private router: Router
  ) {
    // URL değişikliklerini dinle
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe((event: any) => {
      this.currentUrl = event.url;
      this.updateMenuActiveState();
    });
  }

  ngOnInit(): void {
    // Ekran boyutunu kontrol et
    this.checkScreenSize();
    this.currentUrl = this.router.url;
    
    // Kullanıcı giriş yapmışsa menü öğelerini getir
    if (this.authService.isLoggedIn()) {
      this.loadMenuItems();
    }
  }

  /**
   * Ekran boyutunu kontrol eder ve mobil görünümü belirler
   */
  checkScreenSize(): void {
    this.isMobileView = window.innerWidth <= 768;
    // Mobil görünümde sidebar'ı otomatik olarak kapat
    if (this.isMobileView && !this.isCollapsed) {
      this.isCollapsed = true;
      this.sidebarToggled.emit(this.isCollapsed);
    }
  }

  /**
   * Menü öğesine hover yapıldığında tetiklenir
   */
  onMenuItemHover(event: MouseEvent): void {
    if (this.isCollapsed && !this.isMobileView) {
      this.isHoverExpanded = true;
    }
  }

  /**
   * Mouse sidebar'dan ayrıldığında tetiklenir
   */
  onMouseLeave(): void {
    if (this.isCollapsed && this.isHoverExpanded) {
      this.isHoverExpanded = false;
    }
  }

  /**
   * Menü öğeleri yüklenir
   */
  loadMenuItems(): void {
    // Gerçek bir API bağlantısı için:
    // const kisiId = this.authService.currentUserValue?.eid;
    // if (kisiId) {
    //   this.apiService.post<any[]>('Yetki/getMenuByKisiId', { id: kisiId })
    //     .subscribe({
    //       next: (response) => {
    //         if (response.isSuccess && response.data) {
    //           this.menuItems = this.convertToMenuItems(response.data);
    //           this.updateMenuActiveState();
    //         }
    //       },
    //       error: (error) => console.error('Menü yüklenirken hata oluştu:', error)
    //     });
    // }
    
    // Geçici menü öğeleri
    this.createDummyMenuItems();
    this.updateMenuActiveState();
  }
  
  /**
   * Geçici menü öğeleri oluşturur
   */
  createDummyMenuItems(): void {
    this.menuItems = [
      {
        label: 'Gösterge Paneli',
        icon: 'pi pi-home',
        route: '/secure/dashboard',
        routerLink: ['/secure/dashboard']
      },
      {
        label: 'Personel Yönetimi',
        icon: 'pi pi-users',
        children: [
          {
            label: 'Personel Listesi',
            icon: 'pi pi-list',
            route: '/secure/personel/liste',
            routerLink: ['/secure/personel/liste']
          },
          {
            label: 'Yeni Personel',
            icon: 'pi pi-user-plus',
            route: '/secure/personel/yeni',
            routerLink: ['/secure/personel/yeni']
          }
        ]
      },
      {
        label: 'PDKS İşlemleri',
        icon: 'pi pi-clock',
        children: [
          {
            label: 'Giriş-Çıkış Kayıtları',
            icon: 'pi pi-calendar',
            route: '/secure/pdks/kayitlar',
            routerLink: ['/secure/pdks/kayitlar']
          },
          {
            label: 'İzin İşlemleri',
            icon: 'pi pi-calendar-plus',
            route: '/secure/pdks/izinler',
            routerLink: ['/secure/pdks/izinler']
          }
        ]
      },
      {
        label: 'Raporlar',
        icon: 'pi pi-chart-bar',
        children: [
          {
            label: 'Aylık Rapor',
            icon: 'pi pi-file',
            route: '/secure/rapor/aylik',
            routerLink: ['/secure/rapor/aylik']
          },
          {
            label: 'Yıllık Rapor',
            icon: 'pi pi-file',
            route: '/secure/rapor/yillik',
            routerLink: ['/secure/rapor/yillik']
          }
        ]
      },
      {
        label: 'Ayarlar',
        icon: 'pi pi-cog',
        route: '/secure/ayarlar',
        routerLink: ['/secure/ayarlar']
      }
    ];
  }

  /**
   * Menü öğesine tıklandığında ilgili rotaya yönlendirir veya alt menüyü açar/kapatır
   */
  navigateToLink(item: MenuItemModel): void {
    if (item.children && item.children.length > 0) {
      // Alt menüsü varsa, açılıp kapanmasını toggle et
      item.expanded = !item.expanded;
      
      // Diğer tüm açık menüleri kapat
      if (item.expanded) {
        this.menuItems.forEach(menuItem => {
          if (menuItem !== item) {
            menuItem.expanded = false;
          }
        });
      }
    } else if (item.route) {
      // Alt menüsü yoksa ve bir rota tanımlanmışsa, o rotaya git
      this.router.navigate([item.route]);
      
      // Mobil görünümde menüyü kapat
      if (this.isMobileView) {
        this.isCollapsed = true;
        this.sidebarToggled.emit(this.isCollapsed);
      }
    }
  }

  /**
   * Belirtilen menü öğesinin aktif olup olmadığını kontrol eder
   */
  isMenuItemActive(item: MenuItemModel): boolean {
    if (!item.route) return false;
    
    if (this.currentUrl === item.route) {
      return true;
    }
    
    // Alt menüler için üst menüyü de aktif olarak işaretle
    if (item.children && item.children.length > 0) {
      return item.children.some(child => this.currentUrl === child.route);
    }
    
    return false;
  }

  /**
   * Mevcut URL'ye göre menü durumunu günceller
   */
  updateMenuActiveState(): void {
    // Aktif menü öğesini ve üst menüyü bul
    this.menuItems.forEach(item => {
      // Doğrudan eşleşme
      if (item.route === this.currentUrl) {
        item.expanded = true;
      }
      
      // Alt menülerde eşleşme
      if (item.children && item.children.length > 0) {
        const hasActiveChild = item.children.some(child => child.route === this.currentUrl);
        if (hasActiveChild) {
          item.expanded = true;
        }
      }
    });
  }

  /**
   * Sidebar'ı daraltır veya genişletir
   */
  toggleSidebar(): void {
    this.isCollapsed = !this.isCollapsed;
    this.sidebarToggled.emit(this.isCollapsed);
  }
}