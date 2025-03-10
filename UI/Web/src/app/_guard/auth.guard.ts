import {Injectable} from '@angular/core';
import {ActivatedRouteSnapshot, CanActivate, RouterStateSnapshot} from '@angular/router';
import {AuthService} from '../_services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  public static readonly urlKey = "agora--auth-interceptor--url"

  constructor(private authService: AuthService) {}

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    if (this.authService.isAuthenticated) {
      return true;
    }

    if (window.location.pathname !== "/callback") {
      localStorage.setItem(AuthGuard.urlKey, window.location.pathname);
    }
    this.authService.login();
    return false;
  }
}
