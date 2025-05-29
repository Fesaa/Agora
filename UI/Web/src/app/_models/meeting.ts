import {MeetingRoom} from './room';
import {Facility} from './facility';

export type Meeting = {
  id: number,
  creatorId: string,
  title: string,
  description: string,
  externalId: string,
  acknowledged: boolean,
  startTime: Date,
  endTime: Date,
  room: MeetingRoom,
  attendees: string[],
  facilities: Facility[],
}

export type MeetingSlots = {
  start: Date,
  end: Date,
}
