import { deg2rad, isNumber, rad2deg } from "@juniper-lib/util";
import { Mat4, Quat, Vec3 } from "gl-matrix";
import { ResizeEvent } from "./Context3D";


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
    private _proj = new Mat4();
    private _view = new Mat4();

    private _headingDegrees = 0;
    private _pitchDegrees = 0;

    private _aspect: number;
    private _fov: number;
    private _sensorSize = 0.035;
    private _focalLength: number;
    private _near: number;
    private _far: number;

    private _rot = new Quat();
    private _pos = new Vec3();

    public gamma = 1;

    constructor(init?: Partial<CameraInit>) {
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

    get view(): Mat4 {
        return this._view;
    }

    set view(v: Mat4) {
        this._view.copy(v);
    }

    get projection(): Mat4 {
        return this._proj;
    }

    set projection(v: Mat4) {
        this._proj.copy(v);
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

    setProjection(fov = 60, near = 0.1, far = 100): void {
        this._fov = fov;
        this._near = near;
        this._far = far;
        this.refreshProjection();
    }

    refreshProjection(evt?: ResizeEvent) {
        if (evt) {
            this._aspect = evt.width / evt.height;
        }
        
        this._proj.perspectiveNO(
            deg2rad(this._fov),
            this._aspect,
            this._near,
            this._far
        );
    }

    public rotateTo(headingDegrees: number, pitchDegrees: number) {
        this._rotateTo(headingDegrees, pitchDegrees);
        this.refreshView();
    }

    public moveTo(pos: Vec3) {
        this._moveTo(pos);
        this.refreshView();
    }

    public rotateAndMoveTo(headingDegrees: number, pitchDegrees: number, pos: Vec3) {
        this._rotateTo(headingDegrees, pitchDegrees);
        this._moveTo(pos);
        this.refreshView();
    }

    private _rotateTo(headingDegrees: number, pitchDegrees: number) {
        if (headingDegrees !== this._headingDegrees
            || pitchDegrees !== this._pitchDegrees) {
            this._headingDegrees = headingDegrees;
            this._pitchDegrees = pitchDegrees;
            while (this._pitchDegrees < -90)
                this._pitchDegrees = -90;
            while (this._pitchDegrees > 90)
                this._pitchDegrees = 90;
            this._rot.identity()
                .rotateY(deg2rad(this._headingDegrees))
                .rotateX(deg2rad(this._pitchDegrees));
        }
    }

    private _moveTo(pos: Vec3) {
        this._pos.copy(pos);
    }

    private refreshView() {
        Mat4.fromRotationTranslation(this.view, this._rot, this._pos);
        this.view.invert();
    }
}
