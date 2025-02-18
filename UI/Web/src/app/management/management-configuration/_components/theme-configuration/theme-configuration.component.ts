import {Component, OnInit, ViewChild} from '@angular/core';
import {ThemeService} from '../../../../_services/theme.service';
import {Theme} from '../../../../_models/theme';
import {Card} from 'primeng/card';
import {TranslocoDirective, TranslocoService} from '@jsverse/transloco';
import {Tag} from 'primeng/tag';
import {ProviderPipe} from '../../../../_pipes/provider.pipe';
import {NgForOf, NgIf, TitleCasePipe} from '@angular/common';
import {Provider} from '../../../../_models/provider';
import {ToastService} from '../../../../_services/toast-service';
import {FileUpload, FileUploadEvent, FileUploadHandlerEvent} from 'primeng/fileupload';
import {environment} from '../../../../../environments/environment';
import {CdkFixedSizeVirtualScroll, CdkVirtualForOf, CdkVirtualScrollViewport} from '@angular/cdk/scrolling';

@Component({
  selector: 'app-theme-configuration',
  imports: [
    Card,
    TranslocoDirective,
    Tag,
    ProviderPipe,
    TitleCasePipe,
    FileUpload,
    CdkVirtualScrollViewport,
    CdkFixedSizeVirtualScroll,
    CdkVirtualForOf,
  ],
  templateUrl: './theme-configuration.component.html',
  styleUrl: './theme-configuration.component.css'
})
export class ThemeConfigurationComponent implements OnInit{

  baseUrl = environment.apiUrl + "Theme"
  themes: Theme[] = [];

  @ViewChild("themeUploader") themeUploader!: FileUpload;

  constructor(
    private themeService: ThemeService,
    private toastR: ToastService,
    private loco: TranslocoService,
  ) {
  }

  ngOnInit(): void {
    this.themeService.all().subscribe(themes => this.themes = themes);
  }

  deleteTheme(theme: Theme): void {
    this.themeService.delete(theme.id).subscribe({
      next: () => {
        this.toastR.successLoco("management.configuration.branding.themes.actions.delete.success", {}, theme.name)
        this.themes = this.themes.filter(t => t.id !== theme.id);
      },
      error: (err) => {
        this.toastR.errorLoco("management.configuration.branding.themes.actions.delete.error", {}, {
          name: theme.name,
          msg: this.loco.translate(err.error.message),
        });
      }
    })
  }

  setDefaultTheme(theme: Theme): void {
    this.themeService.setDefaultTheme(theme).subscribe({
      next: () => {
        this.toastR.successLoco("management.configuration.branding.themes.actions.update.success", {}, {name: theme.name})
      },
      error: (err) => {
        this.toastR.errorLoco("management.configuration.branding.themes.actions.update.error", {}, {
          name: theme.name,
          msg: err.error.message,
        })
      }
    });
  }

  uploadTheme(e: FileUploadHandlerEvent) {
    if (e.files.length == 0 || e.files[0].size == 0) {
      this.toastR.errorLoco("management.configuration.branding.themes.actions.upload.no-file");
      return;
    }

    this.themeService.upload(e.files[0]).subscribe({
      next: (theme) => {
        this.themes.push(theme);
        this.toastR.successLoco("management.configuration.branding.themes.actions.upload.success", {}, {name: theme.name})
      },
      error: (err) => {
        this.toastR.errorLoco("management.configuration.branding.themes.actions.upload.error", {}, {
          msg: err.error.message,
        })
      }
    }).add(() => this.themeUploader.clear())
  }

  protected readonly Provider = Provider;
}
