import { ApplicationConfig, provideZoneChangeDetection, isDevMode } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import {HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi} from '@angular/common/http';
import {AuthInterceptor} from './_interceptors/auth-cookies.interceptor';
import {AuthRedirectInterceptor} from './_interceptors/auth-redirect.interceptor';
import { TranslocoService } from './_services/transloco-loader';
import { provideTransloco } from '@jsverse/transloco';

export const appConfig: ApplicationConfig = {
  providers: [
    {provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true},
    {provide: HTTP_INTERCEPTORS, useClass: AuthRedirectInterceptor, multi: true},
    provideHttpClient(withInterceptorsFromDi()),
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
      })
  ]
};
