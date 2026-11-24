import { Component, inject, signal, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BookService } from '../book';
import { BookRequestService } from '../book-request';
import { AuthService } from '../auth';
import { Book } from '../models';

@Component({
  selector: 'app-books',
  imports: [FormsModule],
  templateUrl: './books.html'
})
export class Books implements OnInit {
  private bookService = inject(BookService);
  private bookRequestService = inject(BookRequestService);
  private auth = inject(AuthService);

  isAdmin = this.auth.isAdmin;
  isMember = this.auth.isMember;

  books = signal<Book[]>([]);
  loading = signal(true);
  error = signal('');
  info = signal('');

  // search + filter
  search = '';
  available = 'all';

  // add form
  newTitle = '';
  newAuthor = '';

  // inline edit
  editingId = signal<number | null>(null);
  editTitle = '';
  editAuthor = '';

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.error.set('');
    this.bookService.getBooks(this.search, this.available).subscribe({
      next: (data) => {
        this.books.set(data);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load books.');
        this.loading.set(false);
      }
    });
  }

  addBook(): void {
    this.bookService.createBook({ title: this.newTitle, author: this.newAuthor }).subscribe({
      next: () => {
        this.newTitle = '';
        this.newAuthor = '';
        this.load();
      },
      error: (err) =>
        this.error.set(err.status === 403 ? 'Only admins can add books.' : 'Failed to add book.')
    });
  }

  startEdit(book: Book): void {
    this.editingId.set(book.id);
    this.editTitle = book.title;
    this.editAuthor = book.author;
    this.error.set('');
  }

  cancelEdit(): void {
    this.editingId.set(null);
  }

  saveEdit(book: Book): void {
    const updated: Book = {
      id: book.id,
      title: this.editTitle,
      author: this.editAuthor,
      isAvailable: book.isAvailable // keep current availability
    };
    this.bookService.updateBook(book.id, updated).subscribe({
      next: () => {
        this.editingId.set(null);
        this.load();
      },
      error: (err) =>
        this.error.set(err.status === 403 ? 'Only admins can edit books.' : 'Failed to update book.')
    });
  }

  deleteBook(id: number): void {
    if (!confirm('Delete this book?')) return;
    this.bookService.deleteBook(id).subscribe({
      next: () => this.load(),
      error: (err) =>
        this.error.set(err.status === 403 ? 'Only admins can delete books.' : 'Failed to delete book.')
    });
  }

  // Member: request to borrow a book -> goes to an admin for approval
  requestBook(book: Book): void {
    this.error.set('');
    this.info.set('');
    this.bookRequestService.request(book.id).subscribe({
      next: () => this.info.set(`Requested "${book.title}". An admin will review it.`),
      error: (err) => {
        const msg = typeof err.error === 'string' ? err.error : err.error?.text;
        this.error.set(msg || 'Failed to request book.');
      }
    });
  }
}
