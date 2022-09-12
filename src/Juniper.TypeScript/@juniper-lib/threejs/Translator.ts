import { ColorRepresentation, Vector3 } from "three";
import { Cone } from "./Cone";
import { Cube } from "./Cube";
import { RayTarget } from "./eventSystem/RayTarget";
import { lit } from "./materials";
import { obj } from "./objects";
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

export type Sign = "-" | "+";
export type Axis = "x" | "y" | "z";
export type SignedAxis = `${Sign}${Axis}`;
const Axes: readonly Axis[] = ["x", "y", "z"];

export class Translator extends RayTarget<void> {
    private static readonly small = new Vector3(0.1, 0.1, 0.1);
    private readonly bar: Cube;
    private readonly spherePads: Sphere[];
    private readonly conePads: Cone[];
    private readonly arcPad: Torus;
    readonly motionAxisLocal: Vector3;
    readonly rotationAxisLocal: Vector3;

    private _mode: TransformMode = null;

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
        this.motionAxisLocal = new Vector3();
        this.motionAxisLocal[axis] = value;
        this.rotationAxisLocal = new Vector3();
        this.rotationAxisLocal[rotationAxis] = value;

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
