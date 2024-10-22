import { Mesh } from "three";
import { ErsatzObject } from "./objects";
export declare class Fader implements ErsatzObject {
    opacity: number;
    direction: number;
    speed: number;
    readonly content3d: Mesh;
    private readonly material;
    private readonly task;
    constructor(name: string, t?: number);
    private start;
    fadeOut(): Promise<void>;
    fadeIn(): Promise<void>;
    update(dt: number): void;
}
//# sourceMappingURL=Fader.d.ts.map