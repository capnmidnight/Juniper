import { ManagedWebGLObject } from "./ManagedWebGLObject";
export class Uniform extends ManagedWebGLObject {
    constructor(gl, handle, name) {
        super(gl, handle);
        this.name = name;
    }
    set1i(v) {
        this.gl.uniform1i(this.handle, v);
    }
    set2i(x, y) {
        this.gl.uniform2i(this.handle, x, y);
    }
    set1f(v) {
        this.gl.uniform1f(this.handle, v);
    }
    set2f(x, y) {
        this.gl.uniform2f(this.handle, x, y);
    }
    setMatrix4fv(data) {
        this.gl.uniformMatrix4fv(this.handle, false, data);
    }
    setTexture(texture, slot) {
        if (slot < 0 || this.gl.MAX_COMBINED_TEXTURE_IMAGE_UNITS <= slot) {
            throw new Error(`Invalid texture slot: ${slot}`);
        }
        this.gl.activeTexture(this.gl.TEXTURE0 + slot);
        texture.bind();
        this.set1i(slot);
    }
}
//# sourceMappingURL=Uniform.js.map