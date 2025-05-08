import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ConfirmationService } from 'primeng/api';

@Component({
  selector: 'app-confirm-dialog',
  templateUrl: './confirm-dialog.component.html',
  styleUrls: ['./confirm-dialog.component.scss'],
  standalone: true,
  imports: [CommonModule, ConfirmDialogModule],
  providers: [ConfirmationService]
})
export class ConfirmDialogComponent {
  @Input() header: string = 'Onay';
  @Input() message: string = 'Bu işlemi gerçekleştirmek istediğinize emin misiniz?';
  @Input() acceptLabel: string = 'Evet';
  @Input() rejectLabel: string = 'Hayır';
  @Input() icon: string = 'pi pi-exclamation-triangle';
  
  @Output() confirmed = new EventEmitter<void>();
  @Output() rejected = new EventEmitter<void>();
  
  constructor(private confirmationService: ConfirmationService) {}
  
  /**
   * Onay dialogunu gösterir
   */
  show(): void {
    this.confirmationService.confirm({
      header: this.header,
      message: this.message,
      icon: this.icon,
      acceptLabel: this.acceptLabel,
      rejectLabel: this.rejectLabel,
      accept: () => {
        this.confirmed.emit();
      },
      reject: () => {
        this.rejected.emit();
      }
    });
  }
}