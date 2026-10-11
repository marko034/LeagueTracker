import { Component, inject, signal } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from './services/auth-service';
import { SeasonService } from './services/season';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  authService = inject(AuthService);
  seasonService = inject(SeasonService);

  menuOpen = signal(false);

  constructor(){
    this.seasonService.loadAll();
  }

  onSeasonChange(event: Event) {
    const id = Number((event.target as HTMLSelectElement).value);
    this.seasonService.selectSeason(id);
  }

  toggleMenu() {
    this.menuOpen.update(open => !open);
  }

  closeMenu() {
    this.menuOpen.set(false);
  }

  onLogout() {
    this.authService.logout();
    this.closeMenu();
  }
}
