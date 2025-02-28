import { Injectable } from '@angular/core';
import {OAuthService} from 'angular-oauth2-oidc';
import {environment} from '../../environments/environment';
import {OpenIdConnectService} from './open-id-connect.service';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor(private oauthService: OAuthService, private openIdConnectService: OpenIdConnectService) {
    this.configureAuth()
  }

  private configureAuth() {
    this.openIdConnectService.info().subscribe({
      next: info => {
        this.oauthService.configure({
          issuer: info.authority,
          clientId: "agora",
          redirectUri: window.location.origin + '/callback',
          responseType: 'code',
          scope: 'openid profile email offline_access',
          showDebugInformation: true,
          sessionChecksEnabled: true,
          useSilentRefresh: true,
        });
        this.oauthService.loadDiscoveryDocumentAndTryLogin().then(() => {
          this.oauthService.setupAutomaticSilentRefresh()
        })
      },
      error: error => {
        console.log(error);
      }
    })
  }

  login() {
    this.oauthService.initLoginFlow();
  }

  logout() {
    this.oauthService.logOut()
  }

  get accessToken() {
    return this.oauthService.getAccessToken();
  }

  get isAuthenticated(): boolean {
    return this.oauthService.hasValidAccessToken();
  }
}
