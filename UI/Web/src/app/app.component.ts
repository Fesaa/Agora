import {Component, OnInit, ViewContainerRef} from '@angular/core';
import {Router, RouterOutlet} from '@angular/router';
import {AuthService} from './_services/auth.service';
import {OpenIdConnectService} from './_services/open-id-connect.service';
import {Title} from '@angular/platform-browser';
import {ThemeService} from './_services/theme.service';
import {Toast} from 'primeng/toast';
import {PrimeNG} from 'primeng/config';
import {TranslocoService} from '@jsverse/transloco';
import {DialogService} from './_services/dialog.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Toast],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  title = 'Agora';

  constructor(
    private router: Router,
    private authService: AuthService, // Needed so the flow can finish on callback DO NOT REMOVE
    private openIdService: OpenIdConnectService,
    private titleService: Title,
    private vcr: ViewContainerRef,
    private ds: DialogService,
    private themeService: ThemeService,
    private primeNG: PrimeNG,
    private transLoco: TranslocoService
  ) {
    this.ds.viewContainerRef = this.vcr;
  }

  ngOnInit(): void {
    this.titleService.setTitle(this.title)

    this.openIdService.isSetup().subscribe(isSetup => {
      if (isSetup) {
        return;
      }

      this.router.navigateByUrl('/first-setup')
    });

    this.themeService.activatedTheme().subscribe({
      next: theme => {
        if (theme) {
          this.themeService.setTheme(theme.name);
        }
      },
      error: err => {
        console.log(err);
      }
    });

    const primeNGTrans = this.transLoco.getTranslation().get("primeng");
    if (primeNGTrans) {
      this.primeNG.setTranslation(primeNGTrans);
    }


  }

}
