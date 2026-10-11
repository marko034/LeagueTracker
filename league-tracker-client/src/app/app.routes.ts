import { Routes } from '@angular/router';
import { adminGuard } from './guards/admin-guard';
import { authGuard } from './guards/auth-guard';

export const routes: Routes = [
    { path: '', redirectTo: 'teams', pathMatch: 'full' },
    {
        path: 'teams',
        loadComponent: () => import('./components/team-list/team-list').then(m => m.TeamList)
    },
    {
        path: 'teams/:id',
        loadComponent: () => import('./components/team-detail/team-detail').then(m => m.TeamDetail)
    },
    {
        path: 'players/:id',
        loadComponent: () => import('./components/player-detail/player-detail').then(m => m.PlayerDetail)
    },
    {
        path: 'standings',
        loadComponent: () => import('./components/standings/standings').then(m => m.Standings)
    },
    {
        path: 'player-stats',
        loadComponent: () => import('./components/player-stats/player-stats').then(m => m.PlayerStats)
    },
    {
        path: 'matches/:matchId/record-game',
        canActivate: [authGuard],
        loadComponent: () => import('./components/record-game/record-game').then(m => m.RecordGame)
    },
    {
        path: 'matches',
        loadComponent: () => import('./components/match-list/match-list').then(m => m.MatchList)
    },
    {
        path: 'login',
        loadComponent: () => import('./components/login/login').then(m => m.Login)
    },
    {
        path: 'admin',
        canActivate: [adminGuard],
        loadComponent: () => import('./components/admin-dashboard/admin-dashboard').then(m => m.AdminDashboard)
    },
    {
        path: 'news',
        loadComponent: () => import('./components/news/news').then(m => m.News)
    }
];
