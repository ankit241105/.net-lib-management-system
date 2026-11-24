import { Component, inject, signal, OnInit } from '@angular/core';
import { MembershipRequestService } from '../membership-request';

// Shown only to plain "User" accounts: invites them to upgrade to Member for ₹199
// (mock payment). The upgrade is a request an admin must approve.
@Component({
  selector: 'app-upgrade-banner',
  imports: [],
  templateUrl: './upgrade-banner.html'
})
export class UpgradeBanner implements OnInit {
  private membershipService = inject(MembershipRequestService);

  pending = signal(false);
  error = signal('');

  ngOnInit(): void {
    this.membershipService.getMine().subscribe({
      next: (data) => this.pending.set(data.some((r) => r.status === 'Pending')),
      error: () => { /* banner is non-critical */ }
    });
  }

  upgrade(): void {
    this.error.set('');
    this.membershipService.request().subscribe({
      next: () => this.pending.set(true),
      error: (err) => {
        const msg = typeof err.error === 'string' ? err.error : err.error?.text;
        this.error.set(msg || 'Could not submit upgrade request.');
      }
    });
  }
}
