import { Injectable } from '@angular/core';
import {environment} from '../../environments/environment';
import {HttpClient} from '@angular/common/http';
import {OpenIdConnectInfo} from '../_models/openid';

@Injectable({
  providedIn: 'root'
})
export class OpenIdConnectService {

  baseUrl = environment.apiUrl + "OpenIdConnect/";

  constructor(private httpClient: HttpClient) { }

  info() {
    return this.httpClient.get<OpenIdConnectInfo>(this.baseUrl + 'info');
  }

  isSetup() {
    return this.httpClient.get<boolean>(this.baseUrl + 'is-setup')
  }

  /**
   * Set up the OpenId Connect settings, does not need authentication. One time use
   * @param info
   */
  firstSetup(info: OpenIdConnectInfo) {
    return this.httpClient.post<OpenIdConnectInfo>(this.baseUrl + "first-setup", info);
  }

  /**
   * Update the OpenId Connect settings, needs authentication
   * @param info
   */
  update(info: OpenIdConnectInfo) {
    return this.httpClient.post<OpenIdConnectInfo>(this.baseUrl + "update", info);
  }



}
