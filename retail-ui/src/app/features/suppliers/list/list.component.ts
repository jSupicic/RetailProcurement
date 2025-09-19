import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { SuppliersService, Supplier } from '../../../core/services/suppliers.service';
import { TableModule } from 'primeng/table';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { TagModule } from 'primeng/tag';
import { InputTextModule } from 'primeng/inputtext';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ToolbarModule } from 'primeng/toolbar';
import { SkeletonModule } from 'primeng/skeleton';
import { BadgeModule } from 'primeng/badge';
import { DialogModule } from 'primeng/dialog';
import { MessageModule } from 'primeng/message';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-suppliers-list',
  standalone: true,
  imports: [
    CommonModule, 
    RouterModule, 
    TableModule, 
    CardModule, 
    ButtonModule, 
    TagModule, 
    InputTextModule, 
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
  suppliers: Supplier[] = [];
  filteredSuppliers: Supplier[] = [];
  loading = true;
  searchTerm = '';
  supplierId = 0;
  
  // Modal properties
  showAddModal = false;
  supplierForm: FormGroup;
  submitting = false;

  constructor(
    private readonly suppliersService: SuppliersService,
    private readonly fb: FormBuilder,
    private readonly messageService: MessageService
  ) {
    this.supplierForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email]],
      phone: ['', [Validators.required, Validators.pattern(/^[\+]?[1-9][\d]{0,15}$/)]]
    });
  }

  ngOnInit(): void {
    this.loadSuppliers();
  }

  loadSuppliers(): void {
    this.suppliersService.getAll().subscribe({
      next: (suppliers) => {
        this.suppliers = suppliers;
        this.filteredSuppliers = suppliers;
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

  getStatusSeverity(isActive: boolean): string {
    return isActive ? 'success' : 'danger';
  }

  getStatusLabel(isActive: boolean): string {
    return isActive ? 'Active' : 'Inactive';
  }

  getRatingSeverity(rating: number): string {
    if (rating >= 4.5) return 'success';
    if (rating >= 3.5) return 'warning';
    return 'danger';
  }

  onDelete(id: number): void {
    this.loading = true;
    this.suppliersService.delete(id).subscribe({
      next: () => {
        this.suppliers = this.suppliers.filter(supplier => supplier.id !== id);
        this.messageService.add({
          severity: 'success',
          summary: 'Success',
          detail: 'Supplier deleted successfully'
        });
      },
      error: () => {
        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: 'Failed to delete supplier'
        });
        this.loading = false;
      },
      complete: () => {
        this.loadSuppliers();
      }
    });
  }

  // Modal methods
  openAddModal(): void {
    this.supplierId = 0;
    this.supplierForm.reset();
    this.showAddModal = true;
  }

  openEditModal(supplier: Supplier): void {
    this.supplierId = supplier.id;
    this.supplierForm.patchValue(supplier);
    this.showAddModal = true;
  }

  closeAddModal(): void {
    this.showAddModal = false;
    this.supplierForm.reset();
    this.submitting = false;
  }

  onSubmit(): void {
    if (this.supplierForm.valid && !this.submitting) {
      this.submitting = true;
      
      const newSupplier = {
        name: this.supplierForm.value.name,
        email: this.supplierForm.value.email,
        phone: this.supplierForm.value.phone
      };

      if (this.supplierId === 0) {
        this.suppliersService.create(newSupplier).subscribe({
          next: (createdSupplier) => {
            this.suppliers.push(createdSupplier);
            this.filteredSuppliers = [...this.suppliers];
            this.messageService.add({
              severity: 'success',
              summary: 'Success',
              detail: 'Supplier added successfully'
            });
            this.closeAddModal();
          },
          error: (error) => {
            this.messageService.add({
              severity: 'error',
              summary: 'Error',
              detail: 'Failed to add supplier'
            });
            this.submitting = false;
          }
        });
      } else {
        this.suppliersService.update(this.supplierId, newSupplier).subscribe({
          next: (updatedSupplier) => {
            this.suppliers = this.suppliers.map(supplier => supplier.id === this.supplierId ? updatedSupplier : supplier);
            this.filteredSuppliers = [...this.suppliers];
            this.messageService.add({
              severity: 'success',
              summary: 'Success',
              detail: 'Supplier edited successfully'
            });
            this.closeAddModal();
          },
          error: (error) => {
            this.messageService.add({
              severity: 'error',
              summary: 'Error',
              detail: 'Failed to edit supplier'
            });
            this.submitting = false;
          }
        });
      }
    } else {
      this.markFormGroupTouched();
    }
  }

  private markFormGroupTouched(): void {
    Object.keys(this.supplierForm.controls).forEach(key => {
      const control = this.supplierForm.get(key);
      control?.markAsTouched();
    });
  }

  getFieldError(fieldName: string): string {
    const field = this.supplierForm.get(fieldName);
    if (field?.errors && field.touched) {
      if (field.errors['required']) {
        return `${fieldName.charAt(0).toUpperCase() + fieldName.slice(1)} is required`;
      }
      if (field.errors['email']) {
        return 'Please enter a valid email address';
      }
      if (field.errors['minlength']) {
        return `${fieldName.charAt(0).toUpperCase() + fieldName.slice(1)} must be at least 2 characters`;
      }
      if (field.errors['pattern']) {
        return 'Please enter a valid phone number';
      }
    }
    return '';
  }
}
