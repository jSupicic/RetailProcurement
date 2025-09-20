import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Supplier } from './suppliers.service';

export interface SupplierStats {
  supplierId: number;
  supplierName: string;
  totalItemsSold: number;
  totalEarnings: number;
}

export interface BestOffer {
  storeItemName: string;
  supplierName: string;
  storeItemPrice: number;
}

export interface QuarterlyPlan {
  year: number;
  quarter: number;
  supplierIds: number[];
}

@Injectable({
  providedIn: 'root'
})
export class StatisticsService {
  private readonly baseUrl = `${environment.apiUrl}/statistics`;

  constructor(private readonly http: HttpClient) {}

  getSupplierStats(id: number): Observable<SupplierStats> {
    return this.http.get<SupplierStats>(`${this.baseUrl}/supplier/${id}`);
  }

  getBestOffer(productId: number): Observable<BestOffer> {
    return this.http.get<BestOffer>(`${this.baseUrl}/best-offer/${productId}`);
  }

  planQuarter(request: QuarterlyPlan): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/quarterly-plan`, request);
  }

  getQuarterPlan(): Observable<Supplier[]> {
    return this.http.get<Supplier[]>(`${this.baseUrl}/quarterly-plan`);
  }
}
