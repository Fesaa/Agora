import {Component, OnInit} from '@angular/core';
import {Router} from '@angular/router';
import {AuthGuard} from '../_guard/auth.guard';
import {Button} from 'primeng/button';
import {Card} from 'primeng/card';
import {ReactiveFormsModule} from '@angular/forms';
import {TranslocoDirective} from '@jsverse/transloco';
import {AuthService} from '../_services/auth.service';

@Component({
  selector: 'app-callback',
  imports: [
    Button,
    Card,
    ReactiveFormsModule,
    TranslocoDirective
  ],
  templateUrl: './callback.component.html',
  styleUrl: './callback.component.css'
})
export class CallbackComponent implements OnInit {

  pathName: string = '';

  constructor(protected router: Router) {
  }

  ngOnInit(): void {
    const pathName = localStorage.getItem(AuthGuard.urlKey);
    if (pathName) {
      this.pathName = pathName;
      localStorage.removeItem(AuthGuard.urlKey);
    }
  }

  returnToAction() {
    this.router.navigateByUrl(this.pathName ?? "/user/dashboard");
  }
}
