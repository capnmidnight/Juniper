import { Vector3 } from "three";
import { BaseEnvironment } from "../../environment/BaseEnvironment";
import { objectSetVisible } from "../../objects";
import { BaseCursor3D } from "./BaseCursor3D";
import { CursorColor } from "./CursorColor";
import { CursorSystem } from "./CursorSystem";

export class CursorXRMouse extends BaseCursor3D {
    private readonly system: CursorSystem;
    private xr: BaseCursor3D;

    constructor(env: BaseEnvironment) {
        super(env);

        this.xr = env.cursor3D && env.cursor3D.clone() || new CursorColor(this.env);
        this.system = new CursorSystem(this.env.renderer.domElement);
        this.xr.side = this.side;
        this.visible = false;

        Object.seal(this);
    }

    override get content3d() {
        return this.xr.content3d;
    }

    override get side() {
        return this.xr.side;
    }

    override set side(v) {
        this.xr.side = v;
    }

    get cursor() {
        return this.xr;
    }

    set cursor(v) {
        this.xr = v;
        this._refresh();
    }

    override get style() {
        return this.system.style;
    }

    override get visible() {
        return super.visible;
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
        const isPointerLocked = this.env.eventSys
            && this.env.eventSys.mouse
            && this.env.eventSys.mouse.isPointerLocked;

        const showXR = this.env.renderer.xr.isPresenting
            || isPointerLocked;

        objectSetVisible(this.xr, this.visible && showXR);
        this.system.visible = this.visible && !showXR;
    }

    override lookAt(p: Vector3, v: Vector3) {
        this.xr.lookAt(p, v);
    }
}