import { HttpInterceptorFn } from '@angular/common/http';

// No-op currently. Kept for future global headers (e.g., auth) or API base prefixing.
export const apiPrefixInterceptor: HttpInterceptorFn = (req, next) => {
  return next(req);
};
