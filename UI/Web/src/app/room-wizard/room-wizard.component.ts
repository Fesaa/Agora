import { Component } from '@angular/core';
import { StepsModule } from 'primeng/steps';
import { InputTextModule } from 'primeng/inputtext';
import { InputTextarea } from 'primeng/inputtextarea';
import { CheckboxModule } from 'primeng/checkbox';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MeetingRoomService } from '../_services/meeting-room.service';




@Component({
  selector: 'app-room-wizard',
  imports: [StepsModule, InputTextModule, FormsModule,CommonModule, CheckboxModule],
  templateUrl: './room-wizard.component.html',
  styleUrls: ['./room-wizard.component.css']
})
export class RoomWizardComponent {
  // Steps model for the PrimeNG steps component
  steps = [
    { label: 'General Info' },
    { label: 'Capacity' },
    { label: 'Facilities' },
    { label: 'Summary' }
  ];

  currentStep = 0;

  roomData = {
    id: 0,
    displayName: '',
    description: '',
    location: '',
    capacity: 0,
    mayExceedCapacity: false,
    mergeAble: false,
    facilities: []
  };

  constructor(private meetingRoomService: MeetingRoomService) {}


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
    this.meetingRoomService.create(this.roomData).subscribe({
      next: (response) => {
        console.log('Room created successfully:', response);
        // Handle success (e.g., navigate away, show a message, reset the form)
      },
      error: (error) => {
        console.error('Error creating room:', error);
        // Handle errors (e.g., show an error message)
      }
    });
  }
  
}
