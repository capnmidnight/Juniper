import { Vec3 } from "gl-matrix/dist/esm";
export declare class Avatar {
    private motion;
    private direction;
    private velocity;
    speed: number;
    position: Vec3;
    private dHeadingDegrees;
    private dPitchDegrees;
    headingDegrees: number;
    pitchDegrees: number;
    keyLeft: string;
    keyRight: string;
    keyForward: string;
    keyBack: string;
    private get movingLeft();
    private get movingRight();
    private get movingFront();
    private get movingBack();
    setRotation(dHeading: number, dPitch: number): void;
    setMotion(left: boolean, right: boolean, fwd: boolean, back: boolean): void;
    addMotion(left: boolean, right: boolean, fwd: boolean, back: boolean): void;
    addKey(key: string): void;
    removeMotion(left: boolean, right: boolean, fwd: boolean, back: boolean): void;
    removeKey(key: string): void;
    update(dt: number): void;
    reset(): void;
}
//# sourceMappingURL=Avatar.d.ts.map