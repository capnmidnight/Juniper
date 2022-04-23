import { EventedGamepad, GamepadAxisMaxedEvent } from "@juniper/dom/EventedGamepad";
import { MouseButtons } from "@juniper/event-system/MouseButton";
import { PointerName } from "@juniper/event-system/PointerName";
import { VirtualButtons } from "@juniper/event-system/VirtualButtons";
import {
    isChrome,
    isDefined,
    isDesktop,
    isNullOrUndefined,
    isOculusBrowser, PriorityMap
} from "@juniper/tslib";
import { XRControllerModelFactory } from "../examples/webxr/XRControllerModelFactory";
import { XRHandModelFactory } from "../examples/webxr/XRHandModelFactory";
import { white } from "../materials";
import { ErsatzObject } from "../objects";
import { BasePointer } from "./BasePointer";
import { CursorColor } from "./CursorColor";
import type { EventSystem } from "./EventSystem";
import { Laser } from "./Laser";

const mcModelFactory = new XRControllerModelFactory();
const handModelFactory = new XRHandModelFactory();
const riftSCorrection = new THREE.Matrix4().makeRotationX(-7 * Math.PI / 9);
const newOrigin = new THREE.Vector3();
const newDirection = new THREE.Vector3();
const delta = new THREE.Vector3();
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

export class PointerHand
    extends BasePointer
    implements ErsatzObject {
    private readonly laser = new Laser(white, 0.002);
    readonly object = new THREE.Object3D();

    private _handedness: XRHandedness = "none";
    private _isHand = false;
    private inputSource: XRInputSource = null;
    private _gamepad: EventedGamepad = null;

    private readonly controller: THREE.Group;
    private readonly grip: THREE.Group;
    private readonly hand: THREE.Group;
    private readonly onAxisMaxed: (evt: GamepadAxisMaxedEvent) => void;

    constructor(evtSys: EventSystem, private readonly renderer: THREE.WebGLRenderer, index: number) {
        super("hand", PointerName.MotionController, evtSys, new CursorColor());

        this.object.add(
            this.controller = this.renderer.xr.getController(index),
            this.grip = this.renderer.xr.getControllerGrip(index),
            this.hand = this.renderer.xr.getHand(index)
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

        this.controller.add(this.laser);
        this.grip.add(mcModelFactory.createControllerModel(this.controller));

        this.hand.add(handModelFactory.createHandModel(this.hand, "mesh"));

        this.onAxisMaxed = (evt: GamepadAxisMaxedEvent) => {
            if (evt.axis === 2) {
                this.evtSys.onFlick(evt.value);
            }
        };

        this.controller.addEventListener("connected", () => {
            const session = this.renderer.xr.getSession();

            this.inputSource = session.inputSources[index] as any as XRInputSource;
            this.setGamepad(this.inputSource.gamepad);
            this._isHand = isDefined(this.inputSource.hand);
            this._handedness = this.inputSource.handedness;
            this.name = pointerNames.get(this.handedness);
            this.updateCursorSide();

            this.grip.visible = !this.isHand;
            this.controller.visible = !this.isHand;
            this.hand.visible = this.isHand;

            this.enabled = true;
            this.isActive = true;
            this.evtSys.onConnected(this);
        });

        this.controller.addEventListener("disconnected", () => {
            this.inputSource = null;
            this.setGamepad(null);
            this._isHand = false;
            this._handedness = "none";
            this.name = pointerNames.get(this.handedness);
            this.updateCursorSide();

            this.grip.visible = false;
            this.controller.visible = false;
            this.hand.visible = false;

            this.enabled = false;
            this.isActive = false;
            this.evtSys.onDisconnected(this);
            this.isActive = false;
        });

        const buttonDown = (btn: MouseButtons) => {
            this.updateState();
            this.state.buttons = this.state.buttons | btn;
            this.onPointerDown();
        };

        const buttonUp = (btn: MouseButtons) => {
            this.updateState();
            this.state.buttons = this.state.buttons & ~btn;
            this.onPointerUp();
        };

        this.controller.addEventListener("selectstart", () =>
            buttonDown(MouseButtons.Mouse0));
        this.controller.addEventListener("selectend", () =>
            buttonUp(MouseButtons.Mouse0));
        this.controller.addEventListener("squeezestart", () =>
            buttonDown(MouseButtons.Mouse1));
        this.controller.addEventListener("squeezeend", () =>
            buttonUp(MouseButtons.Mouse1));
    }

    override vibrate(): void {
        this._vibrate();
    }

    private useHaptics = true;

    private async _vibrate(): Promise<void> {
        if (this._gamepad && this.useHaptics) {
            try {
                await Promise.all(this._gamepad.hapticActuators.map((actuator) =>
                    actuator.pulse(0.25, 125)));
            }
            catch {
                this.useHaptics = false;
            }
        }
    }

    private setGamepad(pad: Gamepad) {
        if (isDefined(this._gamepad) && isNullOrUndefined(pad)) {
            this._gamepad.clearEventListeners();
            this._gamepad = null;
        }

        if (isDefined(pad)) {
            if (isDefined(this._gamepad)) {
                this._gamepad.setPad(pad);
            }
            else {
                this._gamepad = new EventedGamepad(pad);
                this._gamepad.addEventListener("gamepadaxismaxed", this.onAxisMaxed);
            }
        }
    }

    get gamepad() {
        return this._gamepad;
    }

    get gamepadId() {
        return this._gamepad && this._gamepad.id;
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

    update() {
        if (this.enabled) {
            this.updateState();
            this.onPointerMove();
        }
    }

    private updateState() {
        this.lastStateUpdate(() => {
            this.laser.getWorldPosition(newOrigin);
            this.laser.getWorldDirection(newDirection)
                .multiplyScalar(-1);

            delta.copy(this.origin)
                .add(this.direction);

            this.origin.lerp(newOrigin, 0.9);
            this.direction.lerp(newDirection, 0.9)
                .normalize();

            delta.sub(this.origin)
                .sub(this.direction);

            this.state.moveDistance += 0.001 * delta.length();

            if (isDefined(this._gamepad)
                && isDefined(this.inputSource)) {
                this.setGamepad(this.inputSource.gamepad);
            }
        });
    }

    isPressed(button: VirtualButtons): boolean {
        if (!this._gamepad
            || !buttonIndices.has(this.handedness)
            || !buttonIndices.get(this.handedness).has(button)) {
            return false;
        }

        const index = buttonIndices.get(this.handedness).get(button);
        return this._gamepad.buttons[index].pressed;
    }
}