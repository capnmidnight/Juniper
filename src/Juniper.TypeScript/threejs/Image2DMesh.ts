import type { BaseEnvironment } from "./environment/BaseEnvironment";
import { solid } from "./materials";
import { plane } from "./Plane";
import { TexturedMesh } from "./TexturedMesh";


export class Image2DMesh extends TexturedMesh {
    constructor(env: BaseEnvironment<unknown>, name: string, materialOptions?: THREE.MeshBasicMaterialParameters) {
        super(env, plane, solid(Object.assign(
            { transparent: true, opacity: 1 },
            materialOptions,
            { name })));

        if (name) {
            this.name = name;
        }
    }
}

