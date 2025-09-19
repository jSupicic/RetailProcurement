import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { StoreItemsService, StoreItem } from '../../../core/services/store-items.service';
import { TableModule } from 'primeng/table';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { TagModule } from 'primeng/tag';
import { InputTextModule } from 'primeng/inputtext';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ToolbarModule } from 'primeng/toolbar';
import { SkeletonModule } from 'primeng/skeleton';
import { DialogModule } from 'primeng/dialog';
import { MessageModule } from 'primeng/message';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { InputNumberModule } from 'primeng/inputnumber';
import { DropdownModule } from 'primeng/dropdown';

@Component({
  selector: 'app-store-items-list',
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
    DialogModule,
    MessageModule,
    ToastModule
  ],
  providers: [MessageService],
  templateUrl: './list.component.html',
  styleUrl: './list.component.scss'
})
export class ListComponent implements OnInit {
  items: StoreItem[] = [];
  filteredItems: StoreItem[] = [];
  loading = true;
  searchTerm = '';
  itemId = 0;
  
  // Modal properties
  showAddModal = false;
  itemForm: FormGroup;
  submitting = false;

  constructor(
    private readonly storeItems: StoreItemsService,
    private readonly fb: FormBuilder,
    private readonly messageService: MessageService
  ) {
    this.itemForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(2)]],
      description: [''],
      stockQuantity: [0, [Validators.required, Validators.min(0)]]
    });
  }

  ngOnInit(): void {
    this.loadItems();
  }

  loadItems(): void {
    this.loading = true;
    this.storeItems.getAll().subscribe({
      next: (items) => {
        this.items = items;
        this.filteredItems = items;
        this.loading = false;
      },
      error: () => {
        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: 'Failed to load store items'
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
        item.name.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        item.description?.toLowerCase().includes(this.searchTerm.toLowerCase())
      );
    }
  }

  getSeverity(price: number): string {
    if (price < 50) return 'success';
    if (price < 100) return 'warning';
    return 'danger';
  }

  onDelete(id: number): void {
    this.loading = true;
    this.storeItems.delete(id).subscribe({
      next: () => {
        this.items = this.items.filter(item => item.id !== id);
        this.filteredItems = this.filteredItems.filter(item => item.id !== id);
        this.messageService.add({
          severity: 'success',
          summary: 'Success',
          detail: 'Store item deleted successfully'
        });
        this.loading = false;
      },
      error: () => {
        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: 'Failed to delete store item'
        });
        this.loading = false;
      }
    });
  }

  // Modal methods
  openAddModal(): void {
    this.itemId = 0;
    this.itemForm.reset();
    this.showAddModal = true;
  }

  openEditModal(item: StoreItem): void {
    this.itemId = item.id;
    this.itemForm.patchValue(item);
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
      
      const itemData = {
        name: this.itemForm.value.name,
        description: this.itemForm.value.description || '',
        stockQuantity: this.itemForm.value.stockQuantity
      };

      if (this.itemId === 0) {
        this.storeItems.create(itemData).subscribe({
          next: (createdItem) => {
            this.items.push(createdItem);
            this.filteredItems = [...this.items];
            this.messageService.add({
              severity: 'success',
              summary: 'Success',
              detail: 'Store item added successfully'
            });
            this.closeAddModal();
          },
          error: () => {
            this.messageService.add({
              severity: 'error',
              summary: 'Error',
              detail: 'Failed to add store item'
            });
            this.submitting = false;
          }
        });
      } else {
        this.storeItems.update(this.itemId, itemData).subscribe({
          next: () => {
            // Update the item in the local array
            const updatedItem = { ...this.items.find(item => item.id === this.itemId)!, ...itemData };
            this.items = this.items.map(item => item.id === this.itemId ? updatedItem : item);
            this.filteredItems = [...this.items];
            this.messageService.add({
              severity: 'success',
              summary: 'Success',
              detail: 'Store item updated successfully'
            });
            this.closeAddModal();
          },
          error: () => {
            this.messageService.add({
              severity: 'error',
              summary: 'Error',
              detail: 'Failed to update store item'
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
      if (field.errors['minlength']) {
        return `${fieldName.charAt(0).toUpperCase() + fieldName.slice(1)} must be at least 2 characters`;
      }
      if (field.errors['min']) {
        return `${fieldName.charAt(0).toUpperCase() + fieldName.slice(1)} must be greater than 0`;
      }
    }
    return '';
  }
}
