import { dispose } from "@juniper-lib/tslib/using";
import { ManagedWebGLResource } from "./ManagedWebGLResource";
function resolveShaderType(gl, type) {
    if (type === "vertex") {
        return gl.VERTEX_SHADER;
    }
    else if (type === "fragment") {
        return gl.FRAGMENT_SHADER;
    }
    else {
        throw new Error(`Unknown shader type: ${type}`);
    }
}
// Manage a shader
export class Shader extends ManagedWebGLResource {
    /**
    * @param type - the type of the shader. Expected values are:
        "vertex" - a vertex shader
        "fragment" - a fragment shader.
    * @param gl - the rendering context in which we're working.
    * @param src - the source code for the shader.
    */
    constructor(type, gl, src) {
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
    onDisposing() {
        this.gl.deleteShader(this.handle);
    }
    setSource(src) {
        this.gl.shaderSource(this.handle, src);
    }
    getSource() {
        const ext = this.gl.getExtension("WEBGL_debug_shaders");
        const src = ext.getTranslatedShaderSource(this.handle) || "Debug source not available";
        return src;
    }
    compile() {
        this.gl.compileShader(this.handle);
    }
    getParameter(param) {
        return this.gl.getShaderParameter(this.handle, param);
    }
    getInfoLog() {
        return this.gl.getShaderInfoLog(this.handle);
    }
    addTo(program) {
        program.attachShader(this.handle);
    }
    removeFrom(program) {
        program.detachShader(this.handle);
    }
}
export class ShaderFragment extends Shader {
    constructor(gl, src) {
        super("fragment", gl, src);
    }
}
export class ShaderVertex extends Shader {
    constructor(gl, src) {
        super("vertex", gl, src);
    }
}
//# sourceMappingURL=Shader.js.map