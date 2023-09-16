import { Attrib } from "../object/Attrib";
import { Uniform } from "../object/Uniform";
import { ManagedWebGLResource } from "./ManagedWebGLResource";
import type { ShaderFragment, ShaderVertex } from "./Shader";
export declare class ShaderProgram extends ManagedWebGLResource<WebGLProgram> {
    private vertexShader;
    private fragmentShader;
    /**
     * @param gl - the rendering context in which we're working.
     * @param vertexShader - first half of the shader program.
     * @param fragmentShader - the second half of the shader program.
     */
    constructor(gl: WebGL2RenderingContext, vertexShader: ShaderVertex, fragmentShader: ShaderFragment);
    protected onDisposing(): void;
    link(): void;
    validate(): void;
    use(): void;
    getInfoLog(): string;
    attachShader(shader: WebGLShader): void;
    detachShader(shader: WebGLShader): void;
    getParameter(param: number): any;
    getAttrib(name: string): Attrib;
    getUniform(name: string): Uniform;
    bindAttribLocation(name: string, location: number): void;
}
//# sourceMappingURL=ShaderProgram.d.ts.map