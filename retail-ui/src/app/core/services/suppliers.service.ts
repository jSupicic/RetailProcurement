import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface Supplier {
  id: number;
  name: string;
  email?: string;
  phone?: string;
}

@Injectable({
  providedIn: 'root'
})
export class SuppliersService {
  private readonly baseUrl = `${environment.apiUrl}/suppliers`;

  constructor(private readonly http: HttpClient) {}

  getAll(): Observable<Supplier[]> {
    return this.http.get<Supplier[]>(this.baseUrl);
  }

  getById(id: number): Observable<Supplier> {
    return this.http.get<Supplier>(`${this.baseUrl}/${id}`);
  }

  create(payload: Omit<Supplier, 'id'>): Observable<Supplier> {
    return this.http.post<Supplier>(this.baseUrl, payload);
  }

  update(id: number, payload: Partial<Omit<Supplier, 'id'>>): Observable<Supplier> {
    return this.http.put<Supplier>(`${this.baseUrl}/${id}`, payload);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
