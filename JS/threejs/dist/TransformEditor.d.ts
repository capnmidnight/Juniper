import { TypedEvent, TypedEventTarget } from "@juniper-lib/events";
import { ColorRepresentation, Object3D, Vector3 } from "three";
import { BaseEnvironment } from "./environment/BaseEnvironment";
import { RayTarget } from "./eventSystem/RayTarget";
import { ErsatzObject, Objects } from "./objects";
export declare enum TransformMode {
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
type Axis = "x" | "y" | "z";
type TransformEditorEvents = {
    moving: TypedEvent<"moving">;
    moved: TypedEvent<"moved">;
};
export declare class TransformEditor extends TypedEventTarget<TransformEditorEvents> implements ErsatzObject {
    private readonly env;
    readonly content3d: Object3D;
    readonly modeButtons: HTMLButtonElement[];
    private readonly buttons;
    private readonly translators;
    private readonly movingEvt;
    private readonly movedEvt;
    private dragging;
    private readonly rotationAxisWorld;
    private readonly startWorld;
    private readonly endWorld;
    private readonly startLocal;
    private readonly endLocal;
    private readonly lookDirectionWorld;
    private readonly deltaPosition;
    private readonly deltaQuaternion;
    private readonly motionAxisWorld;
    private readonly testObj;
    private _mode;
    private _target;
    private readonly modes;
    constructor(env: BaseEnvironment);
    get target(): Object3D;
    setTarget(v: Objects, modes?: TransformMode[]): void;
    get mode(): TransformMode;
    set mode(v: TransformMode);
    private readonly prioritizeTransformerSort;
    private setTranslator;
    get size(): number;
    set size(v: number);
    refresh(): void;
}
export declare class Translator extends RayTarget<void> {
    private static readonly small;
    private readonly bars;
    private readonly spherePads;
    private readonly conePads;
    private readonly arcPads;
    private readonly materialFront;
    private readonly materialBack;
    private readonly materialSelected;
    private readonly worldPos;
    private readonly worldQuat;
    private readonly center;
    readonly interactionAxisLocal: Vector3;
    readonly motionAxisLocal: Vector3;
    readonly rotationAxisLocal: Vector3;
    private _mode;
    selected: Object3D;
    constructor(motionAxis: Axis, interactionAxis: Axis, color: ColorRepresentation);
    get mode(): TransformMode;
    set mode(v: TransformMode);
    private readonly delta;
    refresh(center: Vector3, lookAt: Vector3): void;
    private checkMeshes;
}
export {};
//# sourceMappingURL=TransformEditor.d.ts.map