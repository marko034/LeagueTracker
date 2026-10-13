import { AfterViewInit, Component, ElementRef, OnDestroy, forwardRef, input, viewChild } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import flatpickr from 'flatpickr';
import type { Instance } from 'flatpickr/dist/types/instance';
import type { CustomLocale } from 'flatpickr/dist/types/locale';

const SERBIAN: CustomLocale = {
  firstDayOfWeek: 1,
  weekdays: {
    shorthand: ['Ned', 'Pon', 'Uto', 'Sre', 'Čet', 'Pet', 'Sub'],
    longhand: ['Nedelja', 'Ponedeljak', 'Utorak', 'Sreda', 'Četvrtak', 'Petak', 'Subota']
  },
  months: {
    shorthand: ['Jan', 'Feb', 'Mar', 'Apr', 'Maj', 'Jun', 'Jul', 'Avg', 'Sep', 'Okt', 'Nov', 'Dec'],
    longhand: ['Januar', 'Februar', 'Mart', 'April', 'Maj', 'Jun', 'Jul', 'Avgust', 'Septembar', 'Oktobar', 'Novembar', 'Decembar']
  }
};

@Component({
  selector: 'app-date-time-picker',
  template: `<input #input type="text" [placeholder]="placeholder()" />`,
  styles: [`:host { display: block; }`],
  providers: [
    { provide: NG_VALUE_ACCESSOR, useExisting: forwardRef(() => DateTimePicker), multi: true }
  ]
})
export class DateTimePicker implements ControlValueAccessor, AfterViewInit, OnDestroy {
  private inputRef = viewChild.required<ElementRef<HTMLInputElement>>('input');

  placeholder = input('Izaberi datum i vreme');
  withTime = input(true); // čita se jednom, pri kreiranju

  private picker: Instance | null = null;
  private value: string | null = null;
  private disabled = false;
  private onChange: (value: string) => void = () => {};
  private onTouched: () => void = () => {};

  ngAfterViewInit() {
    const withTime = this.withTime();

    this.picker = flatpickr(this.inputRef().nativeElement, {
      locale: SERBIAN,
      enableTime: withTime,
      time_24hr: true,
      minuteIncrement: 5,
      dateFormat: withTime ? 'Y-m-d\\TH:i' : 'Y-m-d',   // vrednost koju dobija forma
      altInput: true,
      altFormat: withTime ? 'd.m.Y. H:i' : 'd.m.Y.',      // ono što korisnik vidi
      disableMobile: true,
      ...(this.value ? { defaultDate: this.value } : {}),
      onChange: (_dates, dateString) => this.onChange(dateString),
      onClose: () => this.onTouched()
    });

    this.applyDisabled();
  }

  ngOnDestroy() {
    this.picker?.destroy();
  }

  writeValue(value: string | null): void {
    this.value = value;

    if (!this.picker) {
      return;
    }

    if (value) {
      this.picker.setDate(value, false);
    } else {
      this.picker.clear(false);
    }
  }

  registerOnChange(fn: (value: string) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
    this.applyDisabled();
  }

  private applyDisabled() {
    if (this.picker?.altInput) {
      this.picker.altInput.disabled = this.disabled;
    }
  }
}