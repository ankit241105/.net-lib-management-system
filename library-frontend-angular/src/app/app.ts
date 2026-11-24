import { Component, inject } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive, Router } from '@angular/router';
import { AuthService } from './auth';
import { UpgradeBanner } from './upgrade-banner/upgrade-banner';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink, RouterLinkActive, UpgradeBanner],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  private auth = inject(AuthService);
  private router = inject(Router);

  isLoggedIn = this.auth.isLoggedIn;
  isAdmin = this.auth.isAdmin;
  isMember = this.auth.isMember;
  isUser = this.auth.isUser;
  role = this.auth.role;
  username = this.auth.username;

  logout(): void {
    this.auth.logout();
    this.router.navigate(['/login']);
  }
}
