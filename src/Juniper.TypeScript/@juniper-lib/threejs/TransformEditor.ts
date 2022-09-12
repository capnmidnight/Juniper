import { TypedEvent, TypedEventBase } from "@juniper-lib/tslib/events/EventBase";
import { isDefined } from "@juniper-lib/tslib/typeChecks";
import { ColorRepresentation, Object3D, Quaternion, Vector3 } from "three";
import { BaseEnvironment } from "./environment/BaseEnvironment";
import { VirtualButton } from "./eventSystem/devices/VirtualButton";
import { blue, green, red } from "./materials";
import { ErsatzObject, obj, objectResolve, Objects, objectSetVisible } from "./objects";
import { SignedAxis, TransformMode, Translator } from "./Translator";

interface TransformEditorEvents {
    moving: TypedEvent<"moving">;
    moved: TypedEvent<"moved">;
    freeze: TypedEvent<"freeze">;
    unfreeze: TypedEvent<"unfreeze">;
}

export class TransformEditor
    extends TypedEventBase<TransformEditorEvents>
    implements ErsatzObject {

    readonly object: Object3D;

    private readonly translators: Translator[];
    private readonly movingEvt = new TypedEvent("moving");
    private readonly movedEvt = new TypedEvent("moved");
    private readonly freezeEvt = new TypedEvent("freeze");
    private readonly unfreezeEvt = new TypedEvent("unfreeze");

    private dragging = false;
    private readonly rotationAxisWorld = new Vector3();
    private readonly startWorld = new Vector3();
    private readonly endWorld = new Vector3();
    private readonly startLocal = new Vector3();
    private readonly endLocal = new Vector3();
    private readonly targetWorldPos = new Vector3();
    private readonly deltaPosition = new Vector3();
    private readonly deltaQuaternion = new Quaternion();
    private deltaScale = 0;

    private _mode: TransformMode = null;
    private _target: Object3D = null;

    constructor(private readonly env: BaseEnvironment) {
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

    setTarget(v: Objects, mode?: TransformMode) {
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

            this.translators[2].object.visible = this.mode === TransformMode.Rotate
                || this.mode === TransformMode.RotateGlobal
                || this.mode === TransformMode.Move
                || this.mode === TransformMode.MoveGlobal;

            this.refresh();
        }
    }

    private setTranslator(name: SignedAxis, color: ColorRepresentation): Translator {
        const translator = new Translator(name, color);
        translator.addEventListener("down", (evt) => {
            if (evt.pointer.isPressed(VirtualButton.Primary)) {
                this.dragging = true;
                this.startWorld.copy(evt.point);
                if (this.mode !== TransformMode.Move
                    && this.mode !== TransformMode.MoveGlobal
                    && this.mode !== TransformMode.Orbit) {
                    this.dispatchEvent(this.freezeEvt);
                }
            }
        });

        translator.addEventListener("move", (evt) => {
            if (this.dragging && evt.point) {
                this.endWorld.copy(evt.point);

                if (this.startWorld.manhattanDistanceTo(this.endWorld) > 0) {

                    this.deltaPosition.setScalar(0);
                    this.deltaQuaternion.identity();
                    this.deltaScale = 0;

                    if (this.mode === TransformMode.Resize) {
                        this.target.getWorldPosition(this.targetWorldPos);
                        const startDist = this.startWorld.distanceTo(this.targetWorldPos);
                        const endDist = this.endWorld.distanceTo(this.targetWorldPos);
                        this.deltaScale = endDist - startDist;
                    }
                    else {
                        this.object.worldToLocal(this.startLocal.copy(this.startWorld));
                        this.object.worldToLocal(this.endLocal.copy(this.endWorld));

                        if (this.mode === TransformMode.Rotate
                            || this.mode === TransformMode.RotateGlobal) {
                            this.startLocal.normalize();
                            this.endLocal.normalize();

                            const mag = this.startLocal.dot(this.endLocal);
                            if (-1 <= mag && mag <= 1) {
                                const sign = this.startLocal.cross(this.endLocal).dot(translator.rotationAxisLocal);
                                const radians = Math.sign(sign) * Math.acos(mag);
                                this.rotationAxisWorld.copy(translator.rotationAxisLocal)
                                    .applyQuaternion(this.object.quaternion);
                                this.deltaQuaternion
                                    .setFromAxisAngle(this.rotationAxisWorld, radians);
                            }
                        }
                        else {
                            const magnitude = this.endLocal
                                .sub(this.startLocal)
                                .dot(translator.motionAxisLocal);

                            this.deltaPosition
                                .copy(translator.motionAxisLocal)
                                .multiplyScalar(magnitude)
                                .applyQuaternion(this.object.quaternion);
                        }
                    }

                    this.target.position.add(this.deltaPosition);
                    this.target.scale.addScalar(this.deltaScale);
                    this.target.quaternion.premultiply(this.deltaQuaternion);

                    this.refresh();

                    this.dispatchEvent(this.movingEvt);
                }

                this.startWorld.copy(this.endWorld);
            }
        });

        translator.addEventListener("up", (evt) => {
            if (!evt.pointer.isPressed(VirtualButton.Primary)) {
                this.dragging = false;

                if (this.mode !== TransformMode.Move
                    && this.mode !== TransformMode.MoveGlobal
                    && this.mode !== TransformMode.Orbit) {
                    this.dispatchEvent(this.unfreezeEvt);
                }

                this.dispatchEvent(this.movedEvt);
            }
        });

        return translator;
    }

    refresh() {
        if (this.target) {
            this.target.getWorldPosition(this.object.position);

            if (this.mode === TransformMode.Move
                || this.mode === TransformMode.Rotate
            ) {
                this.target.getWorldQuaternion(this.object.quaternion);
            }
            else if (this.mode === TransformMode.RotateGlobal
                || this.mode === TransformMode.MoveGlobal) {
                this.object.quaternion.identity();
            }
            else {
                this.object.lookAt(this.env.avatar.worldPos);
            }
        }
    }
}
