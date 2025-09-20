import {
  HttpEvent,
  HttpHandlerFn,
  HttpInterceptorFn,
  HttpRequest
} from '@angular/common/http';
import { Observable } from 'rxjs';

export const authInterceptor = (): HttpInterceptorFn =>
  (
    req: HttpRequest<unknown>,
    next: HttpHandlerFn
  ): Observable<HttpEvent<unknown>> => {

    const token = localStorage.getItem('auth_token');

    if (token) {
      return next(
        req.clone({
          setHeaders: {
            Authorization: `Bearer ${token}`
          },
        })
      )
    }

    return next(req);
  }
