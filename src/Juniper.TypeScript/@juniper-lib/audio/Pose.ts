import { isDefined } from "@juniper-lib/tslib/typeChecks";
import { vec3 } from "gl-matrix";

/**
 * A position and orientation, at a given time.
 **/
export class Pose {
    readonly p = vec3.create();
    readonly f = vec3.fromValues(0, 0, -1);
    readonly u = vec3.fromValues(0, 1, 0);
    readonly r = vec3.fromValues(1, 0, 0);

    /**
     * Creates a new position and orientation, at a given time.
     **/
    constructor() {
        Object.seal(this);
    }

    /**
     * Sets the components of the pose.
     */
    set(px: number, py: number, pz: number, fx: number, fy: number, fz: number): void;
    set(px: number, py: number, pz: number, fx: number, fy: number, fz: number, ux: number, uy: number, uz: number): void;
    set(px: number, py: number, pz: number, fx: number, fy: number, fz: number, ux?: number, uy?: number, uz?: number): void {
        this.setPosition(px, py, pz);
        this.setOrientation(fx, fy, fz, ux, uy, uz);
    }

    setPosition(px: number, py: number, pz: number): void {
        vec3.set(this.p, px, py, pz);
    }

    setOrientation(fx: number, fy: number, fz: number): void;
    setOrientation(fx: number, fy: number, fz: number, ux: number, uy: number, uz: number): void;
    setOrientation(fx: number, fy: number, fz: number, ux?: number, uy?: number, uz?: number): void {
        vec3.set(this.f, fx, fy, fz);
        vec3.set(this.u, 0, 1, 0);
        vec3.set(this.r, 1, 0, 0);
        
        if (isDefined(ux)
            && isDefined(uy)
            && isDefined(uz)) {
            vec3.set(this.u, ux, uy, uz);
            vec3.cross(this.r, this.f, this.u);
        }
        else if (1 - Math.abs(vec3.dot(this.f, this.u)) < 0.001) {
            vec3.cross(this.u, this.r, this.f);
            vec3.cross(this.r, this.f, this.u);
        }
        else {
            vec3.cross(this.r, this.f, this.u);
            vec3.cross(this.u, this.r, this.f)
        }
    }

    /**
     * Copies the components of another pose into this pose.
     */
    copy(other: Pose): void {
        vec3.copy(this.p, other.p);
        vec3.copy(this.f, other.f);
        vec3.copy(this.u, other.u);
        vec3.copy(this.r, other.r);
    }
}
