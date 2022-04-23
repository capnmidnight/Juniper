import { isModifierless } from "@juniper/dom/isModifierless";
import { AvatarMovedEvent } from "@juniper/event-system/AvatarMovedEvent";
import { MouseButtons } from "@juniper/event-system/MouseButton";
import { angleClamp, assertNever, clamp, deg2rad, IDisposable, isFunction, isMobile, isMobileVR, isNullOrUndefined, isString, truncate, TypedEventBase } from "@juniper/tslib";
import type { BodyFollower } from "./animation/BodyFollower";
import { getLookHeading, getLookPitch } from "./animation/lookAngles";
import type { EventSystem } from "./eventSystem/EventSystem";
import type { EventSystemEvent } from "./eventSystem/EventSystemEvent";
import type { Fader } from "./Fader";
import { ErsatzObject, obj } from "./objects";
import { resolveCamera } from "./resolveCamera";
import { setRightUpFwdPosFromMatrix } from "./setRightUpFwdPosFromMatrix";

/**
 * The mouse is not as sensitive as the gamepad, so we have to bump up the
 * sensitivity quite a bit.
 **/
const MOUSE_SENSITIVITY_SCALE = 100;

/**
 * The touch points are not as sensitive as the gamepad, so we have to bump up the
 * sensitivity quite a bit.
 **/
const TOUCH_SENSITIVITY_SCALE = 50;

const GAMEPAD_SENSITIVITY_SCALE = 1;

const MOTIONCONTROLLER_STICK_SENSITIVITY_SCALE = Math.PI / 3;

const B = new THREE.Vector3(0, 0, 1);
const R = new THREE.Vector3();
const F = new THREE.Vector3();
const U = new THREE.Vector3();
const P = new THREE.Vector3();
const M = new THREE.Matrix4();
const E = new THREE.Euler();
const Q1 = new THREE.Quaternion();
const Q2 = new THREE.Quaternion();
const Q3 = new THREE.Quaternion(- Math.sqrt(0.5), 0, 0, Math.sqrt(0.5)); // - PI/2 around the x-axis
const motion = new THREE.Vector3();
const deltaQuat = new THREE.Quaternion();
const nextFlick = new THREE.Vector3();
const rotStage = new THREE.Matrix4();
const userMovedEvt = new AvatarMovedEvent();

interface DeviceOrientationEventWithPermissionRequest extends Function {
    requestPermission(): Promise<PermissionState>;
}

function isPermissionedDeviceOrientationEvent(obj: any): obj is DeviceOrientationEventWithPermissionRequest {
    return obj === DeviceOrientationEvent
        && "requestPermission" in obj
        && isFunction(obj.requestPermission);
}

enum CameraControlMode {
    None = "none",
    MouseFPS = "mousefirstperson",
    MouseDrag = "mousedrag",
    MouseScreenEdge = "mouseedge",
    Touch = "touchswipe",
    Gamepad = "gamepad",
    MotionControllerStick = "motioncontroller",
    MagicWindow = "magicwindow"
}

interface AvatarLocalEvents {
    avatarmoved: AvatarMovedEvent;
}

