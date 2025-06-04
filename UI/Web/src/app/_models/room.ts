import {Facility} from './facility';

export type MeetingRoom = {
  id: number;
  displayName: string;
  description: string;
  location: string;
  requiresAck: boolean;
  capacity: number;
  mayExceedCapacity: boolean;
  mergeAble: boolean;
  facilities: Facility[];
}
