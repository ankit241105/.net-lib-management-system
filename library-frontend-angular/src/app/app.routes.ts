import { Routes } from '@angular/router';
import { authGuard, adminGuard, memberOrAdminGuard } from './auth-guard';

export const routes: Routes = [
  { path: '', redirectTo: 'books', pathMatch: 'full' },
  { path: 'login', loadComponent: () => import('./login/login').then((m) => m.Login) },
  { path: 'register', loadComponent: () => import('./register/register').then((m) => m.Register) },
  {
    path: 'books',
    loadComponent: () => import('./books/books').then((m) => m.Books),
    canActivate: [authGuard]
  },
  {
    path: 'borrowing',
    loadComponent: () => import('./borrowing/borrowing').then((m) => m.Borrowing),
    canActivate: [authGuard, memberOrAdminGuard]
  },
  {
    path: 'approvals',
    loadComponent: () => import('./approvals/approvals').then((m) => m.Approvals),
    canActivate: [authGuard, adminGuard]
  },
  {
    path: 'dashboard',
    loadComponent: () => import('./dashboard/dashboard').then((m) => m.Dashboard),
    canActivate: [authGuard, adminGuard]
  },
  {
    path: 'create-admin',
    loadComponent: () => import('./create-admin/create-admin').then((m) => m.CreateAdmin),
    canActivate: [authGuard, adminGuard]
  },
  { path: '**', redirectTo: 'books' }
];
