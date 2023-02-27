import { isModifierless } from "@juniper-lib/dom/evts";
import { onUserGesture } from "@juniper-lib/dom/onUserGesture";
import { arrayClear } from "@juniper-lib/tslib/collections/arrays";
import { TypedEvent, TypedEventBase } from "@juniper-lib/tslib/events/EventBase";
import { isMobile, isMobileVR, isSafari } from "@juniper-lib/tslib/flags";
import { clamp, deg2rad, HalfPi, Pi, radiansClamp, truncate } from "@juniper-lib/tslib/math";
import { assertNever, isFunction, isGoodNumber } from "@juniper-lib/tslib/typeChecks";
import { IDisposable } from "@juniper-lib/tslib/using";
import { Euler, Matrix4, Object3D, Quaternion, Vector2, Vector3 } from "three";
import type { BodyFollower } from "./animation/BodyFollower";
import { getLookHeadingRadians, getLookPitchRadians } from "./animation/lookAngles";
import { BufferReaderWriter } from "./BufferReaderWriter";
import { BaseEnvironment } from "./environment/BaseEnvironment";
import { IPointer } from "./eventSystem/devices/IPointer";
import { VirtualButton } from "./eventSystem/devices/VirtualButton";
import type { Fader } from "./Fader";
import { ErsatzObject, obj } from "./objects";
import { resolveCamera } from "./resolveCamera";
import { setUpFwdPosFromMatrix } from "./setUpFwdPosFromMatrix";

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
    ScreenEdge = "mouseedge",
    Touch = "touchswipe",
    Gamepad = "gamepad"
}

export class AvatarResetEvent extends TypedEvent<"avatarreset">{
    constructor() {
        super("avatarreset");
    }
}

interface AvatarLocalEvents {
    avatarreset: AvatarResetEvent;
}

