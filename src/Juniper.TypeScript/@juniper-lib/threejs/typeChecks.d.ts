import { Color, CubeTexture, Euler, Material, Mesh, MeshBasicMaterial, MeshPhongMaterial, MeshPhysicalMaterial, MeshStandardMaterial, Object3D, Quaternion, Texture } from "three";
export declare function isMesh(obj: any): obj is Mesh;
export declare function isMaterial(obj: any): obj is Material;
export declare function isMeshBasicMaterial(obj: any): obj is MeshBasicMaterial;
export declare function isMeshStandardMaterial(obj: any): obj is MeshStandardMaterial;
export declare function isMeshPhongMaterial(obj: any): obj is MeshPhongMaterial;
export declare function isMeshPhysicalMaterial(obj: any): obj is MeshPhysicalMaterial;
export declare function isTexture(obj: any): obj is Texture;
export declare function isColor(obj: any): obj is Color;
export declare function isCubeTexture(obj: any): obj is CubeTexture;
export declare function isObject3D(obj: any): obj is Object3D;
export declare function isQuaternion(obj: any): obj is Quaternion;
export declare function isEuler(obj: any): obj is Euler;
//# sourceMappingURL=typeChecks.d.ts.map