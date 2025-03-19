import {MeetingRoom} from './room';
import {Facility} from './facility';

export type Meeting = {
  id: number,
  creatorId: string,
  title: string,
  description: string,
  externalId: string,
  startTime: Date,
  endTime: Date,
  meetingRoom: MeetingRoom,
  attendees: string[],
  facilities: Facility[],
}
