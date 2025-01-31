import { Injectable } from '@angular/core';
import {OAuthService} from 'angular-oauth2-oidc';
import {HttpClient} from '@angular/common/http';
import {environment} from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  baseUrl = environment.apiUrl;

  constructor(private oauthService: OAuthService, private httpClient: HttpClient) {
    this.configureAuth()
  }

  private configureAuth() {
    this.httpClient.get(this.baseUrl + "Settings/open-id-connect-info", {responseType: "text"}).subscribe({
      next: issuer => {
        this.oauthService.configure({
          issuer: issuer,
          clientId: "agora",
          redirectUri: window.location.origin + '/callback',
          responseType: 'code',
          scope: 'openid profile email offline_access',
          showDebugInformation: true,
          sessionChecksEnabled: true,
        });
        this.oauthService.loadDiscoveryDocumentAndTryLogin()
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
