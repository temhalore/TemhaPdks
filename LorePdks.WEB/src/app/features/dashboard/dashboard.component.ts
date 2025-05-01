import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="container mt-4">
      <div class="row">
        <div class="col-12">
          <div class="card">
            <div class="card-header">
              <h4 class="mb-0">Dashboard</h4>
            </div>
            <div class="card-body">
              <p>Lore PDKS sistemine hoş geldiniz!</p>
              <p>Bu bölümden sistemdeki özet bilgileri takip edebilirsiniz.</p>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .card {
      box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
      border-radius: 8px;
      margin-bottom: 20px;
    }
    
    .card-header {
      background-color: #f8f9fa;
      border-radius: 8px 8px 0 0 !important;
      padding: 15px 20px;
    }
  `]
})
export class DashboardComponent {}