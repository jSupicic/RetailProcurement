import { Component } from '@angular/core';
import { RouterOutlet, RouterModule } from '@angular/router';
import { MenubarModule } from 'primeng/menubar';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterModule, MenubarModule, CardModule, ButtonModule, CommonModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'retail-portal';
  
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
}
