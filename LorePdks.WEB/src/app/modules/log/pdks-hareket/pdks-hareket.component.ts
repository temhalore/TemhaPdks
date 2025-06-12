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
import { PdksHareketService } from '../../../core/services/modules/pdks-hareket.service';
import { FirmaService } from '../../../core/services/modules/firma.service';
import { PdksHareketDto } from '../../../core/models/PdksHareketDto';
import { FirmaDto } from '../../../core/models/FirmaDto';

@Component({
  selector: 'app-pdks-hareket',
  templateUrl: './pdks-hareket.component.html',
  styleUrls: ['./pdks-hareket.component.scss'],
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
export class PdksHareketListeComponent implements OnInit {
  // PDKS hareket listesi
  pdksHareketList: PdksHareketDto[] = [];
  
  // Firma listesi (filtreleme için)
  firmaList: FirmaDto[] = [];
  
  // Filtre değişkenleri
  selectedFirma: FirmaDto | null = null;
  baslangicTarihi: Date | null = null;
  bitisTarihi: Date | null = null;
  
  // Loading durumları
  isLoading = false;

  constructor(
    private pdksHareketService: PdksHareketService,
    private firmaService: FirmaService
  ) {}

  ngOnInit(): void {
    this.loadInitialData();
  }

  /**
   * İlk yükleme verilerini getirir
   */
  loadInitialData(): void {
    this.loadFirmaList();
    this.loadPdksHareketList();
  }

  /**
   * Firma listesini yükler
   */
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

  /**
   * PDKS hareket listesini yükler
   */
  loadPdksHareketList(): void {
    this.isLoading = true;
    
    let observable;
    
    if (this.baslangicTarihi && this.bitisTarihi) {
      observable = this.pdksHareketService.getPdksHareketListByDateRange(
        this.baslangicTarihi, 
        this.bitisTarihi
      );
    } else if (this.selectedFirma?.eid) {
      observable = this.pdksHareketService.getPdksHareketListByFirmaId(this.selectedFirma.eid);
    } else {
      observable = this.pdksHareketService.getAllPdksHareketList();
    }
    
    observable.pipe(
      finalize(() => this.isLoading = false)
    ).subscribe({
      next: (data) => {
        this.pdksHareketList = data || [];
      },
      error: (error) => {
        console.error('PDKS hareket listesi yüklenirken hata oluştu:', error);
        this.pdksHareketList = [];
      }
    });
  }

  /**
   * Filtreleri uygular
   */
  applyFilters(): void {
    this.loadPdksHareketList();
  }

  /**
   * Filtreleri temizler
   */
  clearFilters(): void {
    this.selectedFirma = null;
    this.baslangicTarihi = null;
    this.bitisTarihi = null;
    this.loadPdksHareketList();
  }

  /**
   * PDKS hareket düzenleme modalını açar
   */
  editPdksHareket(pdksHareket: PdksHareketDto): void {
    console.log('Düzenleme:', pdksHareket);
  }

  /**
   * PDKS hareket siler
   */
  deletePdksHareket(pdksHareket: PdksHareketDto): void {
    if (!pdksHareket.eid) return;

    if (confirm('Bu PDKS hareketini silmek istediğinizden emin misiniz?')) {
      this.pdksHareketService.deletePdksHareket(pdksHareket.eid)
        .subscribe({
          next: (success) => {
            if (success) {
              this.loadPdksHareketList();
            }
          },
          error: (error) => {
            console.error('PDKS hareket silinirken hata oluştu:', error);
          }
        });
    }
  }
}
