import { ManagedWebGLObject } from "../object/ManagedWebGLObject";
export class ManagedWebGLResource extends ManagedWebGLObject {
    constructor(gl, handle) {
        super(gl, handle);
        this.disposed = false;
    }
    dispose() {
        if (!this.disposed) {
            this.onDisposing();
            this.disposed = true;
        }
    }
}
//# sourceMappingURL=ManagedWebGLResource.js.map