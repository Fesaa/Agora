import {ApplicationRef, ComponentRef, Injectable, ViewContainerRef} from '@angular/core';
import {DialogComponent} from "../shared/components/dialog/dialog.component";
import {TranslocoService} from "@jsverse/transloco";

@Injectable({
  providedIn: 'root'
})
export class DialogService {

  public viewContainerRef: ViewContainerRef | undefined;

  constructor(
    private appRef: ApplicationRef,
    private transLoco: TranslocoService,
  ) {
  }

  openDialog(text: string, textArgs?: any, header: string = "Confirm", headerArgs?: any): Promise<boolean> {
    const component = this.viewContainerRef!.createComponent(DialogComponent)
    component.instance.text = this.transLoco.translate(text, textArgs);
    component.instance.header = this.transLoco.translate(header, headerArgs);

    return new Promise<boolean>((resolve, reject) => {
      component.instance.getResult().subscribe(result => {
        this.closeDialog(component);
        resolve(result);
      });
    });
  }

  closeDialog(componentRef: ComponentRef<any>) {
    this.appRef.detachView(componentRef.hostView);
    componentRef.destroy();
  }
}
