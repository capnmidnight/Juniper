export abstract class ManagedWebGLObject<PointerT> {
    constructor(
        protected readonly gl: WebGL2RenderingContext,
        public readonly handle: PointerT) {
    }
}