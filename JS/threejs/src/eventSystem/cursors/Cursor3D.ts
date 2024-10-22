import { arrayScan } from "@juniper-lib/collections/dist/arrays";
import { Object3D } from "three";
import { BaseEnvironment } from "../../environment/BaseEnvironment";
import { deepEnableLayer, PURGATORY } from "../../layers";
import { obj, objectIsVisible, objectSetVisible, objGraph } from "../../objects";
import { BaseCursor3D } from "./BaseCursor3D";
import { CursorSystem } from "./CursorSystem";

export class Cursor3D extends BaseCursor3D {

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

    override get style() {
        for (const child of this.object.children) {
            if (child.visible) {
                return child.name as CssCursorValue;
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

    clone() {
        const obj = new Cursor3D(this.env);
        for (const child of this.object.children) {
            obj.add(child.name, child.clone());
        }
        return obj;
    }
}