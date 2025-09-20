import { Component, OnInit, OnDestroy } from '@angular/core';
import { RouterOutlet, RouterModule } from '@angular/router';
import { MenubarModule } from 'primeng/menubar';
import { CardModule } from 'primeng/card';
import { CommonModule } from '@angular/common';
import { AuthService } from './core/services/auth.service';
import { Subject, takeUntil } from 'rxjs';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterModule, MenubarModule, CardModule, ButtonModule, CommonModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit, OnDestroy {
  title = 'Retail Procurement';
  isAuthenticated = false;
  
  menuItems = [
    {
      label: 'Store Items',
      icon: 'pi pi-shopping-cart',
      routerLink: '/store-items'
    },
    {
      label: 'Suppliers',
      icon: 'pi pi-building',
      routerLink: '/suppliers'
    },
    {
      label: 'Relations',
      icon: 'pi pi-link',
      routerLink: '/supplier-store-items'
    },
    {
      label: 'Statistics',
      icon: 'pi pi-chart-bar',
      routerLink: '/statistics/quarterly-plan'
    }
  ];
  private destroy$ = new Subject<void>();

  constructor(private readonly authService: AuthService) {}

  ngOnInit(): void {
    // Subscribe to authentication state changes
    this.authService.isAuthenticated$
      .pipe(takeUntil(this.destroy$))
      .subscribe(isAuthenticated => {
        this.isAuthenticated = isAuthenticated;
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
