import { Component, OnInit } from '@angular/core';

import { RateService } from '../services/rate.service';
import { CurrencyService } from '../services/currency.service';
import { Currency } from '../interfaces/currency';
import { Rate } from '../interfaces/rate';

@Component({
  selector: 'app-rate-checker',
  templateUrl: './rate-checker.component.html',
  styles: []
})
export class RateCheckerComponent implements OnInit {

  currencies: Currency[] = [];
  selectedCurrency: string = 'USD';
  selectedDate: string = '01022023';
  rate: Rate | null = null;

  constructor(
    private rateService: RateService,
    private currencyService: CurrencyService) { }

  ngOnInit(): void {
    this.currencyService.getCurrencies().subscribe(value => this.currencies = value);
  }

  getRate() {
    this.rateService.getRate(this.selectedCurrency, this.selectedDate).subscribe(value => this.rate = value);
  }

  onDateSelected(date: string): void {
    this.selectedDate = date;
    this.getRate();
  }
}

