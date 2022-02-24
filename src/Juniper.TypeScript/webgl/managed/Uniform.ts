import { ManagedWebGLObject } from "./ManagedWebGLObject";
import type { BaseTexture } from "./BaseTexture";

export class Uniform extends ManagedWebGLObject<WebGLUniformLocation> {
    readonly name: string;

    constructor(gl: WebGL2RenderingContext, handle: WebGLUniformLocation, name: string) {
        super(gl, handle);
        this.name = name;
    }

    set1i(v: number) {
        this.gl.uniform1i(this.handle, v);
    }

    set2i(x: number, y: number) {
        this.gl.uniform2i(this.handle, x, y);
    }

    set1f(v: number) {
        this.gl.uniform1f(this.handle, v);
    }

    set2f(x: number, y: number) {
        this.gl.uniform2f(this.handle, x, y);
    }

    setMatrix4fv(data: Float32List) {
        this.gl.uniformMatrix4fv(this.handle, false, data);
    }

    setTexture(texture: BaseTexture, slot: number) {
        if (slot < 0 || this.gl.MAX_COMBINED_TEXTURE_IMAGE_UNITS <= slot) {
            throw new Error(`Invalid texture slot: ${slot}`);
        }
        this.gl.activeTexture(this.gl.TEXTURE0 + slot);
        texture.bind();
        this.set1i(slot);
    }

}
