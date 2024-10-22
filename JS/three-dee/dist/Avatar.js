import { deg2rad } from "@juniper-lib/tslib/dist/math";
import { Quat, Vec3 } from "gl-matrix/dist/esm";
export class Avatar {
    constructor() {
        this.motion = new Vec3();
        this.direction = new Quat();
        this.velocity = new Vec3();
        this.speed = 10;
        this.position = new Vec3();
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
//# sourceMappingURL=Avatar.js.map