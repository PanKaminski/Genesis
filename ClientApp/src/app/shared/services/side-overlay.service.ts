import { Overlay, OverlayRef } from "@angular/cdk/overlay";
import { ComponentPortal, ComponentType, PortalInjector } from "@angular/cdk/portal";
import { ComponentRef, Injectable, Injector } from "@angular/core";
import { SideOverlayRef } from "@shared/models/side-overlay-ref";
import { SIDE_OVERLAY_DATA } from "@shared/models/side-overlay.tokens";

@Injectable({providedIn: "root"})
export class SideOverlayService {
  private overlayRef: OverlayRef = null;

  constructor(
    private injector: Injector,
    private overlay: Overlay
    ) { }

  openSidePanel<T>(component: ComponentType<T>, data: any = { }): ComponentRef<any> {
    const positionStrategy = this.overlay.position().global().right();

    this.overlayRef = this.overlay.create({
        positionStrategy,
        scrollStrategy: this.overlay.scrollStrategies.block(),
        ...data
    });

    const injector = this.createInjector(data, new SideOverlayRef(this.overlayRef));
    const componentPortal = new ComponentPortal(component, null, injector);
    const componentRef = this.overlayRef.attach(componentPortal);
  
    return componentRef;
  }

  closeSidePanel(): void {
    if (this.overlayRef) {
      this.overlayRef.dispose();
      this.overlayRef = null;
    }
  }

  private createInjector(data: unknown, dialogRef: SideOverlayRef): Injector {
    return Injector.create({
        parent: this.injector,
        providers: [
            { provide: SideOverlayRef, useValue: dialogRef },
            { provide: SIDE_OVERLAY_DATA, useValue: data }
        ]
    });
  }
}