import { arrayReplace, deg2rad, HalfPi, isDefined } from "@juniper-lib/util";
import { Button, elementSetDisplay, OnClick } from "@juniper-lib/dom";
import { TypedEvent, TypedEventTarget } from "@juniper-lib/events";
import { ColorRepresentation, Euler, ExtrudeGeometry, Material, Mesh, Object3D, Quaternion, Shape, Vector2, Vector3 } from "three";
import { cone } from "./Cone";
import { cube } from "./Cube";
import { BaseEnvironment } from "./environment/BaseEnvironment";
import { VirtualButton } from "./eventSystem/devices/VirtualButton";
import { IntersectionSortFunction } from "./eventSystem/EventSystem";
import { getRayTarget, RayTarget } from "./eventSystem/RayTarget";
import { blue, green, litTransparent, red } from "./materials";
import { ErsatzObject, mesh, obj, objectResolve, Objects, objectSetVisible } from "./objects";
import { sphere } from "./Sphere";
import { isMesh } from "./typeChecks";

export enum TransformMode {
    None = "None",
    MoveObjectSpace = "Object Move",
    MoveGlobalSpace = "Global Move",
    MoveViewSpace = "View Move",
    Orbit = "Orbit",
    RotateObjectSpace = "Object Rotate",
    RotateGlobalSpace = "Global Rotate",
    RotateViewSpace = "View Rotate",
    Resize = "Resize"
}

const orderedTransformModes = [
    TransformMode.RotateViewSpace,
    TransformMode.MoveViewSpace,
    TransformMode.Resize,
    TransformMode.Orbit,
    TransformMode.RotateObjectSpace,
    TransformMode.MoveObjectSpace,
    TransformMode.RotateGlobalSpace,
    TransformMode.MoveGlobalSpace
];

type Axis = "x" | "y" | "z";
const Axes: readonly Axis[] = ["x", "y", "z"];
const size = 0.1;

const correction = new Map([
    [1, new Quaternion().setFromEuler(new Euler(0, -HalfPi, 0))],
    [-1, new Quaternion().setFromEuler(new Euler(0, HalfPi, 0))]
]);

type TransformEditorEvents = {
    moving: TypedEvent<"moving">;
    moved: TypedEvent<"moved">;
}

