import { Component, inject, signal, OnInit } from '@angular/core';
import { DatePipe } from '@angular/common';
import { forkJoin, Observable } from 'rxjs';
import { BookRequestService } from '../book-request';
import { MembershipRequestService } from '../membership-request';
import { BookRequest, MembershipRequest } from '../models';

// Admin-only: review pending book requests and membership-upgrade requests.
@Component({
  selector: 'app-approvals',
  imports: [DatePipe],
  templateUrl: './approvals.html'
})
export class Approvals implements OnInit {
  private bookRequestService = inject(BookRequestService);
  private membershipService = inject(MembershipRequestService);

  bookReqs = signal<BookRequest[]>([]);
  memberReqs = signal<MembershipRequest[]>([]);
  loading = signal(true);
  error = signal('');

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.error.set('');
    forkJoin({
      books: this.bookRequestService.getPending(),
      members: this.membershipService.getPending()
    }).subscribe({
      next: (res) => {
        this.bookReqs.set(res.books);
        this.memberReqs.set(res.members);
        this.loading.set(false);
      },
      error: () => { this.error.set('Failed to load requests.'); this.loading.set(false); }
    });
  }

  private decide(call: Observable<unknown>): void {
    this.error.set('');
    call.subscribe({
      next: () => this.load(),
      error: (err) => {
        const msg = typeof err.error === 'string' ? err.error : err.error?.text;
        this.error.set(msg || 'Action failed.');
      }
    });
  }

  approveBook(id: number): void { this.decide(this.bookRequestService.approve(id)); }
  declineBook(id: number): void { this.decide(this.bookRequestService.decline(id)); }
  approveMember(id: number): void { this.decide(this.membershipService.approve(id)); }
  declineMember(id: number): void { this.decide(this.membershipService.decline(id)); }
}
