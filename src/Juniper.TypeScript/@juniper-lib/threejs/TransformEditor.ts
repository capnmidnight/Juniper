import { TypedEvent, TypedEventBase } from "@juniper-lib/tslib/events/EventBase";
import { deg2rad } from "@juniper-lib/tslib/math";
import { isDefined } from "@juniper-lib/tslib/typeChecks";
import { ColorRepresentation, ExtrudeBufferGeometry, Material, Mesh, Object3D, Quaternion, Shape, Vector2, Vector3 } from "three";
import { Cone } from "./Cone";
import { Cube } from "./Cube";
import { BaseEnvironment } from "./environment/BaseEnvironment";
import { VirtualButton } from "./eventSystem/devices/VirtualButton";
import { RayTarget } from "./eventSystem/RayTarget";
import { blue, green, litTransparent, red } from "./materials";
import { ErsatzObject, obj, objectResolve, Objects, objectSetVisible } from "./objects";
import { Sphere } from "./Sphere";

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
const size = 0.1;

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
                    this.object.worldToLocal(this.startLocal.copy(this.startWorld));
                    this.object.worldToLocal(this.endLocal.copy(this.endWorld));

                    if (this.mode === TransformMode.Resize) {
                        const startDist = this.startLocal.length();
                        const endDist = this.endLocal.length();
                        this.target.scale.addScalar(endDist - startDist);
                    }
                    else if (this.mode === TransformMode.Rotate
                        || this.mode === TransformMode.RotateGlobal) {
                        this.startLocal.normalize();
                        this.endLocal.normalize();

                        const mag = this.startLocal.dot(this.endLocal);
                        if (-1 <= mag && mag <= 1) {
                            const sign = this.startLocal.cross(this.endLocal).dot(translator.rotationAxisLocal);
                            const radians = Math.sign(sign) * Math.acos(mag);
                            this.rotationAxisWorld
                                .copy(translator.rotationAxisLocal)
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

            for (const translator of this.translators) {
                translator.refresh(this.env.avatar.worldPos);
            }
        }
    }
}

const arcPointsForward = new Array<Vector2>();
const arcPointsBack = new Array<Vector2>();
for (let a = 5; a <= 85; a += 5) {
    const rad = deg2rad(a);
    arcPointsForward.push(new Vector2(0.5 * Math.cos(rad), 0.5 * Math.sin(rad)));
    arcPointsBack.unshift(new Vector2(0.48 * Math.cos(rad), 0.48 * Math.sin(rad)));
}
const arcPoints = [...arcPointsForward, ...arcPointsBack];
const arcShape = new Shape(arcPoints);
const arcGeom = new ExtrudeBufferGeometry(arcShape, {
    steps: 1,
    depth: 0.02,
    bevelEnabled: false
});
arcGeom.computeBoundingBox();
arcGeom.computeBoundingSphere();

export class Translator extends RayTarget<void> {
    private static readonly small = new Vector3(0.1, 0.1, 0.1);
    private readonly bars: Mesh[];
    private readonly spherePads: Mesh[];
    private readonly conePads: Mesh[];
    private readonly arcPads: Mesh[];
    private readonly materialFront: Material;
    private readonly materialBack: Material;
    private readonly worldPos = new Vector3();
    private readonly worldQuat = new Quaternion();
    private readonly center = new Vector3();

    readonly motionAxisLocal = new Vector3();
    readonly rotationAxisLocal = new Vector3();

    private _mode: TransformMode = null;

