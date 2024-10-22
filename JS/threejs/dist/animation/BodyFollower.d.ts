import { Object3D, Vector3 } from "three";
export declare class BodyFollower extends Object3D {
    private readonly minDistance;
    private readonly heightOffset;
    private readonly speed;
    private lerp;
    private maxDistance;
    private minHeadingRadians;
    private maxHeadingRadians;
    constructor(name: string, minDistance: number, minHeadingDegrees: number, heightOffset: number, speed?: number);
    copy(source: this, recursive?: boolean): this;
    update(height: number, position: Vector3, headingRadians: number, dt: number): void;
    reset(height: number, position: Vector3, headingRadians: number): void;
    private clampTo;
}
//# sourceMappingURL=BodyFollower.d.ts.map