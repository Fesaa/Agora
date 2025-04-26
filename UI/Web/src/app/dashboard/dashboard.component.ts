import {Component, OnInit, HostListener} from '@angular/core';
import {MeetingService} from '../_services/meeting.service';
import {Meeting} from '../_models/meeting';
import {ToastService} from '../_services/toast-service';
import {MeetingCardComponent} from '../shared/components/meeting-card/meeting-card.component';
import {Carousel} from 'primeng/carousel';
import {NgIf, NgClass} from '@angular/common';
import {PrimeTemplate} from 'primeng/api';
import {TranslocoDirective} from '@jsverse/transloco';

@Component({
  selector: 'app-dashboard',
  imports: [MeetingCardComponent, Carousel, NgIf, NgClass, PrimeTemplate, TranslocoDirective],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit{

  meetings: Meeting[] = [];

  // Window reference for responsive design
  window = window;
  isWideScreen = false;

  // Carousel configuration
  carouselOptions = {
    numVisible: 3,
    numScroll: 1,
    circular: true,
    autoplayInterval: 5000,
    showNavigators: true,
    showIndicators: true
  };

  // Responsive options for different screen sizes
  responsiveOptions = [
    {
      breakpoint: '1400px',
      numVisible: 3,
      numScroll: 1
    },
    {
      breakpoint: '1024px',
      numVisible: 2,
      numScroll: 1
    },
    {
      breakpoint: '768px',
      numVisible: 1,
      numScroll: 1
    }
  ];

  constructor(
    private meetingService: MeetingService,
    private toastR: ToastService,
  ) {
  }

  ngOnInit(): void {
    // Check screen size on init
    this.checkScreenSize();

    // Fetch meetings
    this.meetingService.today().subscribe({
      next: m => {
        this.meetings = m;
      },
      error: err => {
        this.toastR.genericError(err.error.message);
      }
    });
  }

  /**
   * Check if screen is wide and update carousel options if needed
   */
  checkScreenSize(): void {
    this.isWideScreen = window.innerWidth >= 1400;

    // Update carousel options based on screen size
    if (this.isWideScreen) {
      this.carouselOptions.numVisible = 3;
    }
  }

  /**
   * Listen for window resize events
   */
  @HostListener('window:resize')
  onResize(): void {
    this.checkScreenSize();
  }

}
