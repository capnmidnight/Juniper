import { ManagedWebGLResource } from "./ManagedWebGLResource";

export class BaseTexture extends ManagedWebGLResource<WebGLTexture> {
    constructor(gl: WebGL2RenderingContext,
        protected type: GLenum) {
        super(gl, gl.createTexture());

        this.bind();

        gl.texParameteri(this.type, gl.TEXTURE_WRAP_S, gl.CLAMP_TO_EDGE);
        gl.texParameteri(this.type, gl.TEXTURE_WRAP_T, gl.CLAMP_TO_EDGE);
        gl.texParameteri(this.type, gl.TEXTURE_MIN_FILTER, gl.LINEAR);
        gl.texParameteri(this.type, gl.TEXTURE_MAG_FILTER, gl.LINEAR);
    }

    get isStereo() {
        return false;
    }

    bind() {
        this.gl.bindTexture(this.type, this.handle);
    }

    protected onDisposing(): void {
        this.gl.deleteTexture(this.handle);
    }
}
