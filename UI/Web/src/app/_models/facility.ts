
export type Facility = {
  id: number;
  displayName: string;
  description: string;
  alertManagement: boolean;
  cost: number;
  availability: Availability[];
};

export type Availability = {
  id: number;
  dayOfWeek: DayOfWeek;
  timeRange: string;
}

export enum DayOfWeek {
  Sunday = 0,
  Monday = 1,
  Tuesday = 2,
  Wednesday = 3,
  Thursday = 4,
  Friday = 5,
  Saturday = 6,
}
