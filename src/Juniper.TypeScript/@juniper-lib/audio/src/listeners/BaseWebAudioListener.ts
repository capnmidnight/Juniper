import { vec3 } from "gl-matrix";
import { Pose } from "../Pose";
import { BaseListener } from "./BaseListener";

const f = vec3.create();
const u = vec3.create();

export abstract class BaseWebAudioListener extends BaseListener {
    protected get listener(): AudioListener {
        return this.context.listener;
    }

    /**
     * Performs the spatialization operation for the audio source's latest location.
     */
    readPose(loc: Pose): void {
        const { p, q } = loc;
        vec3.set(f, 0, 0, -1);
        vec3.transformQuat(f, f, q);
        vec3.set(u, 0, 1, 0);
        vec3.transformQuat(u, u, q);
        this.setPosition(p[0], p[1], p[2]);
        this.setOrientation(f[0], f[1], f[2], u[0], u[1], u[2]);
    }

    protected abstract setPosition(x: number, y: number, z: number): void;
    protected abstract setOrientation(
        fx: number, fy: number, fz: number,
        ux: number, uy: number, uz: number): void;
}
