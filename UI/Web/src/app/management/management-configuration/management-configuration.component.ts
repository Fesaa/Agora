import {Component, OnInit} from '@angular/core';
import {MenuItem} from 'primeng/api';
import {TranslocoDirective} from '@jsverse/transloco';
import {Menu} from 'primeng/menu';
import {Ripple} from 'primeng/ripple';
import {Card} from 'primeng/card';
import {ThemeConfigurationComponent} from './_components/theme-configuration/theme-configuration.component';
import {FacilityConfigurationComponent} from './_components/facility-configuration/facility-configuration.component';
import {ActivatedRoute, Router} from '@angular/router';
import {RoomConfigurationComponent} from './_components/room-configuration/room-configuration.component';

enum ConfigurationId {
  Rooms = "Rooms",
  Facilities = "Facilities",
  BrandingThemes = "BrandingThemes",
  BrandingLogo = "BrandingLogo",
}

@Component({
  selector: 'app-management-configuration',
  imports: [
    Menu,
    TranslocoDirective,
    Ripple,
    Card,
    ThemeConfigurationComponent,
    FacilityConfigurationComponent,
    RoomConfigurationComponent
  ],
  templateUrl: './management-configuration.component.html',
  styleUrl: './management-configuration.component.css'
})
export class ManagementConfigurationComponent implements OnInit {
  items: (MenuItem & {configID?: ConfigurationId})[] | undefined;
  selected: ConfigurationId | undefined;


  constructor(
    private activatedRoute: ActivatedRoute,
    private router: Router,
  ) {
  }

  protected setSelectedCommand(id: ConfigurationId | undefined): () => void {
    return () => {
      this.selected = id;
      this.router.navigate([], {fragment: id})
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

    this.activatedRoute.fragment.subscribe(fragment => {
      if (fragment) {
        if (Object.values(ConfigurationId).find(id => id === fragment)) {
          this.selected = fragment as ConfigurationId;
        }
      }
    })
  }


  protected readonly ConfigurationId = ConfigurationId;
}
