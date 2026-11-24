import { Component, inject, signal, OnInit } from '@angular/core';
import { DatePipe } from '@angular/common';
import { DashboardService } from '../dashboard-service';
import { DashboardStats } from '../models';

interface StatCard {
  label: string;
  value: number;
  color: string;
}

@Component({
  selector: 'app-dashboard',
  imports: [DatePipe],
  templateUrl: './dashboard.html'
})
export class Dashboard implements OnInit {
  private dashboardService = inject(DashboardService);

  stats = signal<DashboardStats | null>(null);
  cards = signal<StatCard[]>([]);
  loading = signal(true);
  error = signal('');

  ngOnInit(): void {
    this.dashboardService.getStats().subscribe({
      next: (data) => {
        this.stats.set(data);
        this.cards.set([
          { label: 'Total Books', value: data.totalBooks, color: 'primary' },
          { label: 'Available', value: data.availableBooks, color: 'success' },
          { label: 'Currently Borrowed', value: data.currentlyBorrowed, color: 'warning' },
          { label: 'Total Members', value: data.totalMembers, color: 'info' }
        ]);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err.status === 403 ? 'Admins only.' : 'Failed to load dashboard.');
        this.loading.set(false);
      }
    });
  }

  isPast(dueDate: string): boolean {
    return new Date(dueDate) < new Date();
  }
}
