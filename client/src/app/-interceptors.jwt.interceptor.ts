import { HttpInterceptorFn } from '@angular/common/http';

export const interceptorsJwtInterceptor: HttpInterceptorFn = (req, next) => {
  return next(req);
};
