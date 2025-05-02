import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from '../components/header/header.component';
import { SidebarComponent } from '../components/sidebar/sidebar.component';
import { FooterComponent } from '../components/footer/footer.component';
import { AuthService } from '../../modules/auth/services/auth.service';

@Component({
  selector: 'app-main-layout',
  templateUrl: './main-layout.component.html',
  styleUrls: ['./main-layout.component.scss'],
  standalone: true,
  imports: [RouterOutlet, HeaderComponent, SidebarComponent, FooterComponent]
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