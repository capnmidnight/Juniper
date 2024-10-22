import { deg2rad, isNumber, rad2deg } from "@juniper-lib/util";
import { Mat4, Quat, Vec3 } from "gl-matrix";
function fovAndSensor2Focal(fov, sensor) {
    const halfVert = sensor / 3;
    return halfVert / Math.tan(deg2rad(fov));
}
function sensorAndFocal2Fov(sensor, focal) {
    const halfVert = sensor / 3;
    return rad2deg(Math.atan(halfVert / focal));
}
const DEFAULT_CAMERA_SETTINGS = {
    near: 0.01,
    far: 100,
    fov: 50,
    sensorSize: 0.036
};
export class Camera {
    constructor(init) {
        this._proj = new Mat4();
        this._view = new Mat4();
        this._headingDegrees = 0;
        this._pitchDegrees = 0;
        this._sensorSize = 0.035;
        this._rot = new Quat();
        this._pos = new Vec3();
        this.gamma = 1;
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
    get view() {
        return this._view;
    }
    set view(v) {
        this._view.copy(v);
    }
    get projection() {
        return this._proj;
    }
    set projection(v) {
        this._proj.copy(v);
    }
    get fov() {
        return this._fov;
    }
    set fov(v) {
        if (v !== this.fov) {
            this._fov = v;
            this._focalLength = fovAndSensor2Focal(this.fov, this.sensorSize);
            this.refreshProjection();
        }
    }
    get sensorSize() {
        return this._sensorSize;
    }
    set sensorSize(v) {
        this._sensorSize = v;
        this.fov = sensorAndFocal2Fov(this.sensorSize, this.focalLength);
    }
    get focalLength() {
        return this._focalLength;
    }
    set focalLength(v) {
        this._focalLength = v;
        this.fov = sensorAndFocal2Fov(this.sensorSize, this.focalLength);
    }
    get aspect() {
        return this._aspect;
    }
    get near() {
        return this._near;
    }
    set near(v) {
        if (v !== this.near) {
            this._near = v;
            this.refreshProjection();
        }
    }
    get far() {
        return this._far;
    }
    set far(v) {
        if (v !== this.far) {
            this._far = v;
            this.refreshProjection();
        }
    }
    setProjection(fov = 60, near = 0.1, far = 100) {
        this._fov = fov;
        this._near = near;
        this._far = far;
        this.refreshProjection();
    }
    refreshProjection(evt) {
        if (evt) {
            this._aspect = evt.width / evt.height;
        }
        this._proj.perspectiveNO(deg2rad(this._fov), this._aspect, this._near, this._far);
    }
    rotateTo(headingDegrees, pitchDegrees) {
        this._rotateTo(headingDegrees, pitchDegrees);
        this.refreshView();
    }
    moveTo(pos) {
        this._moveTo(pos);
        this.refreshView();
    }
    rotateAndMoveTo(headingDegrees, pitchDegrees, pos) {
        this._rotateTo(headingDegrees, pitchDegrees);
        this._moveTo(pos);
        this.refreshView();
    }
    _rotateTo(headingDegrees, pitchDegrees) {
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
    _moveTo(pos) {
        this._pos.copy(pos);
    }
    refreshView() {
        Mat4.fromRotationTranslation(this.view, this._rot, this._pos);
        this.view.invert();
    }
}
//# sourceMappingURL=Camera.js.map