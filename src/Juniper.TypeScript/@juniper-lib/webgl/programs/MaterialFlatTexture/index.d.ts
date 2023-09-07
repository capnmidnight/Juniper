import type { Geometry } from "../../Geometry";
import type { BaseTexture } from "../../managed/resource/Texture";
import { Material } from "../../Material";
export declare class MaterialFlatTexture extends Material {
    private uTexture;
    private aPosition;
    private aUV;
    constructor(gl: WebGL2RenderingContext);
    setGeometry(geom: Geometry): void;
    setTexture(texture: BaseTexture): void;
}
//# sourceMappingURL=index.d.ts.map