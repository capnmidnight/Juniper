import { arrayScan } from "@juniper-lib/collections/arrays";
import { deepEnableLayer, PURGATORY } from "../../layers";
import { obj, objectIsVisible, objectSetVisible, objGraph } from "../../objects";
import { BaseCursor3D } from "./BaseCursor3D";
export class Cursor3D extends BaseCursor3D {
    constructor(env, cursorSystem) {
        super(env);
        this.cursorSystem = null;
        this.object = obj("Cursor3D");
        this.cursorSystem = cursorSystem;
        this.object.matrixAutoUpdate = false;
    }
    add(name, obj) {
        objGraph(this, obj);
        deepEnableLayer(obj, PURGATORY);
        obj.visible = name === "default";
    }
    get style() {
        for (const child of this.object.children) {
            if (child.visible) {
                return child.name;
            }
        }
        return null;
    }
    set style(v) {
        for (const child of this.object.children) {
            child.visible = child.name === v;
        }
        if (this.style == null && this.object.children.length > 0) {
            const defaultCursor = arrayScan(this.object.children, (child) => child.name === "default", (child) => child != null);
            if (defaultCursor != null) {
                defaultCursor.visible = true;
            }
        }
        if (this.cursorSystem) {
            this.cursorSystem.style = "none";
        }
    }
    get visible() {
        return objectIsVisible(this);
    }
    set visible(v) {
        objectSetVisible(this, v);
    }
    clone() {
        const obj = new Cursor3D(this.env);
        for (const child of this.object.children) {
            obj.add(child.name, child.clone());
        }
        return obj;
    }
}
//# sourceMappingURL=Cursor3D.js.map