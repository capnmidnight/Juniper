import { TypedEvent } from "@juniper-lib/tslib/events/EventBase";
import { ColorRepresentation, Quaternion, Vector3 } from "three";
import { Cone } from "./Cone";
import { Cube } from "./Cube";
import { VirtualButton } from "./eventSystem/devices/VirtualButton";
import { RayTarget } from "./eventSystem/RayTarget";
import { lit } from "./materials";
import { obj } from "./objects";
import { Sphere } from "./Sphere";
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

const P = new Vector3();

export class Translator extends RayTarget<TranslatorDragDirEvents> {
    private static readonly small = new Vector3(0.1, 0.1, 0.1);
    private readonly bar: Cube;
    private readonly spherePad: Sphere;
    private readonly conePad: Cone;
    private _size: number = 1;
    private readonly motionAxis: Vector3;
    private readonly rotationXAxis: Vector3;
    private readonly rotationYAxis: Vector3;

    private _mode: TransformEditorMode = null;

    constructor(
        name: string,
        mx: number,
        my: number,
        mz: number,
        rxx: number,
        rxy: number,
        rxz: number,
        ryx: number,
        ryy: number,
        ryz: number,
        color: ColorRepresentation,
        mode: TransformEditorMode) {
        const material = lit({
            color,
            depthTest: false
        });

        const cube = new Cube(1, 1, 1, material);
        const cone = new Cone(1, 1, 1, material);
        const sphere = new Sphere(1, material);

        super(obj(
            "Translator " + name,
            cube,
            sphere,
            cone
        ));

        this.bar = cube;
        this.spherePad = sphere;
        this.conePad = cone;

        this.addMesh(this.spherePad);
        this.addMesh(this.conePad);

        this.motionAxis = new Vector3(mx, my, mz);
        this.rotationXAxis = new Vector3(rxx, rxy, rxz);
        this.rotationYAxis = new Vector3(ryx, ryy, ryz);        

        const start = new Vector3();
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

                deltaIn
                    .copy(evt.point)
                    .sub(start);

                start.copy(evt.point);

                if (deltaIn.manhattanLength() > 0) {


                    if (this.mode === TransformEditorMode.Rotate) {
                        if (Math.abs(deltaIn.x) > Math.abs(deltaIn.y)) {
                            P.copy(this.rotationXAxis)
                                //.applyQuaternion(this.object.parent.parent.quaternion);

                            dragEvt.deltaRotation
                                .setFromAxisAngle(P, deltaIn.x);
                        }
                        else {
                            P.copy(this.rotationYAxis)
                                //.applyQuaternion(this.object.parent.parent.quaternion);

                            dragEvt.deltaRotation
                                .setFromAxisAngle(P, deltaIn.y);
                        }
                    }
                    else {
                        dragEvt.deltaPosition
                            .copy(this.motionAxis)
                            .applyQuaternion(this.object.parent.parent.quaternion);

                        dragEvt.magnitude = deltaIn.dot(dragEvt.deltaPosition);

                        dragEvt.deltaPosition.multiplyScalar(dragEvt.magnitude);
                    }

                    this.dispatchEvent(dragEvt);
                }
            }
        });

        this.addEventListener("up", (evt) => {
            if (!evt.pointer.isPressed(VirtualButton.Primary)) {
                dragging = false;
                this.dispatchEvent(dragEndEvt);
            }
        });

        this.size = 1;
        this.mode = mode;
    }

    get mode() {
        return this._mode;
    }

    set mode(v) {
        if (v !== this.mode) {
            this._mode = v;
            this.spherePad.visible = this.mode === TransformEditorMode.Resize
                || this.mode === TransformEditorMode.Rotate;
            this.conePad.visible = !this.spherePad.visible;
        }
    }

    get size(): number {
        return this._size;
    }

    set size(v: number) {
        this._size = v;

        this.spherePad.scale.setScalar(v / 3);
        this.spherePad.position
            .copy(this.motionAxis)
            .multiplyScalar(0.5)
            .add(this.motionAxis)
            .multiplyScalar(this.size);

        this.conePad.scale.set(v / 5, v / 3, v / 5);
        this.conePad.position
            .copy(this.motionAxis)
            .multiplyScalar(0.5)
            .add(this.motionAxis)
            .multiplyScalar(this.size);
        this.conePad.quaternion
            .setFromUnitVectors(this.conePad.up, this.motionAxis);

        this.bar.scale
            .copy(this.motionAxis)
            .multiplyScalar(0.9)
            .add(Translator.small)
            .multiplyScalar(this.size);

        this.bar.position
            .copy(this.motionAxis)
            .multiplyScalar(this.size);
    }
}
