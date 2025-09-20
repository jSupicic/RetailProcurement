import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { QuarterlyPlan, StatisticsService } from '../../../core/services/statistics.service';
import { SignalRService, QuarterlyPlanNotification } from '../../../core/services/signalr.service';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { TagModule } from 'primeng/tag';
import { InputTextModule } from 'primeng/inputtext';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { InputNumberModule } from 'primeng/inputnumber';
import { ToolbarModule } from 'primeng/toolbar';
import { SkeletonModule } from 'primeng/skeleton';
import { BadgeModule } from 'primeng/badge';
import { DialogModule } from 'primeng/dialog';
import { MessageModule } from 'primeng/message';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { DropdownModule } from 'primeng/dropdown';
import { Supplier } from 'src/app/core/services/suppliers.service';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-quarterly-plan',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    TableModule,
    CardModule,
    ButtonModule,
    TagModule,
    InputTextModule,
    InputTextareaModule,
    InputNumberModule,
    DropdownModule,
    ToolbarModule,
    SkeletonModule,
    BadgeModule,
    DialogModule,
    MessageModule,
    ToastModule
  ],
  providers: [MessageService],
  templateUrl: './quarterly-plan.component.html',
  styleUrl: './quarterly-plan.component.scss'
})
export class QuarterlyPlanComponent implements OnInit, OnDestroy {
  plans: QuarterlyPlan[] = [];
  filteredPlans: QuarterlyPlan[] = [];
  suppliers: Supplier[] = [];
  filteredSuppliers: Supplier[] = [];
  loading = true;
  searchTerm = '';
  planId = 0;

  // Modal properties
  showAddModal = false;
  planForm: FormGroup;
  submitting = false;

  // SignalR properties
  private destroy$ = new Subject<void>();
  connectionStatus = 'Disconnected';

  // Quarter options
  quarterOptions = [
    { label: 'Q1 (Jan - Mar)', value: 1 },
    { label: 'Q2 (Apr - Jun)', value: 2 },
    { label: 'Q3 (Jul - Sep)', value: 3 },
    { label: 'Q4 (Oct - Dec)', value: 4 }
  ];
  
  // Year options
  yearOptions = [
    { label: (new Date().getFullYear() - 2).toString(), value: new Date().getFullYear() - 2 },
    { label: (new Date().getFullYear() - 1).toString(), value: new Date().getFullYear() - 1 },
    { label: new Date().getFullYear().toString(), value: new Date().getFullYear() },
    { label: (new Date().getFullYear() + 1).toString(), value: new Date().getFullYear() + 1 },
    { label: (new Date().getFullYear() + 2).toString(), value: new Date().getFullYear() + 2 }
  ];

  constructor(
    private readonly stats: StatisticsService,
    private readonly signalRService: SignalRService,
    private readonly fb: FormBuilder,
    private readonly messageService: MessageService
  ) {
    this.planForm = this.fb.group({
      year: [new Date().getFullYear(), [Validators.required]],
      quarter: [this.getCurrentQuarter(), [Validators.required]],
      supplierIds: ['', [Validators.required]]
    });
  }

