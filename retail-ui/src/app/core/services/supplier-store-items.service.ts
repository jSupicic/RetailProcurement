import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface SupplierStoreItem {
  supplierId: number;
  storeItemId: number;
  supplierPrice: number;
  supplierName: string;
  storeItemName: string;
}

@Injectable({
  providedIn: 'root'
})
export class SupplierStoreItemsService {
  private readonly baseUrl = `${environment.apiUrl}/SupplierStoreItems`;

  constructor(private readonly http: HttpClient) {}

  getAll(): Observable<SupplierStoreItem[]> {
    return this.http.get<SupplierStoreItem[]>(this.baseUrl);
  }

  create(link: SupplierStoreItem): Observable<void> {
    return this.http.post<void>(this.baseUrl, link);
  }

  delete(supplierId: number, storeId: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${supplierId}/${storeId}`);
  }
}
