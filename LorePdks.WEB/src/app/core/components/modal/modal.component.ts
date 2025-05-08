import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { DialogModule } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-modal',
  templateUrl: './modal.component.html',
  styleUrls: ['./modal.component.scss'],
  standalone: true,
  imports: [CommonModule, DialogModule, ButtonModule]
})
export class ModalComponent {
  @Input() visible: boolean = false;
  @Input() title: string = '';
  @Input() width: string = '50%';
  @Input() height: string = 'auto';
  @Input() showFooter: boolean = true;
  @Input() showCloseIcon: boolean = true;
  @Input() closeOnEscape: boolean = true;
  @Input() maximizable: boolean = false;
  @Input() saveLabel: string = 'Kaydet';
  @Input() cancelLabel: string = 'İptal';
  @Input() saveBtnDisabled: boolean = false;
  @Input() saveBtnLoading: boolean = false;
  @Input() position: 'center' | 'top' | 'bottom' | 'left' | 'right' | 'topleft' | 'topright' | 'bottomleft' | 'bottomright' = 'center';
  
  @Output() visibleChange = new EventEmitter<boolean>();
  @Output() save = new EventEmitter<void>();
  @Output() cancel = new EventEmitter<void>();
  
  /**
   * Modal görünürlüğünü değiştirir
   * @param value Görünürlük değeri
   */
  onVisibleChange(value: boolean): void {
    this.visible = value;
    this.visibleChange.emit(value);
  }
  
  /**
   * Kaydet butonu tıklandığında tetiklenir
   */
  onSave(): void {
    this.save.emit();
  }
  
  /**
   * İptal butonu tıklandığında tetiklenir
   */
  onCancel(): void {
    this.cancel.emit();
    this.onVisibleChange(false);
  }
}