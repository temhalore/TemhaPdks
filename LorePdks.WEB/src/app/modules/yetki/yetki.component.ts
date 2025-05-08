import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-yetki',
  templateUrl: './yetki.component.html',
  styleUrls: ['./yetki.component.scss'],
  standalone: true,
  imports: [RouterOutlet]
})
export class YetkiComponent {
  constructor() { }
}