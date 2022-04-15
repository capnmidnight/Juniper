import { BaseProgress, IProgress } from "@juniper/progress";
import { Cube } from "./Cube";
import { deepSetLayer, PURGATORY } from "./layers";
import { litGrey, litWhite } from "./materials";
import { ErsatzObject, objectIsVisible, objectSetVisible, objGraph } from "./objects";

function chrome(x: number, y: number, z: number, w: number, h: number, d: number) {
    const chromeMesh = new Cube(w, h, d, litWhite);
    chromeMesh.position.set(x, y, z);
    return chromeMesh;
}

const velocity = 0.1;
export class LoadingBar
    extends BaseProgress
    implements IProgress, ErsatzObject {

    private readonly valueBar: Cube;
    private value = 0;
    private targetValue = 0;
    readonly object = new THREE.Object3D();

    constructor() {
        super();

        this.valueBar = new Cube(0, 1, 1, litGrey);
        this.valueBar.scale.set(0, 1, 1);

        const valueBarContainer = new THREE.Object3D();
        valueBarContainer.scale.set(1, 0.1, 0.1);

        objGraph(this,
            objGraph(valueBarContainer,
                this.valueBar),
            chrome(-0.5, 0, -0.05, 0.01, 0.1, 0.01),
            chrome(-0.5, 0, 0.05, 0.01, 0.1, 0.01),
            chrome(0.5, 0, -0.05, 0.01, 0.1, 0.01),
            chrome(0.5, 0, 0.05, 0.01, 0.1, 0.01),
            chrome(-0.5, -0.05, 0, 0.01, 0.01, 0.1),
            chrome(0.5, -0.05, 0, 0.01, 0.01, 0.1),
            chrome(-0.5, 0.05, 0, 0.01, 0.01, 0.1),
            chrome(0.5, 0.05, 0, 0.01, 0.01, 0.1),
            chrome(0, -0.05, -0.05, 1, 0.01, 0.01),
            chrome(0, 0.05, -0.05, 1, 0.01, 0.01),
            chrome(0, -0.05, 0.05, 1, 0.01, 0.01),
            chrome(0, 0.05, 0.05, 1, 0.01, 0.01));

        deepSetLayer(this, PURGATORY);
    }

    private _enabled: boolean = true;
    get enabled(): boolean {
        return this._enabled;
    }

    set enabled(v: boolean) {
        if (v !== this._enabled) {
            this._enabled = v;
            objectSetVisible(this, objectIsVisible(this) || this.enabled);
        }
    }

    override report(soFar: number, total: number, msg?: string) {
        super.report(soFar, total, msg);
        this.targetValue = this.p;
    }

    update(dt: number) {
        if (this.object.parent.visible) {
            this.value = Math.min(this.targetValue, this.value + velocity * dt);
            this.valueBar.scale.set(this.value, 1, 1);
            this.valueBar.position.x = this.value / 2 - 0.5;
            objectSetVisible(this, this.enabled && this.value > 0);
        }
    }
}