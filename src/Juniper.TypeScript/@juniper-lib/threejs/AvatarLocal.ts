import { isModifierless } from "@juniper-lib/dom/evts";
import { AvatarMovedEvent } from "@juniper-lib/threejs/eventSystem/AvatarMovedEvent";
import { MouseButtons } from "@juniper-lib/threejs/eventSystem/MouseButton";
import { angleClamp, assertNever, clamp, deg2rad, IDisposable, isFunction, isMobile, isMobileVR, isNullOrUndefined, isString, TypedEventBase } from "@juniper-lib/tslib";
import type { BodyFollower } from "./animation/BodyFollower";
import { getLookHeading, getLookPitch } from "./animation/lookAngles";
import { BaseScreenPointer } from "./eventSystem/BaseScreenPointer";
import type { EventSystem } from "./eventSystem/EventSystem";
import { IPointer } from "./eventSystem/IPointer";
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



    private readonly B = new THREE.Vector3(0, 0, 1);
    private readonly R = new THREE.Vector3();
    private readonly F = new THREE.Vector3();
    private readonly U = new THREE.Vector3();
    private readonly P = new THREE.Vector3();
    private readonly M = new THREE.Matrix4();
    private readonly E = new THREE.Euler();
    private readonly Q1 = new THREE.Quaternion();
    private readonly Q2 = new THREE.Quaternion();
    private readonly Q3 = new THREE.Quaternion(- Math.sqrt(0.5), 0, 0, Math.sqrt(0.5)); // - PI/2 around the x-axis
    private readonly motion = new THREE.Vector2();
    private readonly rotStage = new THREE.Matrix4();
    private readonly userMovedEvt = new AvatarMovedEvent();



    private _heading = 0;
    private _pitch = 0;
    private _roll = 0;

    private headX = 0;
    private headZ = 0;

    private fwrd = false;
    private back = false;
    private left = false;
    private rght = false;
    private fwrd2 = false;
    private back2 = false;
    private left2 = false;
    private rght2 = false;
    private up = false;
    private down = false;
    private grow = false;
    private shrk = false;

    private readonly move = new THREE.Vector3();
    private readonly move2 = new THREE.Vector3();

    private readonly followers = new Array<BodyFollower>();

    readonly head: THREE.Object3D;

    readonly worldPos = new THREE.Vector3();

    private _worldHeading: number = 0;
    private _worldPitch: number = 0;
    private lastTouchInputTime: number = Number.MIN_SAFE_INTEGER;

    get worldHeading() {
        return this._worldHeading;
    }

    get worldPitch() {
        return this._worldPitch;
    }

    evtSys: EventSystem = null;
    requiredTouchCount = 1;
    private _disableHorizontal: boolean;
    private _disableVertical: boolean;
    private _invertHorizontal: boolean;
    private _invertVertical: boolean;
    private axisControl = new THREE.Vector2(0, 0);

    set disableVertical(v: boolean) {
        this._disableVertical = v;
        this.axisControl.x = this._disableVertical ? 0 : this._invertVertical ? 1 : -1;
    }

    set invertVertical(v: boolean) {
        this._invertVertical = v;
        this.axisControl.x = this._disableVertical ? 0 : this._invertVertical ? 1 : -1;
    }

    set disableHorizontal(v: boolean) {
        this._disableHorizontal = v;
        this.axisControl.y = this._disableHorizontal ? 0 : this._invertHorizontal ? 1 : -1;
    }

    set invertHorizontal(v: boolean) {
        this._invertHorizontal = v;
        this.axisControl.y = this._disableHorizontal ? 0 : this._invertHorizontal ? 1 : -1;
    }


    minimumX = -85 * Math.PI / 180;
    maximumX = 85 * Math.PI / 180;

    target = new THREE.Quaternion(0, 0, 0, 1);

    edgeFactor = 1 / 3;
    private readonly acceleration = new THREE.Vector2(2, 2);
    private readonly speed = new THREE.Vector2(3, 2);

    private deviceQ = new THREE.Quaternion().identity();

    private _height: number;
    get height(): number {
        return this.head.position.y;
    }

    private readonly uv = new THREE.Vector2();
    private readonly duv = new THREE.Vector2();

    get object() {
        return this.head;
    }

    private _keyboardControlEnabled = false;
    private readonly onKeyDown: (evt: any) => void;
    private readonly onKeyUp: (evt: any) => void;

    get stage() {
        return this.head.parent;
    }

    constructor(private readonly renderer: THREE.WebGLRenderer,
        private readonly camera: THREE.PerspectiveCamera,
        fader: Fader,
        defaultAvatarHeight: number) {
        super();
        this.disableHorizontal = false;
        this.disableVertical = false;
        this.invertHorizontal = false;
        this.invertVertical = true;

        this._height = defaultAvatarHeight;
        this.head = obj("Head", fader);

        const setKey = (key: string, ok: boolean) => {
            if (key === "w") this.fwrd = ok;
            if (key === "s") this.back = ok;
            if (key === "a") this.left = ok;
            if (key === "d") this.rght = ok;
            if (key === "e") this.up = ok;
            if (key === "q") this.down = ok;
            if (key === "ArrowUp") this.fwrd2 = ok;
            if (key === "ArrowDown") this.back2 = ok;
            if (key === "ArrowLeft") this.left2 = ok;
            if (key === "ArrowRight") this.rght2 = ok;
            if (key === "r") this.grow = ok;
            if (key === "f") this.shrk = ok;
        };

        this.onKeyDown = (evt: KeyboardEvent) => setKey(evt.key, isModifierless(evt));
        this.onKeyUp = (evt: KeyboardEvent) => setKey(evt.key, false);

        this.keyboardControlEnabled = true;

        if (matchMedia("(pointer: coarse)").matches) {
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
                    const dt = performance.now() - this.lastTouchInputTime;
                    if (dt > 1000) {
                        this.controlMode = CameraControlMode.MagicWindow;
                    }
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

    snapTurn(direction: number) {
        this.setHeading(this.heading + MOTIONCONTROLLER_STICK_SENSITIVITY_SCALE * direction);
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

    onMove(pointer: BaseScreenPointer, uv: THREE.Vector2, duv: THREE.Vector2) {
        this.setMode(pointer);
        if (pointer.canMoveView
            && this.controlMode !== CameraControlMode.None
            && this.gestureSatisfied(pointer)
            && this.dragSatisfied(pointer)) {
            this.uv.copy(uv);
            this.duv.copy(duv);
        }
    }

    setMode(pointer: IPointer) {
        if (pointer.type === "touch" || pointer.type === "pen") {
            this.lastTouchInputTime = performance.now();
            this.controlMode = CameraControlMode.Touch;
        }
        else if (pointer.type === "gamepad") {
            this.controlMode = CameraControlMode.Gamepad;
        }
        else if (pointer.type !== "mouse") {
            this.controlMode = CameraControlMode.None;
        }
        else if (this.evtSys.mouse.isPointerLocked) {
            this.controlMode = CameraControlMode.MouseFPS;
        }
        else if (pointer.draggedHit) {
            this.controlMode = CameraControlMode.MouseScreenEdge;
        }
        else {
            this.controlMode = CameraControlMode.MouseDrag;
        }
    }

    private gestureSatisfied(pointer: IPointer) {
        const button = this.requiredMouseButton.get(this.controlMode);
        if (isNullOrUndefined(button)) {
            return this.controlMode === CameraControlMode.MouseScreenEdge
                || this.controlMode === CameraControlMode.MouseFPS
                || this.controlMode === CameraControlMode.Touch
                || this.controlMode === CameraControlMode.Gamepad;
        }
        else {
            return pointer.buttons === button;
        }
    }

    private dragSatisfied(pointer: IPointer) {
        return !this.requiredMouseButton.has(this.controlMode)
            || this.requiredMouseButton.get(this.controlMode) == MouseButtons.None
            || pointer.dragging;
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
                this.E.set(beta, alpha, -gamma, "YXZ");
                this.Q2.setFromAxisAngle(this.B, -orient);
                this.deviceQ
                    .setFromEuler(this.E) // orient the device
                    .multiply(this.Q3) // camera looks out the back of the device, not the top
                    .multiply(this.Q2); // adjust for screen orientation
            }
        }
        else if (this.controlMode === CameraControlMode.MouseScreenEdge) {
            if (this.uv.manhattanLength() > 0) {
                this.motion
                    .set(
                        this.scaleRadialComponent(-this.uv.x, this.speed.x, this.acceleration.x),
                        this.scaleRadialComponent(this.uv.y, this.speed.y, this.acceleration.y))
                    .multiplyScalar(dt);

                this.setHeading(this.heading + this.motion.x);
                this.setPitch(this.pitch + this.motion.y, this.minimumX, this.maximumX);
                this.setRoll(0);
            }
        }
        else if (this.controlMode === CameraControlMode.MouseFPS) {
            if (this.duv.manhattanLength() > 0) {
                this.motion
                    .copy(this.duv)
                    .multiply(this.axisControl);
                this.setHeading(this.heading + this.motion.x);
                this.setPitch(this.pitch + this.motion.y, this.minimumX, this.maximumX);
                this.setRoll(0);
            }
        }
        else if (this.controlMode !== CameraControlMode.None) {
            if (this.duv.manhattanLength() > 0) {
                const sensitivity = this.controlMode === CameraControlMode.MouseDrag
                    ? MOUSE_SENSITIVITY_SCALE
                    : this.controlMode === CameraControlMode.Touch
                        ? TOUCH_SENSITIVITY_SCALE
                        : this.controlMode === CameraControlMode.Gamepad
                            ? GAMEPAD_SENSITIVITY_SCALE
                            : assertNever(this.controlMode);

                this.motion
                    .copy(this.duv)
                    .multiplyScalar(sensitivity * dt)
                    .multiply(this.axisControl);

                this.setHeading(this.heading + this.motion.x);
                this.setPitch(this.pitch + this.motion.y, this.minimumX, this.maximumX);
                this.setRoll(0);
            }
        }

        this.Q1.setFromAxisAngle(this.stage.up, this.worldHeading);

        if (this.fwrd || this.back || this.left || this.rght || this.up || this.down) {
            const dx = (this.left ? -1 : 0) + (this.rght ? 1 : 0);
            const dy = (this.down ? -1 : 0) + (this.up ? 1 : 0);
            const dz = (this.fwrd ? -1 : 0) + (this.back ? 1 : 0);
            this.move.set(dx, dy, dz);
            const d = this.move.length();
            if (d > 0) {
                this.move.multiplyScalar(dt / d)
                    .applyQuaternion(this.Q1);
                this.stage.position.add(this.move);
            }
        }

        if (this.fwrd2 || this.back2 || this.left2 || this.rght2) {
            const dx = (this.left2 ? -1 : 0) + (this.rght2 ? 1 : 0);
            const dz = (this.fwrd2 ? -1 : 0) + (this.back2 ? 1 : 0);
            this.move2.set(dx, 0, dz);
            const d = this.move2.length();
            if (d > 0) {
                this.move2.multiplyScalar(dt / d)
                    .applyQuaternion(this.Q1);

                this.headX += this.move2.x;
                this.headZ += this.move2.y;
            }
        }

        if (this.grow || this.shrk) {
            const dy = (this.shrk ? -1 : 0) + (this.grow ? 1 : 0);
            this._height += dy * dt;
            this._height = clamp(this._height, 1, 2);
        }

        this.updateOrientation();

        this.userMovedEvt.set(
            this.P.x, this.P.y, this.P.z,
            this.F.x, this.F.y, this.F.z,
            this.U.x, this.U.y, this.U.z,
            this.height);

        this.dispatchEvent(this.userMovedEvt);

        const decay = Math.pow(0.95, 100 * dt);
        this.duv.multiplyScalar(decay);
        if (this.duv.manhattanLength() <= 0.0001) {
            this.duv.setScalar(0);
        }
    }

    private scaleRadialComponent(n: number, dn: number, ddn: number) {
        const absN = Math.abs(n);
        return Math.sign(n) * Math.pow(Math.max(0, absN - this.edgeFactor) / (1 - this.edgeFactor), ddn) * dn;
    }

    private updateOrientation() {
        const cam = resolveCamera(this.renderer, this.camera);

        this.rotStage.makeRotationY(this._heading);

        this.stage.matrix.makeTranslation(
            this.stage.position.x,
            this.stage.position.y,
            this.stage.position.z)
            .multiply(this.rotStage);

        this.stage.matrix.decompose(
            this.stage.position,
            this.stage.quaternion,
            this.stage.scale);

        if (this.renderer.xr.isPresenting) {
            this.M.copy(this.stage.matrixWorld)
                .invert();

            this.head.position.copy(cam.position)
                .applyMatrix4(this.M);

            this.head.quaternion.copy(this.stage.quaternion)
                .invert()
                .multiply(cam.quaternion);
        }
        else {
            this.head.position.set(this.headX, this._height, this.headZ);

            this.E.set(this._pitch, 0, this._roll, "XYZ");
            this.head.quaternion.setFromEuler(this.E)
                .premultiply(this.deviceQ);
        }


        this.camera.position.copy(this.head.position);
        this.camera.quaternion.copy(this.head.quaternion);

        this.head.getWorldPosition(this.worldPos);
        this.head.getWorldDirection(this.F);
        this._worldHeading = getLookHeading(this.F);
        this._worldPitch = getLookPitch(this.F);
        setRightUpFwdPosFromMatrix(this.head.matrixWorld, this.R, this.U, this.F, this.P);
    }

    reset() {
        this.stage.position.setScalar(0);
        this.setHeadingImmediate(0);
    }

    private resetFollowers() {
        for (const follower of this.followers) {
            follower.reset(this.height, this.worldPos, this.worldHeading);
        }
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