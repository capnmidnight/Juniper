import { ManagedWebGLResource } from "./ManagedWebGLResource";

export class Query extends ManagedWebGLResource<WebGLQuery> {
    constructor(gl: WebGL2RenderingContext) {
        super(gl, gl.createQuery());
    }

    onDisposing(): void {
        this.gl.deleteQuery(this.handle);
    }
}
