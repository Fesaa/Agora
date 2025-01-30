import { ApplicationConfig, provideZoneChangeDetection, isDevMode } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import {provideHttpClient, withInterceptorsFromDi} from '@angular/common/http';
import { TranslocoService } from './_services/transloco-loader';
import { provideTransloco } from '@jsverse/transloco';
import {provideOAuthClient} from 'angular-oauth2-oidc';

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
    provideRouter(routes), provideHttpClient(), provideTransloco({
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
  ]
};
