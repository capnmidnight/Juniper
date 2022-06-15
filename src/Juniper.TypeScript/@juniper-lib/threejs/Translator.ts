import { Cube } from "./Cube";

const P = new THREE.Vector3();
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

        const sel = new THREE.Vector3(sx, sy, sz);
        const start = new THREE.Vector3();
        const deltaIn = new THREE.Vector3();
        const delta = new THREE.Vector3();

        let dragging = false;

        this.addEventListener("dragstart", (evt: THREE.Event) => {
            dragging = true;
            start.set(evt.point.x, evt.point.y, evt.point.z);
        });

        this.addEventListener("dragend", () => {
            dragging = false;
        });

        this.addEventListener("drag", (evt: THREE.Event) => {
            if (dragging) {
                deltaIn
                    .copy(evt.point)
                    .sub(start);

                start.add(deltaIn);

                P.copy(sel)
                    .divide(this.parent.parent.scale)
                    .applyMatrix4(this.parent.matrixWorld)
                    .multiplyScalar(P.dot(deltaIn));

                delta.copy(P);
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
