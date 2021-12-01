import { vec3 } from "gl-matrix";
/**
 * A position and orientation, at a given time.
 **/
export declare class Pose {
    t: number;
    p: vec3;
    f: vec3;
    u: vec3;
    o: vec3;
    /**
     * Creates a new position and orientation, at a given time.
     **/
    constructor();
    /**
     * Sets the components of the pose.
     */
    set(px: number, py: number, pz: number, fx: number, fy: number, fz: number, ux: number, uy: number, uz: number): void;
    setPosition(px: number, py: number, pz: number): void;
    setOrientation(fx: number, fy: number, fz: number, ux: number, uy: number, uz: number): void;
    setOffset(ox: number, oy: number, oz: number): void;
    /**
     * Copies the components of another pose into this pose.
     */
    copy(other: Pose): void;
}
