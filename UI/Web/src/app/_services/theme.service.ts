import {Inject, Injectable, Renderer2, RendererFactory2} from '@angular/core';
import {environment} from '../../environments/environment';
import {HttpClient} from '@angular/common/http';
import {map, ReplaySubject, take} from 'rxjs';
import {Theme} from '../_models/theme';
import {DOCUMENT} from '@angular/common';
import {DomSanitizer} from '@angular/platform-browser';
import {ToastService} from './toast-service';
import {TranslocoService} from '@jsverse/transloco';
import {Provider} from '../_models/provider';

@Injectable({
  providedIn: 'root'
})
export class ThemeService {

  public defaultTheme: string = 'default';

  baseUrl = environment.apiUrl + "Theme";

  private cache: Array<Theme> = [];
  private currentThemeSource = new ReplaySubject<Theme>(1);
  public currentTheme$ = this.currentThemeSource.asObservable();

  private renderer: Renderer2;

  constructor(rendererFactory: RendererFactory2,
              @Inject(DOCUMENT) private document: Document,
              private httpClient: HttpClient,
              private domSanitizer: DomSanitizer,
              private toastService: ToastService,
              private loco: TranslocoService,
  ) {
    this.renderer = rendererFactory.createRenderer(null, null);
  }

  all() {
    return this.httpClient.get<Theme[]>(this.baseUrl + "/all").pipe(map(themes => {
      this.cache = themes;
      this.currentTheme$.pipe(take(1)).subscribe(selectedTheme => {
        if (!themes.find(theme => theme.id === selectedTheme.id)) {
          this.setTheme(this.defaultTheme)
          this.toastService.warningLoco("themes.errors.not-found", {}, {name: selectedTheme.name})
        }
      });

      return themes;
    }))
  }

  content(themeId: number) {
    return this.httpClient.get(this.baseUrl + "?themeId=" + themeId, {responseType: 'text'});
  }

  setTheme(themeName:  string, recursive: boolean = true) {
    const theme = this.cache.find(theme => theme.name === themeName);
    if (!theme) {
      if (recursive) {
        this.all().subscribe(_ => {
          this.setTheme(themeName, false);
        })
      }
      return;
    }

    this.unsetThemes();
    this.renderer.addClass(this.document.querySelector('body'), theme.selector);

    if (theme.themeProvider === Provider.System || this.hasThemeInHead(theme.name)) {
      this.currentThemeSource.next(theme);
      return;
    }

    this.content(theme.id).subscribe({
      next: css => {
        this.setCss(theme, css);
        this.currentThemeSource.next(theme);
      },
      error: err => {
        this.toastService.errorLoco("themes.errors.cant-set", {name: theme.name}, {msg: err.message});
        this.setTheme(this.defaultTheme)
      }
    })
  }

  private setCss(theme: Theme, css: string) {
    const styleElem = this.document.createElement('style');
    styleElem.id = 'theme-' + theme.name;
    styleElem.appendChild(this.document.createTextNode(css));
    this.renderer.appendChild(this.document.head, styleElem);
  }

  unsetThemes() {
    this.cache.forEach(theme => this.document.body.classList.remove(theme.selector))
  }

  private hasThemeInHead(themeName: string) {
    const id = 'theme-' + themeName.toLowerCase();
    return Array.from(this.document.head.children).filter(el => el.tagName === 'STYLE' && el.id.toLowerCase() === id).length > 0;
  }

}
