import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { SeasonService } from '../../../services/season';

@Component({
  selector: 'app-season-picker',
  imports: [RouterLink],
  templateUrl: './season-picker.html',
  styleUrl: './season-picker.scss'
})
export class SeasonPicker {
  seasonService = inject(SeasonService);
}