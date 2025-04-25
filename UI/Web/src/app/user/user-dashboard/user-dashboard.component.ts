import {Component, OnInit} from '@angular/core';
import {AccountService} from '../../_services/account.service';
import {TranslocoDirective} from '@jsverse/transloco';
import {AuthService} from '../../_services/auth.service';
import {Card} from 'primeng/card';
import {RouterLink} from '@angular/router';
import {Meeting} from '../../_models/meeting';
import {MeetingService} from '../../_services/meeting.service';
import {forkJoin} from 'rxjs';
import {UtcToLocalTimePipe} from '../../_pipes/utc-to-local.pipe';
import {NgIf} from '@angular/common';

@Component({
  selector: 'app-user-dashboard',
  imports: [
    TranslocoDirective,
    RouterLink,
    UtcToLocalTimePipe,
    Card,
  ],
  templateUrl: './user-dashboard.component.html',
  styleUrl: './user-dashboard.component.css'
})
export class UserDashboardComponent implements OnInit{

  name: string = '';
  admin: boolean = false;

  todaysMeetings: Meeting[] = [];
  upcomingMeetings: Meeting[] = [];

  constructor(
    private accountService: AccountService,
    private authService: AuthService,
    private meetingService: MeetingService,
  ) {
  }

  ngOnInit(): void {
    this.accountService.name().subscribe(name => {
      this.name = name;
    });

    this.accountService.admin().subscribe(admin => {
      this.admin = admin;
    })

    forkJoin([
      this.meetingService.today(true),
      this.meetingService.upcoming(true, 1)
      ]).subscribe({
      next: ([today, upcoming]) => {
        this.todaysMeetings = today;
        this.upcomingMeetings = upcoming;
      },
      error: (err) => {
        console.error(err);
      }
    })

  }

  logout() {
    this.authService.logout();
  }

}
