import { TypedEvent, TypedEventBase } from "@juniper-lib/tslib/events/EventBase";
import { isDefined } from "@juniper-lib/tslib/typeChecks";
import { ColorRepresentation, Object3D, Quaternion, Vector3 } from "three";
import { Cone } from "./Cone";
import { Cube } from "./Cube";
import { BaseEnvironment } from "./environment/BaseEnvironment";
import { VirtualButton } from "./eventSystem/devices/VirtualButton";
import { RayTarget } from "./eventSystem/RayTarget";
import { blue, green, lit, red } from "./materials";
import { ErsatzObject, obj, objectResolve, Objects, objectSetVisible } from "./objects";
import { Sphere } from "./Sphere";
import { Torus } from "./Torus";

export enum TransformMode {
    Move = "Move",
    MoveGlobal = "Global Move",
    Orbit = "Orbit",
    Rotate = "Rotate",
    RotateGlobal = "Global Rotate",
    Resize = "Resize"
}

type Axis = "x" | "y" | "z";
const Axes: readonly Axis[] = ["x", "y", "z"];

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
    private readonly testObj = new Object3D();

    private _mode: TransformMode = null;
    private _target: Object3D = null;

    constructor(private readonly env: BaseEnvironment) {
        super();

        this.object = obj("Translator",
            ...this.translators = [
                this.setTranslator("x", red),
                this.setTranslator("y", green),
                this.setTranslator("z", blue)
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

    private setTranslator(axis: Axis, color: ColorRepresentation): Translator {
        const translator = new Translator(axis, color);
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

                    if (this.mode === TransformMode.Resize) {
                        this.target.getWorldPosition(this.targetWorldPos);
                        const startDist = this.startWorld.distanceTo(this.targetWorldPos);
                        const endDist = this.endWorld.distanceTo(this.targetWorldPos);
                        this.target.scale.addScalar(endDist - startDist);
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
                                this.target.quaternion.premultiply(this.deltaQuaternion);
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

                            if (this.mode === TransformMode.Orbit) {
                                this.target.parent.add(this.testObj);
                                this.testObj.position.copy(this.target.position);
                                this.testObj.lookAt(this.env.avatar.worldPos);
                                this.testObj.attach(this.target);
                                this.testObj.position.add(this.deltaPosition);
                                this.testObj.lookAt(this.env.avatar.worldPos);
                                this.testObj.parent.attach(this.target);
                                this.testObj.removeFromParent();
                            }
                            else {
                                this.target.position.add(this.deltaPosition);
                            }
                        }
                    }

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

export class Translator extends RayTarget<void> {
    private static readonly small = new Vector3(0.1, 0.1, 0.1);
    private readonly bar: Cube;
    private readonly spherePads: Sphere[];
    private readonly conePads: Cone[];
    private readonly arcPad: Torus;
    readonly motionAxisLocal: Vector3;
    readonly rotationAxisLocal: Vector3;

    private _mode: TransformMode = null;

    constructor(axis: Axis, color: ColorRepresentation) {

        const axisIndex = Axes.indexOf(axis);
        const rotationAxisIndex = (axisIndex + 2) % Axes.length;
        const rotationAxis = Axes[rotationAxisIndex];
        const ringAxisIndex = Axes.length - axisIndex - 1;
        const ringAxis = Axes[ringAxisIndex];
        const material = lit({
            color,
            //depthTest: false
        });

        const bar = new Cube(1, 1, 1, material);
        const spherePads = [
            new Sphere(1, material),
            new Sphere(1, material)
        ];
        const conePads = [
            new Cone(1, 1, 1, material),
            new Cone(1, 1, 1, material)
        ];
        const arcPad = new Torus(.5, .5, .5, material);

        super(obj(
            "Translator " + axis,
            bar,
            ...spherePads,
            ...conePads,
            arcPad
        ));

        this.bar = bar;
        this.spherePads = spherePads;
        this.conePads = conePads;
        this.arcPad = arcPad;

        this.motionAxisLocal = new Vector3();
        this.motionAxisLocal[axis] = 1;
        this.rotationAxisLocal = new Vector3();
        this.rotationAxisLocal[rotationAxis] = 1;

        this.enabled = true;
        this.draggable = true;

        for (let i = 0; i < this.spherePads.length; ++i) {
            const dir = 2 * i - 1;
            this.spherePads[i].scale.setScalar(1 / 10);
            this.spherePads[i].position
                .copy(this.motionAxisLocal)
                .multiplyScalar(dir * 0.375);
        }

        const V = new Vector3();
        for (let i = 0; i < this.conePads.length; ++i) {
            const dir = 2 * i - 1;
            V.copy(this.conePads[i].up).multiplyScalar(dir);
            this.conePads[i].quaternion
                .setFromUnitVectors(V, this.motionAxisLocal);
            this.conePads[i].position
                .copy(this.motionAxisLocal)
                .multiplyScalar(dir * 0.375);
            this.conePads[i].scale.set(1 / 20, 1 / 10, 1 / 20);
        }

        this.bar.scale
            .copy(this.motionAxisLocal)
            .multiplyScalar(2.5)
            .add(Translator.small)
            .multiplyScalar(0.25);

        const ringRotAxis = new Vector3();
        ringRotAxis[ringAxis] = 1;
        this.arcPad.quaternion
            .setFromAxisAngle(ringRotAxis, Math.PI / 2);

        this.addMeshes(
            ...this.spherePads,
            ...this.conePads,
            this.arcPad
        );
    }

    get mode() {
        return this._mode;
    }

    set mode(v) {
        if (v !== this.mode) {
            this._mode = v;

            this.bar.visible = this.mode !== TransformMode.Rotate
                && this.mode !== TransformMode.RotateGlobal;
            this.arcPad.visible = this.mode === TransformMode.Rotate
                || this.mode === TransformMode.RotateGlobal;

            for (const spherePad of this.spherePads) {
                spherePad.visible = this.mode === TransformMode.Resize;
            }

            for (const conePad of this.conePads) {
                conePad.visible = this.mode !== TransformMode.Resize
                    && this.mode !== TransformMode.Rotate
                    && this.mode !== TransformMode.RotateGlobal;
            }
        }
    }
}
