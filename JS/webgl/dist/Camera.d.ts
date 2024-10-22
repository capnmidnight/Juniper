import { Mat4, Vec3 } from "gl-matrix/dist/esm";
import type { Context3D } from "./Context3D";
interface CameraInit {
    near: number;
    far: number;
    fov: number;
    sensorSize: number;
    focalLength: number;
}
export declare class Camera {
    private _proj;
    private _view;
    private _headingDegrees;
    private _pitchDegrees;
    private _aspect;
    private _fov;
    private _sensorSize;
    private _focalLength;
    private _near;
    private _far;
    private _rot;
    private _pos;
    gamma: number;
    constructor(ctx: Context3D, init?: Partial<CameraInit>);
    get view(): Mat4;
    set view(v: Mat4);
    get projection(): Mat4;
    set projection(v: Mat4);
    get fov(): number;
    set fov(v: number);
    get sensorSize(): number;
    set sensorSize(v: number);
    get focalLength(): number;
    set focalLength(v: number);
    get aspect(): number;
    get near(): number;
    set near(v: number);
    get far(): number;
    set far(v: number);
    setProjection(fov?: number, near?: number, far?: number): void;
    private refreshProjection;
    rotateTo(headingDegrees: number, pitchDegrees: number): void;
    moveTo(pos: Vec3): void;
    rotateAndMoveTo(headingDegrees: number, pitchDegrees: number, pos: Vec3): void;
    private _rotateTo;
    private _moveTo;
    private refreshView;
}
export {};
//# sourceMappingURL=Camera.d.ts.map