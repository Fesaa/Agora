import {Provider} from './provider';

export interface Theme {
  id: number;
  name: string;
  fileName: string;
  themeProvider: Provider;
  selector: string;
  default: boolean;
}
