import { objectSetVisible } from "../objects";
import { BaseCursor } from "./BaseCursor";
import { CursorColor } from "./CursorColor";
import { CursorSystem } from "./CursorSystem";

export class CursorXRMouse extends BaseCursor {
    private readonly system: CursorSystem;
    private xr: BaseCursor = new CursorColor();

    constructor(private readonly renderer: THREE.WebGLRenderer) {
        super();

        this.system = new CursorSystem(this.renderer.domElement);
        this.visible = false;
    }

    override get object() {
        return this.xr.object;
    }

    get cursor() {
        return this.xr;
    }

    set cursor(v) {
        this.xr = v;
        this._refresh();
    }

    get position() {
        return this.object.position;
    }

    override get style() {
        return this.system.style;
    }

    override get visible() {
        return this.renderer.xr.isPresenting
            && this.xr.visible
            || !this.renderer.xr.isPresenting
            && this.system.visible;
    }

    override set visible(v) {
        super.visible = v;
        this._refresh();
    }

    override set style(v) {
        this.system.style = v;
        this.xr.style = v;
        this._refresh();
    }

    _refresh() {
        objectSetVisible(this.xr, this.visible
            && (this.renderer.xr.isPresenting
                || document.pointerLockElement != null));
    }

    override lookAt(v: THREE.Vector3) {
        this.xr.lookAt(v);
    }
}