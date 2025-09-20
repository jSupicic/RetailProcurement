import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CardModule } from 'primeng/card';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { MessageModule } from 'primeng/message';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { CheckboxModule } from 'primeng/checkbox';
import { AuthService, LoginRequest } from '../../../core/services/auth.service';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    CardModule,
    InputTextModule,
    ButtonModule,
    MessageModule,
    ToastModule,
    ProgressSpinnerModule,
    CheckboxModule
  ],
  providers: [MessageService],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent implements OnInit, OnDestroy {
  loginForm: FormGroup;
  loading = false;
  rememberMe = false;
  showPassword = false;
  
  private destroy$ = new Subject<void>();

  constructor(
    private readonly authService: AuthService,
    private readonly fb: FormBuilder,
    private readonly messageService: MessageService,
    private readonly router: Router
  ) {
    this.loginForm = this.fb.group({
      username: ['', [Validators.required, Validators.minLength(3)]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      rememberMe: [false]
    });
  }

  ngOnInit(): void {
    // Check if user is already authenticated
    if (this.authService.isAuthenticated()) {
      this.router.navigate(['/store-items']);
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  onSubmit(): void {
    if (this.loginForm.valid && !this.loading) {
      this.loading = true;
      
      const credentials: LoginRequest = {
        username: this.loginForm.value.username,
        password: this.loginForm.value.password
      };

      this.authService.login(credentials)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: (response) => {
            this.messageService.add({
              severity: 'success',
              summary: 'Welcome!',
              detail: `Hello ${response.username}! You have been logged in successfully.`
            });
            
            // Navigate to the main application
            this.router.navigate(['/store-items']);
            this.loading = false;
          },
          error: (error) => {
            console.error('Login error:', error);
            
            let errorMessage = 'Login failed. Please try again.';
            
            if (error.status === 401) {
              errorMessage = 'Invalid username or password.';
            } else if (error.status === 403) {
              errorMessage = 'Account is disabled. Please contact support.';
            } else if (error.status === 0) {
              errorMessage = 'Unable to connect to server. Please check your connection.';
            }
            
            this.messageService.add({
              severity: 'error',
              summary: 'Login Failed',
              detail: errorMessage
            });
            
            this.loading = false;
          }
        });
    } else {
      this.markFormGroupTouched();
    }
  }

  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }

  private markFormGroupTouched(): void {
    Object.keys(this.loginForm.controls).forEach(key => {
      const control = this.loginForm.get(key);
      control?.markAsTouched();
    });
  }

  getFieldError(fieldName: string): string {
    const field = this.loginForm.get(fieldName);
    if (field?.errors && field.touched) {
      if (field.errors['required']) {
        return `${fieldName.charAt(0).toUpperCase() + fieldName.slice(1)} is required`;
      }
      if (field.errors['minlength']) {
        const minLength = field.errors['minlength'].requiredLength;
        return `${fieldName.charAt(0).toUpperCase() + fieldName.slice(1)} must be at least ${minLength} characters`;
      }
    }
    return '';
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.loginForm.get(fieldName);
    return !!(field?.invalid && field.touched);
  }
}
