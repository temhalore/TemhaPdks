import { Component, OnInit, HostListener, Output, EventEmitter, OnDestroy, ElementRef, NgZone, ChangeDetectorRef, ChangeDetectionStrategy } from '@angular/core';
import { Router, RouterModule, NavigationEnd } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ApiService } from '../../../core/services/api.service';
import { AuthService } from '../../../core/services/modules/auth.service';
import { EkranDto } from '../../../core/models/EkranDto';
import { KisiTokenDto } from '../../../core/models/KisiTokenDto';
import { filter, debounceTime } from 'rxjs/operators';
import { Subject, takeUntil, fromEvent } from 'rxjs';
import { trigger, state, style, transition, animate } from '@angular/animations';

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
  showSubmenu?: boolean;
}

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.scss'],
  standalone: true,
  imports: [CommonModule, RouterModule, ButtonModule],
  changeDetection: ChangeDetectionStrategy.OnPush, // Performans iyileştirmesi için değişiklik algılama stratejisini değiştirelim
  animations: [
    trigger('submenuAnimation', [
      state('void', style({
        height: '0',
        opacity: '0',
        overflow: 'hidden'
      })),
      state('*', style({
        height: '*',
        opacity: '1'
      })),
      transition('void <=> *', animate('300ms cubic-bezier(0.25, 0.8, 0.25, 1)'))
    ])
  ]
})
export class SidebarComponent implements OnInit, OnDestroy {
  menuItems: MenuItemModel[] = [];
  
  // Sidebar durumları
  isCollapsed = false; // Kalıcı durum (toggle butonu ile değişir)
  isTemporaryExpanded = false; // Geçici durum (menü öğesine tıklama ile değişir)
  isMobileView = false; // Mobil görünüm durumu
  hasHoveredSubmenu = false; // Alt menüye hover yapıldı mı?
  
  currentUrl: string = '';
  private destroy$ = new Subject<void>();
  
  @Output() sidebarToggled = new EventEmitter<boolean>();

  @HostListener('window:resize', ['$event'])
  onResize() {
    this.checkScreenSize();
  }
  
