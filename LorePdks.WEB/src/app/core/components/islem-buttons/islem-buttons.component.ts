import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnChanges, OnDestroy, Output, SimpleChanges, HostListener, ElementRef } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { TooltipModule } from 'primeng/tooltip';

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
  imports: [CommonModule, ButtonModule, TooltipModule]
})
export class IslemButtonsComponent implements OnChanges, OnDestroy {
  @Input() buttons: IslemButton[] = [];
  @Input() data: any;
  @Input() direction: 'up' | 'down' | 'left' | 'right' = 'up';
  @Input() buttonClass: string = 'p-button-rounded p-button-text p-button-sm';
  
  @Output() buttonClick = new EventEmitter<{action: string, data: any}>();
  
  visibleButtons: IslemButton[] = [];
  isMenuOpen = false;
  
  constructor(private elementRef: ElementRef) {}
  
  ngOnChanges(changes: SimpleChanges): void {
    if (changes['buttons'] || changes['data']) {
      this.updateButtons();
    }
  }
  
  ngOnDestroy(): void {
    // Component yok edildiğinde menu açıksa kapatalım
    this.closeMenu();
  }
  
  /**
   * Document'e tıklandığında tetiklenir
   * Eğer tıklanan eleman menü dışındaysa menüyü kapat
   */
  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    // Tıklanan eleman bu bileşenin içinde değilse menüyü kapat
    if (this.isMenuOpen && !this.elementRef.nativeElement.contains(event.target)) {
      this.closeMenu();
    }
  }
  
  /**
   * Butonları günceller
   */
  updateButtons(): void {
    // Görünür butonları filtrele
    this.visibleButtons = this.buttons.filter(button => this.isButtonVisible(button));
    
    // Eğer hiç buton yoksa veya 1-2 buton varsa menüyü kapat
    if (this.visibleButtons.length <= 2) {
      this.closeMenu();
    }
  }
  
  /**
   * Toggle menüyü açar veya kapatır
   * @param event Olay nesnesi
   */
  toggleMenu(event: MouseEvent): void {
    event.stopPropagation();
    this.isMenuOpen = !this.isMenuOpen;
  }
  
  /**
   * Menüyü kapatır
   */
  closeMenu(): void {
    this.isMenuOpen = false;
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
    
    // Menüyü kapat
    this.closeMenu();
    
    // Tıklama olayını bildir
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
   * Toggle menünün gösterilip gösterilmeyeceğini belirler
   * 3 veya daha fazla buton olduğunda toggle menü gösterilir
   * @returns Toggle menü gösterilmeli mi
   */
  get showToggleMenu(): boolean {
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