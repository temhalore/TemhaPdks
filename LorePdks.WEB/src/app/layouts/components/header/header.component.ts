import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../../modules/auth/services/auth.service';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss'],
  standalone: true,
  imports: [CommonModule, RouterModule]
})
export class HeaderComponent {
  
  constructor(public authService: AuthService) { }

  logout(): void {
    this.authService.logout();
  }
}