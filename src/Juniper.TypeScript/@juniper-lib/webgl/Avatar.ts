import { deg2rad } from "@juniper-lib/tslib/math";
import { quat, vec3 } from "gl-matrix";

export class Avatar {
    private motion = vec3.create();
    private direction = quat.create();
    private velocity = vec3.create();
    speed = 10;
    position = vec3.create();

    private dHeading = 0;
    private dPitch = 0;
    heading = 0;
    pitch = 0;

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
        this.dHeading = dHeading;
        this.dPitch = dPitch;
    }

    setMotion(left: boolean, right: boolean, fwd: boolean, back: boolean): void {
        const dx = (left ? -1 : 0) + (right ? 1 : 0);
        const dz = (fwd ? -1 : 0) + (back ? 1 : 0);
        vec3.set(this.motion, dx, 0, dz);
        vec3.normalize(this.motion, this.motion);
        quat.identity(this.direction);
        quat.rotateY(this.direction, this.direction, deg2rad(this.heading));
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

        this.heading += this.dHeading * dt;
        this.pitch += this.dPitch * dt;
        while (this.pitch < -90)
            this.pitch = -90;
        while (this.pitch > 90)
            this.pitch = 90;

        this.dHeading *= 0.95;
        this.dPitch *= 0.95;
    }

    reset() {
        this.heading = 0;
        this.pitch = 0;
        vec3.zero(this.position);
    }
}