  ngOnInit(): void {
    this.loadPlans();
    this.initializeSignalR();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadPlans(): void {
    this.loading = true;
    this.stats.getQuarterPlan().subscribe({
      next: (suppliers) => {
        this.suppliers = suppliers;
        this.filteredSuppliers = suppliers;
        this.loading = false;
      },
      error: () => {
        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: 'Failed to load quarterly plans'
        });
        this.loading = false;
      }
    });
  }

  onSearch(): void {
    if (!this.searchTerm.trim()) {
      this.filteredSuppliers = this.suppliers;
    } else {
      this.filteredSuppliers = this.suppliers.filter(supplier =>
        supplier.name.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        supplier?.email?.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        supplier?.phone?.toLowerCase().includes(this.searchTerm.toLowerCase())
      );
    }
  }

  onDelete(plan: QuarterlyPlan): void {
    this.loading = true;
    // Since we only have one plan, we'll just reload
    this.loadPlans();
    this.messageService.add({
      severity: 'success',
      summary: 'Success',
      detail: 'Quarterly plan deleted successfully'
    });
  }

  // Modal methods
  openAddModal(): void {
    this.planForm.reset({
      year: new Date().getFullYear(),
      quarter: this.getCurrentQuarter(),
      supplierIds: ''
    });
    this.showAddModal = true;
  }

  closeAddModal(): void {
    this.showAddModal = false;
    this.planForm.reset();
    this.submitting = false;
  }

  onSubmit(): void {
    if (this.planForm.valid && !this.submitting) {
      this.submitting = true;

      const formValue = this.planForm.value;
      const supplierIds: number[] = formValue.supplierIds
        .split(',')
        .map((s: string) => Number(s.trim()))
        .filter((n: number) => !Number.isNaN(n));

      if (supplierIds.length === 0) {
        this.messageService.add({
          severity: 'warn',
          summary: 'Warning',
          detail: 'Please enter valid supplier IDs'
        });
        this.submitting = false;
        return;
      }

      const planData: QuarterlyPlan = {
        year: formValue.year,
        quarter: formValue.quarter,
        supplierIds
      };

      this.stats.planQuarter(planData).subscribe({
        next: () => {
          this.loadPlans();
          this.closeAddModal();
        },
        error: () => {
          this.messageService.add({
            severity: 'error',
            summary: 'Error',
            detail: 'Failed to save quarterly plan'
          });
          this.submitting = false;
        }
      });
    } else {
      this.markFormGroupTouched();
    }
  }

  private markFormGroupTouched(): void {
    Object.keys(this.planForm.controls).forEach(key => {
      const control = this.planForm.get(key);
      control?.markAsTouched();
    });
  }

  getFieldError(fieldName: string): string {
    const field = this.planForm.get(fieldName);
    if (field?.errors && field.touched) {
      if (field.errors['required']) {
        return `${fieldName.charAt(0).toUpperCase() + fieldName.slice(1)} is required`;
      }
    }
    return '';
  }

  getQuarterDisplay(quarter: string): string {
    const quarters: { [key: string]: string } = {
      'Q1': 'Q1 (Jan - Mar)',
      'Q2': 'Q2 (Apr - Jun)', 
      'Q3': 'Q3 (Jul - Sep)',
      'Q4': 'Q4 (Oct - Dec)'
    };
    return quarters[quarter] || quarter;
  }

  getCurrentQuarter(): number {
    const month = new Date().getMonth();
    return Math.floor(month / 3) + 1;
  }

  getQuarterNumber(quarterString: string): number {
    const quarterMap: { [key: string]: number } = {
      'Q1': 1,
      'Q2': 2,
      'Q3': 3,
      'Q4': 4
    };
    return quarterMap[quarterString] || 1;
  }

  getQuarterString(quarterNumber: number): string {
    const quarterMap: { [key: number]: string } = {
      1: 'Q1',
      2: 'Q2',
      3: 'Q3',
      4: 'Q4'
    };
    return quarterMap[quarterNumber] || 'Q1';
  }

  getSeverity(quarter: string): string {
    const currentQuarter = this.getCurrentQuarter();
    const planQuarter = this.getQuarterNumber(quarter);
    if (planQuarter === currentQuarter) return 'success';
    if (planQuarter > currentQuarter) return 'info';
    return 'warning';
  }

  private async initializeSignalR(): Promise<void> {
    try {
      // Start SignalR connection
      await this.signalRService.startConnection();
      
      // Listen to connection state changes
      this.signalRService.getConnectionState()
        .pipe(takeUntil(this.destroy$))
        .subscribe(state => {
          this.connectionStatus = state.toString();
          console.log('SignalR connection state:', state);
        });

      // Listen to quarterly plan notifications
      this.signalRService.getQuarterlyPlanNotifications()
        .pipe(takeUntil(this.destroy$))
        .subscribe(notification => {
          this.handleQuarterlyPlanNotification(notification);
        });

    } catch (error) {
      console.error('Failed to initialize SignalR connection:', error);
      this.messageService.add({
        severity: 'warn',
        summary: 'Connection Warning',
        detail: 'Real-time updates may not be available'
      });
    }
  }

  private handleQuarterlyPlanNotification(notification: QuarterlyPlanNotification): void {
    const { type, plan } = notification;
    console.log(notification);

    switch (type) {
      case 'created':
        // Reload plans to get updated data
        this.loadPlans();
        this.messageService.add({
          severity: 'info',
          summary: 'Quarterly Plan Added',
          detail: `Plan for Q${plan.quarter} ${plan.year} has been added`
        });
        break;

      case 'updated':
        // Reload plans to get updated data
        this.loadPlans();
        this.messageService.add({
          severity: 'info',
          summary: 'Quarterly Plan Updated',
          detail: `Plan for Q${plan.quarter} ${plan.year} has been updated`
        });
        break;

      case 'deleted':
        // Reload plans to get updated data
        this.loadPlans();
        this.messageService.add({
          severity: 'info',
          summary: 'Quarterly Plan Deleted',
          detail: `Quarterly plan has been deleted`
        });
        break;
    }
  }
}