  // Dışarı tıklama olayı
  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent) {
    // Mobil görünümde veya geçici genişleme durumundaysa sidebar dışına tıklama kontrolü
    if ((this.isTemporaryExpanded || (!this.isCollapsed && this.isMobileView)) && 
        this.elementRef.nativeElement && !this.elementRef.nativeElement.contains(event.target)) {
      this.isTemporaryExpanded = false;
      
      // Mobil görünümde sidebar'ı kapat
      if (this.isMobileView) {
        this.isCollapsed = true;
        this.sidebarToggled.emit(this.isCollapsed);
      }
    }
  }

  constructor(
    private apiService: ApiService,
    private authService: AuthService,
    private router: Router,
    private elementRef: ElementRef,
    private ngZone: NgZone,
    private cdr: ChangeDetectorRef
  ) {
    // URL değişikliklerini dinle
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd),
      takeUntil(this.destroy$)
    ).subscribe((event: any) => {
      this.currentUrl = event.url;
      this.updateMenuActiveState();
      
      // Sayfa değiştiğinde geçici genişlemeyi kapat
      if (this.isTemporaryExpanded) {
        this.isTemporaryExpanded = false;
      }
      
      // Mobil görünümde sayfa değiştiğinde sidebar'ı kapat
      if (this.isMobileView && !this.isCollapsed) {
        this.isCollapsed = true;
        this.sidebarToggled.emit(this.isCollapsed);
      }
      
      // Değişiklikleri görüntülemek için change detection'ı tetikle
      this.cdr.detectChanges();
    });
  }

  ngOnInit(): void {
    // Ekran boyutunu kontrol et
    this.checkScreenSize();
    this.currentUrl = this.router.url;
    
    // Kullanıcı giriş yapmışsa menü öğelerini getir
    if (this.authService.isLoggedIn()) {
      this.loadMenuItems();
      // Eski çağrı: this.loadMenuDummyItems();
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  /**
   * Ekran boyutunu kontrol eder ve mobil görünümü belirler
   */
  checkScreenSize(): void {
    const prevIsMobile = this.isMobileView;
    this.isMobileView = window.innerWidth <= 768;
    
    // Mobil görünüme geçiş anında sidebar durumunu değiştir
    if (!prevIsMobile && this.isMobileView) {
      // Mobil görünüme geçildiğinde sidebar'ı kapat
      this.isCollapsed = true;
      this.isTemporaryExpanded = false;
      this.sidebarToggled.emit(this.isCollapsed);
    }
  }

  /**
   * Menü öğesine hover yapıldığında tetiklenir
   */
  onMenuItemHover(event: MouseEvent, item: MenuItemModel): void {
    // Mobil görünümde hover'ı devre dışı bırak
    if (this.isMobileView) return;
    
    // Alt menüsü olan bir öğeye hover yapıldıysa
    if (this.isCollapsed && item.children && item.children.length > 0) {
      this.hasHoveredSubmenu = true;
      // Alt menüsü olan öğelerde hover genişlemesini engelle
    } else if (this.isCollapsed && (!item.children || item.children.length === 0)) {
      // Alt menüsü olmayan öğelerde hover genişlemesini etkinleştir
      this.hasHoveredSubmenu = false;
    }
  }

  /**
   * Mouse sidebar'dan ayrıldığında tetiklenir
   */
  onMouseLeave(): void {
    // Sadece kalıcı durum küçük ve geçici durum genişletilmemiş ise hover efektlerini sıfırla
    if (this.isCollapsed && !this.isTemporaryExpanded) {
      this.hasHoveredSubmenu = false;
      
      // Tüm alt menüleri kapat
      this.menuItems.forEach(item => {
        if (item.showSubmenu) {
          item.showSubmenu = false;
        }
      });
    }
  }

  /**
   * Menü öğesine tıklandığında ilgili rotaya yönlendirir veya alt menüyü açar/kapatır
   * Ayrıca geçici genişleme durumunu kontrol eder
   */
  onMenuItemClick(item: MenuItemModel, event?: MouseEvent): void {
    // Tıklama olayının yayılmasını engelle (varsa)
    if (event) {
      event.stopPropagation();
    }
    
    // Alt menüsü olan öğelere tıklandığında
    if (item.children && item.children.length > 0) {
      // Alt menüyü aç/kapat
      item.expanded = !item.expanded;
      
      // Sidebar küçük durumdaysa geçici olarak genişlet
      if (this.isCollapsed && !this.isTemporaryExpanded) {
        this.isTemporaryExpanded = true;
      }
      
      // Diğer açık menüleri kapat
      if (item.expanded) {
        this.menuItems.forEach(menuItem => {
          if (menuItem !== item) {
            menuItem.expanded = false;
          }
        });
      }
      
      // Change detection mekanizmasını manuel olarak tetikle
      this.cdr.detectChanges();
    } else if (item.route) {
      // Alt menüsü yoksa ve bir rota tanımlanmışsa, o rotaya git
      this.router.navigate([item.route]);
      
      // Router event zaten sayfa değişiminde sidebar'ı kapatacak
    }
  }
  /**
   * Alt menü öğesine tıklandığında rota yönlendirmesi yapar
   */
  onSubMenuItemClick(subItem: MenuItemModel, event: MouseEvent): void {
    // Tıklama olayının yayılmasını engelle
    event.stopPropagation();
    
    // Rota tanımlanmışsa, o rotaya git
    if (subItem.route) {
      this.router.navigate([subItem.route]);
    }
  }

  /**
   * Toggle butonu için - kalıcı durum değişimi
   */
  toggleSidebar(): void {
    this.isCollapsed = !this.isCollapsed;
    
    // Kalıcı durum değiştiğinde geçici genişlemeyi sıfırla
    this.isTemporaryExpanded = false;
    
    // Mobil görünümde overlay kontrolü için durumu dış bileşene bildir
    this.sidebarToggled.emit(this.isCollapsed);
  }

  /**
   * Mobil menü butonuna tıklandığında sidebar'ı aç/kapat
   */
  toggleMobileMenu(): void {
    if (this.isMobileView) {
      this.isCollapsed = !this.isCollapsed;
      this.sidebarToggled.emit(this.isCollapsed);
    }
  }

  /**
   * Ekran boyutuna göre menü öğelerini yükler (dummy veya gerçek veri)
   */
  loadMenuItems(): void {
    // Ekran boyutuna göre dummy veya gerçek menü öğelerini yükle
    // if (this.isMobileView) {
    //   // Mobil görünümde dummy öğeleri yükle
    //   this.createDummyMenuItems();
    // } else {
      // Masaüstü görünümda gerçek öğeleri yükle
      this.authService.getUserFromLocalStorage().subscribe({
        next: (userData: KisiTokenDto | null) => {
          if (userData && userData.ekranDtoList && userData.ekranDtoList.length > 0) {
            // EkranDto listesini MenuItemModel listesine dönüştür
            this.createDummyMenuItems();
            //this.menuItems = this.convertEkranDtoToMenuItems(userData.ekranDtoList);
          } else {
            // Kullanıcının ekran listesi bulunamadıysa dummy data kullan
            this.createDummyMenuItems();
          }
          this.updateMenuActiveState();
        },
        error: (error) => {
          console.error('Kullanıcı bilgileri alınırken hata oluştu:', error);
          // this.createDummyMenuItems();
          this.updateMenuActiveState();
        }
      });
    // }
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
        ]      },      {
        label: 'Yetki Yönetimi',
        icon: 'pi pi-shield',
        children: [
          {
            label: 'Rol Yönetimi',
            icon: 'pi pi-users',
            route: '/secure/yetki/rol',
            routerLink: ['/secure/yetki/rol']
          },          {
            label: 'Ekran Yönetimi',
            icon: 'pi pi-desktop',
            route: '/secure/yetki/ekran',
            routerLink: ['/secure/yetki/ekran']          }
        ]
      },      {
        label: 'Kişi Yönetimi',
        icon: 'pi pi-user',
        children: [
          {
            label: 'Kişi Listesi',
            icon: 'pi pi-list',
            route: '/secure/kisi/liste',
            routerLink: ['/secure/kisi/liste']
          }
        ]
      },      {
        label: 'Kod Yönetimi',
        icon: 'pi pi-list-ol',
        children: [
          {
            label: 'Kod Listesi',
            icon: 'pi pi-list',
            route: '/secure/kod/liste',
            routerLink: ['/secure/kod/liste']
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
}