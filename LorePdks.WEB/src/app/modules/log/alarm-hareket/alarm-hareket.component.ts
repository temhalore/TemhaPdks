import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { finalize } from 'rxjs';

// PrimeNG Modülleri
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { TableModule } from 'primeng/table';
import { TooltipModule } from 'primeng/tooltip';
import { CalendarModule } from 'primeng/calendar';
import { DropdownModule } from 'primeng/dropdown';
import { PanelModule } from 'primeng/panel';

// Servis ve Modeller
import { AlarmHareketService } from '../../../core/services/modules/alarm-hareket.service';
import { FirmaService } from '../../../core/services/modules/firma.service';
import { AlarmHareketDto } from '../../../core/models/AlarmHareketDto';
import { FirmaDto } from '../../../core/models/FirmaDto';

@Component({
  selector: 'app-alarm-hareket',
  templateUrl: './alarm-hareket.component.html',
  styleUrls: ['./alarm-hareket.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ButtonModule,
    InputTextModule,
    TooltipModule,
    CalendarModule,
    DropdownModule,
    PanelModule,
    TableModule
  ]
})
export class AlarmHareketListeComponent implements OnInit {
  // Alarm hareket listesi
  alarmHareketList: AlarmHareketDto[] = [];
  
  // Firma listesi (filtreleme için)
  firmaList: FirmaDto[] = [];
  
  // Filtre değişkenleri
  selectedFirma: FirmaDto | null = null;
  baslangicTarihi: Date | null = null;
  bitisTarihi: Date | null = null;
  
  // Loading durumları
  isLoading = false;

  constructor(
    private alarmHareketService: AlarmHareketService,
    private firmaService: FirmaService
  ) {}

  ngOnInit(): void {
    this.loadInitialData();
  }

  loadInitialData(): void {
    this.loadFirmaList();
    this.loadAlarmHareketList();
  }

  loadFirmaList(): void {
    this.firmaService.getAllFirmaList().subscribe({
      next: (data) => {
        this.firmaList = data || [];
      },
      error: (error) => {
        console.error('Firma listesi yüklenirken hata oluştu:', error);
      }
    });
  }

  loadAlarmHareketList(): void {
    this.isLoading = true;
    
    let observable;
    
    if (this.baslangicTarihi && this.bitisTarihi) {
      observable = this.alarmHareketService.getAlarmHareketListByDateRange(
        this.baslangicTarihi, 
        this.bitisTarihi
      );
    } else if (this.selectedFirma?.eid) {
      observable = this.alarmHareketService.getAlarmHareketListByFirmaId(this.selectedFirma.eid);
    } else {
      observable = this.alarmHareketService.getAllAlarmHareketList();
    }
    
    observable.pipe(
      finalize(() => this.isLoading = false)
    ).subscribe({
      next: (data) => {
        this.alarmHareketList = data || [];
      },
      error: (error) => {
        console.error('Alarm hareket listesi yüklenirken hata oluştu:', error);
        this.alarmHareketList = [];
      }
    });
  }

  applyFilters(): void {
    this.loadAlarmHareketList();
  }

  clearFilters(): void {
    this.selectedFirma = null;
    this.baslangicTarihi = null;
    this.bitisTarihi = null;
    this.loadAlarmHareketList();
  }

  editAlarmHareket(alarmHareket: AlarmHareketDto): void {
    console.log('Düzenleme:', alarmHareket);
  }

  deleteAlarmHareket(alarmHareket: AlarmHareketDto): void {
    if (!alarmHareket.eid) return;

    if (confirm('Bu alarm hareketini silmek istediğinizden emin misiniz?')) {
      this.alarmHareketService.deleteAlarmHareket(alarmHareket.eid)
        .subscribe({
          next: (success) => {
            if (success) {
              this.loadAlarmHareketList();
            }
          },
          error: (error) => {
            console.error('Alarm hareket silinirken hata oluştu:', error);
          }
        });
    }
  }
}
