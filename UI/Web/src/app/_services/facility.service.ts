import { Injectable } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {environment} from '../../environments/environment';
import {Facility} from '../_models/facility';
import {MeetingRoom} from '../_models/room';

@Injectable({
  providedIn: 'root'
})
export class FacilityService {

  baseUrl = environment.apiUrl + "Facilities/";

  constructor(private httpClient: HttpClient) { }

  get(id: number) {
    return this.httpClient.get<Facility>(`${this.baseUrl}${id}`);
  }

  all() {
    return this.httpClient.get<Facility[]>(this.baseUrl + "all");
  }

  create(facility: Facility) {
    return this.httpClient.post<Facility>(this.baseUrl + "create", facility);
  }

  update(facility: Facility) {
    return this.httpClient.post<Facility>(this.baseUrl + "update", facility);
  }

  delete(id: number) {
    return this.httpClient.delete(this.baseUrl + "/" + id);
  }

  activate(id: number) {
    return this.httpClient.post(this.baseUrl + "activate/" + id, {});
  }

  deactivate(id: number) {
    return this.httpClient.post<MeetingRoom[]>(this.baseUrl + "deactivate/" + id, {});
  }
}
