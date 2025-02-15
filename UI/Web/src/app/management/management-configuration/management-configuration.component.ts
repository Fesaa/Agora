import {Component, OnInit} from '@angular/core';
import {MenuItem, MenuItemCommandEvent} from 'primeng/api';
import {TranslocoDirective, TranslocoService} from '@jsverse/transloco';
import {Menu} from 'primeng/menu';
import {Ripple} from 'primeng/ripple';
import {NgIf} from '@angular/common';
import {Card} from 'primeng/card';
import {ThemeConfigurationComponent} from './_components/theme-configuration/theme-configuration.component';

enum ConfigurationId {
  Rooms,
  Facilities,
  BrandingThemes,
  BrandingLogo,
}

@Component({
  selector: 'app-management-configuration',
  imports: [
    Menu,
    TranslocoDirective,
    Ripple,
    NgIf,
    Card,
    ThemeConfigurationComponent
  ],
  templateUrl: './management-configuration.component.html',
  styleUrl: './management-configuration.component.css'
})
export class ManagementConfigurationComponent implements OnInit {
  items: (MenuItem & {configID?: ConfigurationId})[] | undefined;
  selected: ConfigurationId | undefined;


  constructor(
    private loco: TranslocoService,
  ) {
  }

  protected setSelectedCommand(id: ConfigurationId | undefined): () => void {
    return () => {
      this.selected = id;
    }
  }

  ngOnInit(): void {
    this.items = [
      {
        label: 'meetings',
        items: [
          {
            label: 'meetings.rooms',
            icon: 'pi pi-home',
            configID: ConfigurationId.Rooms,
            command: this.setSelectedCommand(ConfigurationId.Rooms),
          },
          {
            label: 'meetings.facilities',
            icon: 'pi pi-receipt',
            configID: ConfigurationId.Facilities,
            command: this.setSelectedCommand(ConfigurationId.Facilities),
          }
        ]
      },
      {
        label: 'branding',
        items: [
          {
            label: 'branding.themes',
            icon: 'pi pi-ethereum',
            configID: ConfigurationId.BrandingThemes,
            command: this.setSelectedCommand(ConfigurationId.BrandingThemes),
          },
          {
            label: 'branding.logo',
            icon: 'pi pi-star-half',
            configID: ConfigurationId.BrandingLogo,
            command: this.setSelectedCommand(ConfigurationId.BrandingLogo),
          }
        ]
      }
    ]
  }


  protected readonly ConfigurationId = ConfigurationId;
}
