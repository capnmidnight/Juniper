import { TypedEvent } from "@juniper-lib/tslib";
import { Cube } from "./Cube";
import { RayTarget } from "./eventSystem/RayTarget";

export class TranslatorDragDirEvent extends TypedEvent<"dragdir">{

    public readonly delta = new THREE.Vector3();

    constructor() {
        super("dragdir");
    }
}

export interface TranslatorDragDirEvents {
    "dragdir": TranslatorDragDirEvent;
}

export class Translator extends RayTarget<TranslatorDragDirEvents> {
    private _size: number = 1;

    private static readonly small = new THREE.Vector3(0.1, 0.1, 0.1);

    constructor(
        name: string,
        private sx: number,
        private sy: number,
        private sz: number,
        color: THREE.MeshBasicMaterial) {
        const cube = new Cube(1, 1, 1, color);
        super(cube);

        this.object.name = "Translator " + name;

        const sel = new THREE.Vector3(sx, sy, sz);
        const start = new THREE.Vector3();
        const deltaIn = new THREE.Vector3();
        const dragEvt = new TranslatorDragDirEvent();

        let dragging = false;

        this.addMesh(cube);
        this.enabled = true;
        this.draggable = true;

        this.addEventListener("dragstart", (evt) => {
            dragging = true;
            start.copy(evt.point);
        });

        this.addEventListener("dragend", () => {
            dragging = false;
        });

        this.addEventListener("drag", (evt) => {
            if (dragging) {

                deltaIn
                    .copy(evt.point)
                    .sub(start);

                start.copy(evt.point);

                if (deltaIn.manhattanLength() > 0) {
                    dragEvt.delta
                        .copy(sel)
                        .applyQuaternion(this.object.parent.parent.quaternion)
                        .multiplyScalar(deltaIn.dot(dragEvt.delta));

                    this.dispatchEvent(dragEvt);
                }
            }
        });
    }

    get size(): number {
        return this._size;
    }

    set size(v: number) {
        this._size = v;

        this.object.scale.set(this.sx, this.sy, this.sz)
            .multiplyScalar(0.9)
            .add(Translator.small)
            .multiplyScalar(this.size);

        this.object.position.set(this.sx, this.sy, this.sz)
            .multiplyScalar(this.size);
    }
}
