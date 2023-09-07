import { IDisposable } from "@juniper-lib/tslib/using";
import { Color, Intersection } from "three";
import { InstancedButtonFactory } from ".";
import { RayTarget } from "../../eventSystem/RayTarget";
export declare class InstancedMeshButton extends RayTarget implements IDisposable {
    private readonly parent;
    readonly icon: Color;
    private _instanceId;
    get instanceId(): number;
    set instanceId(v: number);
    constructor(parent: InstancedButtonFactory, name: string, size: number, icon: Color);
    precheck(hit: Intersection): boolean;
    dispose(): void;
    get size(): number;
    set size(v: number);
}
//# sourceMappingURL=InstancedMeshButton.d.ts.map