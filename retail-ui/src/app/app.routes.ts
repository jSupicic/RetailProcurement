import { Routes } from '@angular/router';
import { ListComponent as StoreItemsListComponent } from './features/store-items/list/list.component';
import { DetailComponent as StoreItemDetailComponent } from './features/store-items/detail/detail.component';
import { ListComponent as SuppliersListComponent } from './features/suppliers/list/list.component';
import { DetailComponent as SupplierDetailComponent } from './features/suppliers/detail/detail.component';
import { SupplierStatsComponent } from './features/statistics/supplier-stats/supplier-stats.component';
import { QuarterlyPlanComponent } from './features/statistics/quarterly-plan/quarterly-plan.component';
import { ListComponent as SupplierStoreItems } from "./features/supplier-store-items/list/list.component";

export const routes: Routes = [
  { path: '', redirectTo: 'store-items', pathMatch: 'full' },
  { path: 'store-items', component: StoreItemsListComponent },
  { path: 'store-items/:id', component: StoreItemDetailComponent },
  { path: 'suppliers', component: SuppliersListComponent },
  { path: 'suppliers/:id', component: SupplierDetailComponent },
  { path: 'supplier-store-items', component: SupplierStoreItems },
  { path: 'statistics/supplier/:id', component: SupplierStatsComponent },
  { path: 'statistics/quarterly-plan', component: QuarterlyPlanComponent },
];
