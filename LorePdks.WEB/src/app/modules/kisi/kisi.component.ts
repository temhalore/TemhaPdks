import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-kisi',
  templateUrl: './kisi.component.html',
  styleUrls: ['./kisi.component.scss'],
  standalone: true,
  imports: [RouterOutlet, CommonModule]
})
export class KisiComponent {
  constructor() { }
}
