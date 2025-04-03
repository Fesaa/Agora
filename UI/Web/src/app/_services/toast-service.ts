import {Injectable} from '@angular/core';
import {MessageService as PrimeNgMessageService, ToastMessageOptions} from "primeng/api";
import {TranslocoService} from '@jsverse/transloco';

@Injectable({
  providedIn: 'root'
})
export class ToastService {

  constructor(private msgService: PrimeNgMessageService, private loco: TranslocoService) {
  }

  infoLoco(key: string, titleValues?: any, summaryValues?: any, opts?: ToastMessageOptions) {
    this.info(
      this.loco.translate(key+".title", titleValues),
      this.loco.translate(key+".summary", summaryValues),
      opts
    )
  }

  info(title: string, message?: string, opts?: ToastMessageOptions) {
    this.msgService.add({
      summary: title,
      detail: message,
      severity: 'info',
      ...opts
    })
  }

  successLoco(key: string, titleValues?: any, summaryValues?: any, opts?: ToastMessageOptions) {
    this.success(
      this.loco.translate(key+".title", titleValues),
      this.loco.translate(key+".summary", summaryValues),
      opts
    )
  }

  success(title: string, message?: string, opts?: ToastMessageOptions) {
    this.msgService.add({
      summary: title,
      detail: message,
      severity: 'success',
      ...opts
    })
  }

  warningLoco(key: string, titleValues?: any, summaryValues?: any, opts?: ToastMessageOptions) {
    this.warning(
      this.loco.translate(key+".title", titleValues),
      this.loco.translate(key+".summary", summaryValues),
      opts
    )
  }

  warning(title: string, message?: string, opts?: ToastMessageOptions) {
    this.msgService.add({
      summary: title,
      detail: message,
      severity: 'warn',
      ...opts
    })
  }

  genericError(err: any, opts?: ToastMessageOptions) {
    this.errorLoco("shared.generic-error", {}, {err: err}, opts);
  }

  errorLoco(key: string, titleValues?: any, summaryValues?: any, opts?: ToastMessageOptions) {
    this.error(
      this.loco.translate(key+".title", titleValues),
      this.loco.translate(key+".summary", summaryValues),
      opts
    )
  }

  error(title: string, message?: string, opts?: ToastMessageOptions) {
    console.debug(`An error occurred ${title}:\n ${message}`);
    this.msgService.add({
      summary: title,
      detail: message,
      severity: 'error',
      ...opts
    })
  }

}
