import { IProgress } from "juniper-tslib";


export interface IModelLoader {
    loadModel(path: string, prog?: IProgress): Promise<THREE.Group>;
}
