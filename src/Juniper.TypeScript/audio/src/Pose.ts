import { vec3 } from "gl-matrix";

/**
 * A position and orientation, at a given time.
 **/
export class Pose {
    t = 0;
    p = vec3.create();
    f = vec3.set(vec3.create(), 0, 0, -1);
    u = vec3.set(vec3.create(), 0, 1, 0);
    o = vec3.create();

    /**
     * Creates a new position and orientation, at a given time.
     **/
    constructor() {
        Object.seal(this);
    }


    /**
     * Sets the components of the pose.
     */
    set(px: number, py: number, pz: number, fx: number, fy: number, fz: number, ux: number, uy: number, uz: number): void {
        this.setPosition(px, py, pz);
        this.setOrientation(fx, fy, fz, ux, uy, uz);
    }

    setPosition(px: number, py: number, pz: number): void {
        vec3.set(this.p, px, py, pz);
    }

    setOrientation(fx: number, fy: number, fz: number, ux: number, uy: number, uz: number): void {
        vec3.set(this.f, fx, fy, fz);
        vec3.set(this.u, ux, uy, uz);
    }

    setOffset(ox: number, oy: number, oz: number) {
        vec3.set(this.o, ox, oy, oz);
    }

    /**
     * Copies the components of another pose into this pose.
     */
    copy(other: Pose): void {
        vec3.copy(this.p, other.p);
        vec3.copy(this.f, other.f);
        vec3.copy(this.u, other.u);
        vec3.copy(this.o, other.o);
    }
}
