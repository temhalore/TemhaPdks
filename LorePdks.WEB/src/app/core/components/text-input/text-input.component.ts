import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';
import { Observable, of } from 'rxjs';

@Component({
  selector: 'app-text-input',
  templateUrl: './text-input.component.html',
  styleUrls: ['./text-input.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    InputTextModule
  ]
})
export class TextInputComponent implements OnInit {
  @Input() class: string = '';
  @Input() title: string = '';
  @Input() type: string = 'text';
  @Input() placeholder: string = '';
  @Input() isRequired: boolean = false;
  @Input() isDisabled: boolean = false;
  @Input() isLoading$: Observable<boolean> = of(false);
  @Input() maxLength?: number;
  
  // İki yönlü veri bağlaması için set/get kullanımı
  private _value: string = '';
  
  @Input()
  get value(): string {
    return this._value;
  }
  
  set value(val: string) {
    this._value = val;
    this.valueChange.emit(this._value);
  }
  
  @Output() valueChange = new EventEmitter<string>();

  constructor() { }

  ngOnInit(): void {
    // Eğer dışarıdan isLoading$ gönderilmezse varsayılan olarak false değeri kullanılır
    if (!this.isLoading$) {
      this.isLoading$ = of(false);
    }
  }

  onChange(event: any): void {
    this.value = event.target.value;
  }
}