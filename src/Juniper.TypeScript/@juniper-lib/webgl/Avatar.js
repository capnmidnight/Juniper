import { deg2rad } from "@juniper-lib/tslib/math";
import { quat, vec3 } from "gl-matrix";
export class Avatar {
    constructor() {
        this.motion = vec3.create();
        this.direction = quat.create();
        this.velocity = vec3.create();
        this.speed = 10;
        this.position = vec3.create();
        this.dHeadingDegrees = 0;
        this.dPitchDegrees = 0;
        this.headingDegrees = 0;
        this.pitchDegrees = 0;
        this.keyLeft = "a";
        this.keyRight = "d";
        this.keyForward = "w";
        this.keyBack = "s";
    }
    get movingLeft() {
        return this.motion[0] < 0;
    }
    get movingRight() {
        return this.motion[0] > 0;
    }
    get movingFront() {
        return this.motion[2] < 0;
    }
    get movingBack() {
        return this.motion[2] > 0;
    }
    setRotation(dHeading, dPitch) {
        this.dHeadingDegrees = dHeading;
        this.dPitchDegrees = dPitch;
    }
    setMotion(left, right, fwd, back) {
        const dx = (left ? -1 : 0) + (right ? 1 : 0);
        const dz = (fwd ? -1 : 0) + (back ? 1 : 0);
        vec3.set(this.motion, dx, 0, dz);
        vec3.normalize(this.motion, this.motion);
        quat.identity(this.direction);
        quat.rotateY(this.direction, this.direction, deg2rad(this.headingDegrees));
        vec3.transformQuat(this.velocity, this.motion, this.direction);
        vec3.scale(this.velocity, this.velocity, this.speed);
    }
    addMotion(left, right, fwd, back) {
        this.setMotion(this.movingLeft || left, this.movingRight || right, this.movingFront || fwd, this.movingBack || back);
    }
    addKey(key) {
        this.addMotion(key === this.keyLeft, key === this.keyRight, key === this.keyForward, key === this.keyBack);
    }
    removeMotion(left, right, fwd, back) {
        this.setMotion(this.movingLeft && left, this.movingRight && right, this.movingFront && fwd, this.movingBack && back);
    }
    removeKey(key) {
        this.removeMotion(key !== this.keyLeft, key !== this.keyRight, key !== this.keyForward, key !== this.keyBack);
    }
    update(dt) {
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
//# sourceMappingURL=Avatar.js.map