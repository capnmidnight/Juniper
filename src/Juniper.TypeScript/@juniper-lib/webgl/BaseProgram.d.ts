import { IDisposable } from "@juniper-lib/tslib/using";
import { ShaderProgram } from "./managed/resource/ShaderProgram";
export declare abstract class BaseProgram implements IDisposable {
    protected gl: WebGL2RenderingContext;
    protected program: ShaderProgram;
    private vertShader;
    private fragShader;
    constructor(gl: WebGL2RenderingContext, vertSrc: string, fragSrc: string);
    use(): void;
    private disposed;
    dispose(): void;
}
//# sourceMappingURL=BaseProgram.d.ts.map