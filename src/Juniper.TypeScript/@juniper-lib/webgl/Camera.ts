import { deg2rad } from "@juniper-lib/tslib/math/deg2rad";
import { rad2deg } from "@juniper-lib/tslib/math/rad2deg";
import { isNumber } from "@juniper-lib/tslib/typeChecks";
import { glMatrix, mat4, quat, vec3 } from "gl-matrix";
import type { Context3D, ResizeEvent } from "./Context3D";


function fovAndSensor2Focal(fov: number, sensor: number) {
    const halfVert = sensor / 3;
    return halfVert / Math.tan(deg2rad(fov));
}

function sensorAndFocal2Fov(sensor: number, focal: number) {
    const halfVert = sensor / 3;
    return rad2deg(Math.atan(halfVert / focal));
}

interface CameraInit {
    near: number;
    far: number;
    fov: number;
    sensorSize: number;
    focalLength: number;
}

const DEFAULT_CAMERA_SETTINGS = {
    near: 0.01,
    far: 100,
    fov: 50,
    sensorSize: 0.036
};

export class Camera {
    private _proj = mat4.create();
    private _view = mat4.create();

    private _heading: number = 0;
    private _pitch: number = 0;

    private _aspect: number;
    private _fov: number;
    private _sensorSize: number = 0.035;
    private _focalLength: number;
    private _near: number;
    private _far: number;

    private _rot: quat = quat.create();
    private _pos: vec3 = vec3.create();

    public gamma = 1;

    constructor(ctx: Context3D, init?: Partial<CameraInit>) {
        ctx.addEventListener("resize", (evt: ResizeEvent) => {
            this._aspect = evt.width / evt.height;
            this.refreshProjection();
        });

        this._aspect = ctx.width / ctx.height;

        const settings = Object.assign({}, DEFAULT_CAMERA_SETTINGS, init);
        this._near = settings.near;
        this._far = settings.far;
        this._fov = settings.fov;
        this._sensorSize = settings.sensorSize;
        this._focalLength = settings.focalLength;

        if (isNumber(init.focalLength)) {
            this._fov = sensorAndFocal2Fov(this._sensorSize, this._focalLength);
        }
        else {
            this._focalLength = fovAndSensor2Focal(this._fov, this._sensorSize);
        }

        this.refreshProjection();
        this.refreshView();
    }

    get view(): mat4 {
        return this._view;
    }

    set view(v: mat4) {
        mat4.copy(this._view, v);
    }

    get projection(): mat4 {
        return this._proj;
    }

    set projection(v: mat4) {
        mat4.copy(this._proj, v);
    }

    get fov(): number {
        return this._fov;
    }

    set fov(v: number) {
        if (v !== this.fov) {
            this._fov = v;
            this._focalLength = fovAndSensor2Focal(this.fov, this.sensorSize);
            this.refreshProjection();
        }
    }

    get sensorSize() {
        return this._sensorSize;
    }

    set sensorSize(v: number) {
        this._sensorSize = v;
        this.fov = sensorAndFocal2Fov(this.sensorSize, this.focalLength);
    }

    get focalLength() {
        return this._focalLength;
    }

    set focalLength(v: number) {
        this._focalLength = v;
        this.fov = sensorAndFocal2Fov(this.sensorSize, this.focalLength);
    }

    get aspect(): number {
        return this._aspect;
    }

    get near(): number {
        return this._near;
    }

    set near(v: number) {
        if (v !== this.near) {
            this._near = v;
            this.refreshProjection();
        }
    }

    get far(): number {
        return this._far;
    }

    set far(v: number) {
        if (v !== this.far) {
            this._far = v;
            this.refreshProjection();
        }
    }

    setProjection(fov: number = 60, near: number = 0.1, far: number = 100): void {
        this._fov = fov;
        this._near = near;
        this._far = far;
        this.refreshProjection();
    }

    private refreshProjection() {
        mat4.perspective(
            this._proj,
            glMatrix.toRadian(this._fov),
            this._aspect,
            this._near,
            this._far
        );
    }

    public rotateTo(heading: number, pitch: number) {
        this._rotateTo(heading, pitch);
        this.refreshView();
    }

    public moveTo(pos: vec3) {
        this._moveTo(pos);
        this.refreshView();
    }

    public rotateAndMoveTo(heading: number, pitch: number, pos: vec3) {
        this._rotateTo(heading, pitch);
        this._moveTo(pos);
        this.refreshView();
    }

    private _rotateTo(heading: number, pitch: number) {
        if (heading !== this._heading
            || pitch !== this._pitch) {
            this._heading = heading;
            this._pitch = pitch;
            while (this._pitch < -90)
                this._pitch = -90;
            while (this._pitch > 90)
                this._pitch = 90;
            quat.identity(this._rot);
            quat.rotateY(this._rot, this._rot, deg2rad(this._heading));
            quat.rotateX(this._rot, this._rot, deg2rad(this._pitch));
        }
    }

    private _moveTo(pos: vec3) {
        vec3.copy(this._pos, pos);
    }

    private refreshView() {
        mat4.fromRotationTranslation(this.view, this._rot, this._pos);
        mat4.invert(this.view, this.view);
    }
}
