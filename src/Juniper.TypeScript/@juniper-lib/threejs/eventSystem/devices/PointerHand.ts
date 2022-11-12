import { PointerID } from "@juniper-lib/tslib/events/Pointers";
import { isChrome, isDesktop, isOculusBrowser } from "@juniper-lib/tslib/flags";
import { Pi } from "@juniper-lib/tslib/math";
import { isDefined, isNullOrUndefined } from "@juniper-lib/tslib/typeChecks";
import { EventedGamepad, GamepadButtonEvent } from "@juniper-lib/widgets/EventedGamepad";
import { Event, Matrix4, Object3D, Quaternion, Vector3, XRGripSpace, XRHandSpace, XRTargetRaySpace } from "three";
import type { BaseEnvironment } from "../../environment/BaseEnvironment";
import { XRControllerModelFactory } from "../../examples/webxr/XRControllerModelFactory";
import { XRHandModelFactory } from "../../examples/webxr/XRHandModelFactory";
import { white } from "../../materials";
import { ErsatzObject, obj, objGraph } from "../../objects";
import { CursorColor } from "../cursors/CursorColor";
import { Laser } from "../Laser";
import { BasePointer } from "./BasePointer";
import { VirtualButton } from "./VirtualButton";

const mcModelFactory = new XRControllerModelFactory();
const handModelFactory = new XRHandModelFactory();
const riftSCorrection = new Matrix4().makeRotationX(-7 * Pi / 9);

const pointerIDs = new Map<XRHandedness, PointerID>([
    ["none", PointerID.MotionController],
    ["left", PointerID.MotionControllerLeft],
    ["right", PointerID.MotionControllerRight]
]);

export enum OculusQuestButton {
    Trigger = 0,
    Grip = 1,
    Stick = 3,
    X_A = 4,
    Y_B = 5
}

const questToVirtualMap = new Map<OculusQuestButton, VirtualButton>([
    //[OculusQuestButton.Trigger, VirtualButton.Primary],
    //[OculusQuestButton.Grip, VirtualButton.Secondary],
    [OculusQuestButton.Y_B, VirtualButton.Menu],
    [OculusQuestButton.X_A, VirtualButton.Info]
]);

type XRControllerConnectionEventTypes = "connected" | "disconnected";
type XRControllerConnectionEvent<T extends XRControllerConnectionEventTypes> = Event & {
    type: T,
    target: XRTargetRaySpace,
    data?: XRInputSource
};

