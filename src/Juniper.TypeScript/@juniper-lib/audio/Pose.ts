import { quat, vec3 } from "gl-matrix";

/**
 * A position and orientation, at a given time.
 **/
export class Pose {
    readonly p = vec3.create();
    readonly q = quat.create();

    /**
     * Creates a new position and orientation, at a given time.
     **/
    constructor() {
        Object.seal(this);
    }

    /**
     * Sets the components of the pose.
     */
    set(px: number, py: number, pz: number, qx: number, qy: number, qz: number, qw: number): void {
        this.setPosition(px, py, pz);
        this.setOrientation(qx, qy, qz, qw);
    }

    setPosition(px: number, py: number, pz: number): void {
        vec3.set(this.p, px, py, pz);
    }

    setOrientation(qx: number, qy: number, qz: number, qw: number): void {
        quat.set(this.q, qx, qy, qz, qw);
    }

    /**
     * Copies the components of another pose into this pose.
     */
    copy(other: Pose): void {
        vec3.copy(this.p, other.p);
        quat.copy(this.q, other.q);
    }
}
