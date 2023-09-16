import { quat, vec3 } from "gl-matrix";
/**
 * A position and orientation, at a given time.
 **/
export class Pose {
    /**
     * Creates a new position and orientation, at a given time.
     **/
    constructor() {
        this.p = vec3.create();
        this.q = quat.create();
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
        vec3.set(this.p, px, py, pz);
    }
    setOrientation(qx, qy, qz, qw) {
        quat.set(this.q, qx, qy, qz, qw);
    }
    /**
     * Copies the components of another pose into this pose.
     */
    copy(other) {
        vec3.copy(this.p, other.p);
        quat.copy(this.q, other.q);
    }
}
//# sourceMappingURL=Pose.js.map