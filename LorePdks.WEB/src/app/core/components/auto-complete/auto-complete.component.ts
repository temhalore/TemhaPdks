import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { Observable, of } from 'rxjs';

@Component({
  selector: 'app-auto-complete',
  templateUrl: './auto-complete.component.html',
  styleUrls: ['./auto-complete.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    AutoCompleteModule
  ]
})
export class AutoCompleteComponent implements OnInit {
  @Input() class: string = '';
  @Input() title: string = '';
  @Input() placeholder: string = '';
  @Input() isRequired: boolean = false;
  @Input() isDisabled: boolean = false;
  @Input() isLoading$: Observable<boolean> = of(false);
  @Input() suggestions: any[] = [];
  @Input() field: string = 'name'; // Varsayılan olarak gösterilecek alan
  @Input() showEmptyMessage: boolean = true;
  @Input() emptyMessage: string = 'Sonuç bulunamadı';
  
  // Filtrelenmiş öneriler
  filteredSuggestions: any[] = [];
  
  // İki yönlü veri bağlaması için set/get kullanımı
  private _selectedItem: any = null;
  
  @Input()
  get selectedItem(): any {
    return this._selectedItem;
  }
  
  set selectedItem(val: any) {
    this._selectedItem = val;
    this.selectedItemChange.emit(this._selectedItem);
  }
  
  @Output() selectedItemChange = new EventEmitter<any>();
  @Output() search = new EventEmitter<string>();

  constructor() { }

  ngOnInit(): void {
    // Eğer dışarıdan isLoading$ gönderilmezse varsayılan olarak false değeri kullanılır
    if (!this.isLoading$) {
      this.isLoading$ = of(false);
    }
  }

  filterSuggestions(event: any): void {
    const query = event.query;
    
    // Dışarıdan arama işlemi sağlanacak ise
    this.search.emit(query);
    
    // Lokalde filtreleme
    if (this.suggestions && this.suggestions.length > 0) {
      this.filteredSuggestions = this.suggestions.filter(item => 
        item[this.field]?.toString().toLowerCase().includes(query.toLowerCase())
      );
    } else {
      this.filteredSuggestions = [];
    }
  }

  onSelect(event: any): void {
    this.selectedItem = event;
  }

  onClear(): void {
    this.selectedItem = null;
  }
}
