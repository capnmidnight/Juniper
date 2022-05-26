import { IProgress } from "@juniper-lib/tslib";


export interface IModelLoader {
    loadModel(path: string, prog?: IProgress): Promise<THREE.Group>;
}
