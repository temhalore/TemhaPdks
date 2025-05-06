import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { DropdownModule } from 'primeng/dropdown';
import { MultiSelectModule } from 'primeng/multiselect';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { SelectInputModel } from './select-input.model';

@Component({
  selector: 'app-select',
  templateUrl: './select.component.html',
  styleUrls: ['./select.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    DropdownModule,
    MultiSelectModule
  ]
})
export class SelectComponent implements OnInit {
  @Input() class: string = '';
  @Input() placeholder: string = 'Seçiniz';
  @Input() isDisabled: boolean = false;
  @Input() isRequired: boolean = false;
  @Input() multiple: boolean = false;
  @Input() itemListDto$: BehaviorSubject<SelectInputModel[]> = new BehaviorSubject<SelectInputModel[]>([]);
  @Input() isLoading$: Observable<boolean> = of(false);
  @Input() selectedValue: any;
  
  @Output() selectionChange = new EventEmitter<any>();

  constructor() { }

  ngOnInit(): void {
    // Eğer dışarıdan isLoading$ gönderilmezse varsayılan olarak false değeri kullanılır
    if (!this.isLoading$) {
      this.isLoading$ = of(false);
    }
  }

  onSelectionChange(event: any): void {
    this.selectionChange.emit(event.value);
  }
}