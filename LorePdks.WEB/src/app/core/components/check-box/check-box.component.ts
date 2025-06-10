import { Component, Input, Output, EventEmitter, forwardRef } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR, FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { CheckboxModule } from 'primeng/checkbox';

@Component({
  selector: 'app-check-box',
  templateUrl: './check-box.component.html',
  styleUrls: ['./check-box.component.scss'],
  standalone: true,
  imports: [CommonModule, CheckboxModule, FormsModule],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => CheckBoxComponent),
      multi: true
    }
  ]
})
export class CheckBoxComponent implements ControlValueAccessor {
  @Input() label: string = '';
  @Input() name: string = '';
  @Input() value: any = true;
  @Input() disabled: boolean = false;
  @Input() readonly: boolean = false;
  @Input() required: boolean = false;
  @Input() tabindex: number = 0;
  @Input() inputId: string = '';
  @Input() ariaLabelledBy: string = '';
  @Input() styleClass: string = '';
  @Input() labelPosition: 'left' | 'right' = 'right';
  @Input() binary: boolean = true;
  
  @Output() onChange = new EventEmitter<any>();

  checked: boolean = false;

  // ControlValueAccessor implementation
  private onChanged: (value: any) => void = () => {};
  private onTouched: () => void = () => {};

  writeValue(value: any): void {
    this.checked = !!value;
  }

  registerOnChange(fn: (value: any) => void): void {
    this.onChanged = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }

  onModelChange(event: any): void {
    this.checked = event.checked || event;
    this.onChanged(this.checked);
    this.onTouched();
    this.onChange.emit(event);
  }
}