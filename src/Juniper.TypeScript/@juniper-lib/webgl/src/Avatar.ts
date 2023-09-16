import { deg2rad } from "@juniper-lib/tslib/dist/math";
import { quat, vec3 } from "gl-matrix";

export class Avatar {
    private motion = vec3.create();
    private direction = quat.create();
    private velocity = vec3.create();
    speed = 10;
    position = vec3.create();

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
        const dx = (left ? -1 : 0) + (right ? 1 : 0);
        const dz = (fwd ? -1 : 0) + (back ? 1 : 0);
        vec3.set(this.motion, dx, 0, dz);
        vec3.normalize(this.motion, this.motion);
        quat.identity(this.direction);
        quat.rotateY(this.direction, this.direction, deg2rad(this.headingDegrees));
        vec3.transformQuat(this.velocity, this.motion, this.direction);
        vec3.scale(this.velocity, this.velocity, this.speed);
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
        vec3.scaleAndAdd(this.position, this.position, this.velocity, dt);

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
        vec3.zero(this.position);
    }
}
