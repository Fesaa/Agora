import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {Meeting, MeetingSlots} from '../../../../_models/meeting';
import {Card} from 'primeng/card';
import {Button} from 'primeng/button';
import {Translation, TranslocoDirective, TranslocoService} from '@jsverse/transloco';
import {DatePicker} from 'primeng/datepicker';
import {MeetingService} from '../../../../_services/meeting.service';
import {ToastService} from '../../../../_services/toast-service';
import {Select} from 'primeng/select';
import {FormsModule} from '@angular/forms';
import {InputText} from 'primeng/inputtext';
import {NgClass, NgIf} from '@angular/common';
import {Listbox} from 'primeng/listbox';
import {MeetingRoom} from '../../../../_models/room';
import {Splitter} from 'primeng/splitter';
import {CdkFixedSizeVirtualScroll, CdkVirtualForOf, CdkVirtualScrollViewport} from '@angular/cdk/scrolling';

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
    Button,
    TranslocoDirective,
    DatePicker,
    Select,
    FormsModule,
    InputText,
    NgClass,
    Splitter,
    CdkVirtualScrollViewport,
    NgIf,
    CdkFixedSizeVirtualScroll,
    CdkVirtualForOf
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
  }

  private setDurations() {
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
    const durationMinutes = this.getDurationMinutes();
    const endTime = date.getTime() + durationMinutes * 60000

    this.meeting.startTime = date;
    this.meeting.endTime = new Date(endTime);

    this.meetingService.roomsOn(this.meeting.startTime, this.meeting.endTime).subscribe({
      next: rooms => {
        this.rooms = rooms.map(room => {
          return {
            label: room.displayName,
            value: room,
          };
        });
      },
      error: err => {
        console.error(err);
        this.toastR.genericError(err.error.message)
      }
    })
  }

  handleRoomPick(room: MeetingRoom) {
    this.meeting.meetingRoom = room;
  }

  handleDatePick(date: Date) {
    // TODO: TimeZones are annoying. This isn't working in the backend
    /*this.meetingService.slots(this.meeting.meetingRoom.id, date).subscribe({
      next: (slots) => {
        this.slots = slots;
      },
      error: (error) => {
        console.error(error);
        this.toastR.genericError(error.error.message);
      }
    });*/
    this.slots = [{
      start: this.toStart(date),
      end: this.toEnd(date),
    }];
    this.generateStartTimes()
  }

  private toStart(date: Date): Date {
    const d = new Date(date);
    d.setHours(8, 0, 0, 0)
    return d;
  }

  private toEnd(date: Date) {
    const d = new Date(date);
    d.setHours(19, 0, 0, 0)
    //d.setDate(date.getDate() +1);
    return d;
  }

  // Cuts up the slots into slots of size selectedMeetingDuration
  generateStartTimes() {
    this.startTimes = [];

    for (const slot of this.slots) {
      const slotStart = new Date(slot.start);
      const slotEnd = new Date(slot.end);
      const durationMinutes = this.getDurationMinutes();

      let currentTime = new Date(slotStart);

      while (currentTime.getTime() + durationMinutes * 60000 <= slotEnd.getTime()) {
        const startTimeHours = currentTime.getHours().toString().padStart(2, '0');
        const startTimeMinutes = currentTime.getMinutes().toString().padStart(2, '0');
        const endTime = new Date(currentTime.getTime() + durationMinutes * 60000);
        const endTimeHours = endTime.getHours().toString().padStart(2, '0');
        const endTimeMinutes = endTime.getMinutes().toString().padStart(2, '0');

        const label = `${startTimeHours}h${startTimeMinutes}m - ${endTimeHours}h${endTimeMinutes}m`;

        this.startTimes.push({ label: label, value: new Date(currentTime) });

        currentTime.setMinutes(currentTime.getMinutes() + 15);
      }
    }
  }

  private getDurationMinutes() {
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
        // TODO: Work out
        return 60;
      default:
        return 0;
    }
  }

  nextSection() {
    if (this.meeting.startTime === null || this.meeting.endTime === null) {
      this.toastR.warningLoco("user.wizard.meeting.room.time-required")
      return;
    }

    if (this.meeting.meetingRoom.id === 0) {
      this.toastR.warningLoco("user.wizard.meeting.room.room-required")
      return;
    }

    this.next.emit();
  }

  protected readonly MeetingDuration = MeetingDuration;
}
