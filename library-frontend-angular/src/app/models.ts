// Shared TypeScript interfaces (the "shape" of data from the API)

export interface Book {
  id: number;
  title: string;
  author: string;
  isAvailable: boolean;
}

export interface BorrowRecord {
  id: number;
  borrowDate: string;
  dueDate: string;
  returnDate: string | null;
  fine: number;
  bookId: number;
  book?: Book | null;
  userId: number;
  username?: string | null;
}

export interface BookRequest {
  id: number;
  bookId: number;
  bookTitle?: string | null;
  userId: number;
  username?: string | null;
  status: string;
  requestedDate: string;
  decisionDate?: string | null;
}

export interface MembershipRequest {
  id: number;
  userId: number;
  username?: string | null;
  amount: number;
  status: string;
  requestedDate: string;
  decisionDate?: string | null;
}

export interface DashboardStats {
  totalBooks: number;
  totalMembers: number;
  availableBooks: number;
  currentlyBorrowed: number;
  activeBorrowings: BorrowRecord[];
}

// Base URL of the ASP.NET Core API
export const API = 'http://localhost:5016';
