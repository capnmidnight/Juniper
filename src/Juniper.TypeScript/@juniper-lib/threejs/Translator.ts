import { TypedEvent } from "@juniper-lib/tslib";
import { Cube } from "./Cube";
import { assureRayTarget, RayTarget } from "./eventSystem/RayTarget";

export class TranslatorDragDirEvent extends TypedEvent<"dragdir">{

    public readonly delta = new THREE.Vector3();

    constructor() {
        super("dragdir");
    }
}

export interface TranslatorDragDirEvents {
    "dragdir": TranslatorDragDirEvent;
}

export class Translator extends Cube {
    private _size: number = 1;

    readonly target: RayTarget<TranslatorDragDirEvents>;

    private static readonly small = new THREE.Vector3(0.1, 0.1, 0.1);

    constructor(
        name: string,
        private sx: number,
        private sy: number,
        private sz: number,
        color: THREE.MeshBasicMaterial) {

        super(1, 1, 1, color);

        this.name = "Translator " + name;

        const sel = new THREE.Vector3(sx, sy, sz);
        const start = new THREE.Vector3();
        const deltaIn = new THREE.Vector3();
        const dragEvt = new TranslatorDragDirEvent();

        let dragging = false;

        this.target = assureRayTarget<TranslatorDragDirEvents>(this);
        this.target.addMesh(this);
        this.target.enabled = true;
        this.target.draggable = true;

        this.target.addEventListener("dragstart", (evt) => {
            dragging = true;
            start.copy(evt.point);
        });

        this.target.addEventListener("dragend", () => {
            dragging = false;
        });

        this.target.addEventListener("drag", (evt) => {
            if (dragging) {

                deltaIn
                    .copy(evt.point)
                    .sub(start);

                start.copy(evt.point);

                if (deltaIn.manhattanLength() > 0) {
                    dragEvt.delta
                        .copy(sel)
                        .applyQuaternion(this.parent.parent.quaternion)
                        .multiplyScalar(deltaIn.dot(dragEvt.delta));

                    this.target.dispatchEvent(dragEvt);
                }
            }
        });
    }

    get size(): number {
        return this._size;
    }

    set size(v: number) {
        this._size = v;

        this.scale.set(this.sx, this.sy, this.sz)
            .multiplyScalar(0.9)
            .add(Translator.small)
            .multiplyScalar(this.size);

        this.position.set(this.sx, this.sy, this.sz)
            .multiplyScalar(this.size);
    }
}