    constructor(axis: Axis, color: ColorRepresentation) {
        const axisIndex = Axes.indexOf(axis);
        const rotationAxisIndex = (axisIndex + 2) % Axes.length;
        const rotationAxis = Axes[rotationAxisIndex];
        const ringAxisIndex = Axes.length - axisIndex - 1;
        const ringAxis = Axes[ringAxisIndex];
        const materialFront = litTransparent({
            color,
            depthTest: false,
            opacity: 0.75
        });
        const materialBack = litTransparent({
            color,
            depthTest: false,
            opacity: 0.25
        });
        const bars = [
            new Cube(1, 1, 1, materialFront),
            new Cube(1, 1, 1, materialFront)
        ];
        const spherePads = [
            new Sphere(1, materialFront),
            new Sphere(1, materialFront)
        ];
        const conePads = [
            new Cone(1, 1, 1, materialFront),
            new Cone(1, 1, 1, materialFront)
        ];
        const arcPads = [
            new Mesh(arcGeom, materialFront),
            new Mesh(arcGeom, materialFront),
            new Mesh(arcGeom, materialFront),
            new Mesh(arcGeom, materialFront)
        ];

        super(obj(
            "Translator " + axis,
            ...bars,
            ...spherePads,
            ...conePads,
            ...arcPads
        ));

        this.bars = bars;
        this.spherePads = spherePads;
        this.conePads = conePads;
        this.arcPads = arcPads;

        this.motionAxisLocal[axis] = 1;
        this.rotationAxisLocal[rotationAxis] = 1;

        this.materialFront = materialFront;
        this.materialBack = materialBack;

        this.enabled = true;
        this.draggable = true;

        for (let i = 0; i < this.spherePads.length; ++i) {
            const dir = 2 * i - 1;
            this.spherePads[i].scale.setScalar(size * 0.4);
            this.spherePads[i].position
                .copy(this.motionAxisLocal)
                .multiplyScalar(dir * size * 1.5);
        }

        const V = new Vector3();
        for (let i = 0; i < this.conePads.length; ++i) {
            const dir = 2 * i - 1;
            V.copy(this.conePads[i].up).multiplyScalar(dir);
            this.conePads[i].quaternion
                .setFromUnitVectors(V, this.motionAxisLocal);
            this.conePads[i].position
                .copy(this.motionAxisLocal)
                .multiplyScalar(dir * size * 1.5);
            this.conePads[i].scale
                .set(.2, .4, .2)
                .multiplyScalar(size);
        }

        for (let i = 0; i < this.bars.length; ++i) {
            const dir = 2 * i - 1;
            this.bars[i].scale
                .copy(this.motionAxisLocal)
                .multiplyScalar(1.25)
                .add(Translator.small)
                .multiplyScalar(size);
            this.bars[i].position
                .copy(this.motionAxisLocal)
                .multiplyScalar(dir * size * 0.8);
        }

        const Q = new Quaternion();
        const Z = new Vector3(0, 0, -1);
        const ringRotAxis = new Vector3();
        ringRotAxis[ringAxis] = 1;


        for (let i = 0; i < this.arcPads.length; ++i) {
            const a = i * Math.PI / 2;
            Q.setFromAxisAngle(Z, a)
            this.arcPads[i].quaternion
                .setFromAxisAngle(ringRotAxis, Math.PI / 2)
                .multiply(Q);
            this.arcPads[i].scale
                .setScalar(size * 4);
        }

        this.addMeshes(
            ...this.spherePads,
            ...this.conePads,
            ...this.arcPads
        );
    }

    get mode() {
        return this._mode;
    }

    set mode(v) {
        if (v !== this.mode) {
            this._mode = v;

            for (const arcPad of this.arcPads) {
                arcPad.visible = this.mode === TransformMode.Rotate
                    || this.mode === TransformMode.RotateGlobal;
            }

            for (const bar of this.bars) {
                bar.visible = this.mode !== TransformMode.Rotate
                    && this.mode !== TransformMode.RotateGlobal;
            }
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

    refresh(center: Vector3) {
        const distA = this.object.parent.position.distanceToSquared(center);
        this.checkMeshes(center, distA, this.bars);
        this.checkMeshes(center, distA, this.spherePads);
        this.checkMeshes(center, distA, this.conePads);
        this.checkMeshes(center, distA, this.arcPads);
    }

    private checkMeshes(center: Vector3, distA: number, arr: Mesh[]) {
        for (const pad of arr) {
            pad.getWorldPosition(this.worldPos);
            pad.getWorldQuaternion(this.worldQuat);
            pad.geometry.boundingBox.getCenter(this.center);
            this.center.add(pad.position);
            pad.localToWorld(this.center);
            const distB = this.center.distanceToSquared(center);
            pad.material = distB >= distA ? this.materialBack : this.materialFront;
        }
    }
}
