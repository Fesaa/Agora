import {Component, OnInit} from '@angular/core';
import {ThemeService} from '../../../../_services/theme.service';
import {Theme} from '../../../../_models/theme';
import {Card} from 'primeng/card';
import {TranslocoDirective} from '@jsverse/transloco';
import {Tag} from 'primeng/tag';
import {ProviderPipe} from '../../../../_pipes/provider.pipe';
import {TitleCasePipe} from '@angular/common';

@Component({
  selector: 'app-theme-configuration',
  imports: [
    Card,
    TranslocoDirective,
    Tag,
    ProviderPipe,
    TitleCasePipe
  ],
  templateUrl: './theme-configuration.component.html',
  styleUrl: './theme-configuration.component.css'
})
export class ThemeConfigurationComponent implements OnInit{

  themes: Theme[] = [];

  constructor(
    private themeService: ThemeService,
  ) {
  }

  ngOnInit(): void {
    this.themeService.all().subscribe(themes => this.themes = themes);
  }

}
