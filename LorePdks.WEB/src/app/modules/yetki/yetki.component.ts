import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-yetki',
  templateUrl: './yetki.component.html',
  styleUrls: ['./yetki.component.scss'],
  standalone: true,
  imports: [RouterOutlet, CommonModule]
})
export class YetkiComponent {
  constructor() { }
}