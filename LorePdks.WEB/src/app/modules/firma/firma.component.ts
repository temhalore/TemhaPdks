import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-firma',
  templateUrl: './firma.component.html',
  styleUrls: ['./firma.component.scss'],
  standalone: true,
  imports: [RouterOutlet, CommonModule]
})
export class FirmaComponent {
  constructor() { }
}
