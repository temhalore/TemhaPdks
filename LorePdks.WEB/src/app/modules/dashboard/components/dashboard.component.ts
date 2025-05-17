import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

// PrimeNG imports
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { ChartModule } from 'primeng/chart';
import { TableModule } from 'primeng/table';
import { SkeletonModule } from 'primeng/skeleton';
import { BadgeModule } from 'primeng/badge';
import { TagModule } from 'primeng/tag';

// NGX-Charts imports
import { NgxChartsModule } from '@swimlane/ngx-charts';

// NGX-Spinner import
import { NgxSpinnerModule, NgxSpinnerService } from 'ngx-spinner';

// Custom Components
import { IzinDagilimiComponent } from './izin-dagilimi/izin-dagilimi.component';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
  standalone: true,  imports: [
    CommonModule,
    CardModule,
    ButtonModule,
    ChartModule,
    TableModule,
    SkeletonModule,
    BadgeModule,
    TagModule,
    NgxChartsModule,
    NgxSpinnerModule,
    IzinDagilimiComponent
  ]
})
export class DashboardComponent implements OnInit {
  // NGX-Charts için veriler
  personelDurumData: any[] = [];
  izinDurumData: any[] = [];
  
  // Chart.js için veriler (PrimeNG Chart)
  girisDataChart: any;
  girisOptionsChart: any;
  
  // Tablo için veriler
  sonGirisKayitlari: any[] = [];
  
  // Dashboard özet verileri
  ozet = {
    toplamPersonel: 0,
    bugunIzinli: 0,
    bugunMesaide: 0,
    gecGiris: 0
  };
  
  // Yükleniyor durumu
  loading = true;
  
  constructor(private spinner: NgxSpinnerService) {}
  
  ngOnInit(): void {
    this.spinner.show();
    
    // Burada gerçek uygulamada API'den veri alacağız
    // Şimdilik test verileri oluşturacağız
    setTimeout(() => {
      this.initTestData();
      this.spinner.hide();
      this.loading = false;
    }, 1500);
  }
  
  /**
   * Test verileri oluşturur
   */
  initTestData(): void {
    // Özet verileri
    this.ozet = {
      toplamPersonel: 125,
      bugunIzinli: 8,
      bugunMesaide: 112,
      gecGiris: 5
    };
    
    // NGX-Charts için personel durum verileri
    this.personelDurumData = [
      {
        name: "İzinli",
        value: 8
      },
      {
        name: "Mesaide",
        value: 112
      },
      {
        name: "Geç Giriş",
        value: 5
      }
    ];
      // NGX-Charts için izin durum verileri
    this.izinDurumData = [
      {
        name: "Yıllık İzin",
        value: 5
      },
      {
        name: "Mazeret",
        value: 2
      },
      {
        name: "Rapor",
        value: 1
      },
      {
        name: "Doğum İzni",
        value: 0
      },
      {
        name: "Babalık İzni",
        value: 0
      }
    ];
    
    // PrimeNG Chart.js için giriş-çıkış istatistikleri
    this.girisDataChart = {
      labels: ['Pazartesi', 'Salı', 'Çarşamba', 'Perşembe', 'Cuma', 'Cumartesi', 'Pazar'],
      datasets: [
        {
          label: 'Giriş Sayısı',
          data: [110, 118, 115, 120, 116, 45, 12],
          backgroundColor: 'rgba(75, 192, 192, 0.2)',
          borderColor: 'rgb(75, 192, 192)',
          borderWidth: 1
        }
      ]
    };
    
    this.girisOptionsChart = {
      plugins: {
        legend: {
          labels: {
            font: {
              size: 14
            }
          }
        }
      },
      scales: {
        y: {
          beginAtZero: true
        }
      }
    };
    
    // Son giriş kayıtları tablosu için veriler
    this.sonGirisKayitlari = [
      {
        adSoyad: 'Ahmet Yılmaz',
        sicilNo: 'P001',
        girisTarihi: new Date(2025, 5, 6, 8, 2, 15),
        cikisTarihi: null,
        durumu: 'Mesaide'
      },
      {
        adSoyad: 'Ayşe Demir',
        sicilNo: 'P002',
        girisTarihi: new Date(2025, 5, 6, 8, 15, 0),
        cikisTarihi: null,
        durumu: 'Geç Giriş'
      },
      {
        adSoyad: 'Mehmet Kaya',
        sicilNo: 'P003',
        girisTarihi: new Date(2025, 5, 6, 8, 0, 45),
        cikisTarihi: null,
        durumu: 'Mesaide'
      },
      {
        adSoyad: 'Fatma Şahin',
        sicilNo: 'P004',
        girisTarihi: new Date(2025, 5, 6, 7, 55, 30),
        cikisTarihi: null,
        durumu: 'Mesaide'
      },
      {
        adSoyad: 'Ali Öztürk',
        sicilNo: 'P005',
        girisTarihi: new Date(2025, 5, 6, 8, 10, 12),
        cikisTarihi: null,
        durumu: 'Geç Giriş'
      }
    ];
  }
  
  /**
   * Durum etiketi için renk belirler
   */
  getDurumSeverity(durum: string): 'success' | 'secondary' | 'info' | 'warning' | 'danger' | 'contrast' | undefined {
    switch(durum) {
      case 'Mesaide':
        return 'success';
      case 'Geç Giriş':
        return 'warning';
      default:
        return 'info';
    }
  }
}