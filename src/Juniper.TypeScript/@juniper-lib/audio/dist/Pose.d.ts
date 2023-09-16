import { quat, vec3 } from "gl-matrix";
/**
 * A position and orientation, at a given time.
 **/
export declare class Pose {
    readonly p: vec3;
    readonly q: quat;
    /**
     * Creates a new position and orientation, at a given time.
     **/
    constructor();
    /**
     * Sets the components of the pose.
     */
    set(px: number, py: number, pz: number, qx: number, qy: number, qz: number, qw: number): void;
    setPosition(px: number, py: number, pz: number): void;
    setOrientation(qx: number, qy: number, qz: number, qw: number): void;
    /**
     * Copies the components of another pose into this pose.
     */
    copy(other: Pose): void;
}
//# sourceMappingURL=Pose.d.ts.map