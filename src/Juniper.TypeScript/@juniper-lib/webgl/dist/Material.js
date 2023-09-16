import { assertNever } from "@juniper-lib/tslib/dist/typeChecks";
import { BaseProgram } from "./BaseProgram";
export class BaseMaterial extends BaseProgram {
    constructor(gl, vertSrc, fragSrc) {
        super(gl, vertSrc, fragSrc);
        this.uGamma = this.program.getUniform("uGamma");
        this.uModel = this.program.getUniform("uModel");
    }
    setGamma(gamma) {
        this.uGamma.set1f(gamma);
    }
    setModel(model) {
        this.uModel.setMatrix4fv(model);
    }
}
export class Material extends BaseMaterial {
    constructor(gl, vertSrc, fragSrc) {
        super(gl, vertSrc, fragSrc);
        this.uProjection = this.program.getUniform("uProjection");
        this.uView = this.program.getUniform("uView");
        this.uEye = this.program.getUniform("uEye");
    }
    setEye(eye, isStereo) {
        switch (eye) {
            case "none":
                this.uEye.set1i(isStereo ? -1 : 0);
                break;
            case "left":
                this.uEye.set1i(-1);
                break;
            case "right":
                this.uEye.set1i(1);
                break;
            default: assertNever(eye);
        }
    }
    setProjection(proj) {
        this.uProjection.setMatrix4fv(proj);
    }
    setView(view) {
        this.uView.setMatrix4fv(view);
    }
}
//# sourceMappingURL=Material.js.map