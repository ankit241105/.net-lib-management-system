import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../auth';

@Component({
  selector: 'app-create-admin',
  imports: [FormsModule],
  templateUrl: './create-admin.html'
})
export class CreateAdmin {
  private auth = inject(AuthService);

  username = '';
  password = '';
  message = signal('');
  error = signal('');

  submit(): void {
    this.message.set('');
    this.error.set('');
    this.auth.registerAdmin(this.username, this.password).subscribe({
      next: (res) => {
        this.message.set(res || 'Admin created successfully.');
        this.username = '';
        this.password = '';
      },
      error: (err) => {
        if (err.status === 403) {
          this.error.set('Only admins can create admins.');
          return;
        }
        const msg = typeof err.error === 'string' ? err.error : 'Failed to create admin.';
        this.error.set(msg);
      }
    });
  }
}
