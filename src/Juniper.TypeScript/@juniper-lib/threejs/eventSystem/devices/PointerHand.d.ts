/// <reference types="webxr" />
import { EventedGamepad } from "@juniper-lib/widgets/EventedGamepad";
import { Object3D, XRGripSpace, XRHandSpace, XRTargetRaySpace } from "three";
import { BufferReaderWriter } from "../../BufferReaderWriter";
import type { BaseEnvironment } from "../../environment/BaseEnvironment";
import { XRControllerModel } from "../../examples/webxr/XRControllerModelFactory";
import { XRHandModel } from "../../examples/webxr/XRHandModelFactory";
import { ErsatzObject } from "../../objects";
import { BasePointer } from "./BasePointer";
export declare enum OculusQuestButton {
    Trigger = 0,
    Grip = 1,
    Stick = 3,
    X_A = 4,
    Y_B = 5
}
export declare class PointerHand extends BasePointer implements ErsatzObject {
    private readonly laser;
    readonly object: Object3D;
    private _isHand;
    private inputSource;
    private readonly _gamepad;
    readonly controller: XRTargetRaySpace;
    readonly grip: XRGripSpace;
    readonly hand: XRHandSpace;
    private readonly delta;
    private readonly newOrigin;
    private readonly quaternion;
    private readonly newQuat;
    readonly handModel: XRHandModel;
    readonly gripModel: XRControllerModel;
    constructor(env: BaseEnvironment, index: number);
    vibrate(): void;
    private useHaptics;
    private _vibrate;
    get gamepad(): EventedGamepad;
    get handedness(): XRHandedness;
    get isHand(): boolean;
    get cursor(): import("../cursors/BaseCursor3D").BaseCursor3D;
    set cursor(v: import("../cursors/BaseCursor3D").BaseCursor3D);
    private updateCursorSide;
    protected updatePointerOrientation(): void;
    protected onUpdate(): void;
    get bufferSize(): number;
    writeState(buffer: BufferReaderWriter): void;
}
//# sourceMappingURL=PointerHand.d.ts.map