export class TransformEditor
    extends TypedEventTarget<TransformEditorEvents>
    implements ErsatzObject {

    readonly content3d: Object3D;

    public readonly modeButtons: HTMLButtonElement[];
    private readonly buttons = new Map<TransformMode, HTMLButtonElement>();

    private readonly translators: Translator[];
    private readonly movingEvt = new TypedEvent("moving");
    private readonly movedEvt = new TypedEvent("moved");

    private dragging = false;
    private readonly rotationAxisWorld = new Vector3();
    private readonly startWorld = new Vector3();
    private readonly endWorld = new Vector3();
    private readonly startLocal = new Vector3();
    private readonly endLocal = new Vector3();
    private readonly lookDirectionWorld = new Vector3();
    private readonly deltaPosition = new Vector3();
    private readonly deltaQuaternion = new Quaternion();
    private readonly motionAxisWorld = new Vector3();
    private readonly testObj = new Object3D();

    private _mode: TransformMode = null;
    private _target: Object3D = null;
    private readonly modes = new Array<TransformMode>();

    constructor(private readonly env: BaseEnvironment) {
        super();

        this.content3d = obj("Translator",
            ...this.translators = [
                this.setTranslator("x", "x", red),
                this.setTranslator("y", "y", green),
                this.setTranslator("z", "x", blue)
            ]
        );

        objectSetVisible(this, false);

        env.timer.addTickHandler(() => this.refresh());

        this.modeButtons = orderedTransformModes
            .map(mode => {
                const btn = Button(
                    mode,
                    OnClick(() =>
                        this.mode = btn.classList.contains("btn-secondary")
                            ? mode
                            : TransformMode.None)
                );
                this.buttons.set(mode, btn);
                elementSetDisplay(btn, false);
                return btn;
            });
    }

    get target(): Object3D {
        return this._target;
    }

    setTarget(v: Objects, modes?: TransformMode[]) {
        v = objectResolve(v);
        if (v !== this.target) {
            this._target = v;
            const hasTarget = isDefined(this.target);
            objectSetVisible(this, hasTarget);
            this.refresh();
        }

        if (isDefined(v) && isDefined(modes)) {
            arrayReplace(this.modes, modes);
            for (const [mode, btn] of this.buttons) {
                elementSetDisplay(btn, this.modes.indexOf(mode) !== -1);
            }

            if (this.modes.indexOf(this.mode) === -1) {
                this.mode = TransformMode.None;
            }
        }
    }

    get mode() {
        return this._mode;
    }

    set mode(v) {
        if (v !== this.mode) {
            this._mode = v;

            if (this.mode === "None") {
                this.env.eventSys.sortFunction = null;
            }
            else {
                this.env.eventSys.sortFunction = this.prioritizeTransformerSort;
            }

            for (const translator of this.translators) {
                translator.mode = v;
            }

            this.translators[2].content3d.visible = this.mode !== TransformMode.None
                && this.mode !== TransformMode.Resize;

            for (const [mode, btn] of this.buttons) {
                btn.classList.toggle("btn-primary", mode === this.mode);
                btn.classList.toggle("btn-secondary", mode !== this.mode);
            }

            this.refresh();
        }
    }


    private readonly prioritizeTransformerSort: IntersectionSortFunction = (a, b) => {
        const rayTargetA = getRayTarget(a.object) as Translator;
        const rayTargetB = getRayTarget(b.object) as Translator;
        const isTranslatorA = isDefined(rayTargetA) && this.translators.indexOf(rayTargetA) >= 0;
        const isTranslatorB = isDefined(rayTargetB) && this.translators.indexOf(rayTargetB) >= 0;
        if (isTranslatorA === isTranslatorB) {
            return a.distance - b.distance;
        }
        else if (isTranslatorA) {
            return -1;
        }
        else {
            return 1;
        }
    };

    private setTranslator(motionAxis: Axis, interactionAxis: Axis, color: ColorRepresentation): Translator {
        const translator = new Translator(motionAxis, interactionAxis, color);
        translator.addEventListener("down", (evt) => {
            if (evt.pointer.isPressed(VirtualButton.Primary)) {
                this.dragging = true;
                this.startWorld.copy(evt.point);
                if (this.mode !== TransformMode.MoveObjectSpace
                    && this.mode !== TransformMode.MoveGlobalSpace
                    && this.mode !== TransformMode.MoveViewSpace
                    && this.mode !== TransformMode.Orbit) {
                    this.env.avatar.lockMovement = true;
                }

                if (isDefined(evt.hit)) {
                    translator.selected = evt.hit.object;
                }
                else {
                    translator.selected = null;
                }
            }
        });

        translator.addEventListener("move", (evt) => {
            if (this.dragging && evt.point) {
                this.endWorld.copy(evt.point);

                if (this.startWorld.manhattanDistanceTo(this.endWorld) > 0) {
                    this.content3d.worldToLocal(this.startLocal.copy(this.startWorld));
                    this.content3d.worldToLocal(this.endLocal.copy(this.endWorld));

                    if (this.mode === TransformMode.Resize) {
                        const startDist = this.startLocal.length();
                        const endDist = this.endLocal.length();
                        this.target.scale.addScalar(endDist - startDist);
                    }
                    else if (this.mode === TransformMode.RotateObjectSpace
                        || this.mode === TransformMode.RotateGlobalSpace
                        || this.mode === TransformMode.RotateViewSpace) {
                        this.startLocal.normalize();
                        this.endLocal.normalize();

                        const mag = this.startLocal.dot(this.endLocal);
                        if (-1 <= mag && mag <= 1) {
                            const sign = this.startLocal.cross(this.endLocal).dot(translator.rotationAxisLocal);
                            const radians = Math.sign(sign) * Math.acos(mag);
                            this.rotationAxisWorld
                                .copy(translator.rotationAxisLocal)
                                .applyQuaternion(this.content3d.quaternion);
                            this.deltaQuaternion
                                .setFromAxisAngle(this.rotationAxisWorld, radians);
                            this.target.quaternion.premultiply(this.deltaQuaternion);
                        }
                    }
                    else {
                        this.motionAxisWorld
                            .copy(translator.motionAxisLocal)
                            .applyQuaternion(this.content3d.quaternion);

                        this.lookDirectionWorld
                            .copy(this.env.avatar.worldPos)
                            .sub(this.content3d.position)
                            .normalize();

                        this.deltaPosition
                            .copy(this.endWorld)
                            .sub(this.startWorld);

                        const parallelity = this.lookDirectionWorld.dot(this.motionAxisWorld);
                        if (Math.abs(parallelity) > 0.7) {
                            const side = Math.abs(parallelity) < 0.98
                                && Math.sign(this.lookDirectionWorld.cross(this.motionAxisWorld).y)
                                || 1;
                            this.deltaPosition.applyQuaternion(correction.get(Math.sign(parallelity) * side));
                        }

                        const mag = this.size * this.deltaPosition.dot(this.motionAxisWorld);

                        this.deltaPosition
                            .copy(this.motionAxisWorld)
                            .multiplyScalar(mag);

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

                if (this.mode !== TransformMode.MoveObjectSpace
                    && this.mode !== TransformMode.MoveGlobalSpace
                    && this.mode !== TransformMode.MoveViewSpace
                    && this.mode !== TransformMode.Orbit) {
                    this.env.avatar.lockMovement = false;
                }

                translator.selected = null;

                this.dispatchEvent(this.movedEvt);
            }
        });

        return translator;
    }

    get size(): number {
        return this.content3d.scale.x;
    }

    set size(v: number) {
        this.content3d.scale.setScalar(v);
    }

    refresh() {
        if (this.target) {
            this.target.getWorldPosition(this.content3d.position);
            const dist = this.content3d.position.distanceTo(this.env.avatar.worldPos);
            this.size = 0.5 * dist;

            if (this.mode === TransformMode.MoveObjectSpace
                || this.mode === TransformMode.RotateObjectSpace) {
                this.target.getWorldQuaternion(this.content3d.quaternion);
            }
            else if (this.mode === TransformMode.RotateGlobalSpace
                || this.mode === TransformMode.MoveGlobalSpace) {
                this.content3d.quaternion.identity();
            }
            else {
                this.content3d.lookAt(this.env.avatar.worldPos);
            }

            for (const translator of this.translators) {
                translator.refresh(this.content3d.position, this.env.avatar.worldPos);
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
const arcGeom = new ExtrudeGeometry(arcShape, {
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
    private readonly materialSelected: Material;
    private readonly worldPos = new Vector3();
    private readonly worldQuat = new Quaternion();
    private readonly center = new Vector3();

    readonly interactionAxisLocal = new Vector3();
    readonly motionAxisLocal = new Vector3();
    readonly rotationAxisLocal = new Vector3();

    private _mode: TransformMode = null;
    selected: Object3D = null;

    constructor(motionAxis: Axis, interactionAxis: Axis, color: ColorRepresentation) {
        const axisIndex = Axes.indexOf(motionAxis);
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
        const materialSelected = litTransparent({
            color,
            depthTest: false,
            opacity: 1
        });

        const bars = [
            cube(`Bar_${motionAxis}1`, 1, 1, 1, materialFront),
            cube(`Bar_${motionAxis}1`, 1, 1, 1, materialFront)
        ];
        const spherePads = [
            sphere(`ScalePad_${motionAxis}1`, 1, materialFront),
            sphere(`ScalePad_${motionAxis}2`, 1, materialFront)
        ];
        const conePads = [
            cone(`TranslatePad_${motionAxis}1`, 1, 1, 1, materialFront),
            cone(`TranslatePad_${motionAxis}2`, 1, 1, 1, materialFront)
        ];
        const arcPads = [
            mesh(`RotatePad_${motionAxis}1`, arcGeom, materialFront),
            mesh(`RotatePad_${motionAxis}2`, arcGeom, materialFront),
            mesh(`RotatePad_${motionAxis}3`, arcGeom, materialFront),
            mesh(`RotatePad_${motionAxis}4`, arcGeom, materialFront)
        ];

        super(obj(
            `Transformer_${motionAxis}`,
            ...bars,
            ...spherePads,
            ...conePads,
            ...arcPads
        ));

        for (const obj of this.content3d.children) {
            if (isMesh(obj)) {
                obj.renderOrder = Number.MAX_SAFE_INTEGER;
            }
        }

        this.bars = bars;
        this.spherePads = spherePads;
        this.conePads = conePads;
        this.arcPads = arcPads;

        this.interactionAxisLocal[interactionAxis] = 1;
        this.motionAxisLocal[motionAxis] = 1;
        this.rotationAxisLocal[rotationAxis] = 1;

        this.materialFront = materialFront;
        this.materialBack = materialBack;
        this.materialSelected = materialSelected;

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
                .multiplyScalar(1.2)
                .add(Translator.small)
                .multiplyScalar(size);
            this.bars[i].position
                .copy(this.motionAxisLocal)
                .multiplyScalar(dir * size * 0.7);
        }

        const Q = new Quaternion();
        const Z = new Vector3(0, 0, -1);
        const ringRotAxis = new Vector3();
        ringRotAxis[ringAxis] = 1;


        for (let i = 0; i < this.arcPads.length; ++i) {
            const a = i * Math.PI / 2;
            Q.setFromAxisAngle(Z, a);
            this.arcPads[i].quaternion
                .setFromAxisAngle(ringRotAxis, Math.PI / 2)
                .multiply(Q);
            this.arcPads[i].scale
                .setScalar(size * 4);
        }

        this.addMeshes(
            ...this.bars,
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

            const isRotate = this.mode === TransformMode.RotateObjectSpace
                || this.mode === TransformMode.RotateGlobalSpace
                || this.mode === TransformMode.RotateViewSpace;

            const isLateral = this.mode !== TransformMode.None
                && !isRotate;

            for (const arcPad of this.arcPads) {
                arcPad.visible = isRotate;
            }

            for (const bar of this.bars) {
                bar.visible = isLateral;
            }
            for (const spherePad of this.spherePads) {
                spherePad.visible = this.mode === TransformMode.Resize;
            }

            for (const conePad of this.conePads) {
                conePad.visible = this.mode !== TransformMode.Resize
                    && isLateral;
            }

            for (const mesh of this.meshes) {
                if (mesh.visible !== isDefined(mesh.parent)) {
                    if (mesh.visible) {
                        this.content3d.add(mesh);
                    }
                    else {
                        mesh.removeFromParent();
                    }
                }
            }
        }
    }

    private readonly delta = new Vector3();
    refresh(center: Vector3, lookAt: Vector3) {
        this.delta.subVectors(lookAt, center);
        this.checkMeshes(center, this.bars);
        this.checkMeshes(center, this.spherePads);
        this.checkMeshes(center, this.conePads);
        this.checkMeshes(center, this.arcPads);
    }

    private checkMeshes(center: Vector3, arr: Mesh[]) {
        for (const pad of arr) {
            if (pad === this.selected) {
                pad.material = this.materialSelected;
            }
            else {
                pad.getWorldPosition(this.worldPos);
                pad.getWorldQuaternion(this.worldQuat);
                pad.geometry.boundingBox.getCenter(this.center);
                this.center.add(pad.position);
                pad.localToWorld(this.center);
                this.center.sub(center);
                const distB = this.center.dot(this.delta);
                pad.material = distB < 0 ? this.materialBack : this.materialFront;
            }
        }
    }
}
