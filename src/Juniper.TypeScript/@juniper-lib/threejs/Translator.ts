import { TypedEvent } from "@juniper-lib/tslib";
import { Cube } from "./Cube";
import { RayTarget } from "./eventSystem/RayTarget";
import { obj } from "./objects";
import { Sphere } from "./Sphere";

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
    private static readonly small = new THREE.Vector3(0.1, 0.1, 0.1);
    private readonly bar: Cube;
    private readonly pad: Sphere;
    private _size: number = 1;
    private readonly sel: THREE.Vector3;

    constructor(
        name: string,
        sx: number,
        sy: number,
        sz: number,
        color: THREE.MeshBasicMaterial) {
        const cube = new Cube(1, 1, 1, color);
        const sphere = new Sphere(1, color);
        super(obj(
            "Translator " + name,
            cube,
            sphere
        ));

        this.bar = cube;
        this.pad = sphere;

        this.sel = new THREE.Vector3(sx, sy, sz);
        const start = new THREE.Vector3();
        const deltaIn = new THREE.Vector3();
        const dragEvt = new TranslatorDragDirEvent();

        let dragging = false;

        this.addMesh(cube);
        this.addMesh(sphere);

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
                        .copy(this.sel)
                        .applyQuaternion(this.object.parent.parent.quaternion)
                        .multiplyScalar(deltaIn.dot(dragEvt.delta));

                    this.dispatchEvent(dragEvt);
                }
            }
        });

        this.size = 1;
    }

    get size(): number {
        return this._size;
    }

    set size(v: number) {
        this._size = v;

        this.pad.scale.setScalar(v / 3);
        this.pad.position
            .copy(this.sel)
            .multiplyScalar(0.5)
            .add(this.sel)
            .multiplyScalar(this.size);

        this.bar.scale
            .copy(this.sel)
            .multiplyScalar(0.9)
            .add(Translator.small)
            .multiplyScalar(this.size);

        this.bar.position
            .copy(this.sel)
            .multiplyScalar(this.size);
    }
}
