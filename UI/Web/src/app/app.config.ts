import { ApplicationConfig, provideZoneChangeDetection, isDevMode } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import {provideHttpClient, withInterceptorsFromDi} from '@angular/common/http';
import { TranslocoService } from './_services/transloco-loader';
import { provideTransloco } from '@jsverse/transloco';
import {provideOAuthClient} from 'angular-oauth2-oidc';
import {providePrimeNG} from 'primeng/config';
import Aura from '@primeng/themes/aura';
import {provideAnimations} from '@angular/platform-browser/animations';
import {MessageService} from 'primeng/api';

export const appConfig: ApplicationConfig = {
  providers: [
    provideHttpClient(withInterceptorsFromDi()),
    provideOAuthClient({
      resourceServer: {
        allowedUrls: ["http://localhost:5050"],
        sendAccessToken: true,
      }
    }),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(),
    provideTransloco({
        config: {
          availableLangs: ['en'],
          defaultLang: 'en',
          missingHandler: {
            useFallbackTranslation: true,
          },
          reRenderOnLangChange: true,
          prodMode: !isDevMode(),
        },
        loader: TranslocoService
      }),
    provideAnimations(),
    providePrimeNG({
      theme: {
        preset: Aura
      }
    }),
    MessageService,
  ]
};
