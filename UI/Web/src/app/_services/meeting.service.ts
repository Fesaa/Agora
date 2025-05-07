import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {environment} from '../../environments/environment';
import {Meeting, MeetingSlots} from '../_models/meeting';
import {UserEmail} from '../_models/user-email';
import {MeetingRoom} from '../_models/room';

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

  today(userOnly: boolean = false, roomId: number | null = null) {
    if (roomId) {
      return this.httpClient.get<Meeting[]>(`${this.baseUrl}Meeting/today?userOnly=${userOnly}&roomId=${roomId}`)
    }
    return this.httpClient.get<Meeting[]>(`${this.baseUrl}Meeting/today?userOnly=${userOnly}`)
  }

  upcoming(userOnly: boolean = false, dayOffSet: number = 0) {
    return this.httpClient.get<Meeting[]>(`${this.baseUrl}Meeting/upcoming?userOnly=${userOnly}&dayOffSet=${dayOffSet}`)
  }

  attendees(s: string) {
    return this.httpClient.get<UserEmail[]>(`${this.baseUrl}Meeting/attendees?mustContain=${s}`)
  }

  slots(roomId: number, date: Date) {
    return this.httpClient.get<MeetingSlots[]>(this.baseUrl + `Meeting/slots/${roomId}?unixTime=${date.getTime()/1000}`);
  }

  roomsOn(start: Date, end: Date) {
    return this.httpClient.post<MeetingRoom[]>(this.baseUrl + `Meeting/rooms`, {start, end});
  }
}
