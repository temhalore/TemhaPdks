import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-log',
  template: `
    <div class="log-layout">
      <router-outlet></router-outlet>
    </div>
  `,
  styleUrls: ['./log.component.scss'],
  standalone: true,
  imports: [CommonModule, RouterOutlet]
})
export class LogComponent {
  constructor() { }
}
