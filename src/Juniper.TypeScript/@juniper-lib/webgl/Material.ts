import { assertNever } from "@juniper-lib/tslib/typeChecks";
import type { mat4 } from "gl-matrix";
import { BaseProgram } from "./BaseProgram";
import type { Geometry } from "./Geometry";
import type { Uniform } from "./managed/object/Uniform";
import type { BaseTexture } from "./managed/resource/Texture";

export abstract class BaseMaterial extends BaseProgram {
    private uGamma: Uniform;
    private uModel: Uniform;

    constructor(gl: WebGL2RenderingContext, vertSrc: string, fragSrc: string) {
        super(gl, vertSrc, fragSrc);

        this.uGamma = this.program.getUniform("uGamma");
        this.uModel = this.program.getUniform("uModel");
    }

    setGamma(gamma: number) {
        this.uGamma.set1f(gamma);
    }

    setModel(model: mat4) {
        this.uModel.setMatrix4fv(model);
    }

    abstract setGeometry(geom: Geometry): void;
    abstract setTexture(texture: BaseTexture): void;
}

export abstract class Material extends BaseMaterial {

    private uProjection: Uniform;
    private uView: Uniform;
    private uEye: Uniform;

    constructor(gl: WebGL2RenderingContext, vertSrc: string, fragSrc: string) {
        super(gl, vertSrc, fragSrc);

        this.uProjection = this.program.getUniform("uProjection");
        this.uView = this.program.getUniform("uView");
        this.uEye = this.program.getUniform("uEye");
    }

    setEye(eye: XREye, isStereo: boolean) {
        switch (eye) {
            case "none": this.uEye.set1i(isStereo ? -1 : 0); break;
            case "left": this.uEye.set1i(-1); break;
            case "right": this.uEye.set1i(1); break;
            default: assertNever(eye);
        }
    }

    setProjection(proj: mat4) {
        this.uProjection.setMatrix4fv(proj);
    }

    setView(view: mat4) {
        this.uView.setMatrix4fv(view);
    }
}