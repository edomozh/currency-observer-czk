import { Component, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-date-picker',
  templateUrl: './date-picker.component.html',
  styleUrls: ['./date-picker.component.css']
})
export class DatePickerComponent {
  selectedDate: string = new Date().toISOString().split('T')[0];

  @Output() dateSelected: EventEmitter<string> = new EventEmitter<string>();

  onDateSelected(): void {
    this.dateSelected.emit(this.selectedDate.split('-').reverse().join(''));
  }
}