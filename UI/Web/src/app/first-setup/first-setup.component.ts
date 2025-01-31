import {Component, OnInit} from '@angular/core';
import {OpenIdConnectService} from '../_services/open-id-connect.service';
import {Router} from '@angular/router';
import {OpenIdConnectInfo, OpenIdProvider} from '../_models/openid';
import {TranslocoDirective, TranslocoService} from '@jsverse/transloco';
import {Card} from 'primeng/card';
import {SSO_PROVIDER} from '../_constants/links';
import {FloatLabel} from 'primeng/floatlabel';
import {IconField} from 'primeng/iconfield';
import {InputIcon} from 'primeng/inputicon';
import {InputText} from 'primeng/inputtext';
import {FormsModule} from '@angular/forms';
import {Select} from 'primeng/select';
import {Button} from 'primeng/button';
import {MessageService} from 'primeng/api';
import {Toast} from 'primeng/toast';

@Component({
  selector: 'app-first-setup',
  imports: [
    TranslocoDirective,
    Card,
    FloatLabel,
    IconField,
    InputIcon,
    InputText,
    FormsModule,
    Select,
    Button,
    Toast
  ],
  templateUrl: './first-setup.component.html',
  styleUrl: './first-setup.component.css'
})
export class FirstSetupComponent implements OnInit {

  model: OpenIdConnectInfo = {
    authority: '',
    provider: OpenIdProvider.KeyCloak,
  }

  providerOptions = [
    {
      label: "KeyCloak",
      value: OpenIdProvider.KeyCloak,
    },
    {
      label: "Azure",
      value: OpenIdProvider.AzureAd,
    }
  ]

  constructor(
    private openIdService: OpenIdConnectService,
    private router: Router,
    private msgService: MessageService,
    private translocoService: TranslocoService
  ) {
  }

  ngOnInit(): void {
    this.openIdService.isSetup().subscribe((isSetup: boolean) => {
      if (!isSetup) {
        return;
      }

      this.router.navigateByUrl('/user/dashboard');
    })
  }

  private submitError(key: string) {
    this.msgService.add({
      severity: 'error',
      summary: this.translocoService.translate("first-setup.errors.cannot-submit"),
      detail: this.translocoService.translate("first-setup.errors."+key)
    });
  }

  submit() {
    if (this.model.authority === '') {
      this.submitError("missing-authority");
      return;
    }

    if (this.model.authority.endsWith('/')) {
      this.model.authority = this.model.authority.substring(0, this.model.authority.length - 1);
    }

    try {
      new URL(this.model.authority)
    } catch (_) {
      this.submitError("no-url");
      return;
    }

    this.openIdService.firstSetup(this.model).subscribe({
      next: _ => {
        this.msgService.add({
          severity: 'success',
          summary: this.translocoService.translate("first-setup.success.summary"),
          detail: this.translocoService.translate("first-setup.success.detail"),
        })
      },
      error: err => {
        this.msgService.add({
          severity: 'error',
          summary: this.translocoService.translate("first-setup.errors.failed"),
          detail: err.message,
        })
      }
    })

  }

  protected readonly SSO_PROVIDER = SSO_PROVIDER;
}
