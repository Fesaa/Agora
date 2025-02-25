import { Injectable } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {environment} from '../../environments/environment';
import {Facility} from '../_models/facility';

@Injectable({
  providedIn: 'root'
})
export class FacilityService {

  baseUrl = environment.apiUrl + "Facilities/";

  constructor(private httpClient: HttpClient) { }

  all() {
    return this.httpClient.get(this.baseUrl + "all");
  }

  create(facility: Facility) {
    return this.httpClient.post(this.baseUrl + "create", facility);
  }

  update(facility: Facility) {
    return this.httpClient.put(this.baseUrl + "update", facility);
  }

  delete(id: number) {
    return this.httpClient.delete(this.baseUrl + "/" + id);
  }
}
