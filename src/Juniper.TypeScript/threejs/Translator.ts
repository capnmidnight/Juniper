import { Cube } from "./Cube";

const P = new THREE.Vector4();
const small = new THREE.Vector3(0.1, 0.1, 0.1);

export class Translator extends Cube {
    private _size: number = 1;

    constructor(
        name: string,
        private sx: number,
        private sy: number,
        private sz: number,
        color: THREE.MeshBasicMaterial) {

        super(1, 1, 1, color);

        this.name = "Translator " + name;
        this.isDraggable = true;
        this.isCollider = true;

        const sel = new THREE.Vector4(sx, sy, sz, 0);
        const start = new THREE.Vector4();
        const deltaIn = new THREE.Vector4();
        const delta = new THREE.Vector3();

        let dragging = false;

        this.addEventListener("dragstart", (evt: THREE.Event) => {
            dragging = true;
            start.set(evt.point.x, evt.point.y, evt.point.z, 1);
        });

        this.addEventListener("dragend", () => {
            dragging = false;
        });

        this.addEventListener("drag", (evt: THREE.Event) => {
            if (dragging) {
                deltaIn
                    .set(evt.point.x, evt.point.y, evt.point.z, 1)
                    .sub(start);
                start.add(deltaIn);

                P.set(
                    sel.x / this.parent.parent.scale.x,
                    sel.y / this.parent.parent.scale.y,
                    sel.z / this.parent.parent.scale.z,
                    0)
                    .applyMatrix4(this.parent.matrixWorld)
                    .multiplyScalar(P.dot(deltaIn));

                delta.set(P.x, P.y, P.z);
                this.dispatchEvent({ type: "dragdir", delta });
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
            .add(small)
            .multiplyScalar(this.size);

        this.position.set(this.sx, this.sy, this.sz)
            .multiplyScalar(this.size);
    }
}
