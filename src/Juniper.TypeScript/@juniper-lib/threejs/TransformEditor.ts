import { TypedEvent, TypedEventBase } from "@juniper-lib/tslib/events/EventBase";
import { assertNever } from "@juniper-lib/tslib/typeChecks";
import { ColorRepresentation, Object3D, Quaternion, Vector3 } from "three";
import { blue, green, red } from "./materials";
import { ErsatzObject, obj, objectIsVisible, objectSetVisible } from "./objects";
import { Translator } from "./Translator";

function v(v: Vector3) {
    return `<${v.toArray().map(v => v.toFixed(4)).join(",")}>\n`;
}

interface TransformEditorEvents {
    moving: TypedEvent<"moving">;
    moved: TypedEvent<"moved">;
    freeze: TypedEvent<"freeze">;
    unfreeze: TypedEvent<"unfreeze">;
}

export enum TransformEditorMode {
    Move = "Move",
    Orbit = "Orbit",
    Rotate = "Rotate",
    Resize = "Resize"
}

export class TransformEditor
    extends TypedEventBase<TransformEditorEvents>
    implements ErsatzObject {
    readonly object: Object3D;

    private translators: Translator[];

    private _size: number = 1;

    private readonly movingEvt = new TypedEvent("moving");
    private readonly movedEvt = new TypedEvent("moved");
    private readonly freezeEvt = new TypedEvent("freeze");
    private readonly unfreezeEvt = new TypedEvent("unfreeze");

    private _mode: TransformEditorMode = null;

    constructor(mode: TransformEditorMode, defaultAvatarHeight: number) {
        super();

        this.object = obj("Translator",
            ...this.translators = [
                this.setTranslator("+X", 1, 0, 0, red, defaultAvatarHeight, mode),
                this.setTranslator("-X", -1, 0, 0, red, defaultAvatarHeight, mode),
                this.setTranslator("+Y", 0, 1, 0, green, defaultAvatarHeight, mode),
                this.setTranslator("-Y", 0, -1, 0, green, defaultAvatarHeight, mode),
                this.setTranslator("+Z", 0, 0, 1, blue, defaultAvatarHeight, mode),
                this.setTranslator("-Z", 0, 0, -1, blue, defaultAvatarHeight, mode)
            ]
        );

        this.mode = mode;
    }

    get mode() {
        return this._mode;
    }

    set mode(v) {
        if (v !== this.mode) {
            this._mode = v;
            for (const translator of this.translators) {
                translator.mode = v;
            }
        }
    }

    get size(): number {
        return this._size;
    }

    set size(v: number) {
        this._size = v;
        for (const translator of this.translators) {
            translator.size = this.size * 0.5;
        }
    }

    private readonly p = new Vector3();    
    private readonly start = new Vector3();
    private readonly end = new Vector3();
    private readonly up = new Vector3();
    private readonly q = new Quaternion();

    private setTranslator(name: string, sx: number, sy: number, sz: number, color: ColorRepresentation, defaultAvatarHeight: number, mode: TransformEditorMode): Translator {
        const translator = new Translator(name, sx, sy, sz, color, mode);
        translator.size = this.size * 0.5;
        translator.addEventListener("dragdir", (evt) => {
            this.object.parent.position.y -= defaultAvatarHeight;

            const dist = this.object.parent.position.length();

            this.start
                .copy(this.object.parent.position)
                .sub(this.p)
                .normalize();

            if (this.mode === TransformEditorMode.Orbit
                || this.mode === TransformEditorMode.Move) {

                this.start
                    .copy(this.object.parent.position)
                    .sub(this.p)
                    .normalize();

                this.object
                    .parent
                    .position
                    .add(evt.delta);

                if (this.mode === TransformEditorMode.Orbit) {
                    this.object.parent.parent.getWorldPosition(this.p);

                    this.object.parent.position
                        .normalize()
                        .multiplyScalar(dist);

                    this.end
                        .copy(this.object.parent.position)
                        .sub(this.p)
                        .normalize();

                    const d = this.start.dot(this.end);
                    if (-1 <= d && d <= 1) {
                        const a = Math.acos(d);
                        this.up.crossVectors(this.start, this.end).normalize();
                        this.q.setFromAxisAngle(this.up, a);
                        this.object.parent.quaternion.premultiply(this.q);
                    }
                }
            }
            else if (this.mode === TransformEditorMode.Resize) {
                this.object.parent.scale.addScalar(evt.magnitude);
                this.size = 1 / this.object.parent.scale.x;
            }
            else if (this.mode === TransformEditorMode.Rotate) {

            }
            else {
                assertNever(this.mode);
            }

            this.object.parent.position.y += defaultAvatarHeight;

            this.dispatchEvent(this.movingEvt);
        });
        
        translator.addEventListener("dragstart", () => {
            if (this.mode !== TransformEditorMode.Move
                && this.mode !== TransformEditorMode.Orbit) {
                this.dispatchEvent(this.freezeEvt);
            }
        });

        translator.addEventListener("dragend", () => {
            if (this.mode !== TransformEditorMode.Move
                && this.mode !== TransformEditorMode.Orbit) {
                this.dispatchEvent(this.unfreezeEvt);
            }

            this.dispatchEvent(this.movedEvt);
        });

        return translator;
    }

    get visible() {
        return objectIsVisible(this);
    }

    set visible(v) {
        objectSetVisible(this, v);
    }
}
