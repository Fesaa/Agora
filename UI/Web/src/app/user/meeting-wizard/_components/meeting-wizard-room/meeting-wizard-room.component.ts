import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {Meeting, MeetingSlots} from '../../../../_models/meeting';
import {Card} from 'primeng/card';
import {TranslocoDirective, TranslocoService} from '@jsverse/transloco';
import {DatePicker} from 'primeng/datepicker';
import {MeetingService} from '../../../../_services/meeting.service';
import {ToastService} from '../../../../_services/toast-service';
import {Select} from 'primeng/select';
import {FormsModule} from '@angular/forms';
import {InputText} from 'primeng/inputtext';
import {NgClass, NgForOf, NgIf} from '@angular/common';
import {MeetingRoom} from '../../../../_models/room';
import {AgoraButtonComponent} from '../../../../shared/components/agora-button/agora-button.component';
import {UtcToLocalTimePipe} from '../../../../_pipes/utc-to-local.pipe';
import {DateTime} from 'luxon';

enum MeetingDuration {
  QUARTER,
  HALF_HOUR,
  HOUR,
  HOUR_AND_HALF,
  TWO_HOURS,
  CUSTOM
}

const customLengthRegex = /(?:(\d+)h)?(?:(\d+)m)?/;

@Component({
  selector: 'app-meeting-wizard-room',
  imports: [
    Card,
    AgoraButtonComponent,
    TranslocoDirective,
    DatePicker,
    Select,
    FormsModule,
    InputText,
    NgClass,
    NgIf,
    NgForOf,
    UtcToLocalTimePipe
  ],
  templateUrl: './meeting-wizard-room.component.html',
  styleUrl: './meeting-wizard-room.component.css'
})
export class MeetingWizardRoomComponent implements OnInit{

  @Input({required: true}) meeting!: Meeting;
  @Output() prev: EventEmitter<void> = new EventEmitter();
  @Output() next: EventEmitter<void> = new EventEmitter();

  today = new Date();
  slots: MeetingSlots[] = []
  startTimes: {label: string, value: Date}[] = []
  rooms: {label: string, value: MeetingRoom}[] = [];

  selectedMeetingDuration: MeetingDuration = MeetingDuration.HOUR;
  customLength: string = '';
  meetingDurations: {label: string, value: MeetingDuration}[] = [];

  dateSelected: boolean = false;
  slotPickerMinimized: boolean = false;

  constructor(
    private meetingService: MeetingService,
    private toastR: ToastService,
    private transLoco: TranslocoService,
  ) {
  }

  ngOnInit(): void {
    this.setDurations();

    this.transLoco.events$.subscribe((event) => {
      if (event.type !== "translationLoadSuccess") {
        return;
      }

      this.setDurations();
    });

    if (this.hasMeetingData()) {
      this.meeting.startTime = typeof this.meeting.startTime === 'string'
        ? DateTime.fromISO(this.meeting.startTime, { zone: 'utc' }).toLocal().toJSDate()
        : DateTime.fromJSDate(this.meeting.startTime).toLocal().toJSDate();
      this.meeting.endTime = typeof this.meeting.endTime === 'string'
        ? DateTime.fromISO(this.meeting.endTime, { zone: 'utc' }).toLocal().toJSDate()
        : DateTime.fromJSDate(this.meeting.endTime).toLocal().toJSDate();
      this.initializeWithExistingMeeting();
    }
  }

  private hasMeetingData(): boolean {
    return this.meeting &&
           this.meeting.startTime &&
           this.meeting.endTime &&
           this.meeting.room &&
           this.meeting.room.id !== 0;
  }

  private initializeWithExistingMeeting(): void {
    const durationMinutes = this.calculateDurationInMinutes();
    this.setDurationFromMinutes(durationMinutes);

    const meetingDate = this.meeting.startTime;
    this.dateSelected = true;
    this.slots = [{
      start: this.toStart(meetingDate),
      end: this.toEnd(meetingDate),
    }];

    this.generateStartTimes();
    this.fetchRoomsForTimeSlot(this.meeting.startTime, this.meeting.endTime);
  }

  private calculateDurationInMinutes(): number {
    const durationMs = this.meeting.endTime.getTime() - this.meeting.startTime.getTime();
    return Math.floor(durationMs / 60000);
  }

