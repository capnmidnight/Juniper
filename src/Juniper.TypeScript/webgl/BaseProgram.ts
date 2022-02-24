import type { IDisposable } from "juniper-tslib";
import { isDefined } from "juniper-tslib";
import { ShaderFragment } from "./managed/ShaderFragment";
import { ShaderProgram } from "./managed/ShaderProgram";
import { ShaderVertex } from "./managed/ShaderVertex";


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
