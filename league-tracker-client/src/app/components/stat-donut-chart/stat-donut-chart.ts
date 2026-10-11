import { Component, effect, ElementRef, input, ViewChild } from '@angular/core';
import { ArcElement, Chart, DoughnutController, Tooltip } from 'chart.js';

Chart.register(DoughnutController, ArcElement, Tooltip);

@Component({
  selector: 'app-stat-donut-chart',
  imports: [],
  templateUrl: './stat-donut-chart.html',
  styleUrl: './stat-donut-chart.scss',
})
export class StatDonutChart {
  @ViewChild('canvas') canvasRef!: ElementRef<HTMLCanvasElement>;

  value = input.required<number>();
  total = input.required<number>();
  color = input<string>('#1e9e63');

  private chart: Chart | null = null;

  constructor() {
    effect(() => {
      // Prati promenu value/total i ažurira postojeći grafik (ne pravi novi svaki put)
      const v = this.value();
      const t = this.total();
      if (this.chart) {
        this.chart.data.datasets[0].data = [v, Math.max(t - v, 0)];
        this.chart.update();
      }
    });
  }

  ngAfterViewInit() {
    this.chart = new Chart(this.canvasRef.nativeElement, {
      type: 'doughnut',
      data: {
        labels: ['Ostvareno', 'Preostalo'],
        datasets: [{
          data: [this.value(), Math.max(this.total() - this.value(), 0)],
          backgroundColor: [this.color(), '#2c303a'],
          borderWidth: 0
        }]
      },
      options: {
        cutout: '70%',
        plugins: {
          legend: { display: false },
          tooltip: { enabled: true }
        }
      }
    });
  }

  ngOnDestroy() {
    this.chart?.destroy();
  }
}
