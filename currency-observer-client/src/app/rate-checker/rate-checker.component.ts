import { Component, OnInit } from '@angular/core';

import { Currency } from '../currency';
import { Rate } from '../rate';

@Component({
  selector: 'app-rate-checker',
  template: `
    <p>
    <select [(ngModel)]="selectedCurrency">
      <option *ngFor="let currency of currencies" [value]="currency">{{ currency.code }}</option>
    </select>
    </p>
  `,
  styles: [
  ]
})
export class RateCheckerComponent implements OnInit {

  constructor() { }

  currencies: Currency[] = [];
  selectedCurrency: Currency | null = null;
  rate: Rate | null = null;

  ngOnInit(): void {
  }

}
