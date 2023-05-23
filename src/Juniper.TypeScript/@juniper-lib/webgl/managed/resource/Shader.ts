import { dispose } from "@juniper-lib/tslib/using";
import { ManagedWebGLResource } from "./ManagedWebGLResource";
import type { ShaderProgram } from "./ShaderProgram";

function resolveShaderType(gl: WebGL2RenderingContext, type: string): GLenum {
    if (type === "vertex") {
        return gl.VERTEX_SHADER;
    } else if (type === "fragment") {
        return gl.FRAGMENT_SHADER;
    } else {
        throw new Error(`Unknown shader type: ${type}`);
    }
}

// Manage a shader
export class Shader extends ManagedWebGLResource<WebGLShader> {
    /**
    * @param type - the type of the shader. Expected values are:
        "vertex" - a vertex shader
        "fragment" - a fragment shader.
    * @param gl - the rendering context in which we're working.
    * @param src - the source code for the shader.
    */
    constructor(type: string, gl: WebGL2RenderingContext, src: string) {
        super(gl, gl.createShader(resolveShaderType(gl, type)));

        this.setSource(src);
        this.compile();
        if (!this.getParameter(gl.COMPILE_STATUS)) {
            const errorMessage = `${this.getInfoLog() || "Unknown error"}
${this.getSource()}`;
            dispose(this);
            throw new Error(errorMessage);
        }
    }

    protected onDisposing(): void {
        this.gl.deleteShader(this.handle);
    }

    setSource(src: string) {
        this.gl.shaderSource(this.handle, src);
    }

    getSource(): string {
        const ext = this.gl.getExtension("WEBGL_debug_shaders");
        const src = ext.getTranslatedShaderSource(this.handle) || "Debug source not available";
        return src;
    }

    compile() {
        this.gl.compileShader(this.handle);
    }

    getParameter(param: number) {
        return this.gl.getShaderParameter(this.handle, param);
    }

    getInfoLog() {
        return this.gl.getShaderInfoLog(this.handle);
    }

    addTo(program: ShaderProgram) {
        program.attachShader(this.handle);
    }

    removeFrom(program: ShaderProgram) {
        program.detachShader(this.handle);
    }
}


export class ShaderFragment extends Shader {
    constructor(gl: WebGL2RenderingContext, src: string) {
        super("fragment", gl, src);
    }
}

export class ShaderVertex extends Shader {
    constructor(gl: WebGL2RenderingContext, src: string) {
        super("vertex", gl, src);
    }
}
