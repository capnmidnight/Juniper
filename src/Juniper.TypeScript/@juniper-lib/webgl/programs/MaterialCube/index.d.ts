import type { Geometry } from "../../Geometry";
import type { TextureImageArray } from "../../managed/resource/Texture";
import { Material } from "../../Material";
export declare class MaterialEquirectangular extends Material {
    private uTexture;
    private aPosition;
    constructor(gl: WebGL2RenderingContext);
    setGeometry(geom: Geometry): void;
    setTexture(texture: TextureImageArray): void;
}
//# sourceMappingURL=index.d.ts.map