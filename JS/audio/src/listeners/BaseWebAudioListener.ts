import { Vec3 } from "gl-matrix";
import { Pose } from "../Pose";
import { BaseListener } from "./BaseListener";

const f = new Vec3();
const u = new Vec3();

export abstract class BaseWebAudioListener extends BaseListener {
    protected get listener(): AudioListener {
        return this.context.listener;
    }

    /**
     * Performs the spatialization operation for the audio source's latest location.
     */
    readPose(loc: Pose): void {
        const { p, q } = loc;
        f.x = 0;
        f.y = 0;
        f.z = -1;
        Vec3.transformQuat(f, f, q);
        u.x = 0;
        u.y = 1;
        u.z = 0;
        Vec3.transformQuat(u, u, q);
        this.setPosition(p[0], p[1], p[2]);
        this.setOrientation(f[0], f[1], f[2], u[0], u[1], u[2]);
    }

    protected abstract setPosition(x: number, y: number, z: number): void;
    protected abstract setOrientation(
        fx: number, fy: number, fz: number,
        ux: number, uy: number, uz: number): void;
}
