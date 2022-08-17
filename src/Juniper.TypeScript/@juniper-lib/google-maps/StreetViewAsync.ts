import { TypedEvent, TypedEventBase } from "@juniper-lib/tslib/events/EventBase";
import { once } from "@juniper-lib/tslib/events/once";

interface StreetViewAsyncEvents {
    positionchanged: TypedEvent<"positionchanged">;
    _positionchanged: TypedEvent<"_positionchanged">
}

export class StreetViewAsync extends TypedEventBase<StreetViewAsyncEvents> {
    private publicChangedEvt = new TypedEvent("positionchanged");
    private privateChangedEvt = new TypedEvent("_positionchanged");
    private firePublicEvt = true;

    constructor(private streetView: google.maps.StreetViewPanorama) {
        super();

        this.streetView.addListener("position_changed", () => {
            this.dispatchEvent(this.privateChangedEvt);
            if (this.firePublicEvt) {
                this.visible = true;
                this.dispatchEvent(this.publicChangedEvt);
            }
        });
    }

    private async hasChanged(pano: string): Promise<void> {
        const task = once(this, "_positionchanged", 3000);
        this.streetView.setPano(pano);
        await task;
    }

    async searchPano(pano: string): Promise<void> {
        const changing = pano !== this.streetView.getPano();
        await this.setPano(pano);
        if (changing) {
            this.dispatchEvent(this.publicChangedEvt);
        }
    }

    async setPano(pano: string) {
        this.firePublicEvt = false;
        pano = pano || null;
        this.visible = !!pano;
        const changing = pano !== this.streetView.getPano();
        if (changing && pano) {
            try {
                await this.hasChanged(pano);
            }
            catch {
                console.warn("TIMEOUT");
                this.visible = false;
            }
        }

        this.firePublicEvt = true;
    }

    getPano(): string {
        if (this.visible) {
            return this.streetView.getPano();
        }
        else {
            return null;
        }
    }

    getLocation(): google.maps.StreetViewLocation {
        return this.streetView.getLocation();
    }

    get visible(): boolean {
        return this.streetView.getVisible();
    }

    set visible(v: boolean) {
        this.streetView.setVisible(v);
    }
}
