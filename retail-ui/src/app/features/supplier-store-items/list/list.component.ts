import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { TableModule } from 'primeng/table';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { TagModule } from 'primeng/tag';
import { InputTextModule } from 'primeng/inputtext';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { InputNumberModule } from 'primeng/inputnumber';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ToolbarModule } from 'primeng/toolbar';
import { SkeletonModule } from 'primeng/skeleton';
import { BadgeModule } from 'primeng/badge';
import { DialogModule } from 'primeng/dialog';
import { MessageModule } from 'primeng/message';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { DropdownModule } from 'primeng/dropdown';
import { SupplierStoreItem, SupplierStoreItemsService } from 'src/app/core/services/supplier-store-items.service';

@Component({
  selector: 'app-supplier-store-items-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    TableModule,
    CardModule,
    ButtonModule,
    TagModule,
    InputTextModule,
    InputTextareaModule,
    InputNumberModule,
    DropdownModule,
    FormsModule,
    ReactiveFormsModule,
    ToolbarModule,
    SkeletonModule,
    BadgeModule,
    DialogModule,
    MessageModule,
    ToastModule
  ],
  providers: [MessageService],
  templateUrl: './list.component.html',
  styleUrl: './list.component.scss'
})
export class ListComponent implements OnInit {
  items: SupplierStoreItem[] = [];
  filteredItems: SupplierStoreItem[] = [];
  loading = true;
  searchTerm = '';
  itemId = 0;

  // Modal properties
  showAddModal = false;
  itemForm: FormGroup;
  submitting = false;

  constructor(
    private readonly supplierStoreItemsService: SupplierStoreItemsService,
    private readonly fb: FormBuilder,
    private readonly messageService: MessageService
  ) {
    this.itemForm = this.fb.group({
      supplierId: [0, [Validators.required, Validators.min(1)]],
      storeItemId: [0, [Validators.required, Validators.min(1)]],
      supplierPrice: [0, [Validators.required, Validators.min(0)]]
    });
  }

  ngOnInit(): void {
    this.loadItems();
  }

  loadItems(): void {
    this.loading = true;
    this.supplierStoreItemsService.getAll().subscribe({
      next: (items) => {
        this.items = items;
        this.filteredItems = items;
        this.loading = false;
      },
      error: () => {
        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: 'Failed to load supplier-store item relationships'
        });
        this.loading = false;
      }
    });
  }

  onSearch(): void {
    if (!this.searchTerm.trim()) {
      this.filteredItems = this.items;
    } else {
      this.filteredItems = this.items.filter(item =>
        item.supplierName.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        item.storeItemName.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        item.supplierId.toString().includes(this.searchTerm) ||
        item.storeItemId.toString().includes(this.searchTerm)
      );
    }
  }

  getSeverity(price: number): string {
    if (price < 50) return 'success';
    if (price < 100) return 'warning';
    return 'danger';
  }

  onDelete(supplierId: number, storeItemId: number): void {
    this.loading = true;
    this.supplierStoreItemsService.delete(supplierId, storeItemId).subscribe({
      next: () => {
        this.items = this.items.filter(item => !(item.supplierId === supplierId && item.storeItemId === storeItemId));
        this.filteredItems = this.filteredItems.filter(item => !(item.supplierId === supplierId && item.storeItemId === storeItemId));
        this.messageService.add({
          severity: 'success',
          summary: 'Success',
          detail: 'Supplier-store item relationship deleted successfully'
        });
        this.loading = false;
      },
      error: () => {
        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: 'Failed to delete supplier-store item relationship'
        });
        this.loading = false;
      }
    });
  }

  // Modal methods
  openAddModal(): void {
    this.itemId = 0;
    this.itemForm.reset({
      supplierId: 0,
      storeItemId: 0,
      supplierPrice: 0
    });
    this.showAddModal = true;
  }

  openEditModal(item: SupplierStoreItem): void {
    this.itemId = 1; // Since we're using composite key, we'll use 1 for edit mode
    this.itemForm.patchValue({
      supplierId: item.supplierId,
      storeItemId: item.storeItemId,
      supplierPrice: item.supplierPrice
    });
    this.showAddModal = true;
  }

  closeAddModal(): void {
    this.showAddModal = false;
    this.itemForm.reset();
    this.submitting = false;
  }

  onSubmit(): void {
    if (this.itemForm.valid && !this.submitting) {
      this.submitting = true;

      const itemData: SupplierStoreItem = {
        supplierId: this.itemForm.value.supplierId,
        storeItemId: this.itemForm.value.storeItemId,
        supplierPrice: this.itemForm.value.supplierPrice,
        supplierName: '', // Will be populated by backend
        storeItemName: '' // Will be populated by backend
      };

      this.supplierStoreItemsService.create(itemData).subscribe({
        next: () => {
          this.messageService.add({
            severity: 'success',
            summary: 'Success',
            detail: 'Supplier-store item relationship created successfully'
          });
          this.loadItems();
          this.closeAddModal();
        },
        error: () => {
          this.messageService.add({
            severity: 'error',
            summary: 'Error',
            detail: 'Failed to create supplier-store item relationship'
          });
          this.submitting = false;
        }
      });
    } else {
      this.markFormGroupTouched();
    }
  }

  private markFormGroupTouched(): void {
    Object.keys(this.itemForm.controls).forEach(key => {
      const control = this.itemForm.get(key);
      control?.markAsTouched();
    });
  }

  getFieldError(fieldName: string): string {
    const field = this.itemForm.get(fieldName);
    if (field?.errors && field.touched) {
      if (field.errors['required']) {
        return `${fieldName.charAt(0).toUpperCase() + fieldName.slice(1)} is required`;
      }
      if (field.errors['min']) {
        return `${fieldName.charAt(0).toUpperCase() + fieldName.slice(1)} must be greater than 0`;
      }
    }
    return '';
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD'
    }).format(value);
  }
}
