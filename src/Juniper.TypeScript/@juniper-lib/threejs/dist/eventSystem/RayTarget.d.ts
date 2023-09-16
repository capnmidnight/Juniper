import { TypedEventTarget } from "@juniper-lib/events/dist/TypedEventTarget";
import { Mesh, Object3D } from "three";
import { ErsatzObject, Objects } from "../objects";
import { Pointer3DEvents } from "./devices/Pointer3DEvent";
export declare class RayTarget<EventsT = void> extends TypedEventTarget<EventsT & Pointer3DEvents> implements ErsatzObject {
    readonly object: Object3D;
    readonly meshes: Mesh<import("three").BufferGeometry, import("three").Material | import("three").Material[]>[];
    private _disabled;
    private _clickable;
    private _draggable;
    private _navigable;
    constructor(object: Object3D);
    addMesh(mesh: Mesh): this;
    removeMesh(mesh: Mesh): this;
    addMeshes(...meshes: Mesh[]): this;
    removeMeshes(...meshes: Mesh[]): this;
    get disabled(): boolean;
    set disabled(v: boolean);
    get enabled(): boolean;
    set enabled(v: boolean);
    get clickable(): boolean;
    set clickable(v: boolean);
    get draggable(): boolean;
    set draggable(v: boolean);
    get navigable(): boolean;
    set navigable(v: boolean);
}
export declare function isRayTarget<T = void>(obj: Objects): obj is RayTarget<T>;
export declare function getRayTarget<T = void>(obj: Objects): RayTarget<T>;
//# sourceMappingURL=RayTarget.d.ts.map