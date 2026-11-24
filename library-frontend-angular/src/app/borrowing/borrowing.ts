import { Component, inject, signal, OnInit } from '@angular/core';
import { DatePipe } from '@angular/common';
import { forkJoin } from 'rxjs';
import { BorrowService } from '../borrow';
import { BookRequestService } from '../book-request';
import { AuthService } from '../auth';
import { BorrowRecord, BookRequest } from '../models';

// Role-aware:
//  - Admin  -> all loans, with a Return action.
//  - Member -> their own loans + their own book requests (read-only).
@Component({
  selector: 'app-borrowing',
  imports: [DatePipe],
  templateUrl: './borrowing.html'
})
export class Borrowing implements OnInit {
  private borrowService = inject(BorrowService);
  private bookRequestService = inject(BookRequestService);
  private auth = inject(AuthService);

  isAdmin = this.auth.isAdmin;

  records = signal<BorrowRecord[]>([]);
  myRequests = signal<BookRequest[]>([]);
  loading = signal(true);
  error = signal('');

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.error.set('');

    if (this.isAdmin()) {
      this.borrowService.getRecords().subscribe({
        next: (data) => { this.records.set(data); this.loading.set(false); },
        error: () => { this.error.set('Failed to load loans.'); this.loading.set(false); }
      });
    } else {
      forkJoin({
        loans: this.borrowService.getMine(),
        requests: this.bookRequestService.getMine()
      }).subscribe({
        next: (res) => {
          this.records.set(res.loans);
          this.myRequests.set(res.requests);
          this.loading.set(false);
        },
        error: () => { this.error.set('Failed to load your data.'); this.loading.set(false); }
      });
    }
  }

  returnBook(id: number): void {
    this.error.set('');
    this.borrowService.returnBook(id).subscribe({
      next: () => this.load(),
      error: (err) => this.error.set(this.extractMsg(err) || 'Failed to return book.')
    });
  }

  isOverdue(r: BorrowRecord): boolean {
    return !r.returnDate && new Date(r.dueDate) < new Date();
  }

  badgeClass(status: string): string {
    switch (status) {
      case 'Pending': return 'bg-warning text-dark';
      case 'Approved': return 'bg-success';
      case 'Declined': return 'bg-danger';
      default: return 'bg-secondary';
    }
  }

  private extractMsg(err: any): string {
    if (typeof err.error === 'string') return err.error;
    if (err.error?.text) return err.error.text;
    return '';
  }
}
