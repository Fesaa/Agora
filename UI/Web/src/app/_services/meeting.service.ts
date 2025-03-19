import { Injectable } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {environment} from '../../environments/environment';
import {Meeting} from '../_models/meeting';

@Injectable({
  providedIn: 'root'
})
export class MeetingService {

  private readonly baseUrl = environment.apiUrl;

  constructor(private httpClient: HttpClient) { }

  get(id: number) {
    return this.httpClient.get<Meeting>(`${this.baseUrl}/Meeting/${id}`)
  }

  create(meeting: Meeting) {
    return this.httpClient.post(`${this.baseUrl}/Meeting`, meeting)
  }

  update(meeting: Meeting) {
    return this.httpClient.post(`${this.baseUrl}/Meeting/update`, meeting)
  }

  delete(id: number) {
    return this.httpClient.delete(`${this.baseUrl}/Meeting/${id}`)
  }

  today() {
    return this.httpClient.get<Meeting[]>(`${this.baseUrl}/Meeting/today`)
  }

  upcoming() {
    return this.httpClient.get(`${this.baseUrl}/Meeting/upcoming`)
  }
}
