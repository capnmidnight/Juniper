import type { Geometry } from "../../Geometry";
import { VertexComponent } from "../../geometry/GeometryDescription";
import type { Attrib } from "../../managed/Attrib";
import type { TextureImageArray } from "../../managed/TextureImage";
import type { Uniform } from "../../managed/Uniform";
import { Material } from "../../Material";
import fragSrc from "./fragment.glsl";
import vertSrc from "./vertex.glsl";

export class MaterialEquirectangular extends Material {

    private uTexture: Uniform;

    private aPosition: Attrib;

    constructor(gl: WebGL2RenderingContext) {
        super(gl, vertSrc, fragSrc);

        this.aPosition = this.program.getAttrib("aPosition");
        this.uTexture = this.program.getUniform("uTexture");
    }

    setGeometry(geom: Geometry) {
        geom.bind();
        geom.setAttribute(this.aPosition, VertexComponent.Position);
    }

    setTexture(texture: TextureImageArray) {
        this.uTexture.setTexture(texture, 0);
    }
}