  private setDurationFromMinutes(durationMinutes: number): void {
    if (durationMinutes === 15) {
      this.selectedMeetingDuration = MeetingDuration.QUARTER;
    } else if (durationMinutes === 30) {
      this.selectedMeetingDuration = MeetingDuration.HALF_HOUR;
    } else if (durationMinutes === 60) {
      this.selectedMeetingDuration = MeetingDuration.HOUR;
    } else if (durationMinutes === 90) {
      this.selectedMeetingDuration = MeetingDuration.HOUR_AND_HALF;
    } else if (durationMinutes === 120) {
      this.selectedMeetingDuration = MeetingDuration.TWO_HOURS;
    } else {
      this.selectedMeetingDuration = MeetingDuration.CUSTOM;
      this.customLength = this.formatCustomDuration(durationMinutes);
    }
  }

  private fetchRoomsForTimeSlot(startTime: Date, endTime: Date): void {
    this.meetingService.roomsOn(startTime, endTime).subscribe({
      next: rooms => {
        this.rooms = this.mapRoomsToSelectItems(rooms);
        // Minimize the slot picker since we already have a selection
        this.slotPickerMinimized = true;
      },
      error: err => {
        console.error(err);
        this.toastR.warningLoco("user.wizard.meeting.room.error-fetching-rooms");
      }
    });
  }

  private mapRoomsToSelectItems(rooms: MeetingRoom[]): {label: string, value: MeetingRoom}[] {
    return rooms.map(room => ({
      label: room.displayName,
      value: room,
    }));
  }

  private formatCustomDuration(minutes: number): string {
    const hours = Math.floor(minutes / 60);
    const remainingMinutes = minutes % 60;

    if (hours > 0 && remainingMinutes > 0) {
      return `${hours}h${remainingMinutes}m`;
    } else if (hours > 0) {
      return `${hours}h`;
    } else {
      return `${remainingMinutes}m`;
    }
  }

  private setDurations(): void {
    if (Object.keys(this.transLoco.getTranslation('en')).length === 0) {
      return;
    }

    this.meetingDurations = [
      {label: this.transLoco.translate("user.wizard.meeting.room.durations.quarter"), value: MeetingDuration.QUARTER},
      {label: this.transLoco.translate("user.wizard.meeting.room.durations.half-hour"), value: MeetingDuration.HALF_HOUR},
      {label: this.transLoco.translate("user.wizard.meeting.room.durations.hour"), value: MeetingDuration.HOUR},
      {label: this.transLoco.translate("user.wizard.meeting.room.durations.hour-and-half"), value: MeetingDuration.HOUR_AND_HALF},
      {label: this.transLoco.translate("user.wizard.meeting.room.durations.two-hours"), value: MeetingDuration.TWO_HOURS},
      {label: this.transLoco.translate("user.wizard.meeting.room.durations.custom"), value: MeetingDuration.CUSTOM},
    ];
  }

  /**
   * Validates if the custom time format is valid (e.g., "1h30m")
   * @returns True if the custom time format is valid, false otherwise
   */
  isCustomTimeValid(): boolean {
    const t = this.customLength.toLowerCase().replaceAll(/[^0-9hm]/g, '');
    if (t.length === 0) {
      return false;
    }

    const match = customLengthRegex.exec(t);
    if (!match) {
      return false;
    }
    return match[0].length > 0;
  }

  handleTimePick(date: Date) {
    this.setMeetingTimes(date);
    this.fetchRoomsForTimeSlot(this.meeting.startTime, this.meeting.endTime);
  }

  private setMeetingTimes(startTime: Date): void {
    const durationMinutes = this.getDurationMinutes();
    const endTime = startTime.getTime() + durationMinutes * 60000;

    this.meeting.startTime = startTime;
    this.meeting.endTime = new Date(endTime);
  }

  handleRoomPick(room: MeetingRoom): void {
    this.meeting.room = room;
  }

  handleDatePick(date: Date) {
    // TODO: TimeZones are annoying. This isn't working in the backend
    // this.fetchSlotsForDate(date);

    this.createDefaultSlotsForDate(date);
    this.generateStartTimes();
    this.slotPickerMinimized = false;
  }

  /**
   * Fetches available slots for a given date
   */
  private fetchSlotsForDate(date: Date): void {
    if (!this.meeting.room || this.meeting.room.id === 0) {
      return;
    }

    this.meetingService.slots(this.meeting.room.id, date).subscribe({
      next: (slots) => {
        this.slots = slots;
        this.dateSelected = true;
      },
      error: (error) => {
        console.error(error);
        this.toastR.warningLoco("user.wizard.meeting.room.error-fetching-slots");
      }
    });
  }

