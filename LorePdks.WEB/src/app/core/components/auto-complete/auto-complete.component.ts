import { Component, EventEmitter, Input, OnInit, Output, ContentChild, TemplateRef, OnChanges, SimpleChanges } from '@angular/core';
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
export class AutoCompleteComponent implements OnInit, OnChanges {
  @Input() class: string = '';
  @Input() title: string = '';
  @Input() placeholder: string = '';
  @Input() isRequired: boolean = false;
  @Input() isDisabled: boolean = false;
  @Input() isLoading$: Observable<boolean> = of(false);
  @Input() suggestions: any[] = [];
  @Input() field: string = 'name';
  @Input() showEmptyMessage: boolean = true;
  @Input() emptyMessage: string = 'Sonuç bulunamadı';
  
  @ContentChild('itemTemplate') contentTemplateRef: TemplateRef<any> | null = null;
  
  // Bu filteredSuggestions son arama sonuçlarını tutacak
  filteredSuggestions: any[] = [];
  
  private _selectedItem: any = null;
  
  @Input()
  get selectedItem(): any {
    return this._selectedItem;
  }
  
  set selectedItem(val: any) {
    console.log('AutoComplete - selectedItem set edildi:', val);
    this._selectedItem = val;
    this.selectedItemChange.emit(this._selectedItem);
  }
  
  @Output() selectedItemChange = new EventEmitter<any>();
  @Output() search = new EventEmitter<string>();

  constructor() { }

  ngOnInit(): void {
    if (!this.isLoading$) {
      this.isLoading$ = of(false);
    }
    console.log('AutoComplete - ngOnInit - field:', this.field);
  }
  
  ngOnChanges(changes: SimpleChanges): void {
    // suggestions değiştiğinde filteredSuggestions'ı güncelle
    if (changes['suggestions'] && changes['suggestions'].currentValue) {
      console.log('AutoComplete - Suggestions güncellendi:', changes['suggestions'].currentValue);
      this.filteredSuggestions = [...changes['suggestions'].currentValue];
    }
  }
  
  /**
   * Öğe için görüntüleme metni oluşturur
   */
  getDisplayText(item: any): string {
    if (!item) return '';
    
    // Kişiler için özel durum (ad ve soyad birleştirme)
    if (item && item.ad && item.soyad) {
      return `${item.ad} ${item.soyad}`;
    }
    
    // Belirtilen alanı kullan
    if (item && item[this.field] !== undefined) {
      return item[this.field];
    }
    
    // Eğer item string ise
    if (typeof item === 'string') {
      return item;
    }
    
    // Nesneyi metine çevirme
    if (item && typeof item === 'object') {
      try {
        return JSON.stringify(item);
      } catch (e) {
        return '[Nesne gösterilemiyor]';
      }
    }
    
    return '';
  }

  filterSuggestions(event: any): void {
    const query = event.query;
    console.log('AutoComplete - Arama yapılıyor:', query);
    
    // Dışarıdan arama isteğini emit et
    this.search.emit(query);
  }

  onSelect(event: any): void {
    console.log('AutoComplete - Seçilen:', event);
    this.selectedItem = event;
  }

  onClear(): void {
    console.log('AutoComplete - Seçim temizlendi');
    this.selectedItem = null;
  }
}

