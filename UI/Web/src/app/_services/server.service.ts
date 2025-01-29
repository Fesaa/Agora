import { Injectable } from '@angular/core';
import {environment} from '../../environments/environment';
import {HttpClient} from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class ServerService {

  baseUrl = environment.apiUrl;

  constructor(private httpClient: HttpClient) { }

  firstStartup() {
    return this.httpClient.get<boolean>(this.baseUrl + "Server/is-first-startup")
  }

}
