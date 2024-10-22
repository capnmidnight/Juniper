import { dispose } from "@juniper-lib/util";
import { Attrib } from "../object/Attrib";
import { Uniform } from "../object/Uniform";
import { ManagedWebGLResource } from "./ManagedWebGLResource";
// Manage the shader program
export class ShaderProgram extends ManagedWebGLResource {
    /**
     * @param gl - the rendering context in which we're working.
     * @param vertexShader - first half of the shader program.
     * @param fragmentShader - the second half of the shader program.
     */
    constructor(gl, vertexShader, fragmentShader) {
        super(gl, gl.createProgram());
        this.vertexShader = vertexShader;
        this.fragmentShader = fragmentShader;
        this.vertexShader.addTo(this);
        this.fragmentShader.addTo(this);
        this.link();
        this.validate();
        if (!this.getParameter(gl.LINK_STATUS)) {
            const errorMessage = this.getInfoLog() || "Unknown error";
            dispose(this);
            throw new Error(errorMessage);
        }
    }
    onDisposing() {
        this.vertexShader.removeFrom(this);
        this.fragmentShader.removeFrom(this);
        this.gl.deleteProgram(this.handle);
    }
    link() {
        this.gl.linkProgram(this.handle);
    }
    validate() {
        this.gl.validateProgram(this.handle);
    }
    use() {
        this.gl.useProgram(this.handle);
    }
    getInfoLog() {
        return this.gl.getProgramInfoLog(this.handle);
    }
    attachShader(shader) {
        this.gl.attachShader(this.handle, shader);
    }
    detachShader(shader) {
        this.gl.detachShader(this.handle, shader);
    }
    getParameter(param) {
        return this.gl.getProgramParameter(this.handle, param);
    }
    getAttrib(name) {
        return new Attrib(this.gl, this.gl.getAttribLocation(this.handle, name), name);
    }
    getUniform(name) {
        return new Uniform(this.gl, this.gl.getUniformLocation(this.handle, name), name);
    }
    bindAttribLocation(name, location) {
        this.gl.bindAttribLocation(this.handle, location, name);
    }
}
//# sourceMappingURL=ShaderProgram.js.map