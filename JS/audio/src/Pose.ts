import { Quat, Vec3 } from "gl-matrix";

/**
 * A position and orientation, at a given time.
 **/
export class Pose {
    readonly p = new Vec3();
    readonly q = new Quat();

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
        this.p.x = px;
        this.p.y = py;
        this.p.z = pz;
    }

    setOrientation(qx: number, qy: number, qz: number, qw: number): void {
        this.q.x = qx;
        this.q.y = qy;
        this.q.z = qz;
        this.q.w = qw;
    }

    /**
     * Copies the components of another pose into this pose.
     */
    copy(other: Pose): void {
        this.p.copy(other.p);
        this.q.copy(other.q);
    }
}
