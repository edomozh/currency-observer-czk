import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { Currency } from '../interfaces/currency';

@Injectable({
  providedIn: 'root'
})
export class CurrencyService {
  private apiUrl = 'https://localhost:7190/currency';

  constructor(private http: HttpClient) { }

  getCurrencies(): Observable<Currency[]> {
    const url = `${this.apiUrl}`;
    return this.http.get<Currency[]>(url);
  }

  getAvailableCurrencies(date: string): Observable<Currency[]> {
    const url = `${this.apiUrl}/GetAvailable/?ddMMyyyyDate=${date}`;
    return this.http.get<Currency[]>(url);
  }
}
