import { Injectable } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {environment} from '../../environments/environment';
import {Meeting, MeetingSlots} from '../_models/meeting';
import {UserEmail} from '../_models/user-email';

@Injectable({
  providedIn: 'root'
})
export class MeetingService {

  private readonly baseUrl = environment.apiUrl;

  constructor(private httpClient: HttpClient) { }

  get(id: number) {
    return this.httpClient.get<Meeting>(`${this.baseUrl}Meeting/${id}`)
  }

  create(meeting: Meeting) {
    return this.httpClient.post(`${this.baseUrl}Meeting`, meeting)
  }

  update(meeting: Meeting) {
    return this.httpClient.post(`${this.baseUrl}Meeting/update`, meeting)
  }

  delete(id: number) {
    return this.httpClient.delete(`${this.baseUrl}Meeting/${id}`)
  }

  today() {
    return this.httpClient.get<Meeting[]>(`${this.baseUrl}Meeting/today`)
  }

  upcoming() {
    return this.httpClient.get(`${this.baseUrl}Meeting/upcoming`)
  }

  attendees(s: string) {
    return this.httpClient.get<UserEmail[]>(`${this.baseUrl}Meeting/attendees?mustContain=${s}`)
  }

  slots(roomId: number, date: Date) {
    return this.httpClient.get<MeetingSlots[]>(this.baseUrl + `Meeting/slots/${roomId}?unixTime=${date.getTime()/1000}`);
  }
}
