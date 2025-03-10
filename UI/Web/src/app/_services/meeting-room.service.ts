import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {environment} from '../../environments/environment';
import {MeetingRoom} from '../_models/room';

@Injectable({
  providedIn: 'root'
})
export class MeetingRoomService {

  baseUrl = environment.apiUrl + "Room/";

  constructor(private httpClient: HttpClient) { }

  all() {
    return this.httpClient.get<MeetingRoom[]>(this.baseUrl + "all-rooms");
  }

  create(meetingRoom: MeetingRoom) {
    return this.httpClient.post(this.baseUrl + "create", meetingRoom);
  }

  update(meetingRoom: MeetingRoom) {
    return this.httpClient.post(this.baseUrl + "update", meetingRoom);
  }

  delete(id: number) {
    return this.httpClient.delete(this.baseUrl  + id);
  }

  get(id: number) {
    return this.httpClient.get<MeetingRoom>(this.baseUrl + id);
  }
}
