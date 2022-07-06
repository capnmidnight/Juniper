import { IProgress } from "@juniper-lib/tslib";


export interface IModelLoader {
    loadModel(path: string, convertMaterials?: boolean, prog?: IProgress): Promise<THREE.Group>;
}
