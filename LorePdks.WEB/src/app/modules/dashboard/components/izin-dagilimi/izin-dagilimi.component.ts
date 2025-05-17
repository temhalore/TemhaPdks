import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChartModule } from 'primeng/chart';
import { CardModule } from 'primeng/card';
import { SkeletonModule } from 'primeng/skeleton';

interface IzinVerisi {
  name: string;
  value: number;
  renk: string;
}

@Component({
  selector: 'app-izin-dagilimi',
  templateUrl: './izin-dagilimi.component.html',
  styleUrls: ['./izin-dagilimi.component.scss'],
  standalone: true,
  imports: [CommonModule, ChartModule, CardModule, SkeletonModule]
})
export class IzinDagilimiComponent implements OnChanges {
  @Input() loading = false;
  @Input() izinDurumData: any[] = [];

  // Pasta grafik verileri
  pieChartData: any;
  pieChartOptions: any;
  
  // Donut grafik verileri
  donutChartData: any;
  donutChartOptions: any;
  
  // İzin verileri zenginleştirilmiş hali
  izinVerileri: IzinVerisi[] = [];
  
  constructor() {
    this.initChartOptions();
  }
    ngOnChanges(changes: SimpleChanges): void {
    if (changes['izinDurumData']) {
      if (this.izinDurumData && this.izinDurumData.length > 0) {
        // Sadece değeri 0'dan büyük izin türlerini filtrele
        const gecerliIzinVerileri = this.izinDurumData.filter(item => item.value > 0);
        this.izinDurumData = gecerliIzinVerileri;
        this.izinVerileriniZenginlestir();
        this.updateChartData();
      }
    }
  }
  
  private izinVerileriniZenginlestir(): void {
    // Sabit renkler tanımlayalım
    const renkler = ['#FF6384', '#36A2EB', '#FFCE56', '#4BC0C0', '#9966FF', '#FF9F40'];
    
    // Gelen verileri zenginleştir
    this.izinVerileri = this.izinDurumData.map((item, index) => ({
      ...item,
      renk: renkler[index % renkler.length]
    }));
  }
  
  private updateChartData(): void {
    // Pasta grafik için verileri hazırla
    this.pieChartData = {
      labels: this.izinVerileri.map(item => item.name),
      datasets: [
        {
          data: this.izinVerileri.map(item => item.value),
          backgroundColor: this.izinVerileri.map(item => item.renk),
          hoverBackgroundColor: this.izinVerileri.map(item => this.adjustBrightness(item.renk, 20))
        }
      ]
    };

    // Donut grafik için aynı verileri kullan
    this.donutChartData = { ...this.pieChartData };
  }
  
  private initChartOptions(): void {
    // Pasta grafik seçenekleri
    this.pieChartOptions = {
      plugins: {
        legend: {
          position: 'right',
          labels: {
            font: {
              family: 'Roboto, sans-serif',
              size: 12
            }
          }
        },
        tooltip: {
          callbacks: {
            label: (context: any) => {
              return `${context.label}: ${context.raw} kişi`;
            }
          }
        }
      },
      responsive: true,
      maintainAspectRatio: false
    };

    // Donut grafik seçenekleri
    this.donutChartOptions = {
      cutout: '60%',
      plugins: {
        legend: {
          position: 'bottom',
          labels: {
            font: {
              family: 'Roboto, sans-serif',
              size: 12
            }
          }
        },
        tooltip: {
          callbacks: {
            label: (context: any) => {
              const total = context.dataset.data.reduce((sum: number, num: number) => sum + num, 0);
              const percentage = Math.round((context.raw / total) * 100);
              return `${context.label}: ${context.raw} kişi (${percentage}%)`;
            }
          }
        }
      },
      responsive: true,
      maintainAspectRatio: false
    };
  }
  
  // Renk parlaklığını ayarlamak için yardımcı metod
  adjustBrightness(hex: string, percent: number): string {
    // Hex'i RGB'ye dönüştür
    let r = parseInt(hex.substring(1, 3), 16);
    let g = parseInt(hex.substring(3, 5), 16);
    let b = parseInt(hex.substring(5, 7), 16);

    // Parlaklığı ayarla
    r = Math.min(255, Math.max(0, r + percent));
    g = Math.min(255, Math.max(0, g + percent));
    b = Math.min(255, Math.max(0, b + percent));

    // RGB'yi tekrar Hex'e çevir
    return "#" + ((1 << 24) + (r << 16) + (g << 8) + b).toString(16).slice(1);
  }

  // İzin sayılarının toplamını hesapla
  get toplamIzin(): number {
    return this.izinVerileri.reduce((toplam, izin) => toplam + izin.value, 0);
  }
}