export class PointerHand
    extends BasePointer
    implements ErsatzObject {
    private readonly laser = new Laser(white, 0.002);
    readonly object: Object3D;

    private _isHand = false;
    private inputSource: XRInputSource = null;

    private readonly _gamepad = new EventedGamepad();

    readonly controller: XRTargetRaySpace;
    readonly grip: XRGripSpace;
    readonly hand: XRHandSpace;

    private readonly delta = new Vector3();
    private readonly newOrigin = new Vector3();
    private readonly quaternion = new Quaternion();
    private readonly newQuat = new Quaternion();

    constructor(env: BaseEnvironment, index: number) {
        super("hand", PointerID.MotionController, env, new CursorColor(env));

        this.mayTeleport = true;

        this.object = obj("PointerHand" + index);
        this.quaternion.identity();

        objGraph(this,
            this.controller = this.env.renderer.xr.getController(index),
            this.grip = this.env.renderer.xr.getControllerGrip(index),
            this.hand = this.env.renderer.xr.getHand(index)
        );

        // isDesktop and isOculus can both be true if the user
        // has requested the Desktop version of a site in Oculus Browser.
        if (isDesktop() && isChrome() && !isOculusBrowser) {
            let maybeOculusRiftS = false;
            this.controller.traverse((child) => {
                const key = child.name.toLocaleLowerCase();
                if (key.indexOf("oculus") >= 0) {
                    maybeOculusRiftS = true;
                }
            });
            if (maybeOculusRiftS) {
                this.laser.matrix.copy(riftSCorrection);
            }
        }

        objGraph(this.controller, this.laser);
        objGraph(this.grip, mcModelFactory.createControllerModel(this.controller));
        objGraph(this.hand, handModelFactory.createHandModel(this.hand, "mesh"));

        this.gamepad.addEventListener("gamepadaxismaxed", (evt) => {
            if (evt.axis === 2) {
                this.env.avatar.snapTurn(evt.value);
            }
        });

        const setButton = (pressed: boolean) => {
            return (evt: GamepadButtonEvent) => {
                if (questToVirtualMap.has(evt.button)) {
                    this.setButton(questToVirtualMap.get(evt.button), pressed);
                }
            };
        };

        this.gamepad.addEventListener("gamepadbuttondown", setButton(true));
        this.gamepad.addEventListener("gamepadbuttonup", setButton(false));

        const setHandButton = (btn: VirtualButton, pressed: boolean) =>
            () => this.setButton(btn, pressed);

        this.controller.addEventListener("selectstart", setHandButton(VirtualButton.Primary, true));
        this.controller.addEventListener("selectend", setHandButton(VirtualButton.Primary, false));
        this.controller.addEventListener("squeezestart", setHandButton(VirtualButton.Secondary, true));
        this.controller.addEventListener("squeezeend", setHandButton(VirtualButton.Secondary, false));

        this.controller.addEventListener("connected", (evt: XRControllerConnectionEvent<"connected">) => {
            if (evt.target === this.controller) {
                this.inputSource = evt.data;
                this.gamepad.pad = this.inputSource.gamepad;
                this._isHand = isDefined(this.inputSource.hand);
                this.id = pointerIDs.get(this.handedness);

                this.grip.visible = !this.isHand;
                this.controller.visible = !this.isHand;
                this.hand.visible = this.isHand;

                this.enabled = true;
                this._isActive = true;
                this.env.eventSys.checkXRMouse();
                this.updateCursorSide();
                console.log(this.handedness, "connected");
            }
        });

        this.controller.addEventListener("disconnected", (evt: XRControllerConnectionEvent<"disconnected">) => {
            if (evt.target === this.controller) {
                this.inputSource = null;
                this.gamepad.pad = null;
                this._isHand = false;
                this.id = pointerIDs.get(this.handedness);

                this.grip.visible = false;
                this.controller.visible = false;
                this.hand.visible = false;

                this.enabled = false;
                this._isActive = false;
                this.env.eventSys.checkXRMouse();
                this.updateCursorSide();
                console.log(this.handedness, "disconnected");
            }
        });

        Object.seal(this);
    }

    override vibrate(): void {
        this._vibrate();
    }

    private useHaptics = true;

    private async _vibrate(): Promise<void> {
        if (this.useHaptics && this.gamepad.hapticActuators) {
            try {
                await Promise.all(this.gamepad.hapticActuators.map((actuator) =>
                    actuator.pulse(0.25, 125)));
            }
            catch {
                this.useHaptics = false;
            }
        }
    }

    get gamepad() {
        return this._gamepad;
    }

    get handedness() {
        if (isNullOrUndefined(this.inputSource)) {
            return null;
        }
        return this.inputSource.handedness;
    }

    get isHand() {
        return this._isHand;
    }

    override get cursor() {
        return super.cursor;
    }

    override set cursor(v) {
        super.cursor = v;
        this.updateCursorSide();
    }

    private updateCursorSide() {
        this.cursor.side = this.handedness === "left" ? 1 : -1;
    }

    protected updatePointerOrientation() {
        this.laser.getWorldPosition(this.newOrigin);
        this.laser.getWorldQuaternion(this.newQuat);
        this.origin.lerp(this.newOrigin, 0.9);
        this.quaternion.slerp(this.newQuat, 0.9);

        this.delta
            .copy(this.origin)
            .add(this.direction);
        this.direction.set(0, 0, -1).applyQuaternion(this.quaternion);
        this.up.set(0, 1, 0).applyQuaternion(this.quaternion);
        this.delta
            .sub(this.direction)
            .sub(this.origin);

        this.moveDistance += 50 * this.delta.length();
    }

    protected override onUpdate() {
        this.gamepad.pad = this.inputSource && this.inputSource.gamepad || null;

        super.onUpdate();
    }
}