import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { StoreItemsService, StoreItem } from '../../../core/services/store-items.service';
import { BestOffer, StatisticsService } from 'src/app/core/services/statistics.service';

@Component({
  selector: 'app-store-item-detail',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './detail.component.html',
  styleUrl: './detail.component.scss'
})
export class DetailComponent implements OnInit {
  item?: StoreItem;
  bestOffer?: BestOffer;

  constructor(
    private readonly route: ActivatedRoute, 
    private readonly storeItems: StoreItemsService,
    private readonly statisticsService: StatisticsService,
  ) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (id) {
      this.storeItems.getById(id).subscribe(item => (this.item = item));
      this.statisticsService.getBestOffer(id).subscribe(offer => this.bestOffer = offer);
    }
  }
}
