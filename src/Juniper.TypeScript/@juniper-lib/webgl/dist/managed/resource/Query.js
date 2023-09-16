import { ManagedWebGLResource } from "./ManagedWebGLResource";
export class Query extends ManagedWebGLResource {
    constructor(gl) {
        super(gl, gl.createQuery());
    }
    onDisposing() {
        this.gl.deleteQuery(this.handle);
    }
}
//# sourceMappingURL=Query.js.map