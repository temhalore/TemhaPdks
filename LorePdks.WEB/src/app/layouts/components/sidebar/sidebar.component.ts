import { Component, OnInit } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { ApiService } from '../../../core/services/api.service';
import { ServiceResponse } from '../../../core/models/ServiceResponse';
import { CommonModule } from '@angular/common';
import { EkranDTO } from '../../../core/models/EkranDTO';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.scss'],
  standalone: true,
  imports: [CommonModule, RouterModule]
})
export class SidebarComponent implements OnInit {
  menuItems: EkranDTO[] = [];
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
    //         }
    //       },
    //       error: (error: unknown) => {
    //         console.error('Menü yüklenirken hata oluştu:', error);
    //       }
    //     });
    // }
  }

  /**
   * Sidebar'ı daraltır veya genişletir
   */
  toggleSidebar(): void {
    this.isCollapsed = !this.isCollapsed;
  }
}