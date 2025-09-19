import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { SuppliersService, Supplier } from '../../../core/services/suppliers.service';
import { StatisticsService, SupplierStats } from 'src/app/core/services/statistics.service';

@Component({
  selector: 'app-supplier-detail',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './detail.component.html',
  styleUrl: './detail.component.scss'
})
export class DetailComponent implements OnInit {
  supplier?: Supplier;
  supplierStats?: SupplierStats;

  constructor(
    private readonly route: ActivatedRoute, 
    private readonly suppliers: SuppliersService,
    private readonly statisticsService: StatisticsService,
  ) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (id) {
      this.suppliers.getById(id).subscribe(s => (this.supplier = s));
      this.statisticsService.getSupplierStats(id).subscribe(s => this.supplierStats = s);
    }
  }
}
