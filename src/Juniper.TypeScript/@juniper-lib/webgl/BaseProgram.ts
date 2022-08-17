import { isDefined } from "@juniper-lib/tslib/typeChecks";
import type { IDisposable } from "@juniper-lib/tslib/using";
import { ShaderFragment, ShaderVertex } from "./managed/resource/Shader";
import { ShaderProgram } from "./managed/resource/ShaderProgram";


export abstract class BaseProgram implements IDisposable {
    protected program: ShaderProgram;
    private vertShader: ShaderVertex;
    private fragShader: ShaderFragment;

    constructor(protected gl: WebGL2RenderingContext, vertSrc: string, fragSrc: string) {
        try {
            this.vertShader = new ShaderVertex(gl, vertSrc);
            this.fragShader = new ShaderFragment(gl, fragSrc);
            this.program = new ShaderProgram(gl, this.vertShader, this.fragShader);
        }
        catch (exp) {
            this.dispose();
            throw exp;
        }
    }

    use() {
        if (!this.program) {
            throw new Error("Program is not ready to use.");
        }

        this.program.use();
    }

    private disposed = false;
    dispose(): void {
        if (!this.disposed) {
            if (isDefined(this.program)) {
                this.program.dispose();
                this.program = null;
            }

            if (isDefined(this.vertShader)) {
                this.vertShader.dispose();
                this.vertShader = null;
            }

            if (isDefined(this.fragShader)) {
                this.fragShader.dispose();
                this.fragShader = null;
            }

            this.disposed = true;
        }
    }
}
