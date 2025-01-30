import {Component, OnInit} from '@angular/core';
import {AccountService} from '../../_services/account.service';
import {TranslocoDirective} from '@jsverse/transloco';
import {AuthService} from '../../_services/auth.service';

@Component({
  selector: 'app-user-dashboard',
  imports: [
    TranslocoDirective
  ],
  templateUrl: './user-dashboard.component.html',
  styleUrl: './user-dashboard.component.css'
})
export class UserDashboardComponent implements OnInit{

  name: string = '';

  constructor(
    private accountService: AccountService,
    private authService: AuthService
              ) {
  }

  ngOnInit(): void {
    this.accountService.name().subscribe(name => {
      this.name = name;
    });
  }

  logout() {
    this.authService.logout();
  }

}
