import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideClientHydration } from '@angular/platform-browser';
import { provideHttpClient, withInterceptors, withFetch } from '@angular/common/http';
import { apiPrefixInterceptor } from './core/interceptors/api-prefix.interceptor';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { SignalRService } from './core/services/signalr.service';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideClientHydration(),
    provideHttpClient(withFetch(), withInterceptors([apiPrefixInterceptor])),
    provideAnimationsAsync(),
    SignalRService
  ]
};
