import {Injectable} from '@angular/core';
import {environment} from '../../environments/environment';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';

export enum Role {
  Admin = 'Admin',
  CreateForOther = 'CreateForOther',
}

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  baseUrl = environment.apiUrl;

  constructor(private httpClient: HttpClient) { }

  name() {
    return this.httpClient.get(`${this.baseUrl}Account/name`, {responseType: "text"});
  }

  admin() {
    return this.httpClient.get<boolean>(`${this.baseUrl}Account/admin`);
  }

  roles() {
    return this.httpClient.get<Role[]>(`${this.baseUrl}Account/roles`);
  }
}
