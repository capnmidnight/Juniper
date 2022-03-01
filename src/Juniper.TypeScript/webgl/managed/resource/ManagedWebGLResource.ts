import type { IDisposable } from "juniper-tslib";
import { ManagedWebGLObject } from "../object/ManagedWebGLObject";

export abstract class ManagedWebGLResource<PointerT>
    extends ManagedWebGLObject<PointerT>
    implements IDisposable {

    private disposed = false;

    constructor(gl: WebGL2RenderingContext, handle: PointerT) {
        super(gl, handle);
    }

    dispose(): void {
        if (!this.disposed) {
            this.onDisposing();
            this.disposed = true;
        }
    }

    protected abstract onDisposing(): void;
}