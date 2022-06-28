import { MouseButtons } from "@juniper-lib/threejs/eventSystem/MouseButton";
import { VirtualButtons } from "@juniper-lib/threejs/eventSystem/VirtualButtons";
import {
    isChrome,
    isDefined,
    isDesktop, isOculusBrowser, PriorityMap
} from "@juniper-lib/tslib";
import { PointerName } from "@juniper-lib/tslib/events/PointerName";
import { EventedGamepad } from "@juniper-lib/widgets/EventedGamepad";
import type { BaseEnvironment } from "../environment/BaseEnvironment";
import { XRControllerModelFactory } from "../examples/webxr/XRControllerModelFactory";
import { XRHandModelFactory } from "../examples/webxr/XRHandModelFactory";
import { white } from "../materials";
import { ErsatzObject, objGraph } from "../objects";
import { BasePointer } from "./BasePointer";
import { CursorColor } from "./CursorColor";
import { Laser } from "./Laser";

const mcModelFactory = new XRControllerModelFactory();
const handModelFactory = new XRHandModelFactory();
const riftSCorrection = new THREE.Matrix4().makeRotationX(-7 * Math.PI / 9);
const newOrigin = new THREE.Vector3();
const newDirection = new THREE.Vector3();
const buttonIndices = new PriorityMap<XRHandedness, VirtualButtons, number>([
    ["left", VirtualButtons.Primary, 0],
    ["left", VirtualButtons.Secondary, 1],
    ["left", VirtualButtons.Menu, 2],
    ["left", VirtualButtons.Info, 3],
    ["right", VirtualButtons.Primary, 0],
    ["right", VirtualButtons.Secondary, 1],
    ["right", VirtualButtons.Menu, 2],
    ["right", VirtualButtons.Info, 3]
]);

const pointerNames = new Map<XRHandedness, PointerName>([
    ["none", PointerName.MotionController],
    ["left", PointerName.MotionControllerLeft],
    ["right", PointerName.MotionControllerRight]
]);

export enum OculusQuestButtons {
    Trigger = 0,
    Grip = 1,
    Stick = 3,
    X_A = 4,
    Y_B = 5
}

type XRControllerConnectionEventTypes = "connected" | "disconnected";
type XRControllerConnectionEvent<T extends XRControllerConnectionEventTypes> = THREE.Event & {
    type: T,
    target: THREE.XRTargetRaySpace,
    data?: XRInputSource
};

export class PointerHand
    extends BasePointer
    implements ErsatzObject {
    private readonly laser = new Laser(white, 0.002);
    readonly object = new THREE.Object3D();

    private _handedness: XRHandedness = "none";
    private _isHand = false;
    private inputSource: XRInputSource = null;
    private readonly _gamepad = new EventedGamepad();

    private readonly controller: THREE.XRTargetRaySpace;
    private readonly grip: THREE.XRGripSpace;
    private readonly hand: THREE.XRHandSpace;

    constructor(env: BaseEnvironment, index: number) {
        super("hand", PointerName.MotionController, env, new CursorColor());

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

        this.controller.addEventListener("connected", (evt: XRControllerConnectionEvent<"connected">) => {
            if (evt.target === this.controller) {
                this.inputSource = evt.data;
                this.gamepad.pad = this.inputSource.gamepad;
                this._isHand = isDefined(this.inputSource.hand);
                this._handedness = this.inputSource.handedness;
                this.name = pointerNames.get(this.handedness);
                this.updateCursorSide();

                this.grip.visible = !this.isHand;
                this.controller.visible = !this.isHand;
                this.hand.visible = this.isHand;

                this.enabled = true;
                this.isActive = true;
                this.env.eventSystem.onConnected(this);
            }
        });

        this.controller.addEventListener("disconnected", (evt: XRControllerConnectionEvent<"disconnected">) => {
            if (evt.target === this.controller) {
                this.inputSource = null;
                this.gamepad.pad = null;
                this._isHand = false;
                this._handedness = "none";
                this.name = pointerNames.get(this.handedness);
                this.updateCursorSide();

                this.grip.visible = false;
                this.controller.visible = false;
                this.hand.visible = false;

                this.enabled = false;
                this.isActive = false;
                this.env.eventSystem.onDisconnected(this);
            }
        });

        const buttonDown = (btn: MouseButtons) => {
            this._buttons = this.buttons | btn;
            this.onPointerDown();
        };

        const buttonUp = (btn: MouseButtons) => {
            this._buttons = this.buttons & ~btn;
            this.onPointerUp();
        };

        this.controller.addEventListener("selectstart", () => buttonDown(MouseButtons.Mouse0));
        this.controller.addEventListener("selectend", () => buttonUp(MouseButtons.Mouse0));
        this.controller.addEventListener("squeezestart", () => buttonDown(MouseButtons.Mouse1));
        this.controller.addEventListener("squeezeend", () => buttonUp(MouseButtons.Mouse1));
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
        return this._handedness;
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
        const obj = this.cursor.object;
        if (obj) {
            const sx = this.handedness === "left" ? -1 : 1;
            obj.scale.set(sx, 1, 1);
        }
    }

    protected onUpdate(): void {
        this.laser.getWorldPosition(newOrigin);
        this.laser.getWorldDirection(newDirection)
            .multiplyScalar(-1);

        this.origin.lerp(newOrigin, 0.9);
        this.direction.lerp(newDirection, 0.9)
            .normalize();

        this.gamepad.pad = this.inputSource && this.inputSource.gamepad || null;
    }

    isPressed(button: VirtualButtons): boolean {
        if (!buttonIndices.has(this.handedness)
            || !buttonIndices.get(this.handedness).has(button)) {
            return false;
        }

        const index = buttonIndices.get(this.handedness).get(button);
        return index < this.gamepad.buttons.length
            && this.gamepad.buttons[index].pressed;
    }
}