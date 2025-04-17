import { Injectable } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {environment} from '../../environments/environment';
import {StatsRecord} from '../_models/stats';

@Injectable({
  providedIn: 'root'
})
export class StatsService {

  baseUrl = environment.apiUrl;

  constructor(private httpClient: HttpClient) { }

  rooms() {
    return this.httpClient.get<StatsRecord[]>(this.baseUrl + "Stats/rooms")
  }

  facilities() {
    return this.httpClient.get<StatsRecord[]>(this.baseUrl + "Stats/facilities")
  }


}