  private createDefaultSlotsForDate(date: Date): void {
    this.dateSelected = true;
    this.slots = [{
      start: this.toStart(date),
      end: this.toEnd(date),
    }];
  }

  /**
   * Sets the start time of a slot to 8:00 AM on the given date
   * @param date The date to set the start time for
   * @returns A new Date object set to 8:00 AM on the given date
   */
  private toStart(date: Date): Date {
    const d = new Date(date);
    d.setHours(8, 0, 0, 0)
    return d;
  }

  /**
   * Sets the end time of a slot to 7:00 PM on the given date
   * @param date The date to set the end time for
   * @returns A new Date object set to 7:00 PM on the given date
   */
  private toEnd(date: Date): Date {
    const d = new Date(date);
    d.setHours(19, 0, 0, 0)
    return d;
  }

  generateStartTimes() {
    this.startTimes = [];

    for (const slot of this.slots) {
      this.generateTimesForSlot(slot);
    }
  }

  private generateTimesForSlot(slot: MeetingSlots): void {
    const slotStart = new Date(slot.start);
    const slotEnd = new Date(slot.end);
    const durationMinutes = this.getDurationMinutes();
    let currentTime = new Date(slotStart);

    while (this.canFitMeeting(currentTime, slotEnd, durationMinutes)) {
      const timeSlotLabel = this.formatTimeSlotLabel(currentTime, durationMinutes);

      this.startTimes.push({ label: timeSlotLabel, value: new Date(currentTime) });

      currentTime.setMinutes(currentTime.getMinutes() + 15);
    }
  }

  private canFitMeeting(currentTime: Date, slotEnd: Date, durationMinutes: number): boolean {
    return currentTime.getTime() + durationMinutes * 60000 <= slotEnd.getTime();
  }

  /**
   * Formats the label for a time slot (e.g., "09h00m - 10h00m")
   */
  private formatTimeSlotLabel(startTime: Date, durationMinutes: number): string {
    const startTimeHours = startTime.getHours().toString().padStart(2, '0');
    const startTimeMinutes = startTime.getMinutes().toString().padStart(2, '0');

    const endTime = new Date(startTime.getTime() + durationMinutes * 60000);
    const endTimeHours = endTime.getHours().toString().padStart(2, '0');
    const endTimeMinutes = endTime.getMinutes().toString().padStart(2, '0');

    return `${startTimeHours}h${startTimeMinutes}m - ${endTimeHours}h${endTimeMinutes}m`;
  }

  /**
   * Gets the duration in minutes based on the selected meeting duration
   * @returns The duration in minutes
   */
  private getDurationMinutes(): number {
    switch (this.selectedMeetingDuration) {
      case MeetingDuration.QUARTER:
        return 15;
      case MeetingDuration.HALF_HOUR:
        return 30;
      case MeetingDuration.HOUR:
        return 60;
      case MeetingDuration.HOUR_AND_HALF:
        return 90;
      case MeetingDuration.TWO_HOURS:
        return 120;
      case MeetingDuration.CUSTOM:
        return this.parseCustomDuration();
      default:
        return 0;
    }
  }

  /**
   * Parses the custom duration string (e.g., "1h30m") into minutes
   * @returns The duration in minutes, defaults to 60 if parsing fails
   */
  private parseCustomDuration(): number {
    if (!this.customLength) {
      return 60;
    }

    const match = customLengthRegex.exec(this.customLength.toLowerCase().replaceAll(/[^0-9hm]/g, ''));
    if (!match) {
      return 60;
    }

    const hours = parseInt(match[1] || '0', 10);
    const minutes = parseInt(match[2] || '0', 10);

    return (hours * 60) + minutes || 60;
  }

  nextSection(): void {
    if (this.meeting.startTime === null || this.meeting.endTime === null) {
      this.toastR.warningLoco("user.wizard.meeting.room.time-required");
      return;
    }

    if (this.meeting.room.id === 0) {
      this.toastR.warningLoco("user.wizard.meeting.room.room-required");
      return;
    }

    this.next.emit();
  }

  protected readonly MeetingDuration = MeetingDuration;

  toggleSlotPicker(): void {
    this.slotPickerMinimized = !this.slotPickerMinimized;
  }
}
