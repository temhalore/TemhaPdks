import { CommonModule } from '@angular/common';
import { Component, ContentChild, EventEmitter, Input, Output, TemplateRef } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { TooltipModule } from 'primeng/tooltip';
import { IslemButtonsComponent, IslemButton } from '../islem-buttons/islem-buttons.component';

export interface ActionButtonConfig extends IslemButton {}

@Component({
  selector: 'app-data-card',
  templateUrl: './data-card.component.html',
  styleUrls: ['./data-card.component.scss'],
  standalone: true,
  imports: [CommonModule, CardModule, ButtonModule, TooltipModule, IslemButtonsComponent]
})
export class DataCardComponent {
  @Input() data: any[] = [];
  @Input() columns: any[] = [];
  @Input() loading: boolean = false;
  @Input() headerTitle: string = '';
  @Input() actionButtons: ActionButtonConfig[] = [];
  
  @ContentChild('customActions') customActionsTemplate!: TemplateRef<any>;

  @Output() rowSelect = new EventEmitter<any>();
  @Output() rowAction = new EventEmitter<{action: string, data: any}>();

  selectedRow: any;

  /**
   * Kart seçildiğinde tetiklenir
   * @param data Seçilen kart verisi
   */
  onCardSelect(data: any): void {
    this.selectedRow = data;
    this.rowSelect.emit(data);
  }

  /**
   * Kart üzerinde bir aksiyon gerçekleştiğinde tetiklenir
   * @param action Aksiyon adı (edit, delete vb.)
   * @param data İlgili kart verisi
   */
  onAction(action: string, data: any): void {
    this.rowAction.emit({ action, data });
  }

  /**
   * Belirli bir alanı formatlayarak görüntüler
   * @param field Alan adı
   * @param row Veri satırı
   * @returns Formatlanmış değer
   */
  formatField(field: string, row: any): any {
    const column = this.columns.find(col => col.field === field);
    if (column && column.format) {
      return column.format(row[field], row);
    }
    return row[field];
  }
}