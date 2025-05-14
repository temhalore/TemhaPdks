import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-kod',
  templateUrl: './kod.component.html',
  standalone: true,
  imports: [RouterOutlet,CommonModule,]
})
export class KodComponent { }