export class AvatarLocal
    extends TypedEventBase<AvatarLocalEvents>
    implements ErsatzObject, IDisposable {

    private readonly avatarResetEvt = new AvatarResetEvent();
    private controlMode = CameraControlMode.None;

    private snapTurnRadians = deg2rad(30);

    private readonly sensitivities = new Map<CameraControlMode, number>([
        /**
         * The mouse is not as sensitive as the gamepad, so we have to bump up the
         * sensitivity quite a bit.
         **/
        [CameraControlMode.MouseDrag, 100],
        [CameraControlMode.MouseFPS, -100],
        /**
         * The touch points are not as sensitive as the gamepad, so we have to bump up the
         * sensitivity quite a bit.
         **/
        [CameraControlMode.Touch, 50],
        [CameraControlMode.Gamepad, 1]
    ]);

    private readonly B = new Vector3(0, 0, 1);
    private readonly F = new Vector3();
    private readonly U = new Vector3();
    private readonly P = new Vector3();
    private readonly M = new Matrix4();
    private readonly E = new Euler();
    private readonly Q1 = new Quaternion();
    private readonly Q2 = new Quaternion();
    private readonly Q3 = new Quaternion(- Math.sqrt(0.5), 0, 0, Math.sqrt(0.5)); // - PI/2 around the x-axis
    private readonly Q4 = new Quaternion();
    private readonly motion = new Vector2();
    private readonly rotStage = new Matrix4();
    private readonly axisControl = new Vector2(0, 0);
    private readonly deviceQ = new Quaternion().identity();
    private readonly uv = new Vector2();
    private readonly duv = new Vector2();
    private readonly move = new Vector3();
    private readonly move2 = new Vector3();
    private readonly radialAcceleration = new Vector2(1.5, 1.5);
    private readonly radialSpeed = new Vector2(1, 1);
    private readonly radialEdgeFactor = 3 / 4;
    private readonly minFOVDegrees = 15;
    private readonly maxFOVDegrees = 120;
    private readonly minPitchRadians = deg2rad(-85);
    private readonly maxPitchRadians = deg2rad(85);
    private readonly followers = new Array<BodyFollower>();
    private readonly onKeyDown: (evt: KeyboardEvent) => void;
    private readonly onKeyUp: (evt: KeyboardEvent) => void;

    private dz = 0;
    private _headingRadians = 0;
    private _pitchRadians = 0;
    private _rollRadians = 0;
    private headX = 0;
    private headZ = 0;
    private _worldHeadingRadians = 0;
    private _worldPitchRadians = 0;
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
    private _keyboardControlEnabled = false;

    private _height: number;
    private _disableHorizontal: boolean;
    private _disableVertical: boolean;
    private _invertHorizontal: boolean;
    private _invertVertical: boolean;

    readonly head: Object3D;

    readonly worldPos = new Vector3();
    readonly worldQuat = new Quaternion()

    lockMovement = false;
    fovZoomEnabled = true;


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

    get height(): number {
        return this.head.position.y;
    }

    get object() {
        return this.head;
    }

    get worldHeadingRadians() {
        return this._worldHeadingRadians;
    }

    get worldPitchRadians() {
        return this._worldPitchRadians;
    }

    private get fov() {
        return this.env.camera.fov;
    }

    private set fov(v) {
        if (v !== this.fov) {
            this.env.camera.fov = v;
            this.env.camera.updateProjectionMatrix();
        }
    }

    get stage() {
        return this.head.parent;
    }

    constructor(private readonly env: BaseEnvironment, fader: Fader) {
        super();
        this.disableHorizontal = false;
        this.disableVertical = false;
        this.invertHorizontal = false;
        this.invertVertical = true;

        this._height = this.env.defaultAvatarHeight;
        this.head = obj("Head", fader);

        let homeHit = false;
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
            if (key === "Home") {
                const wasHome = homeHit;
                homeHit = ok;
                if (wasHome && !homeHit) {
                    this.reset();
                    this.dispatchEvent(this.avatarResetEvt);
                }
            }
        };

        this.onKeyDown = (evt) => setKey(evt.key, isModifierless(evt));
        this.onKeyUp = (evt) => setKey(evt.key, false);

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
            };

            this.onScreenOrientationChangeEvent = () => {
                if ("screen" in globalThis && "orientation" in screen) {
                    this.screenOrientation = screen.orientation.angle;
                }
                else if ("window" in globalThis && "orientation" in globalThis.window) {
                    // @ts-ignore deprecated "orientation" access
                    this.screenOrientation = globalThis.window.orientation || 0;
                }
            };

            // Safar is a bit of a point in the butt on this issue.
            if (isSafari) {
                onUserGesture(() => this.startMotionControl());
            }
            else {
                this.startMotionControl();
            }
        }
    }

    snapTurn(direction: number) {
        this.setHeading(this.headingRadians - this.snapTurnRadians * direction);
    }

    get keyboardControlEnabled(): boolean {
        return this._keyboardControlEnabled;
    }

    set keyboardControlEnabled(v: boolean) {
        if (this._keyboardControlEnabled !== v) {
            this._keyboardControlEnabled = v;
            if (this._keyboardControlEnabled) {
                this.env.renderer.domElement.addEventListener("keydown", this.onKeyDown);
                this.env.renderer.domElement.addEventListener("keyup", this.onKeyUp);
            }
            else {
                this.env.renderer.domElement.removeEventListener("keydown", this.onKeyDown);
                this.env.renderer.domElement.removeEventListener("keyup", this.onKeyUp);
            }
        }
    }

    addFollower(follower: BodyFollower) {
        this.followers.push(follower);
    }

    onMove(pointer: IPointer, uv: Vector2, duv: Vector2) {
        this.setMode(pointer);
        if (pointer.canMoveView
            && this.controlMode !== CameraControlMode.None
            && this.gestureSatisfied(pointer)) {
            this.uv.copy(uv);
            this.duv.copy(duv);
        }
    }

    setMode(pointer: IPointer) {
        if (pointer.type === "remote"
            || pointer.type === "nose") {
            // never change control mode for remote or nose pointers
        }
        else if (pointer.type === "hand") {
            this.controlMode = CameraControlMode.None;
        }
        else if (pointer.type === "gamepad") {
            this.controlMode = CameraControlMode.Gamepad;
        }
        else if (pointer.rayTarget
            && pointer.rayTarget.draggable
            && !this.env.eventSys.mouse.isPointerLocked
            && pointer.isPressed(VirtualButton.Primary)) {
            this.controlMode = CameraControlMode.ScreenEdge;
        }
        else if (pointer.type === "touch" || pointer.type === "pen") {
            this.controlMode = CameraControlMode.Touch;
        }
        else if (pointer.type === "mouse") {
            this.controlMode = this.env.eventSys.mouse.isPointerLocked
                ? CameraControlMode.MouseFPS
                : CameraControlMode.MouseDrag;
        }
        else {
            assertNever(pointer.type);
        }
    }

    private gestureSatisfied(pointer: IPointer) {
        if (this.controlMode === CameraControlMode.None) {
            return false;
        }

        return this.controlMode === CameraControlMode.Gamepad
            || this.controlMode === CameraControlMode.MouseFPS
            || this.controlMode === CameraControlMode.ScreenEdge
            || pointer.isPressed(VirtualButton.Primary);
    }

    get name(): string {
        return this.object.name;
    }

    set name(v: string) {
        this.object.name = v;
    }

    get headingRadians() {
        return this._headingRadians;
    }

    private setHeading(radians: number) {
        this._headingRadians = radiansClamp(radians);
    }

    get pitchRadians() {
        return this._pitchRadians;
    }

    private setPitch(radians: number, min: number, max: number) {
        this._pitchRadians = radiansClamp(radians + Pi) - Pi;
        this._pitchRadians = clamp(this._pitchRadians, min, max);
    }

    get rollRadians() {
        return this._rollRadians;
    }

    private setRoll(radians: number) {
        this._rollRadians = radiansClamp(radians);
    }

    setHeadingImmediate(radians: number) {
        this.setHeading(radians);
        this.updateOrientation();
        this.resetFollowers();
    }

    setOrientationImmediate(headingRadians: number, pitchRadians: number) {
        this.setHeading(headingRadians);
        this._pitchRadians = radiansClamp(pitchRadians);
        this.updateOrientation();
    }

    zoom(dz: number) {
        this.dz = dz;
    }

    update(dt: number) {
        dt *= 0.001;

        const device = this.deviceOrientation;
        if (device
            && isGoodNumber(device.alpha)
            && isGoodNumber(device.beta)
            && isGoodNumber(device.gamma)) {
            const alpha = deg2rad(device.alpha) + this.alphaOffset;
            const beta = deg2rad(device.beta);
            const gamma = deg2rad(device.gamma);
            const orient = this.screenOrientation ? deg2rad(this.screenOrientation) : 0;
            this.E.set(beta, alpha, -gamma, "YXZ");
            this.Q2.setFromAxisAngle(this.B, -orient);
            this.Q4
                .setFromEuler(this.E) // orient the device
                .multiply(this.Q3) // camera looks out the back of the device, not the top
                .multiply(this.Q2); // adjust for screen orientation
            this.deviceQ.slerp(this.Q4, .8);
        }

        if (!this.lockMovement) {

            if (this.fovZoomEnabled
                && Math.abs(this.dz) > 0) {
                const smoothing = Math.pow(0.95, 5000 * dt);
                this.dz = truncate(smoothing * this.dz);
                this.fov = clamp(this.env.camera.fov - this.dz, this.minFOVDegrees, this.maxFOVDegrees);
            }

            if (this.controlMode === CameraControlMode.ScreenEdge) {
                if (this.uv.manhattanLength() > 0) {
                    this.motion
                        .set(
                            this.scaleRadialComponent(-this.uv.x, this.radialSpeed.x, this.radialAcceleration.x),
                            this.scaleRadialComponent(this.uv.y, this.radialSpeed.y, this.radialAcceleration.y))
                        .multiplyScalar(dt);
                    this.setHeading(this.headingRadians + this.motion.x);
                    this.setPitch(this.pitchRadians + this.motion.y, this.minPitchRadians, this.maxPitchRadians);
                    this.setRoll(0);
                }
            }
            else if (this.sensitivities.has(this.controlMode)) {
                if (this.duv.manhattanLength() > 0) {
                    const sensitivity = this.sensitivities.get(this.controlMode) || 1;
                    this.motion
                        .copy(this.duv)
                        .multiplyScalar(sensitivity * dt)
                        .multiply(this.axisControl);
                    this.setHeading(this.headingRadians + this.motion.x);
                    this.setPitch(this.pitchRadians + this.motion.y, this.minPitchRadians, this.maxPitchRadians);
                    this.setRoll(0);
                }
            }

            this.Q1.setFromAxisAngle(this.stage.up, this.worldHeadingRadians);

            if (this.fwrd || this.back || this.left || this.rght || this.up || this.down) {
                const dx = (this.left ? 1 : 0) + (this.rght ? -1 : 0);
                const dy = (this.down ? 1 : 0) + (this.up ? -1 : 0);
                const dz = (this.fwrd ? 1 : 0) + (this.back ? -1 : 0);
                this.move.set(dx, dy, dz);
                const d = this.move.length();
                if (d > 0) {
                    this.move
                        .multiplyScalar(dt / d)
                        .applyQuaternion(this.Q1);
                    this.stage.position.add(this.move);
                }
            }

            if (this.fwrd2 || this.back2 || this.left2 || this.rght2) {
                const dx = (this.left2 ? 1 : 0) + (this.rght2 ? -1 : 0);
                const dz = (this.fwrd2 ? 1 : 0) + (this.back2 ? -1 : 0);
                this.move2.set(dx, 0, dz);
                const d = this.move2.length();
                if (d > 0) {
                    this.move2
                        .multiplyScalar(dt / d)
                        .applyQuaternion(this.Q1);

                    this.headX += this.move2.x;
                    this.headZ += this.move2.z;
                }
            }

            if (this.grow || this.shrk) {
                const dy = (this.shrk ? -1 : 0) + (this.grow ? 1 : 0);
                this._height += dy * dt;
                this._height = clamp(this._height, 1, 2);
            }

            this.updateOrientation();

            const decay = Math.pow(0.95, 100 * dt);
            this.duv.multiplyScalar(decay);
            if (this.duv.manhattanLength() <= 0.0001) {
                this.duv.setScalar(0);
            }
        }
    }

    private scaleRadialComponent(n: number, dn: number, ddn: number) {
        const absN = Math.abs(n);
        return Math.sign(n) * Math.pow(Math.max(0, absN - this.radialEdgeFactor) / (1 - this.radialEdgeFactor), ddn) * dn;
    }

    lookAt(obj: Object3D) {
        obj.getWorldPosition(this.P);
        this.P.sub(this.worldPos);
        const heading = 3 * HalfPi - Math.atan2(this.P.z, this.P.x);
        const pitch = Math.atan2(this.P.y, this.P.length());
        this.setOrientationImmediate(heading, pitch);
    }

    private updateOrientation() {
        const cam = resolveCamera(this.env.renderer, this.env.camera);

        this.rotStage.makeRotationY(this._headingRadians);

        this.stage.matrix.makeTranslation(
            this.stage.position.x,
            this.stage.position.y,
            this.stage.position.z)
            .multiply(this.rotStage);

        this.stage.matrix.decompose(
            this.stage.position,
            this.stage.quaternion,
            this.stage.scale);

        if (this.env.renderer.xr.isPresenting) {
            this.M.copy(this.stage.matrixWorld)
                .invert();

            this.head.position
                .copy(cam.position)
                .applyMatrix4(this.M);

            this.head.quaternion
                .copy(this.stage.quaternion)
                .invert()
                .multiply(cam.quaternion);
        }
        else {
            this.head.position.set(this.headX, this._height, this.headZ);
            this.E.set(this._pitchRadians, 0, this._rollRadians, "XYZ");
            this.head.quaternion
                .setFromEuler(this.E)
                .premultiply(this.deviceQ);
        }


        this.env.camera.position.copy(this.head.position);
        this.env.camera.quaternion.copy(this.head.quaternion);

        this.head.getWorldPosition(this.worldPos);
        this.head.getWorldQuaternion(this.worldQuat);

        this.F
            .set(0, 0, -1)
            .applyQuaternion(this.worldQuat);

        this._worldHeadingRadians = getLookHeadingRadians(this.F);
        this._worldPitchRadians = getLookPitchRadians(this.F);
        setUpFwdPosFromMatrix(this.head.matrixWorld, this.U, this.F, this.P);
    }

    reset() {
        this.stage.position.setScalar(0);
        this.setHeadingImmediate(0);
    }

    private resetFollowers() {
        for (const follower of this.followers) {
            follower.reset(this.height, this.worldPos, this.worldHeadingRadians);
        }
    }

    private deviceOrientation: DeviceOrientationEvent = null;
    private screenOrientation = 0;
    private alphaOffset = 0;

    private onDeviceOrientationChangeEvent: (event: DeviceOrientationEvent) => void = null;
    private onScreenOrientationChangeEvent: () => void = null;

    private motionEnabled = false;

    private async getPermission(): Promise<PermissionState | "not-supported"> {
        if (!("DeviceOrientationEvent" in window)) {
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
                if ("ScreenOrientation" in window) {
                    screen.orientation.addEventListener("change", this.onScreenOrientationChangeEvent);
                }
                else {
                    (window as Window).addEventListener("orientationchange", this.onScreenOrientationChangeEvent);
                }
                window.addEventListener("deviceorientation", this.onDeviceOrientationChangeEvent);
            }
        }
    }

    stopMotionControl() {
        if (this.motionEnabled) {
            if ("ScreenOrientation" in window) {
                screen.orientation.removeEventListener("change", this.onScreenOrientationChangeEvent);
            }
            else {
                (window as Window).removeEventListener("orientationchange", this.onScreenOrientationChangeEvent);
            }
            window.removeEventListener("deviceorientation", this.onDeviceOrientationChangeEvent);
            this.motionEnabled = false;
        }
    }

    dispose() {
        this.stopMotionControl();
    }

    get bufferSize() {
        //   height = 4 bytes
        // + pose =
        //   16 elements
        // * 4 bytes per element
        // + pointer count = 1 byte
        // = 4 + 16 * 4 + 1
        // = 4 + 64 + 1
        // = 69
        return 133; // pointer count
    }

    private readonly pointersToSend = new Array<IPointer>();

    writeState(buffer: BufferReaderWriter) {
        // count buffer size
        arrayClear(this.pointersToSend);
        let size = this.env.avatar.bufferSize;
        for (const pointer of this.env.eventSys.pointers) {
            if (pointer.canSend) {
                size += pointer.bufferSize;
                this.pointersToSend.push(pointer);
            }
        }
        buffer.length = size;

        // write data to buffer
        buffer.position = 0;

        buffer.writeFloat32(this.height);
        buffer.writeMatrix512(this.stage.matrix);
        buffer.writeMatrix512(this.head.matrixWorld);
        buffer.writeUint8(this.pointersToSend.length);

        for (const pointer of this.pointersToSend) {
            pointer.writeState(buffer);
        }
    }
}