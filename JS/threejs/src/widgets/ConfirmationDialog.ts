import {
    display,
    ElementChild,
    H1,
    HtmlProp,
    P,
    registerFactory,
    rgb,
    rule,
    SingletonStyleBlob
} from "@juniper-lib/dom";
import { Animator, jump, TextImageOptions } from "@juniper-lib/graphics2d";
import { BaseDialogElement, CancelButtonText, SaveButtonText } from "@juniper-lib/widgets";
import { Cancelable } from "@juniper-lib/widgets/src/Cancelable";
import { BaseEnvironment } from "../environment";
import { obj, objectSetVisible, objGraph } from "../objects";
import { TextMesh } from "./TextMesh";
import { TextMeshButton } from "./TextMeshButton";
import type { IWidget } from "./widgets";
import { singleton } from "@juniper-lib/util";

const baseTextStyle: Partial<TextImageOptions> = {
    bgStrokeColor: "black",
    bgStrokeSize: 0.04,
    textFillColor: "white",
    scale: 150
};

const textButtonStyle: Partial<TextImageOptions> = Object.assign({}, baseTextStyle, {
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

const confirmButton3DStyle: Partial<TextImageOptions> = Object.assign({}, textButtonStyle, {
    bgFillColor: rgb(0, 117, 215)
});

const cancelButton3DStyle: Partial<TextImageOptions> = Object.assign({}, textButtonStyle, {
    bgFillColor: rgb(117, 0, 0)
});

const textLabelStyle: Partial<TextImageOptions> = Object.assign({}, baseTextStyle, {
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
function newStyle(baseStyle: Partial<TextImageOptions>, fontFamily: string): Partial<TextImageOptions> {
    return Object.assign({}, baseStyle, { fontFamily });
}

export function EnvironmentAttr(env: BaseEnvironment) {
    return new HtmlProp("env", env);
}

export class ConfirmationDialogElement extends BaseDialogElement<void, boolean> implements IWidget {
    get content() { return this; }
    readonly content3d = obj("ConfirmationDialog");

    private readonly root = obj("Root");
    readonly mesh: TextMesh;
    private readonly confirmButton3D: TextMeshButton;
    private readonly cancelButton3D: TextMeshButton;

    private readonly animator = new Animator();

    private a = 0;
    private b = 0;
    private readonly onTick: (t: number) => void;

    #env: BaseEnvironment = null;
    get env() { return this.#env; }
    set env(v) { this.#env = v; }

    constructor(fontFamily: string) {

        super(
            H1(),
            P(),
            Cancelable(true),
            CancelButtonText("No"),
            SaveButtonText("Yes")
        );

        SingletonStyleBlob("Juniper::ThreeJS::ConfirmationDialog", () =>
            rule("confirmation-dialog",
                display("contents")
            )
        );

        this.mesh = new TextMesh(this.env, "confirmationDialogLabel", "none", newStyle(textLabelStyle, fontFamily));

        this.confirmButton3D = new TextMeshButton(this.env, "confirmationDialogConfirmButton", "Yes", newStyle(confirmButton3DStyle, fontFamily));
        this.confirmButton3D.addEventListener("click", () =>
            this.confirm());
        this.confirmButton3D.content3d.position.set(1, -0.5, 0.5);

        this.cancelButton3D = new TextMeshButton(this.env, "confirmationDialogCancelButton", "No", newStyle(cancelButton3DStyle, fontFamily));
        this.cancelButton3D.addEventListener("click", () =>
            this.cancel());

        this.cancelButton3D.content3d.position.set(2, -0.5, 0.5);

        objGraph(this,
            objGraph(this.root,
                this.mesh,
                this.confirmButton3D,
                this.cancelButton3D));

        objectSetVisible(this.root, false);
        this.root.scale.setScalar(0);
        this.onTick = (t: number) => {
            const scale = jump(this.a + this.b * t, JUMP_FACTOR);
            this.root.scale.set(scale, scale, 0.01);
        };

        this.dialog.addEventListener("showing", async evt => {
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

    update(dt: number) {
        this.animator.update(dt);
    }

    private async showHide(a: number, b: number): Promise<void> {
        this.a = a;
        this.b = b;
        await this.animator.start(0, 0.25, this.onTick);
        this.animator.clear();
    }

    private get use3D() {
        return this.env.renderer.xr.isPresenting || this.env.testSpaceLayout;
    }

    prompt(title: string, message: string): Promise<boolean> {
        this.title = title;
        this.body.replaceChildren(message);
        this.mesh.image.value = message;

        return this.show();
    }

    static install() { return singleton("Juniper::ThreeJS::ConfirmationDialog", () => registerFactory("confirmation-dialog", ConfirmationDialogElement)); }
}

export function ConfirmationDialog(...rest: ElementChild<ConfirmationDialogElement>[]) { return ConfirmationDialogElement.install()(...rest); }