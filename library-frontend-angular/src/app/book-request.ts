import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API, BookRequest } from './models';

@Injectable({ providedIn: 'root' })
export class BookRequestService {
  private http = inject(HttpClient);

  // Member: request to borrow a book
  request(bookId: number): Observable<BookRequest> {
    return this.http.post<BookRequest>(`${API}/api/bookrequests`, { bookId });
  }

  // Member: the caller's own requests
  getMine(): Observable<BookRequest[]> {
    return this.http.get<BookRequest[]>(`${API}/api/bookrequests/mine`);
  }

  // Admin: all pending requests
  getPending(): Observable<BookRequest[]> {
    return this.http.get<BookRequest[]>(`${API}/api/bookrequests/pending`);
  }

  // Admin: approve (creates the loan) / decline
  approve(id: number): Observable<unknown> {
    return this.http.put(`${API}/api/bookrequests/${id}/approve`, {});
  }

  decline(id: number): Observable<unknown> {
    return this.http.put(`${API}/api/bookrequests/${id}/decline`, {});
  }
}
