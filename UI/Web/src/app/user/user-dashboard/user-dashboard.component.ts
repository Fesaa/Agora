import {Component, OnInit} from '@angular/core';
import {AccountService} from '../../_services/account.service';

@Component({
  selector: 'app-user-dashboard',
  imports: [],
  templateUrl: './user-dashboard.component.html',
  styleUrl: './user-dashboard.component.css'
})
export class UserDashboardComponent implements OnInit{

  name: string = '';

  constructor(private accountService: AccountService) {
  }

  ngOnInit(): void {
    this.accountService.name().subscribe(name => {
      this.name = name;
    });
  }

}
