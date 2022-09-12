import { TypedEvent, TypedEventBase } from "@juniper-lib/tslib/events/EventBase";
import { assertNever, isDefined } from "@juniper-lib/tslib/typeChecks";
import { ColorRepresentation, Object3D, Quaternion, Vector3 } from "three";
import { BaseEnvironment } from "./environment/BaseEnvironment";
import { blue, green, red } from "./materials";
import { ErsatzObject, obj, objectResolve, Objects, objectSetVisible } from "./objects";
import { SignedAxis, Translator } from "./Translator";

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

    private readonly translators: Translator[];
    private readonly p = new Vector3();
    private readonly start = new Vector3();
    private readonly end = new Vector3();
    private readonly up = new Vector3();
    private readonly q = new Quaternion();
    private readonly movingEvt = new TypedEvent("moving");
    private readonly movedEvt = new TypedEvent("moved");
    private readonly freezeEvt = new TypedEvent("freeze");
    private readonly unfreezeEvt = new TypedEvent("unfreeze");

    private _mode: TransformEditorMode = null;
    private _target: Object3D = null;

    constructor(private readonly env: BaseEnvironment, private readonly defaultAvatarHeight: number) {
        super();

        this.object = obj("Translator",
            ...this.translators = [
                this.setTranslator("+x", red),
                this.setTranslator("+y", green),
                this.setTranslator("+z", blue)
            ]
        );

        objectSetVisible(this, false);

        env.timer.addTickHandler(() => this.refresh());
    }

    get target(): Object3D {
        return this._target;
    }

    setTarget(v: Objects, mode?: TransformEditorMode) {
        v = objectResolve(v);
        if (v !== this.target) {
            this._target = v;
            objectSetVisible(this, isDefined(this.target));
            this.refresh();
        }

        if (isDefined(v) && isDefined(mode)) {
            this.mode = mode;
        }
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

            this.translators[2].object.visible
                = this.mode === TransformEditorMode.Rotate
                || this.mode === TransformEditorMode.Move;

            this.refresh();
        }
    }

    private setTranslator(name: SignedAxis, color: ColorRepresentation
    ): Translator {
        const translator = new Translator(name, color);
        translator.addEventListener("dragdir", (evt) => {
            const dist = this.target.position.length();

            this.target.parent.getWorldPosition(this.p);
            this.p.y += this.defaultAvatarHeight;

            this.start
                .copy(this.target.position)
                .sub(this.p)
                .normalize();

            if (this.mode === TransformEditorMode.Orbit
                || this.mode === TransformEditorMode.Move) {

                this.start
                    .copy(this.target.position)
                    .sub(this.p)
                    .normalize();

                this.target
                    .position
                    .add(evt.deltaPosition);

                if (this.mode === TransformEditorMode.Orbit) {

                    this.target.position
                        .normalize()
                        .multiplyScalar(dist);

                    this.end
                        .copy(this.target.position)
                        .sub(this.p)
                        .normalize();

                    const d = this.start.dot(this.end);
                    if (-1 <= d && d <= 1) {
                        const a = Math.acos(d);
                        this.up.crossVectors(this.start, this.end).normalize();
                        this.q.setFromAxisAngle(this.up, a);
                        this.target.quaternion.premultiply(this.q);
                    }
                }
            }
            else if (this.mode === TransformEditorMode.Resize) {
                this.target.scale.addScalar(evt.magnitude);
            }
            else if (this.mode === TransformEditorMode.Rotate) {
                this.target.quaternion.multiply(evt.deltaRotation);
            }
            else {
                assertNever(this.mode);
            }
            this.refresh();
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

    refresh() {
        if (this.target) {
            this.target.getWorldPosition(this.object.position);

            if (this.mode === TransformEditorMode.Move
                || this.mode === TransformEditorMode.Rotate
            ) {
                this.target.getWorldQuaternion(this.object.quaternion);
            }
            else {
                this.object.lookAt(this.env.avatar.worldPos);
            }
        }
    }
}
