import { arrayScan } from "@juniper-lib/tslib/collections/arrayScan";
import { Object3D, Vector3 } from "three";
import { BaseEnvironment } from "../../environment/BaseEnvironment";
import { deepEnableLayer, PURGATORY } from "../../layers";
import { ErsatzObject, obj, objectIsVisible, objectSetVisible, objGraph } from "../../objects";
import { setMatrixFromUpFwdPos } from "../../setMatrixFromUpFwdPos";
import { BaseCursor } from "./BaseCursor";
import { CursorSystem } from "./CursorSystem";

export class Cursor3D
    extends BaseCursor
    implements ErsatzObject {

    private readonly cursorSystem: CursorSystem = null;

    constructor(env: BaseEnvironment, cursorSystem?: CursorSystem) {
        super(env);
        this.object = obj("Cursor3D");
        this.cursorSystem = cursorSystem;
        this.object.matrixAutoUpdate = false;
    }

    add(name: string, obj: Object3D) {
        objGraph(this, obj);
        deepEnableLayer(obj, PURGATORY);
        obj.visible = name === "default";
    }

    get position() {
        return this.object.position;
    }

    override get style() {
        for (const child of this.object.children) {
            if (child.visible) {
                return child.name;
            }
        }

        return null;
    }

    override set style(v) {
        for (const child of this.object.children) {
            child.visible = child.name === v;
        }

        if (this.style == null && this.object.children.length > 0) {
            const defaultCursor = arrayScan(this.object.children,
                (child) => child.name === "default",
                (child) => child != null);
            if (defaultCursor != null) {
                defaultCursor.visible = true;
            }
        }

        if (this.cursorSystem) {
            this.cursorSystem.style = "none";
        }
    }

    override get visible() {
        return objectIsVisible(this);
    }

    override set visible(v) {
        objectSetVisible(this, v);
    }

    private readonly f = new Vector3();
    private readonly up = new Vector3();
    private readonly right = new Vector3();

    override lookAt(p: Vector3, v: Vector3) {
        this.f
            .copy(v)
            .sub(p)
            .multiplyScalar(this.side)
            .normalize();

        this.up
            .set(0, 1, 0)
            .applyQuaternion(this.env.avatar.worldQuat);

        this.right.crossVectors(this.up, this.f);
        this.up.crossVectors(this.f, this.right);

        setMatrixFromUpFwdPos(this.up, this.f, p, this.object.matrixWorld);
        this.object.matrix
            .copy(this.object.parent.matrixWorld)
            .invert()
            .multiply(this.object.matrixWorld);

        this.object.matrix
            .decompose(this.object.position, this.object.quaternion, this.object.scale);
    }

    clone() {
        const obj = new Cursor3D(this.env);
        for (const child of this.object.children) {
            obj.add(child.name, child.clone());
        }
        return obj;
    }
}