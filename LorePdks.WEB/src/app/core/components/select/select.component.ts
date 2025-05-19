// filepath: d:\Development\ozel\TemhaPdks\LorePdks.WEB\src\app\core\components\select\select.component.ts
import { Component, EventEmitter, Input, OnInit, Output, OnDestroy, AfterViewInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { DropdownModule } from 'primeng/dropdown';
import { MultiSelectModule } from 'primeng/multiselect';
import { BehaviorSubject, Observable, Subscription, of } from 'rxjs';
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
export class SelectComponent implements OnInit, OnDestroy, AfterViewInit {
  @Input() class: string = '';
  @Input() title: string = ''; // Eklenen başlık alanı
  @Input() placeholder: string = 'Seçiniz';
  @Input() isDisabled: boolean = false;
  @Input() isRequired: boolean = false;
  @Input() multiple: boolean = false;
  @Input() itemListDto$: BehaviorSubject<SelectInputModel[]> = new BehaviorSubject<SelectInputModel[]>([]);
  @Input() isLoading$: Observable<boolean> = of(false);
  @Input() selectedValue: any;
  
  @Output() selectionChange = new EventEmitter<any>();
  
  // BehaviorSubject'ten gelen verileri tutacak dizi
  itemList: SelectInputModel[] = [];
  private subscription: Subscription = new Subscription();

  constructor(private cdr: ChangeDetectorRef) { }
  
  ngOnInit(): void {
    // Eğer dışarıdan isLoading$ gönderilmezse varsayılan olarak false değeri kullanılır
    if (!this.isLoading$) {
      this.isLoading$ = of(false);
    }
    
    // BehaviorSubject'ten gelen verileri izle ve itemList dizisine aktar
    this.subscription.add(
      this.itemListDto$.subscribe(items => {
        console.log('SelectComponent - itemListDto$ değerleri:', items);
        console.log('SelectComponent - seçili değer:', this.selectedValue);
        this.itemList = items || [];
        this.cdr.detectChanges(); // Değişiklikleri zorla uygula
      })
    );
  }
  
  ngAfterViewInit(): void {
    // View oluşturulduktan sonra değişiklikleri zorla uygula
    this.cdr.detectChanges();
  }
  
  ngOnDestroy(): void {
    // Subscription'ları temizle
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }
  
  onSelectionChange(event: any): void {
    console.log('Select component - Seçim değişti:', event);
    // event.value değerini emit et
    this.selectionChange.emit(event.value);
  }
}
