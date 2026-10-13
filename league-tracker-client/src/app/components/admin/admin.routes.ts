import { Routes } from '@angular/router';

export const ADMIN_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./admin-layout/admin-layout').then(m => m.AdminLayout),
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'pregled' },
      { path: 'pregled', loadComponent: () => import('./admin-overview/admin-overview').then(m => m.AdminOverview) },
      { path: 'timovi', loadComponent: () => import('./admin-teams/admin-teams').then(m => m.AdminTeams) },
      { path: 'igraci', loadComponent: () => import('./admin-players/admin-players').then(m => m.AdminPlayers) },
      {
        path: 'sezona',
        children: [
          {
            path: '',
            pathMatch: 'full',
            loadComponent: () => import('./season-picker/season-picker').then(m => m.SeasonPicker)
          },
          {
            path: ':seasonId',
            loadComponent: () => import('./season-layout/season-layout').then(m => m.SeasonLayout),
            children: [
              { path: '', pathMatch: 'full', redirectTo: 'timovi' },
              { path: 'timovi', loadComponent: () => import('./season-teams/season-teams').then(m => m.SeasonTeams) },
              { path: 'sastavi', loadComponent: () => import('./season-rosters/season-rosters').then(m => m.SeasonRosters) },
              { path: 'mecevi', loadComponent: () => import('./season-matches/season-matches').then(m => m.SeasonMatches) }
            ]
          }
        ]
      },
      { path: 'lige', loadComponent: () => import('./admin-leagues/admin-leagues').then(m => m.AdminLeagues) },
      { path: 'najave', loadComponent: () => import('./admin-announcements/admin-announcements').then(m => m.AdminAnnouncements) }
    ]
  }
];