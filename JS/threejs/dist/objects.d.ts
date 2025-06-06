import { BufferGeometry, Material, Mesh, Object3D, Vector3 } from "three";
export interface ErsatzObject<T extends Object3D = Object3D> {
    content3d: T;
}
export declare function isErsatzObject<T extends Object3D>(obj: unknown): obj is ErsatzObject<T>;
export type Objects<T extends Object3D = Object3D> = T | ErsatzObject<T>;
export declare function isObjects(obj: any): obj is Objects;
export declare function objectResolve(obj: Objects): Object3D;
export declare function objectSetVisible(obj: Objects, visible: boolean): boolean;
export declare function objectIsVisible(obj: Objects): boolean;
export declare function objectIsFullyVisible(obj: Objects): boolean;
export declare function objectToggleVisible(obj: Objects): void;
export declare function objGraph<T extends Objects>(obj: T, ...children: Objects[]): T;
export declare function objRemoveFromParent(obj: Objects): void;
export declare function obj(name: string, ...rest: Objects[]): Object3D;
export declare function objectClearChildren(obj: Objects): void;
export declare function objectSetEnabled(obj: Objects, enabled: boolean): void;
export declare function objectSetWorldPosition(obj: Objects, pos: Vector3): void;
export declare function mesh<TGeometry extends BufferGeometry = BufferGeometry, TMaterial extends Material | Material[] = Material | Material[]>(name: string, geom?: TGeometry, mat?: TMaterial): Mesh<TGeometry, TMaterial>;
//# sourceMappingURL=objects.d.ts.map