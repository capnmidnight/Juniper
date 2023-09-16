import type { Geometry } from "../../Geometry";
import { VertexComponent } from "../../geometry/GeometryDescription";
import type { Attrib } from "../../managed/object/Attrib";
import type { Uniform } from "../../managed/object/Uniform";
import type { TextureImageArray } from "../../managed/resource/Texture";
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
