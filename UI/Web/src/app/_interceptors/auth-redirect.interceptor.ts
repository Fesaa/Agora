import {HttpEvent, HttpHandler, HttpInterceptor, HttpRequest} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {catchError, Observable} from 'rxjs';
import {Router} from '@angular/router';
import {environment} from '../../environments/environment';

@Injectable()
export class AuthRedirectInterceptor implements HttpInterceptor {

  baseUrl = environment.apiUrl;

  constructor(private router: Router) {
  }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      catchError(err => {
        if (err.status == 401) {
          this.router.navigateByUrl(this.baseUrl = "Account/logout")
        }
        throw err;
      })
    )
  }

}
