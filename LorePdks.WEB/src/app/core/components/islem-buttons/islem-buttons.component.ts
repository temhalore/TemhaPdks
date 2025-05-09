import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { TooltipModule } from 'primeng/tooltip';
import { SpeedDialModule } from 'primeng/speeddial';
import { MenuItem } from 'primeng/api';

export interface IslemButton {
  icon: string;
  tooltip: string;
  action: string;
  class?: string;
  disabled?: boolean | ((data: any) => boolean);
  visible?: boolean | ((data: any) => boolean);
}

@Component({
  selector: 'app-islem-buttons',
  templateUrl: './islem-buttons.component.html',
  styleUrls: ['./islem-buttons.component.scss'],
  standalone: true,
  imports: [CommonModule, ButtonModule, TooltipModule, SpeedDialModule]
})
export class IslemButtonsComponent implements OnChanges {
  @Input() buttons: IslemButton[] = [];
  @Input() data: any;
  @Input() direction: 'up' | 'down' | 'left' | 'right' = 'up';
  @Input() radius: number = 80;
  @Input() type: 'linear' | 'circle' | 'semi-circle' | 'quarter-circle' = 'semi-circle';
  @Input() mask: boolean = true;
  @Input() buttonClass: string = 'p-button-rounded p-button-text p-button-sm';
  @Input() buttonIcon: string = 'pi pi-ellipsis-v';
  
  @Output() buttonClick = new EventEmitter<{action: string, data: any}>();
  
  speedDialItems: MenuItem[] = [];
  visibleButtons: IslemButton[] = [];
  
  ngOnChanges(changes: SimpleChanges): void {
    if (changes['buttons'] || changes['data']) {
      this.updateButtons();
    }
  }
  
  /**
   * Butonları günceller ve SpeedDial için menü öğelerini oluşturur
   */
  updateButtons(): void {
    // Görünür butonları filtrele
    this.visibleButtons = this.buttons.filter(button => this.isButtonVisible(button));
    
    // SpeedDial için menü öğelerini oluştur - Tüm butonları SpeedDial'a ekle (3+ buton varsa)
    if (this.visibleButtons.length > 2) {
      this.speedDialItems = this.visibleButtons.map(button => ({
        icon: button.icon,
        disabled: this.isButtonDisabled(button),
        tooltipOptions: {
          tooltipLabel: button.tooltip,
          tooltipPosition: 'left'
        },
        command: () => {
          this.onButtonClick(button.action, this.data);
        },
        styleClass: button.class || ''
      }));
    } else {
      this.speedDialItems = [];
    }
  }
  
  /**
   * Buton tıklandığında çalışacak metod
   * @param action Aksiyon tipi
   * @param data İlgili veri
   * @param event Olay (event propagation'ı durdurmak için)
   */
  onButtonClick(action: string, data: any, event?: MouseEvent): void {
    if (event) {
      event.stopPropagation();
    }
    
    this.buttonClick.emit({ action, data });
  }
  
  /**
   * Butonun görünür olup olmadığını kontrol eder
   * @param button Buton tanımı
   * @returns Görünür olup olmadığı
   */
  isButtonVisible(button: IslemButton): boolean {
    if (button.visible === undefined) {
      return true;
    }
    
    if (typeof button.visible === 'function') {
      return button.visible(this.data);
    }
    
    return button.visible;
  }
  
  /**
   * Butonun devre dışı olup olmadığını kontrol eder
   * @param button Buton tanımı
   * @returns Devre dışı olup olmadığı
   */
  isButtonDisabled(button: IslemButton): boolean {
    if (button.disabled === undefined) {
      return false;
    }
    
    if (typeof button.disabled === 'function') {
      return button.disabled(this.data);
    }
    
    return button.disabled;
  }
  
  /**
   * SpeedDial butonunun gösterilip gösterilmeyeceğini belirler
   * 3 veya daha fazla buton olduğunda SpeedDial gösterilir
   * @returns SpeedDial butonu gösterilmeli mi
   */
  get showSpeedDial(): boolean {
    return this.visibleButtons.length > 2;
  }
  
  /**
   * Normal butonların gösterilip gösterilmeyeceğini belirler
   * 1 veya 2 buton olduğunda normal butonlar gösterilir
   * @returns Normal butonlar gösterilmeli mi
   */
  get showNormalButtons(): boolean {
    return this.visibleButtons.length > 0 && this.visibleButtons.length <= 2;
  }
  
  /**
   * Hiç buton olmadığında bileşenin gösterilip gösterilmeyeceğini belirler
   * @returns Bileşen gösterilmeli mi
   */
  get hasButtons(): boolean {
    return this.visibleButtons.length > 0;
  }
}