import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-bilgi-penceresi',
  templateUrl: './bilgi-penceresi.component.html',
  styleUrls: ['./bilgi-penceresi.component.scss'],
  standalone: true,
  imports: [CommonModule]
})
export class BilgiPenceresiComponent {
  @Input() htmlIcerik: string = '';
  @Input() baslik: string = '';
}