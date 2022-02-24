import type { Geometry } from "../../Geometry";
import { VertexComponent } from "../../geometry/GeometryDescription";
import type { Attrib } from "../../managed/Attrib";
import type { BaseTexture } from "../../managed/BaseTexture";
import type { Uniform } from "../../managed/Uniform";
import { Material } from "../../Material";
import fragSrc from "./fragment.glsl";
import vertSrc from "./vertex.glsl";

export class MaterialFlatTexture extends Material {

    private uTexture: Uniform;
    private aPosition: Attrib;
    private aUV: Attrib;

    constructor(gl: WebGL2RenderingContext) {
        super(gl, vertSrc, fragSrc);

        this.aPosition = this.program.getAttrib("aPosition");
        this.aUV = this.program.getAttrib("aUV");

        this.uTexture = this.program.getUniform("uTexture");
    }

    setGeometry(geom: Geometry) {
        geom.bind();
        geom.setAttribute(this.aPosition, VertexComponent.Position);
        geom.setAttribute(this.aUV, VertexComponent.UV);
    }

    setTexture(texture: BaseTexture) {
        this.uTexture.setTexture(texture, 0);
    }
}
