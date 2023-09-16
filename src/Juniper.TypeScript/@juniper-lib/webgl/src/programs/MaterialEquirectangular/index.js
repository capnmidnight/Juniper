import { VertexComponent } from "../../geometry/GeometryDescription";
import { Material } from "../../Material";
import fragSrc from "./fragment.glsl";
import vertSrc from "./vertex.glsl";
export class MaterialEquirectangular extends Material {
    constructor(gl) {
        super(gl, vertSrc, fragSrc);
        this.aPosition = this.program.getAttrib("aPosition");
        this.uTexture = this.program.getUniform("uTexture");
    }
    setGeometry(geom) {
        geom.bind();
        geom.setAttribute(this.aPosition, VertexComponent.Position);
    }
    setTexture(texture) {
        this.uTexture.setTexture(texture, 0);
    }
}
//# sourceMappingURL=index.js.map