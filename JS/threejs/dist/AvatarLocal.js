import { arrayClear, assertNever, clamp, deg2rad, HalfPi, isFunction, isGoodNumber, Pi, radiansClamp, truncate } from "@juniper-lib/util";
import { isMobile, isMobileVR, isModifierless, isSafari, onUserGesture } from "@juniper-lib/dom";
import { TypedEvent, TypedEventTarget } from "@juniper-lib/events";
import { Euler, Matrix4, Quaternion, Vector2, Vector3 } from "three";
import { getLookHeadingRadians, getLookPitchRadians } from "./animation/lookAngles";
import { VirtualButton } from "./eventSystem/devices/VirtualButton";
import { obj } from "./objects";
import { resolveCamera } from "./resolveCamera";
import { setUpFwdPosFromMatrix } from "./setUpFwdPosFromMatrix";
function isPermissionedDeviceOrientationEvent(obj) {
    return obj === DeviceOrientationEvent
        && "requestPermission" in obj
        && isFunction(obj.requestPermission);
}
var CameraControlMode;
(function (CameraControlMode) {
    CameraControlMode["None"] = "none";
    CameraControlMode["MouseFPS"] = "mousefirstperson";
    CameraControlMode["MouseDrag"] = "mousedrag";
    CameraControlMode["ScreenEdge"] = "mouseedge";
    CameraControlMode["Touch"] = "touchswipe";
    CameraControlMode["Gamepad"] = "gamepad";
})(CameraControlMode || (CameraControlMode = {}));
export class AvatarResetEvent extends TypedEvent {
    constructor() {
        super("avatarreset");
    }
}
export class AvatarLocal extends TypedEventTarget {
    set disableVertical(v) {
        this._disableVertical = v;
        this.axisControl.x = this._disableVertical ? 0 : this._invertVertical ? 1 : -1;
    }
    set invertVertical(v) {
        this._invertVertical = v;
        this.axisControl.x = this._disableVertical ? 0 : this._invertVertical ? 1 : -1;
    }
    set disableHorizontal(v) {
        this._disableHorizontal = v;
        this.axisControl.y = this._disableHorizontal ? 0 : this._invertHorizontal ? 1 : -1;
    }
    set invertHorizontal(v) {
        this._invertHorizontal = v;
        this.axisControl.y = this._disableHorizontal ? 0 : this._invertHorizontal ? 1 : -1;
    }
    get height() {
        return this.head.position.y;
    }
    get content3d() {
        return this.head;
    }
    get worldHeadingRadians() {
        return this._worldHeadingRadians;
    }
    get worldPitchRadians() {
        return this._worldPitchRadians;
    }
    get fov() {
        return this.env.camera.fov;
    }
    set fov(v) {
        if (v !== this.fov) {
            this.env.camera.fov = v;
            this.env.camera.updateProjectionMatrix();
        }
    }
    get stage() {
        return this.head.parent;
    }
    constructor(env, fader) {
        super();
        this.env = env;
        this.avatarResetEvt = new AvatarResetEvent();
        this.controlMode = CameraControlMode.None;
        this.snapTurnRadians = deg2rad(30);
        this.sensitivities = new Map([
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
        this.B = new Vector3(0, 0, 1);
        this.F = new Vector3();
        this.U = new Vector3();
        this.P = new Vector3();
        this.M = new Matrix4();
        this.E = new Euler();
        this.Q1 = new Quaternion();
        this.Q2 = new Quaternion();
        this.Q3 = new Quaternion(-Math.sqrt(0.5), 0, 0, Math.sqrt(0.5)); // - PI/2 around the x-axis
        this.Q4 = new Quaternion();
        this.motion = new Vector2();
        this.rotStage = new Matrix4();
        this.axisControl = new Vector2(0, 0);
        this.deviceQ = new Quaternion().identity();
        this.uv = new Vector2();
        this.duv = new Vector2();
        this.move = new Vector3();
        this.move2 = new Vector3();
        this.radialAcceleration = new Vector2(1.5, 1.5);
        this.radialSpeed = new Vector2(1, 1);
        this.radialEdgeFactor = 3 / 4;
        this.minFOVDegrees = 15;
        this.maxFOVDegrees = 120;
        this.minPitchRadians = deg2rad(-85);
        this.maxPitchRadians = deg2rad(85);
        this.followers = new Array();
        this.dz = 0;
        this._headingRadians = 0;
        this._pitchRadians = 0;
        this._rollRadians = 0;
        this.headX = 0;
        this.headZ = 0;
        this._worldHeadingRadians = 0;
        this._worldPitchRadians = 0;
        this.fwrd = false;
        this.back = false;
        this.left = false;
        this.rght = false;
        this.fwrd2 = false;
        this.back2 = false;
        this.left2 = false;
        this.rght2 = false;
        this.up = false;
        this.down = false;
        this.grow = false;
        this.shrk = false;
        this._keyboardControlEnabled = false;
        this.worldPos = new Vector3();
        this.worldQuat = new Quaternion();
        this.lockMovement = false;
        this.fovZoomEnabled = true;
        this.deviceOrientation = null;
        this.screenOrientation = 0;
        this.alphaOffset = 0;
        this.onDeviceOrientationChangeEvent = null;
        this.onScreenOrientationChangeEvent = null;
        this.motionEnabled = false;
        this.pointersToSend = new Array();
        this.disableHorizontal = false;
        this.disableVertical = false;
        this.invertHorizontal = false;
        this.invertVertical = true;
        this._height = this.env.defaultAvatarHeight;
        this.head = obj("Head", fader);
        let homeHit = false;
        const setKey = (key, ok) => {
            if (key === "w")
                this.fwrd = ok;
            if (key === "s")
                this.back = ok;
            if (key === "a")
                this.left = ok;
            if (key === "d")
                this.rght = ok;
            if (key === "e")
                this.up = ok;
            if (key === "q")
                this.down = ok;
            if (key === "ArrowUp")
                this.fwrd2 = ok;
            if (key === "ArrowDown")
                this.back2 = ok;
            if (key === "ArrowLeft")
                this.left2 = ok;
            if (key === "ArrowRight")
                this.rght2 = ok;
            if (key === "r")
                this.grow = ok;
            if (key === "f")
                this.shrk = ok;
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
            this.onDeviceOrientationChangeEvent = (event) => {
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
    snapTurn(direction) {
        this.setHeading(this.headingRadians - this.snapTurnRadians * direction);
    }
    get keyboardControlEnabled() {
        return this._keyboardControlEnabled;
    }
    set keyboardControlEnabled(v) {
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
    addFollower(follower) {
        this.followers.push(follower);
    }
    onMove(pointer, uv, duv) {
        this.setMode(pointer);
        if (pointer.canMoveView
            && this.controlMode !== CameraControlMode.None
            && this.gestureSatisfied(pointer)) {
            this.uv.copy(uv);
            this.duv.copy(duv);
        }
    }
    setMode(pointer) {
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
    gestureSatisfied(pointer) {
        if (this.controlMode === CameraControlMode.None) {
            return false;
        }
        return this.controlMode === CameraControlMode.Gamepad
            || this.controlMode === CameraControlMode.MouseFPS
            || this.controlMode === CameraControlMode.ScreenEdge
            || pointer.isPressed(VirtualButton.Primary);
    }
    get name() {
        return this.content3d.name;
    }
    set name(v) {
        this.content3d.name = v;
    }
    get headingRadians() {
        return this._headingRadians;
    }
    setHeading(radians) {
        this._headingRadians = radiansClamp(radians);
    }
    get pitchRadians() {
        return this._pitchRadians;
    }
    setPitch(radians, min, max) {
        this._pitchRadians = radiansClamp(radians + Pi) - Pi;
        this._pitchRadians = clamp(this._pitchRadians, min, max);
    }
    get rollRadians() {
        return this._rollRadians;
    }
    setRoll(radians) {
        this._rollRadians = radiansClamp(radians);
    }
    setHeadingImmediate(radians) {
        this.setHeading(radians);
        this.updateOrientation();
        this.resetFollowers();
    }
    setOrientationImmediate(headingRadians, pitchRadians) {
        this.setHeading(headingRadians);
        this._pitchRadians = radiansClamp(pitchRadians);
        this.updateOrientation();
    }
    zoom(dz) {
        this.dz = dz;
    }
    update(dt) {
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
                        .set(this.scaleRadialComponent(-this.uv.x, this.radialSpeed.x, this.radialAcceleration.x), this.scaleRadialComponent(this.uv.y, this.radialSpeed.y, this.radialAcceleration.y))
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
    scaleRadialComponent(n, dn, ddn) {
        const absN = Math.abs(n);
        return Math.sign(n) * Math.pow(Math.max(0, absN - this.radialEdgeFactor) / (1 - this.radialEdgeFactor), ddn) * dn;
    }
    lookAt(obj) {
        obj.getWorldPosition(this.P);
        this.P.sub(this.worldPos);
        const heading = 3 * HalfPi - Math.atan2(this.P.z, this.P.x);
        const pitch = Math.atan2(this.P.y, this.P.length());
        this.setOrientationImmediate(heading, pitch);
    }
    updateOrientation() {
        const cam = resolveCamera(this.env.renderer, this.env.camera);
        this.rotStage.makeRotationY(this._headingRadians);
        this.stage.matrix.makeTranslation(this.stage.position.x, this.stage.position.y, this.stage.position.z)
            .multiply(this.rotStage);
        this.stage.matrix.decompose(this.stage.position, this.stage.quaternion, this.stage.scale);
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
    resetFollowers() {
        for (const follower of this.followers) {
            follower.reset(this.height, this.worldPos, this.worldHeadingRadians);
        }
    }
    async getPermission() {
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
                    window.addEventListener("orientationchange", this.onScreenOrientationChangeEvent);
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
                window.removeEventListener("orientationchange", this.onScreenOrientationChangeEvent);
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
    writeState(buffer) {
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
//# sourceMappingURL=AvatarLocal.js.map