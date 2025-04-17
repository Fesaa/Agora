import {Component, OnInit} from '@angular/core';
import {Meeting} from '../../_models/meeting';
import {MeetingService} from '../../_services/meeting.service';
import {ActivatedRoute, NavigationExtras, Router} from '@angular/router';
import {ToastService} from '../../_services/toast-service';
import {Steps} from 'primeng/steps';
import {MeetingWizardGeneralComponent} from './_components/meeting-wizard-general/meeting-wizard-general.component';
import {MeetingWizardRoomComponent} from './_components/meeting-wizard-room/meeting-wizard-room.component';
import {
  MeetingWizardAttendeesComponent
} from './_components/meeting-wizard-attendees/meeting-wizard-attendees.component';
import {MeetingWizardFacilityComponent} from './_components/meeting-wizard-facility/meeting-wizard-facility.component';
import {MeetingWizardSaveComponent} from './_components/meeting-wizard-save/meeting-wizard-save.component';

export enum MeetingWizardId {
  General = 'General',
  Room = 'Room',
  Attendees = 'Attendees',
  Facility = 'Facility',

  Save = 'Save',
}

@Component({
  selector: 'app-meeting-wizard',
  imports: [
    Steps,
    MeetingWizardGeneralComponent,
    MeetingWizardRoomComponent,
    MeetingWizardAttendeesComponent,
    MeetingWizardFacilityComponent,
    MeetingWizardSaveComponent
  ],
  templateUrl: './meeting-wizard.component.html',
  styleUrl: './meeting-wizard.component.css'
})
export class MeetingWizardComponent implements OnInit{

  meeting: Meeting | undefined;
  index: number = 0;

  sections: {id: MeetingWizardId, label: string}[] = [
    {id: MeetingWizardId.General, label: 'General'},
    {id: MeetingWizardId.Room, label: 'Room'},
    {id: MeetingWizardId.Attendees, label: 'Attendees'},
    {id: MeetingWizardId.Facility, label: 'Facility'},
    {id: MeetingWizardId.Save, label: 'Save'},
  ]

  private readonly defaultMeeting: Meeting = {
    id: 0,
    title: '',
    description: '',
    creatorId: '',
    externalId: '',
    startTime: null!,
    endTime: null!,
    room: {
      id: 0,
      displayName: '',
      description: '',
      location: '',
      capacity: 0,
      mayExceedCapacity: false,
      mergeAble: false,
      facilities: []
    },
    facilities: [],
    attendees: []
  }

  constructor(
    private meetingService: MeetingService,
    private route: ActivatedRoute,
    private router: Router,
    private toastR: ToastService,
  ) {
  }

  ngOnInit(): void {
    this.route.queryParams.subscribe((params) => {
      const meetingIdParam = params['meetingId'];
      if (!meetingIdParam) {
        this.meeting = this.defaultMeeting;
        this.loadStage() // Only during testing
        return;
      }

      let meetingId;
      try {
        meetingId = parseInt(meetingIdParam);
      } catch (e) {
        console.error(e);
        this.toastR.genericError(e);
        this.router.navigateByUrl("/user/dashboard")
        return;
      }

      this.meetingService.get(meetingId).subscribe({
        next: (meeting) => {
          this.meeting = meeting;
          this.loadStage();
        },
        error: (e) => {
          console.error(e);
          this.toastR.genericError(e);
          this.router.navigateByUrl("/user/dashboard")
        }
      })
    })
  }

  private loadStage() {
    if (!this.meeting) {
      return;
    }

    if (this.meeting.id === 0) {
      //this.navigateToPage(0, true); // Disabled during testing
    }

    this.route.fragment.subscribe(fragment => {
      const section = this.sections.find(section => section.id == fragment)
      if (section) {
        this.navigateToPage(this.sections.indexOf(section), true)
      } else {
        this.navigateToPage(0, true)
      }
    })
  }

  navigateToPage(index: number, skipLocation: boolean = false) {
    this.index = index;

    const sectionId = this.sections[index].id;

    const extras: NavigationExtras = {
      fragment: sectionId,
      skipLocationChange: skipLocation,
    };

    if (this.meeting && this.meeting.id !== 0) {
      extras.queryParams = {
        meetingId: this.meeting.id
      }
    }

    this.router.navigate([], extras)
  }


  protected readonly MeetingWizardId = MeetingWizardId;
}
