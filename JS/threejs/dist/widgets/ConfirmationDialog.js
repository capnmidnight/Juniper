import { display, H1, HtmlProp, P, registerFactory, rgb, rule, SingletonStyleBlob } from "@juniper-lib/dom";
import { Animator, jump } from "@juniper-lib/graphics2d";
import { BaseDialogElement, CancelButtonText, SaveButtonText } from "@juniper-lib/widgets";
import { Cancelable } from "@juniper-lib/widgets/src/Cancelable";
import { obj, objectSetVisible, objGraph } from "../objects";
import { TextMesh } from "./TextMesh";
import { TextMeshButton } from "./TextMeshButton";
import { singleton } from "@juniper-lib/util";
const baseTextStyle = {
    bgStrokeColor: "black",
    bgStrokeSize: 0.04,
    textFillColor: "white",
    scale: 150
};
const textButtonStyle = Object.assign({}, baseTextStyle, {
    padding: {
        left: 0.1,
        right: 0.1,
        top: 0.025,
        bottom: 0.025
    },
    fontSize: 20,
    minHeight: 0.5,
    maxHeight: 0.5
});
const confirmButton3DStyle = Object.assign({}, textButtonStyle, {
    bgFillColor: rgb(0, 117, 215)
});
const cancelButton3DStyle = Object.assign({}, textButtonStyle, {
    bgFillColor: rgb(117, 0, 0)
});
const textLabelStyle = Object.assign({}, baseTextStyle, {
    bgFillColor: "white",
    textFillColor: "black",
    padding: {
        top: 0.1,
        left: 0.1,
        bottom: 0.4,
        right: 0.1
    },
    fontSize: 25,
    minHeight: 1,
    maxHeight: 1
});
const JUMP_FACTOR = 0.9;
function newStyle(baseStyle, fontFamily) {
    return Object.assign({}, baseStyle, { fontFamily });
}
export function EnvironmentAttr(env) {
    return new HtmlProp("env", env);
}
export class ConfirmationDialogElement extends BaseDialogElement {
    get content() { return this; }
    #env;
    get env() { return this.#env; }
    set env(v) { this.#env = v; }
    constructor(fontFamily) {
        super(H1(), P(), Cancelable(true), CancelButtonText("No"), SaveButtonText("Yes"));
        this.content3d = obj("ConfirmationDialog");
        this.root = obj("Root");
        this.animator = new Animator();
        this.a = 0;
        this.b = 0;
        this.#env = null;
        SingletonStyleBlob("Juniper::ThreeJS::ConfirmationDialog", () => rule("confirmation-dialog", display("contents")));
        this.mesh = new TextMesh(this.env, "confirmationDialogLabel", "none", newStyle(textLabelStyle, fontFamily));
        this.confirmButton3D = new TextMeshButton(this.env, "confirmationDialogConfirmButton", "Yes", newStyle(confirmButton3DStyle, fontFamily));
        this.confirmButton3D.addEventListener("click", () => this.confirm());
        this.confirmButton3D.content3d.position.set(1, -0.5, 0.5);
        this.cancelButton3D = new TextMeshButton(this.env, "confirmationDialogCancelButton", "No", newStyle(cancelButton3DStyle, fontFamily));
        this.cancelButton3D.addEventListener("click", () => this.cancel());
        this.cancelButton3D.content3d.position.set(2, -0.5, 0.5);
        objGraph(this, objGraph(this.root, this.mesh, this.confirmButton3D, this.cancelButton3D));
        objectSetVisible(this.root, false);
        this.root.scale.setScalar(0);
        this.onTick = (t) => {
            const scale = jump(this.a + this.b * t, JUMP_FACTOR);
            this.root.scale.set(scale, scale, 0.01);
        };
        this.dialog.addEventListener("showing", async (evt) => {
            if (this.use3D) {
                this.root.visible = true;
                await this.showHide(0, 1);
            }
            evt.resolve();
        });
        this.dialog.addEventListener("shown", () => {
            if (this.use3D) {
                this.dialog.style.display = "none";
            }
        });
        this.dialog.addEventListener("closing", async () => {
            if (this.use3D) {
                await this.showHide(1, -1);
                this.root.visible = false;
            }
        });
    }
    get name() {
        return this.content3d.name;
    }
    get visible() {
        return this.open;
    }
    set visible(visible) {
        this.open = visible;
        this.mesh.visible = visible;
    }
    update(dt) {
        this.animator.update(dt);
    }
    async showHide(a, b) {
        this.a = a;
        this.b = b;
        await this.animator.start(0, 0.25, this.onTick);
        this.animator.clear();
    }
    get use3D() {
        return this.env.renderer.xr.isPresenting || this.env.testSpaceLayout;
    }
    prompt(title, message) {
        this.title = title;
        this.body.replaceChildren(message);
        this.mesh.image.value = message;
        return this.show();
    }
    static install() { return singleton("Juniper::ThreeJS::ConfirmationDialog", () => registerFactory("confirmation-dialog", ConfirmationDialogElement)); }
}
export function ConfirmationDialog(...rest) { return ConfirmationDialogElement.install()(...rest); }
//# sourceMappingURL=ConfirmationDialog.js.map