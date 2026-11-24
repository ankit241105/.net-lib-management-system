import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API, MembershipRequest } from './models';

@Injectable({ providedIn: 'root' })
export class MembershipRequestService {
  private http = inject(HttpClient);

  // User: request to upgrade to Member (mock ₹199)
  request(): Observable<MembershipRequest> {
    return this.http.post<MembershipRequest>(`${API}/api/membershiprequests`, {});
  }

  // User: the caller's own upgrade requests
  getMine(): Observable<MembershipRequest[]> {
    return this.http.get<MembershipRequest[]>(`${API}/api/membershiprequests/mine`);
  }

  // Admin: all pending upgrade requests
  getPending(): Observable<MembershipRequest[]> {
    return this.http.get<MembershipRequest[]>(`${API}/api/membershiprequests/pending`);
  }

  // Admin: approve (flips role to Member) / decline
  approve(id: number): Observable<unknown> {
    return this.http.put(`${API}/api/membershiprequests/${id}/approve`, {});
  }

  decline(id: number): Observable<unknown> {
    return this.http.put(`${API}/api/membershiprequests/${id}/decline`, {});
  }
}
