import { isArray } from "@juniper-lib/tslib/typeChecks";
import { Color, MeshBasicMaterial } from "three";
import { Cube } from "../../Cube";
import { BaseEnvironment } from "../../environment/BaseEnvironment";
import { solid } from "../../materials";
import { objectIsVisible, objectSetVisible } from "../../objects";
import { isMesh } from "../../typeChecks";
import { BaseCursor } from "./BaseCursor";

export class CursorColor extends BaseCursor {
    private _currentStyle: string;
    private material: MeshBasicMaterial;

    constructor(env: BaseEnvironment) {
        super(env);
        this.material = solid({
            name: "CursorMat",
            color: 0xffff00
        });
        this.object = new Cube(0.01, 0.01, 0.01, this.material);
    }

    get position() {
        return this.object.position;
    }

    override get style() {
        return this._currentStyle;
    }

    override set style(v) {
        this._currentStyle = v;
        if (isMesh(this.object)
            && !isArray(this.object.material)) {
            switch (this._currentStyle) {
                case "pointer":
                    this.material.color = new Color(0x00ff00);
                    this.material.needsUpdate = true;
                    break;
                case "not-allowed":
                    this.material.color = new Color(0xff0000);
                    this.material.needsUpdate = true;
                    break;
                case "move":
                    this.material.color = new Color(0x0000ff);
                    this.material.needsUpdate = true;
                    break;
                case "grab":
                    this.material.color = new Color(0xff00ff);
                    this.material.needsUpdate = true;
                    break;
                case "grabbing":
                    this.material.color = new Color(0x00ffff);
                    this.material.needsUpdate = true;
                    break;
                default:
                    this._currentStyle = "default";
                    this.material.color = new Color(0xffff00);
                    this.material.needsUpdate = true;
                    break;
            }
        }
    }

    override get visible() {
        return objectIsVisible(this);
    }

    override set visible(v) {
        objectSetVisible(this, v);
    }
}