export class AvatarLocal
    extends TypedEventBase<AvatarLocalEvents>
    implements ErsatzObject, IDisposable {
    private controlMode = CameraControlMode.None;

    private requiredMouseButton = new Map<CameraControlMode, MouseButtons>([
        [CameraControlMode.MouseDrag, MouseButtons.Mouse0],
        [CameraControlMode.Touch, MouseButtons.Mouse0]
    ]);


    private _heading = 0;
    private _pitch = 0;
    private _roll = 0;

    private fwrd = false;
    private back = false;
    private left = false;
    private rgth = false;
    private up = false;
    private down = false;
    private grow = false;
    private shrk = false;

    private readonly viewEuler = new THREE.Euler();
    private readonly move = new THREE.Vector3();

    private readonly followers = new Array<BodyFollower>();

    readonly head: THREE.Object3D;

    readonly worldPos = new THREE.Vector3();

    private _worldHeading: number = 0;
    private _worldPitch: number = 0;

    get worldHeading() {
        return this._worldHeading;
    }

    get worldPitch() {
        return this._worldPitch;
    }

    evtSys: EventSystem = null;
    requiredTouchCount = 1;
    disableHorizontal = false;
    disableVertical = false;
    invertHorizontal = false;
    invertVertical = true;

    minimumX = -85 * Math.PI / 180;
    maximumX = 85 * Math.PI / 180;

    target = new THREE.Quaternion(0, 0, 0, 1);

    edgeFactor = 1 / 3;
    accelerationX = 2;
    accelerationY = 2;
    speedX = 3;
    speedY = 2;

    private deviceQ = new THREE.Quaternion().identity();

    private _height: number;
    get height(): number {
        return this.head.position.y;
    }

    private u: number = 0;
    private v: number = 0;
    private du: number = 0;
    private dv: number = 0;

    get object() {
        return this.head;
    }

    private _keyboardControlEnabled = false;
    private readonly onKeyDown: (evt: any) => void;
    private readonly onKeyUp: (evt: any) => void;

    private get stage() {
        return this.head.parent;
    }

    constructor(private readonly renderer: THREE.WebGLRenderer,
        private readonly camera: THREE.PerspectiveCamera,
        fader: Fader,
        defaultAvatarHeight: number) {
        super();

        this._height = defaultAvatarHeight;
        this.head = obj("Head", fader);

        this.onKeyDown = (evt) => {
            const ok = isModifierless(evt);
            if (evt.key === "w") this.fwrd = ok;
            if (evt.key === "s") this.back = ok;
            if (evt.key === "a") this.left = ok;
            if (evt.key === "d") this.rgth = ok;
            if (evt.key === "e") this.up = ok;
            if (evt.key === "q") this.down = ok;

            if (evt.key === "r") this.grow = ok;
            if (evt.key === "f") this.shrk = ok;
        };

        this.onKeyUp = (evt) => {
            if (evt.key === "w") this.fwrd = false;
            if (evt.key === "s") this.back = false;
            if (evt.key === "a") this.left = false;
            if (evt.key === "d") this.rgth = false;
            if (evt.key === "e") this.up = false;
            if (evt.key === "q") this.down = false;

            if (evt.key === "r") this.grow = false;
            if (evt.key === "f") this.shrk = false;
        };

        this.keyboardControlEnabled = true;

        if (isMobileVR()) {
            this.controlMode = CameraControlMode.MotionControllerStick;
        }
        else if (matchMedia("(pointer: coarse)").matches) {
            this.controlMode = CameraControlMode.Touch;
        }
        else if (matchMedia("(pointer: fine)").matches) {
            this.controlMode = CameraControlMode.MouseDrag;
        }

        if (globalThis.isSecureContext && isMobile() && !isMobileVR()) {
            this.onDeviceOrientationChangeEvent = (event: DeviceOrientationEvent) => {
                this.deviceOrientation = event;
                if (event
                    && (event.alpha || event.beta || event.gamma)
                    && this.motionEnabled) {
                    this.controlMode = CameraControlMode.MagicWindow;
                }
            };

            this.onScreenOrientationChangeEvent = () => {
                if (!isString(globalThis.orientation)) {
                    this.screenOrientation = globalThis.orientation || 0;
                }
            };

            this.startMotionControl();
        }
    }

    onFlick(direction: number) {
        nextFlick.set(MOTIONCONTROLLER_STICK_SENSITIVITY_SCALE * direction, 0, 0);
    }

    get keyboardControlEnabled(): boolean {
        return this._keyboardControlEnabled;
    }

    set keyboardControlEnabled(v: boolean) {
        if (this._keyboardControlEnabled !== v) {
            this._keyboardControlEnabled = v;
            if (this._keyboardControlEnabled) {
                globalThis.addEventListener("keydown", this.onKeyDown);
                globalThis.addEventListener("keyup", this.onKeyUp);
            }
            else {
                globalThis.removeEventListener("keydown", this.onKeyDown);
                globalThis.removeEventListener("keyup", this.onKeyUp);
            }
        }
    }

    addFollower(follower: BodyFollower) {
        this.followers.push(follower);
    }

    onDown(evt: EventSystemEvent<"down">) {
        if (evt.pointer.enabled) {
            this.setMode(evt);
        }
    }

    onMove(evt: EventSystemEvent<"move">) {
        if (evt.pointer.enabled) {
            this.setMode(evt);

            if (evt.pointer.canMoveView
                && this.checkMode(this.controlMode, evt)) {
                this.u = evt.pointer.state.u;
                this.v = evt.pointer.state.v;
                this.du = evt.pointer.state.du;
                this.dv = evt.pointer.state.dv;
            }
        }
    }

    private setMode(evt: EventSystemEvent<string>) {
        if (evt.pointer.type === "mouse") {
            if (evt.pointer.draggedHit) {
                this.controlMode = CameraControlMode.MouseScreenEdge;
            }
            else {
                this.controlMode = CameraControlMode.MouseDrag;
            }
        }
        else if (evt.pointer.type === "touch"
            || evt.pointer.type === "pen") {
            this.controlMode = CameraControlMode.Touch;
        }
        else if (evt.pointer.type === "gamepad") {
            this.controlMode = CameraControlMode.Gamepad;
        }
        else if (evt.pointer.type === "hand") {
            this.controlMode = CameraControlMode.MotionControllerStick;
        }
        else {
            this.controlMode = CameraControlMode.None;
        }
    }

    private checkMode(mode: CameraControlMode, evt: EventSystemEvent<string>) {
        return mode !== CameraControlMode.None
            && this.gestureSatisfied(mode, evt)
            && this.dragSatisfied(mode, evt);
    }

    private gestureSatisfied(mode: CameraControlMode, evt: EventSystemEvent<string>) {
        const button = this.requiredMouseButton.get(mode);
        if (isNullOrUndefined(button)) {
            return mode === CameraControlMode.MouseScreenEdge
                || mode === CameraControlMode.Touch
                || mode === CameraControlMode.Gamepad;
        }
        else {
            return evt.pointer.state.buttons === button;
        }
    }

    private dragSatisfied(mode: CameraControlMode, evt: EventSystemEvent<string>) {
        return !this.requiredMouseButton.has(mode)
            || this.requiredMouseButton.get(mode) == MouseButtons.None
            || evt.pointer.state.dragging;
    }

    get name(): string {
        return this.object.name;
    }

    set name(v: string) {
        this.object.name = v;
    }

    get heading() {
        return this._heading;
    }

    private setHeading(angle: number) {
        this._heading = angleClamp(angle);
    }

    get pitch() {
        return this._pitch;
    }

    private setPitch(x: number, minX: number, maxX: number) {
        this._pitch = angleClamp(x + Math.PI) - Math.PI;
        this._pitch = clamp(this._pitch, minX, maxX);
    }

    get roll() {
        return this._roll;
    }

    private setRoll(z: number) {
        this._roll = angleClamp(z);
    }

    setHeadingImmediate(heading: number) {
        this.setHeading(heading);
        this.updateOrientation();
        this.resetFollowers();
    }

    setOrientationImmediate(heading: number, pitch: number) {
        this.setHeading(heading);
        this._pitch = angleClamp(pitch);
        this.updateOrientation();
    }

    update(dt: number) {

        dt *= 0.001;

        if (this.controlMode === CameraControlMode.MagicWindow) {
            const device = this.deviceOrientation;

            if (device) {
                const alpha = device.alpha ? deg2rad(device.alpha) + this.alphaOffset : 0;
                const beta = device.beta ? deg2rad(device.beta) : 0;
                const gamma = device.gamma ? deg2rad(device.gamma) : 0;
                const orient = this.screenOrientation ? deg2rad(this.screenOrientation) : 0;
                E.set(beta, alpha, -gamma, "YXZ");
                Q2.setFromAxisAngle(B, -orient);
                this.deviceQ.setFromEuler(E) // orient the device
                    .multiply(Q3) // camera looks out the back of the device, not the top
                    .multiply(Q2); // adjust for screen orientation
            }
        }
        else if (this.controlMode !== CameraControlMode.None) {
            const startPitch = this.pitch;
            const startHeading = this.heading;
            const dQuat = this.orientationDelta(this.controlMode, this.disableVertical, dt);
            this.rotateView(
                dQuat,
                this.minimumX,
                this.maximumX);

            if (this.evtSys) {
                const viewChanged = startPitch !== this.pitch
                    || startHeading !== this.heading;

                if (viewChanged
                    && this.controlMode === CameraControlMode.MouseScreenEdge) {
                    this.evtSys.recheckPointers();
                }
            }
        }

        if (this.fwrd || this.back || this.left || this.rgth || this.up || this.down) {
            Q1.setFromAxisAngle(this.stage.up, this.worldHeading);
            const dx = (this.left ? -1 : 0) + (this.rgth ? 1 : 0);
            const dy = (this.down ? -1 : 0) + (this.up ? 1 : 0);
            const dz = (this.fwrd ? -1 : 0) + (this.back ? 1 : 0);
            this.move.set(dx, dy, dz);
            const d = this.move.length();
            if (d > 0) {
                this.move.multiplyScalar(dt / d)
                    .applyQuaternion(Q1);
                this.stage.position.add(this.move);
            }
        }

        if (this.grow || this.shrk) {
            const dy = (this.shrk ? -1 : 0) + (this.grow ? 1 : 0);
            this._height += dy * dt;
            this._height = clamp(this._height, 1, 2);
        }

        this.updateOrientation();

        userMovedEvt.set(
            P.x, P.y, P.z,
            F.x, F.y, F.z,
            U.x, U.y, U.z,
            this.height);

        this.dispatchEvent(userMovedEvt);
    }

    private rotateView(dQuat: THREE.Quaternion, minX = -Math.PI, maxX = Math.PI) {
        this.viewEuler.setFromQuaternion(dQuat, "YXZ");
        let { x, y } = this.viewEuler;

        this.setHeading(this.heading + y);
        this.setPitch(this.pitch + x, minX, maxX);
        this.setRoll(0);
    }

    private updateOrientation() {
        const cam = resolveCamera(this.renderer, this.camera);

        rotStage.makeRotationY(this._heading);

        this.stage.matrix.makeTranslation(
            this.stage.position.x,
            this.stage.position.y,
            this.stage.position.z)
            .multiply(rotStage);

        this.stage.matrix.decompose(
            this.stage.position,
            this.stage.quaternion,
            this.stage.scale);

        if (this.renderer.xr.isPresenting) {
            M.copy(this.stage.matrixWorld)
                .invert();

            this.head.position.copy(cam.position)
                .applyMatrix4(M);

            this.head.quaternion.copy(this.stage.quaternion)
                .invert()
                .multiply(cam.quaternion);
        }
        else {
            this.head.position.set(0, this._height, 0);

            E.set(this._pitch, 0, this._roll, "XYZ");
            this.head.quaternion.setFromEuler(E)
                .premultiply(this.deviceQ);
        }


        this.camera.position.copy(this.head.position);
        this.camera.quaternion.copy(this.head.quaternion);

        this.head.getWorldPosition(this.worldPos);
        this.head.getWorldDirection(F);
        this._worldHeading = getLookHeading(F);
        this._worldPitch = getLookPitch(F);
        setRightUpFwdPosFromMatrix(this.head.matrixWorld, R, U, F, P);
    }

    reset() {
        this.stage.position.set(0, 0, 0);
        this.setHeadingImmediate(0);
    }

    private resetFollowers() {
        for (const follower of this.followers) {
            follower.reset(this.height, this.worldPos, this.worldHeading);
        }
    }

    private orientationDelta(mode: CameraControlMode, disableVertical: boolean, dt: number) {
        var move = this.pointerMovement(mode);

        if (this.controlMode === CameraControlMode.MouseDrag
            || this.controlMode === CameraControlMode.Touch) {
            const factor = Math.pow(0.95, 100 * dt);
            this.du = truncate(factor * this.du);
            this.dv = truncate(factor * this.dv);
        }

        if (disableVertical) {
            move.x = 0;
        }
        else if (this.invertVertical) {
            move.x *= -1;
        }

        if (this.disableHorizontal) {
            move.y = 0;
        }
        else if (this.invertHorizontal) {
            move.y *= -1;
        }

        if (mode !== CameraControlMode.MotionControllerStick) {
            move.multiplyScalar(dt);
        }

        E.set(move.y, move.x, 0, "YXZ");
        deltaQuat.setFromEuler(E);

        return deltaQuat;
    }

    private pointerMovement(mode: CameraControlMode): THREE.Vector3 {
        switch (mode) {
            case CameraControlMode.MouseDrag:
                return this.getAxialMovement(MOUSE_SENSITIVITY_SCALE);

            case CameraControlMode.MouseFPS:
                return this.getAxialMovement(MOUSE_SENSITIVITY_SCALE);

            case CameraControlMode.Touch:
                return this.getAxialMovement(TOUCH_SENSITIVITY_SCALE);

            case CameraControlMode.Gamepad:
                return this.getAxialMovement(GAMEPAD_SENSITIVITY_SCALE);

            case CameraControlMode.MouseScreenEdge:
                return this.getRadiusMovement();

            case CameraControlMode.MotionControllerStick:
                motion.copy(nextFlick);
                nextFlick.set(0, 0, 0);
                return motion;

            case CameraControlMode.None:
            case CameraControlMode.MagicWindow:
                return motion.set(0, 0, 0);

            default: assertNever(mode);
        }
    }

    private getAxialMovement(sense: number): THREE.Vector3 {
        motion.set(
            -sense * this.du,
            sense * this.dv,
            0);

        return motion;
    }

    private getRadiusMovement() {
        motion.set(
            this.scaleRadialComponent(this.u, this.speedX, this.accelerationX),
            this.scaleRadialComponent(-this.v, this.speedY, this.accelerationY),
            0);

        return motion;
    }

    private scaleRadialComponent(n: number, dn: number, ddn: number) {
        const absN = Math.abs(n);
        return Math.sign(n) * Math.pow(Math.max(0, absN - this.edgeFactor) / (1 - this.edgeFactor), ddn) * dn;
    }

    private deviceOrientation: DeviceOrientationEvent = null;
    private screenOrientation = 0;
    private alphaOffset = 0;

    private onDeviceOrientationChangeEvent: (event: DeviceOrientationEvent) => void = null;
    private onScreenOrientationChangeEvent: () => void = null;

    private motionEnabled = false;

    private async getPermission(): Promise<PermissionState | "not-supported"> {
        if (!("DeviceOrientationEvent" in globalThis)) {
            return "not-supported";
        }

        // iOS 13+
        if (isPermissionedDeviceOrientationEvent(DeviceOrientationEvent)) {
            return await DeviceOrientationEvent.requestPermission();
        }

        return "granted";
    }

    async startMotionControl() {
        if (!this.motionEnabled) {
            this.onScreenOrientationChangeEvent(); // run once on load
            const permission = await this.getPermission();
            this.motionEnabled = permission === "granted";
            if (this.motionEnabled) {
                globalThis.addEventListener("orientationchange", this.onScreenOrientationChangeEvent);
                globalThis.addEventListener("deviceorientation", this.onDeviceOrientationChangeEvent);
            }
        }
    }

    stopMotionControl() {
        if (this.motionEnabled) {
            globalThis.removeEventListener("orientationchange", this.onScreenOrientationChangeEvent);
            globalThis.removeEventListener("deviceorientation", this.onDeviceOrientationChangeEvent);
            this.motionEnabled = false;
        }
    }

    dispose() {
        this.stopMotionControl();
    }
}