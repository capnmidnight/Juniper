import { dispose, isDefined } from "@juniper-lib/util";
import { ShaderFragment, ShaderVertex } from "./managed/resource/Shader";
import { ShaderProgram } from "./managed/resource/ShaderProgram";
export class BaseProgram {
    constructor(gl, vertSrc, fragSrc) {
        this.gl = gl;
        this.disposed = false;
        try {
            this.vertShader = new ShaderVertex(gl, vertSrc);
            this.fragShader = new ShaderFragment(gl, fragSrc);
            this.program = new ShaderProgram(gl, this.vertShader, this.fragShader);
        }
        catch (exp) {
            dispose(this);
            throw exp;
        }
    }
    use() {
        if (!this.program) {
            throw new Error("Program is not ready to use.");
        }
        this.program.use();
    }
    dispose() {
        if (!this.disposed) {
            if (isDefined(this.program)) {
                dispose(this.program);
                this.program = null;
            }
            if (isDefined(this.vertShader)) {
                dispose(this.vertShader);
                this.vertShader = null;
            }
            if (isDefined(this.fragShader)) {
                dispose(this.fragShader);
                this.fragShader = null;
            }
            this.disposed = true;
        }
    }
}
//# sourceMappingURL=BaseProgram.js.map