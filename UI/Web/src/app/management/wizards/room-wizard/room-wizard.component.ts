import {Component, OnInit} from '@angular/core';
import {StepsModule} from 'primeng/steps';
import {InputTextModule} from 'primeng/inputtext';
import {InputTextarea} from 'primeng/inputtextarea';
import {CheckboxModule} from 'primeng/checkbox';
import {FormsModule} from '@angular/forms';
import {CommonModule} from '@angular/common';
import {MeetingRoomService} from '../../../_services/meeting-room.service';
import {ActivatedRoute, Router} from '@angular/router';
import {ToastService} from '../../../_services/toast-service';
import {MeetingRoom} from '../../../_models/room';
import {RoomWizardFacilityComponent} from './_components/room-wizard-facility/room-wizard-facility.component';
import {Observable} from 'rxjs';


@Component({
  selector: 'app-room-wizard',
  imports: [StepsModule, InputTextModule, FormsModule, CommonModule, CheckboxModule, InputTextarea, RoomWizardFacilityComponent],
  templateUrl: './room-wizard.component.html',
  styleUrls: ['./room-wizard.component.css']
})
export class RoomWizardComponent implements OnInit {
  // Steps model for the PrimeNG steps component
  steps = [
    { label: 'General Info' },
    { label: 'Capacity' },
    { label: 'Facilities' },
    { label: 'Summary' }
  ];

  currentStep = 0;

  roomData: MeetingRoom = {
    id: 0,
    displayName: '',
    description: '',
    location: '',
    capacity: 0,
    mayExceedCapacity: false,
    mergeAble: false,
    facilities: []
  };

  constructor(
    private meetingRoomService: MeetingRoomService,
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private toastService: ToastService,
  ) {}

  ngOnInit(): void {
    this.activatedRoute.queryParams.subscribe((params) => {
      const roomIdParam = params['roomId'];
      if (!roomIdParam) {
        return;
      }

      let roomId;
      try {
        roomId = parseInt(roomIdParam);
      } catch (e) {
        console.error(e);
        this.router.navigateByUrl("/management/configuration#facilities")
        return
      }

      this.meetingRoomService.get(roomId).subscribe({
        next: (room) => {
          this.roomData = room;
          // this.loadStage(); TODO: See https://github.com/Fesaa/Agora/blob/a7433cba2ad794016dac931cdee3203f04e56238/UI/Web/src/app/management/wizards/facility-wizard/facility-wizard.component.ts#L93
        },
        error: (error) => {
          console.error(error);
          this.toastService.errorLoco("shared.generic-error", {}, {err: error.message});
          this.router.navigateByUrl("/management/configuration#facilities")
        }
      })
    })
  }


  nextStep() {
    if (this.currentStep < this.steps.length - 1) {
      this.currentStep++;
    }
  }

  prevStep() {
    if (this.currentStep > 0) {
      this.currentStep--;
    }
  }

  submitRoom() {
    let obs: Observable<any>
    if (this.roomData.id > 0) {
      obs = this.meetingRoomService.update(this.roomData)
    } else {
      obs = this.meetingRoomService.create(this.roomData);
    }


    obs.subscribe({
      next: (response) => {
        console.log('Room created successfully:', response);
        this.toastService.success("Room created successfully");
        this.router.navigateByUrl("/management/configuration#rooms");
      },
      error: (error) => {
        console.error('Error creating room:', error);
        this.toastService.errorLoco("shared.generic-error", {}, {err: error.message});
      }
    });
  }

}
