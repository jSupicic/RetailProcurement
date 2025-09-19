import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { StatisticsService, SupplierStats } from '../../../core/services/statistics.service';

@Component({
  selector: 'app-supplier-stats',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './supplier-stats.component.html',
  styleUrl: './supplier-stats.component.scss'
})
export class SupplierStatsComponent implements OnInit {
  stats?: SupplierStats;

  constructor(private readonly route: ActivatedRoute, private readonly statsService: StatisticsService) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (id) {
      this.statsService.getSupplierStats(id).subscribe(s => (this.stats = s));
    }
  }
}
