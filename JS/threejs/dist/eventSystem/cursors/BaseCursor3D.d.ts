import { Intersection, Object3D, Vector3 } from "three";
import { BaseEnvironment } from "../../environment/BaseEnvironment";
import { ErsatzObject } from "../../objects";
import { RayTarget } from "../RayTarget";
import { BaseCursor } from "./BaseCursor";
export declare abstract class BaseCursor3D extends BaseCursor implements ErsatzObject {
    #private;
    protected readonly env: BaseEnvironment;
    private readonly T;
    private readonly V;
    private readonly Q;
    private _side;
    get side(): number;
    set side(v: number);
    get content3d(): Object3D<import("three").Event>;
    set content3d(v: Object3D<import("three").Event>);
    constructor(env: BaseEnvironment);
    get position(): Vector3;
    update(avatarHeadPos: Vector3, comfortOffset: Vector3, hit: Intersection, target: RayTarget, defaultDistance: number, isLocal: boolean, canDragView: boolean, canTeleport: boolean, origin: Vector3, direction: Vector3, isPrimaryPressed: boolean): void;
    private readonly f;
    private readonly up;
    private readonly right;
    lookAt(p: Vector3, v: Vector3): void;
}
//# sourceMappingURL=BaseCursor3D.d.ts.map