import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface StoreItem {
  id: number;
  name: string;
  description?: string;
  stockQuantity?: number;
}

@Injectable({
  providedIn: 'root'
})
export class StoreItemsService {
  private readonly baseUrl = `${environment.apiUrl}/StoreItems`;

  constructor(private readonly http: HttpClient) {}

  getAll(): Observable<StoreItem[]> {
    return this.http.get<StoreItem[]>(this.baseUrl);
  }

  getById(id: number): Observable<StoreItem> {
    return this.http.get<StoreItem>(`${this.baseUrl}/${id}`);
  }

  create(payload: Omit<StoreItem, 'id'>): Observable<StoreItem> {
    return this.http.post<StoreItem>(this.baseUrl, payload);
  }

  update(id: number, payload: Partial<Omit<StoreItem, 'id'>>): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, payload);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
