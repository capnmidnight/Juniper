import { arrayScan } from "@juniper/collections";
import { deepEnableLayer, PURGATORY } from "../layers";
import { ErsatzObject, objectIsVisible, objectSetVisible } from "../objects";
import { BaseCursor } from "./BaseCursor";
import { CursorSystem } from "./CursorSystem";

export class Cursor3D
    extends BaseCursor
    implements ErsatzObject {

    private readonly cursorSystem: CursorSystem = null;

    constructor(cursorSystem?: CursorSystem) {
        super();
        this.object = new THREE.Object3D();
        this.cursorSystem = cursorSystem;
    }

    add(name: string, obj: THREE.Object3D) {
        this.object.add(obj);
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

    override lookAt(v: THREE.Vector3) {
        this.object.lookAt(v);
    }

    clone() {
        const obj = new Cursor3D();
        for (const child of this.object.children) {
            obj.add(child.name, child.clone());
        }
        return obj;
    }
}