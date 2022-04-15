import { IProgress } from "@juniper/progress";


export interface IModelLoader {
    loadModel(path: string, prog?: IProgress): Promise<THREE.Group>;
}
