import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, signal } from '@angular/core';

type AnnouncementCategory = 'season' | 'tournament' | 'news';
type CategoryFilter = AnnouncementCategory | 'all';

interface Announcement {
  id: number;
  title: string;
  category: AnnouncementCategory;
  categoryLabel: string;
  date: string;
  excerpt: string;
  featured?: boolean;
}

@Component({
  selector: 'app-announcement-board',
  imports: [DatePipe],
  templateUrl: './announcement-board.html',
  styleUrl: './announcement-board.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AnnouncementBoard {
  readonly categories: { id: CategoryFilter; label: string }[] = [
    { id: 'all', label: 'Sve objave' },
    { id: 'season', label: 'Sezone' },
    { id: 'tournament', label: 'Turniri' },
    { id: 'news', label: 'Vesti' },
  ];

  readonly selectedCategory = signal<CategoryFilter>('all');

  readonly announcements = signal<Announcement[]>([
    {
      id: 1,
      title: 'Pripreme za novu sezonu su počele',
      category: 'season',
      categoryLabel: 'Nova sezona',
      date: '2026-10-06T12:00:00',
      excerpt:
        'Uskoro počinje nova sezona lige. Pratite oglasnu tablu za informacije o rasporedu, pravilima i važnim datumima.',
      featured: true,
    },
    {
      id: 2,
      title: 'Otvorene prijave za jesenji turnir',
      category: 'tournament',
      categoryLabel: 'Turnir',
      date: '2026-10-04T12:00:00',
      excerpt:
        'Ekipe mogu da se prijave za jesenji turnir. Objavićemo format takmičenja i raspored nakon završetka prijava.',
    },
    {
      id: 3,
      title: 'Podsetnik: ažurirajte spiskove igrača',
      category: 'news',
      categoryLabel: 'Važna vest',
      date: '2026-10-02T12:00:00',
      excerpt:
        'Molimo predstavnike ekipa da provere spiskove igrača pre početka narednog kola.',
    },
    {
      id: 4,
      title: 'Objavljen okvirni kalendar takmičenja',
      category: 'season',
      categoryLabel: 'Nova sezona',
      date: '2026-09-28T12:00:00',
      excerpt:
        'Pogledajte okvirne datume kola i ostavite dovoljno prostora za eventualne promene rasporeda.',
    },
  ]);

  readonly featuredAnnouncement = computed(() =>
    this.announcements().find(announcement => announcement.featured)
  );

  readonly filteredAnnouncements = computed(() => {
    const category = this.selectedCategory();

    return this.announcements().filter(
      announcement =>
        !announcement.featured &&
        (category === 'all' || announcement.category === category)
    );
  });

  selectCategory(category: CategoryFilter): void {
    this.selectedCategory.set(category);
  }
}