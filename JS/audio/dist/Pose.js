import { Quat, Vec3 } from "gl-matrix";
/**
 * A position and orientation, at a given time.
 **/
export class Pose {
    /**
     * Creates a new position and orientation, at a given time.
     **/
    constructor() {
        this.p = new Vec3();
        this.q = new Quat();
        Object.seal(this);
    }
    /**
     * Sets the components of the pose.
     */
    set(px, py, pz, qx, qy, qz, qw) {
        this.setPosition(px, py, pz);
        this.setOrientation(qx, qy, qz, qw);
    }
    setPosition(px, py, pz) {
        this.p.x = px;
        this.p.y = py;
        this.p.z = pz;
    }
    setOrientation(qx, qy, qz, qw) {
        this.q.x = qx;
        this.q.y = qy;
        this.q.z = qz;
        this.q.w = qw;
    }
    /**
     * Copies the components of another pose into this pose.
     */
    copy(other) {
        this.p.copy(other.p);
        this.q.copy(other.q);
    }
}
//# sourceMappingURL=Pose.js.map