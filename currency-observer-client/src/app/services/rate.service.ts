import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { Rate } from '../interfaces/rate';

@Injectable({
  providedIn: 'root'
})
export class RateService {
  private apiUrl = 'https://localhost:7190/rate';

  constructor(private http: HttpClient) { }

  getRate(currencyCode: string, date: string): Observable<Rate> {
    const url = `${this.apiUrl}/?currencyCode=${currencyCode}&ddMMyyyyDate=${date}`;
    return this.http.get<Rate>(url);
  }
}
