import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API, Book } from './models';

@Injectable({ providedIn: 'root' })
export class BookService {
  private http = inject(HttpClient);

  getBooks(search?: string, available?: string, sortBy?: string): Observable<Book[]> {
    let params = new HttpParams();
    if (search) params = params.set('search', search);
    if (available && available !== 'all') params = params.set('available', available);
    if (sortBy) params = params.set('sortBy', sortBy);
    return this.http.get<Book[]>(`${API}/api/books`, { params });
  }

  createBook(book: Partial<Book>): Observable<Book> {
    return this.http.post<Book>(`${API}/api/books`, book);
  }

  updateBook(id: number, book: Book): Observable<void> {
    return this.http.put<void>(`${API}/api/books/${id}`, book);
  }

  deleteBook(id: number): Observable<void> {
    return this.http.delete<void>(`${API}/api/books/${id}`);
  }
}
