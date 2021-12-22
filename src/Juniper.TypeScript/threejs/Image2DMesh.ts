import { solid } from "./materials";
import { plane } from "./Plane";
import { TexturedMesh } from "./TexturedMesh";


export class Image2DMesh extends TexturedMesh {
    constructor(name: string, materialOptions?: THREE.MeshBasicMaterialParameters) {
        super(plane, solid(Object.assign(
            { transparent: true, opacity: 1 },
            materialOptions,
            { name })));

        if (name) {
            this.name = name;
        }
    }
}

