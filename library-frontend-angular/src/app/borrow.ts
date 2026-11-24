import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API, BorrowRecord } from './models';

@Injectable({ providedIn: 'root' })
export class BorrowService {
  private http = inject(HttpClient);

  // Admin: all loans
  getRecords(): Observable<BorrowRecord[]> {
    return this.http.get<BorrowRecord[]>(`${API}/api/borrowrecords`);
  }

  // Member: the caller's own loans
  getMine(): Observable<BorrowRecord[]> {
    return this.http.get<BorrowRecord[]>(`${API}/api/borrowrecords/mine`);
  }

  // Admin: mark returned
  returnBook(id: number): Observable<unknown> {
    return this.http.put(`${API}/api/borrowrecords/${id}/return`, {});
  }
}
