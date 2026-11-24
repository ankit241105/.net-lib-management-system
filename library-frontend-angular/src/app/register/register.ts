import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AuthService } from '../auth';

// Public self-registration. Creates a plain "User".
@Component({
  selector: 'app-register',
  imports: [FormsModule, RouterLink],
  templateUrl: './register.html'
})
export class Register {
  private auth = inject(AuthService);

  username = '';
  email = '';
  password = '';
  error = signal('');
  done = signal(false);

  submit(): void {
    this.error.set('');
    this.auth.register(this.username, this.email, this.password).subscribe({
      next: () => this.done.set(true),
      error: (err) => this.error.set(this.extractMsg(err) || 'Registration failed.')
    });
  }

  private extractMsg(err: any): string {
    if (typeof err.error === 'string') return err.error.replace(/^"|"$/g, '');
    return '';
  }
}
