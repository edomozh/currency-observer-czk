import { Component, OnInit } from '@angular/core';

import { RateService } from '../services/rate.service';
import { CurrencyService } from '../services/currency.service';
import { Currency } from '../interfaces/currency';
import { Rate } from '../interfaces/rate';

@Component({
  selector: 'app-rate-checker',
  template: `
    <p>
    <select [(ngModel)]="selectedCurrency">
      <option value="USD" selected>USD</option>
      <option *ngFor="let currency of currencies" [value]="currency">{{ currency.code }}</option>
    </select>
    </p>
  `,
  styles: [
  ]
})
export class RateCheckerComponent implements OnInit {
  currencies: Currency[] = [];
  selectedCurrency: string = 'USD';
  rate: Rate | null = null;

  constructor(
    private rateService: RateService,
    private currencyService: CurrencyService) { }

  ngOnInit(): void {
    this.currencyService.getCurrencies().subscribe(value => this.currencies = value);
  }

  getRate() {
    this.rateService.getRate(this.selectedCurrency, '02282023').subscribe(value => this.rate = value);
  }
}

