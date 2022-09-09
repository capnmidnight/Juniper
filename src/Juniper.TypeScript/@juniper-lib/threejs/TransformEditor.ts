import { TypedEvent, TypedEventBase } from "@juniper-lib/tslib/events/EventBase";
import { assertNever } from "@juniper-lib/tslib/typeChecks";
import { ColorRepresentation, Object3D, Quaternion, Vector3 } from "three";
import { blue as blu, green as grn, red } from "./materials";
import { ErsatzObject, obj, objectIsVisible, objectSetVisible } from "./objects";
import { Translator } from "./Translator";

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
                this.setTranslator("+X", red, defaultAvatarHeight, mode,
                    +1, +0, +0,
                    +0, +1, +0,
                    +0, +0, +1),
                this.setTranslator("-X", red, defaultAvatarHeight, mode,
                    -1, +0, +0,
                    +0, +1, +0,
                    +0, +0, -1),
                this.setTranslator("+Y", grn, defaultAvatarHeight, mode,
                    +0, +1, +0,
                    +0, +0, -1,
                    -1, +0, +0),
                this.setTranslator("-Y", grn, defaultAvatarHeight, mode,
                    +0, -1, +0,
                    +0, +0, +1,
                    -1, +0, +0),
                this.setTranslator("+Z", blu, defaultAvatarHeight, mode,
                    +0, +0, +1,
                    +0, +1, +0,
                    -1, +0, +0),
                this.setTranslator("-Z", blu, defaultAvatarHeight, mode,
                    +0, +0, -1,
                    +0, -1, +0,
                    +1, +0, +0)
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

    private setTranslator(
        name: string,
        color: ColorRepresentation,
        defaultAvatarHeight: number,
        mode: TransformEditorMode,
        mx: number,
        my: number,
        mz: number,
        rxx: number,
        rxy: number,
        rxz: number,
        ryx: number,
        ryy: number,
        ryz: number
    ): Translator {
        const translator = new Translator(name, mx, my, mz, rxx, rxy, rxz, ryx, ryy, ryz, color, mode);
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
                    .add(evt.deltaPosition);

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
                this.object.parent.quaternion.premultiply(evt.deltaRotation);
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
