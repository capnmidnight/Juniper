import { TypedEvent } from "@juniper-lib/tslib/events/EventBase";
import { ColorRepresentation, Quaternion, Vector3 } from "three";
import { Cone } from "./Cone";
import { Cube } from "./Cube";
import { VirtualButton } from "./eventSystem/devices/VirtualButton";
import { RayTarget } from "./eventSystem/RayTarget";
import { lit } from "./materials";
import { obj } from "./objects";
import { Sphere } from "./Sphere";
import { Torus } from "./Torus";
import { TransformEditorMode } from "./TransformEditor";

export class TranslatorDragDirEvent extends TypedEvent<"dragdir">{

    public readonly deltaPosition = new Vector3();
    public readonly deltaRotation = new Quaternion();
    public magnitude: number = 0;

    constructor() {
        super("dragdir");
    }
}

export interface TranslatorDragDirEvents {
    "dragdir": TranslatorDragDirEvent;
    "dragstart": TypedEvent<"dragstart">;
    "dragend": TypedEvent<"dragend">;
}

export type Sign = "-" | "+";
export type Axis = "x" | "y" | "z";
export type SignedAxis = `${Sign}${Axis}`;
const Axes: Axis[] = ["x", "y", "z"];
const P = new Vector3();
const Q = new Quaternion();

export class Translator extends RayTarget<TranslatorDragDirEvents> {
    private static readonly small = new Vector3(0.1, 0.1, 0.1);
    private readonly bar: Cube;
    private readonly spherePads: Sphere[];
    private readonly conePads: Cone[];
    private readonly arcPad: Torus;
    private readonly motionAxis: Vector3;
    private readonly rotationAxis: Vector3;

    private _mode: TransformEditorMode = null;

    constructor(name: SignedAxis, color: ColorRepresentation) {

        const sign = (name[0] as Sign);
        const axis = (name[1] as Axis);
        const axisIndex = Axes.indexOf(axis);
        const rotationAxisIndex = (axisIndex + 2) % Axes.length;
        const rotationAxis = Axes[rotationAxisIndex];
        const ringAxisIndex = Axes.length - axisIndex - 1;
        const ringAxis = Axes[ringAxisIndex];
        const material = lit({
            color,
            depthTest: false
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
            "Translator " + name,
            bar,
            ...spherePads,
            ...conePads,
            arcPad
        ));

        this.bar = bar;
        this.spherePads = spherePads;
        this.conePads = conePads;
        this.arcPad = arcPad;

        const value = sign === "+" ? 1 : -1;
        this.motionAxis = new Vector3();
        this.motionAxis[axis] = value;
        this.rotationAxis = new Vector3();
        this.rotationAxis[rotationAxis] = value;

        const start = new Vector3();
        const end = new Vector3();
        const from = new Vector3();
        const to = new Vector3();
        const deltaIn = new Vector3();
        const dragEvt = new TranslatorDragDirEvent();
        const dragStartEvt = new TypedEvent("dragstart");
        const dragEndEvt = new TypedEvent("dragend");

        let dragging = false;

        this.enabled = true;
        this.draggable = true;

        this.addEventListener("down", (evt) => {
            if (evt.pointer.isPressed(VirtualButton.Primary)) {
                dragging = true;
                start.copy(evt.point);
                this.dispatchEvent(dragStartEvt);
            }
        });

        this.addEventListener("move", (evt) => {
            if (dragging && evt.point) {
                end.copy(evt.point);
                deltaIn.copy(end).sub(start);

                this.object.parent.worldToLocal(from.copy(start));
                this.object.parent.worldToLocal(to.copy(end));

                if (deltaIn.manhattanLength() > 0) {
                    if (this.mode === TransformEditorMode.Rotate) {
                        from.normalize();
                        to.normalize();

                        const mag = from.dot(to);
                        if (-1 <= mag && mag <= 1) {
                            const sign = from.cross(to).dot(this.rotationAxis);
                            const radians = Math.sign(sign) * Math.acos(mag);
                            dragEvt.deltaRotation.setFromAxisAngle(this.rotationAxis, radians);
                        }
                    }
                    else {
                        dragEvt.deltaPosition.copy(to)
                            .sub(from);

                        dragEvt.magnitude = this.motionAxis.dot(dragEvt.deltaPosition);

                        dragEvt.deltaPosition
                            .copy(this.motionAxis)
                            .multiplyScalar(dragEvt.magnitude);

                        this.object.parent.localToWorld(dragEvt.deltaPosition);
                        this.object.parent.getWorldPosition(P);
                        dragEvt.deltaPosition.sub(P);
                    }

                    this.dispatchEvent(dragEvt);
                }

                start.copy(end);
            }
        });

        this.addEventListener("up", (evt) => {
            if (!evt.pointer.isPressed(VirtualButton.Primary)) {
                dragging = false;
                this.dispatchEvent(dragEndEvt);
            }
        });

        for (let i = 0; i < this.spherePads.length; ++i) {
            const dir = 2 * i - 1;
            this.spherePads[i].scale.setScalar(1 / 10);
            this.spherePads[i].position
                .copy(this.motionAxis)
                .multiplyScalar(dir * 0.375);
        }

        for (let i = 0; i < this.conePads.length; ++i) {
            const dir = 2 * i - 1;
            P.copy(this.conePads[i].up).multiplyScalar(dir);
            this.conePads[i].quaternion
                .setFromUnitVectors(P, this.motionAxis);
            this.conePads[i].position
                .copy(this.motionAxis)
                .multiplyScalar(dir * 0.375);
            this.conePads[i].scale.set(1 / 20, 1 / 10, 1 / 20);
        }

        this.bar.scale
            .copy(this.motionAxis)
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

            this.bar.visible = this.mode !== TransformEditorMode.Rotate;
            this.arcPad.visible = this.mode === TransformEditorMode.Rotate;

            for (const spherePad of this.spherePads) {
                spherePad.visible = this.mode === TransformEditorMode.Resize;
            }

            for (const conePad of this.conePads) {
                conePad.visible = this.mode !== TransformEditorMode.Resize && this.mode !== TransformEditorMode.Rotate;
            }
        }
    }
}
