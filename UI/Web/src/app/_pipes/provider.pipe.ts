import {Pipe, PipeTransform} from '@angular/core';
import {Provider} from '../_models/provider';
import {TranslocoService} from '@jsverse/transloco';

@Pipe({
  name: 'provider'
})
export class ProviderPipe implements PipeTransform {

  constructor(private loco: TranslocoService) {
  }

  transform(value: Provider): string {
    switch (value) {
      case Provider.System:
        return this.loco.translate("shared.pipes.provider.system");
      case Provider.User:
        return this.loco.translate("shared.pipes.provider.user");
    }
    return this.loco.translate("shared.unknown")
  }

}
