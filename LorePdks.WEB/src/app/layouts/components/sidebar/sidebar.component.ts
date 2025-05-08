import { Component, OnInit, HostListener, Output, EventEmitter } from '@angular/core';
import { Router, RouterModule, NavigationEnd } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ApiService } from '../../../core/services/api.service';
import { AuthService } from '../../../core/services/modules/auth.service';
import { EkranDto } from '../../../core/models/EkranDto';
import { KisiTokenDto } from '../../../core/models/KisiTokenDto';
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
      this.loadMenuDummyItems();
      
      // Gerçek veri kullanımı için yorum satırı olarak ekleyelim
      // this.loadMenuItems();
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
  loadMenuDummyItems(): void {
    this.createDummyMenuItems();
    this.updateMenuActiveState();
  }
  
  /**
   * Gerçek veri kullanarak menü öğeleri yüklenir
   * Not: Bu metot şu an aktif değil, EkranDto yapısını kullanan gerçek veri için
   */
  loadMenuItems(): void {
    this.authService.getUserFromLocalStorage().subscribe({
      next: (userData: KisiTokenDto | null) => {
        if (userData && userData.ekranDtoList && userData.ekranDtoList.length > 0) {
          // EkranDto listesini MenuItemModel listesine dönüştür
          this.menuItems = this.convertEkranDtoToMenuItems(userData.ekranDtoList);
          this.updateMenuActiveState();
        }
        // else {
        //   // Kullanıcının ekran listesi bulunamadıysa dummy data kullan
        //   this.createDummyMenuItems();
        //   this.updateMenuActiveState();
        // }
      },
      error: (error) => {
        console.error('Kullanıcı bilgileri alınırken hata oluştu:', error);
        this.createDummyMenuItems();
        this.updateMenuActiveState();
      }
    });
  }
  
  /**
   * EkranDto listesini MenuItemModel listesine dönüştürür
   * @param ekranList EkranDto listesi
   * @returns MenuItemModel listesi
   */
  convertEkranDtoToMenuItems(ekranList: EkranDto[]): MenuItemModel[] {
    // Üst seviye ekranları filtrele
    const topLevelItems = ekranList.filter(ekran => !ekran.ustEkranEidDto || !ekran.ustEkranEidDto.eid);
    
    // Sıra numarasına göre sırala
    topLevelItems.sort((a, b) => (a.siraNo || 0) - (b.siraNo || 0));
    
    // MenuItemModel listesine dönüştür
    return topLevelItems.map(ekran => this.convertEkranToMenuItem(ekran, ekranList));
  }
  
  /**
   * Tek bir EkranDto'yu MenuItemModel'e dönüştürür ve alt ekranları (varsa) ekler
   * @param ekran EkranDto
   * @param ekranList Tüm ekranların listesi
   * @returns MenuItemModel
   */
  convertEkranToMenuItem(ekran: EkranDto, ekranList: EkranDto[]): MenuItemModel {
    // Alt ekranları bul ve sırala
    let children: MenuItemModel[] = [];
    
    if (ekran.altEkranlar && ekran.altEkranlar.length > 0) {
      // Alt ekranları sırala
      ekran.altEkranlar.sort((a, b) => (a.siraNo || 0) - (b.siraNo || 0));
      
      // Alt ekranları dönüştür
      children = ekran.altEkranlar.map(altEkran => this.convertEkranToMenuItem(altEkran, ekranList));
    }
    
    // MenuItemModel oluştur
    return {
      label: ekran.ekranAdi,
      icon: ekran.ikon || 'pi pi-circle', // İkon boşsa varsayılan ikon
      route: ekran.ekranYolu,
      routerLink: ekran.ekranYolu ? [ekran.ekranYolu] : undefined,
      children: children.length > 0 ? children : undefined,
      expanded: false
    };
  }
  
  /**
   * Geçici menü öğeleri oluşturur (EkranDto yapısını taklit eder)
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
        label: 'Yetki Yönetimi',
        icon: 'pi pi-shield',
        children: [
          {
            label: 'Rol Yönetimi',
            icon: 'pi pi-users',
            route: '/secure/yetki/rol',
            routerLink: ['/secure/yetki/rol']
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