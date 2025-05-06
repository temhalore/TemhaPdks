import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-button',
  templateUrl: './button.component.html',
  styleUrls: ['./button.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    ButtonModule
  ]
})
export class ButtonComponent implements OnInit {
  @Input() class: string = 'p-button-primary';
  @Input() name: string = '';
  @Input() isDisabled: boolean = false;
  @Input() icon: string = '';
  @Input() set isLoading(value: boolean) {
    this._isLoading$.next(value);
  }
  @Input() isLoading$: Observable<boolean> = of(false);
  
  @Output() onClick = new EventEmitter<any>();

  public _isLoading$ = new BehaviorSubject<boolean>(false);
  
  constructor() { }

  ngOnInit(): void {
    // Eğer dışarıdan isLoading$ gönderilmezse varsayılan olarak false değeri kullanılır
    if (!this.isLoading$) {
      this.isLoading$ = of(false);
    }
  }

  clickHandler(event: any): void {
    if (!this.isDisabled && !(this._isLoading$.value)) {
      this.onClick.emit(event);
    }
  }
}