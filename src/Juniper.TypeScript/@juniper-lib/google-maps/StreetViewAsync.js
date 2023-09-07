import { TypedEvent, TypedEventTarget } from "@juniper-lib/events/TypedEventTarget";
import { once } from "@juniper-lib/events/once";
export class StreetViewAsync extends TypedEventTarget {
    constructor(streetView) {
        super();
        this.streetView = streetView;
        this.publicChangedEvt = new TypedEvent("positionchanged");
        this.privateChangedEvt = new TypedEvent("_positionchanged");
        this.firePublicEvt = true;
        this.streetView.addListener("position_changed", () => {
            this.dispatchEvent(this.privateChangedEvt);
            if (this.firePublicEvt) {
                this.visible = true;
                this.dispatchEvent(this.publicChangedEvt);
            }
        });
    }
    async hasChanged(pano) {
        const task = once(this, "_positionchanged", 3000);
        this.streetView.setPano(pano);
        await task;
    }
    async searchPano(pano) {
        const changing = pano !== this.streetView.getPano();
        await this.setPano(pano);
        if (changing) {
            this.dispatchEvent(this.publicChangedEvt);
        }
    }
    async setPano(pano) {
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
    close() {
        this.streetView.setPano(null);
        this.visible = false;
    }
    getPano() {
        if (this.visible) {
            return this.streetView.getPano();
        }
        else {
            return null;
        }
    }
    getLocation() {
        return this.streetView.getLocation();
    }
    get visible() {
        return this.streetView.getVisible();
    }
    set visible(v) {
        this.streetView.setVisible(v);
    }
}
//# sourceMappingURL=StreetViewAsync.js.map