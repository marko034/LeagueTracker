import { Component, computed, inject, input, numberAttribute } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { SeasonService } from '../../../services/season';

@Component({
  selector: 'app-season-layout',
  imports: [RouterLink, RouterLinkActive, RouterOutlet],
  templateUrl: './season-layout.html',
  styleUrl: './season-layout.scss'
})
export class SeasonLayout {
  seasonService = inject(SeasonService);
  private router = inject(Router);

  // vrednost dolazi iz adrese (:seasonId), numberAttribute je pretvara iz stringa u broj
  seasonId = input.required({ transform: numberAttribute });

  season = computed(() => this.seasonService.seasons().find(s => s.id === this.seasonId()) ?? null);

  // promena sezone zadržava stranicu na kojoj si (timovi, sastavi ili mečevi)
  onSeasonChange(event: Event) {
    const newId = Number((event.target as HTMLSelectElement).value);
    const section = this.router.url.split('?')[0].split('/').pop() ?? 'timovi';
    this.router.navigate(['/admin/sezona', newId, section]);
  }
}