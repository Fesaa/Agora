import {Provider} from './provider';

export interface SiteTheme {
  id: number;
  name: string;
  fileName: string;
  themeProvider: Provider;
}
