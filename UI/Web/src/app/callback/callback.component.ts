import {Component, OnInit} from '@angular/core';
import {Router} from '@angular/router';
import {AuthGuard} from '../_guard/auth.guard';
import {Button} from 'primeng/button';
import {Card} from 'primeng/card';
import {ReactiveFormsModule} from '@angular/forms';
import {TranslocoDirective} from '@jsverse/transloco';
import {AgoraButtonComponent} from '../shared/components/agora-button/agora-button.component';

@Component({
  selector: 'app-callback',
  imports: [
    Card,
    ReactiveFormsModule,
    TranslocoDirective,
    AgoraButtonComponent
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
