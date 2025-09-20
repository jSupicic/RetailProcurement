import { Routes } from '@angular/router';
import { ListComponent as StoreItemsListComponent } from './features/store-items/list/list.component';
import { DetailComponent as StoreItemDetailComponent } from './features/store-items/detail/detail.component';
import { ListComponent as SuppliersListComponent } from './features/suppliers/list/list.component';
import { DetailComponent as SupplierDetailComponent } from './features/suppliers/detail/detail.component';
import { QuarterlyPlanComponent } from './features/statistics/quarterly-plan/quarterly-plan.component';
import { ListComponent as SupplierStoreItems } from "./features/supplier-store-items/list/list.component";
import { LoginComponent } from './features/auth/login/login.component';
import { AuthGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  // Public routes
  { 
    path: 'login', 
    component: LoginComponent,
  },
  
  // Protected routes
  { 
    path: 'store-items', 
    component: StoreItemsListComponent,
    canActivate: [AuthGuard]
  },
  { 
    path: 'store-items/:id', 
    component: StoreItemDetailComponent,
    canActivate: [AuthGuard]
  },
  { 
    path: 'suppliers', 
    component: SuppliersListComponent,
    canActivate: [AuthGuard]
  },
  { 
    path: 'suppliers/:id', 
    component: SupplierDetailComponent,
    canActivate: [AuthGuard]
  },
  { 
    path: 'supplier-store-items', 
    component: SupplierStoreItems,
    canActivate: [AuthGuard]
  },
  { 
    path: 'statistics/quarterly-plan', 
    component: QuarterlyPlanComponent,
    canActivate: [AuthGuard]
  },
  
  // Wildcard route - redirect to login
  { 
    path: '**', 
    redirectTo: 'login' 
  }
];
