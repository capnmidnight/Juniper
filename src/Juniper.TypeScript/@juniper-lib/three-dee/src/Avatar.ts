import { deg2rad } from "@juniper-lib/tslib/dist/math";
import { Quat, Vec3 } from "gl-matrix/dist/esm";

export class Avatar {
    private motion = new Vec3();
    private direction = new Quat();
    private velocity = new Vec3();
    speed = 10;
    position = new Vec3();

    private dHeadingDegrees = 0;
    private dPitchDegrees = 0;
    headingDegrees = 0;
    pitchDegrees = 0;

    keyLeft = "a";
    keyRight = "d";
    keyForward = "w";
    keyBack = "s";

    private get movingLeft() {
        return this.motion[0] < 0;
    }

    private get movingRight() {
        return this.motion[0] > 0;
    }

    private get movingFront() {
        return this.motion[2] < 0;
    }

    private get movingBack() {
        return this.motion[2] > 0;
    }

    setRotation(dHeading: number, dPitch: number) {
        this.dHeadingDegrees = dHeading;
        this.dPitchDegrees = dPitch;
    }

    setMotion(left: boolean, right: boolean, fwd: boolean, back: boolean): void {
        this.motion.x = (left ? -1 : 0) + (right ? 1 : 0);
        this.motion.y = 0;
        this.motion.z = (fwd ? -1 : 0) + (back ? 1 : 0);
        this.motion.normalize();
        this.direction
            .identity()
            .rotateY(deg2rad(this.headingDegrees));

        Vec3.transformQuat(this.velocity, this.motion, this.direction);
        this.velocity.scale(this.speed);
    }

    addMotion(left: boolean, right: boolean, fwd: boolean, back: boolean): void {
        this.setMotion(
            this.movingLeft || left,
            this.movingRight || right,
            this.movingFront || fwd,
            this.movingBack || back);
    }

    addKey(key: string) {
        this.addMotion(
            key === this.keyLeft,
            key === this.keyRight,
            key === this.keyForward,
            key === this.keyBack);
    }

    removeMotion(left: boolean, right: boolean, fwd: boolean, back: boolean): void {
        this.setMotion(
            this.movingLeft && left,
            this.movingRight && right,
            this.movingFront && fwd,
            this.movingBack && back);
    }

    removeKey(key: string) {
        this.removeMotion(
            key !== this.keyLeft,
            key !== this.keyRight,
            key !== this.keyForward,
            key !== this.keyBack);
    }

    update(dt: number): void {
        this.position.scaleAndAdd(this.velocity, dt);

        this.headingDegrees += this.dHeadingDegrees * dt;
        this.pitchDegrees += this.dPitchDegrees * dt;
        while (this.pitchDegrees < -90)
            this.pitchDegrees = -90;
        while (this.pitchDegrees > 90)
            this.pitchDegrees = 90;

        this.dHeadingDegrees *= 0.95;
        this.dPitchDegrees *= 0.95;
    }

    reset() {
        this.headingDegrees = 0;
        this.pitchDegrees = 0;
        this.position.x = 0;
        this.position.y = 0;
    }
}
