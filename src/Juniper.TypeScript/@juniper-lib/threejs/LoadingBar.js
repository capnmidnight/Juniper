import { BaseProgress } from "@juniper-lib/progress/BaseProgress";
import { Cube } from "./Cube";
import { deepSetLayer, PURGATORY } from "./layers";
import { litGrey, litWhite } from "./materials";
import { obj, objectIsVisible, objectSetVisible, objGraph } from "./objects";
function chrome(x, y, z, w, h, d) {
    const chromeMesh = new Cube(w, h, d, litWhite);
    chromeMesh.position.set(x, y, z);
    return chromeMesh;
}
const velocity = 0.1;
export class LoadingBar extends BaseProgress {
    constructor() {
        super();
        this.value = 0;
        this.targetValue = 0;
        this.object = obj("LoadingBar");
        this._enabled = true;
        this.valueBar = new Cube(0, 1, 1, litGrey);
        this.valueBar.scale.set(0, 1, 1);
        const valueBarContainer = obj("ValueBarContainer");
        valueBarContainer.scale.set(1, 0.1, 0.1);
        objGraph(this, objGraph(valueBarContainer, this.valueBar), chrome(-0.5, 0, -0.05, 0.01, 0.1, 0.01), chrome(-0.5, 0, 0.05, 0.01, 0.1, 0.01), chrome(0.5, 0, -0.05, 0.01, 0.1, 0.01), chrome(0.5, 0, 0.05, 0.01, 0.1, 0.01), chrome(-0.5, -0.05, 0, 0.01, 0.01, 0.1), chrome(0.5, -0.05, 0, 0.01, 0.01, 0.1), chrome(-0.5, 0.05, 0, 0.01, 0.01, 0.1), chrome(0.5, 0.05, 0, 0.01, 0.01, 0.1), chrome(0, -0.05, -0.05, 1, 0.01, 0.01), chrome(0, 0.05, -0.05, 1, 0.01, 0.01), chrome(0, -0.05, 0.05, 1, 0.01, 0.01), chrome(0, 0.05, 0.05, 1, 0.01, 0.01));
        deepSetLayer(this, PURGATORY);
    }
    get enabled() {
        return this._enabled;
    }
    set enabled(v) {
        if (v !== this._enabled) {
            this._enabled = v;
            objectSetVisible(this, objectIsVisible(this) || this.enabled);
        }
    }
    report(soFar, total, msg) {
        super.report(soFar, total, msg);
        this.targetValue = this.p;
    }
    update(dt) {
        if (this.object.parent.visible) {
            this.value = Math.min(this.targetValue, this.value + velocity * dt);
            this.valueBar.scale.set(this.value, 1, 1);
            this.valueBar.position.x = this.value / 2 - 0.5;
            objectSetVisible(this, this.enabled && this.value > 0);
        }
    }
}
//# sourceMappingURL=LoadingBar.js.map