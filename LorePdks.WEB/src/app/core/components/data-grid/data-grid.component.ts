import { CommonModule } from '@angular/common';
import { Component, ContentChild, EventEmitter, Input, Output, TemplateRef, HostListener, OnInit } from '@angular/core';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { TooltipModule } from 'primeng/tooltip';
import { PaginatorModule } from 'primeng/paginator';
import { DataCardComponent } from '../data-card/data-card.component';
import { IslemButtonsComponent, IslemButton } from '../islem-buttons/islem-buttons.component';

export interface ActionButtonConfig extends IslemButton {}

enum ViewMode {
  LIST = 'list',
  CARD = 'card'
}

@Component({
  selector: 'app-data-grid',
  templateUrl: './data-grid.component.html',
  styleUrls: ['./data-grid.component.scss'],
  standalone: true,
  imports: [CommonModule, TableModule, ButtonModule, TooltipModule, DataCardComponent, IslemButtonsComponent, PaginatorModule]
})
export class DataGridComponent implements OnInit {
  @Input() data: any[] = [];
  @Input() columns: any[] = [];
  @Input() loading: boolean = false;
  @Input() responsive: boolean = true;
  @Input() paginator: boolean = true;
  @Input() rows: number = 10;
  @Input() rowsPerPageOptions: number[] = [5, 10, 20, 50];
  @Input() headerTitle: string = '';
  @Input() tableStyle: any;
  @Input() actionButtons: ActionButtonConfig[] = [];
  
  @ContentChild('customActions') customActionsTemplate!: TemplateRef<any>;

  @Output() rowSelect = new EventEmitter<any>();
  @Output() rowUnselect = new EventEmitter<any>();
  @Output() rowAction = new EventEmitter<{action: string, data: any}>();

  selectedRow: any;
  viewMode: ViewMode = ViewMode.LIST;
  isMobile: boolean = false;
  readonly ViewMode = ViewMode;

  // Sayfalama Değişkenleri
  first: number = 0;
  currentPage: number = 0;
  totalRecords: number = 0;
  
  // Sayfalanmış veri
  pagedData: any[] = [];

  constructor() {
    this.checkScreenSize();
  }

  ngOnInit(): void {
    this.checkScreenSize();
    this.totalRecords = this.data.length;
    this.updatePagedData();
  }

  ngOnChanges(): void {
    this.totalRecords = this.data.length;
    this.updatePagedData();
  }

  /**
   * Sayfalanmış veriyi günceller
   */
  updatePagedData(): void {
    if (this.paginator && this.data.length > 0) {
      const startIndex = this.currentPage * this.rows;
      const endIndex = startIndex + this.rows;
      this.pagedData = this.data.slice(startIndex, endIndex);
    } else {
      this.pagedData = this.data;
    }
  }

  /**
   * Sayfalama değiştiğinde tetiklenir
   * @param event Sayfalama olayı
   */
  onPageChange(event: any): void {
    this.first = event.first;
    this.rows = event.rows;
    this.currentPage = event.page;
    this.updatePagedData();
  }

  @HostListener('window:resize', ['$event'])
  onResize(event: any): void {
    this.checkScreenSize();
  }

  /**
   * Ekran boyutunu kontrol ederek mobil cihaz olup olmadığını belirler
   * Mobil cihazlarda otomatik olarak kart görünümüne geçer
   */
  private checkScreenSize(): void {
    this.isMobile = window.innerWidth < 768;
    if (this.isMobile) {
      this.viewMode = ViewMode.CARD;
    }
  }

  /**
   * Görünüm modunu değiştirir
   * @param mode Görünüm modu (liste veya kart)
   */
  changeViewMode(mode: ViewMode): void {
    this.viewMode = mode;
  }

  /**
   * Satır seçildiğinde tetiklenir
   * @param event Seçilen satır verisi
   */
  onRowSelect(event: any): void {
    this.rowSelect.emit(event.data);
  }

  /**
   * Satır seçimi kaldırıldığında tetiklenir
   * @param event Seçimi kaldırılan satır verisi
   */
  onRowUnselect(event: any): void {
    this.rowUnselect.emit(event.data);
  }

  /**
   * Satır üzerinde bir aksiyon gerçekleştiğinde tetiklenir
   * @param action Aksiyon adı (edit, delete vb.)
   * @param data İlgili satır verisi
   */
  onAction(action: string, data: any): void {
    this.rowAction.emit({ action, data });
  }
}