import {
    em,
    fontSize, maxWidth,
    padding,
    perc,
    pt,
    rgb,
    textAlign,
    width
} from "@juniper-lib/dom/dist/css";
import {
    HtmlRender,
    elementIsDisplayed,
    elementSetDisplay,
    elementSetText
} from "@juniper-lib/dom/dist/tags";
import { Animator } from "@juniper-lib/graphics2d/dist/animation/Animator";
import { jump } from "@juniper-lib/graphics2d/dist/animation/tween";
import { TextImageOptions } from "@juniper-lib/graphics2d/dist/TextImage";
import { DialogBox } from "@juniper-lib/widgets/dist/DialogBox";
import type { Environment } from "../environment/Environment";
import { obj, objectSetVisible, objGraph } from "../objects";
import { TextMesh } from "./TextMesh";
import { TextMeshButton } from "./TextMeshButton";
import type { IWidget } from "./widgets";

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
export class ConfirmationDialog extends DialogBox implements IWidget {
    readonly object = obj("ConfirmationDialog");

    private readonly root = obj("Root");
    readonly mesh: TextMesh;
    private readonly confirmButton3D: TextMeshButton;
    private readonly cancelButton3D: TextMeshButton;

    private readonly animator = new Animator();

    private a = 0;
    private b = 0;
    private readonly onTick: (t: number) => void;

    constructor(private readonly env: Environment, fontFamily: string) {
        super("Confirm action");

        this.confirmButton.innerText = "Yes";
        this.cancelButton.innerText = "No";

        this.mesh = new TextMesh(this.env, "confirmationDialogLabel", "none", newStyle(textLabelStyle, fontFamily));

        this.confirmButton3D = new TextMeshButton(this.env, "confirmationDialogConfirmButton", "Yes", newStyle(confirmButton3DStyle, fontFamily));
        this.confirmButton3D.addEventListener("click", () =>
            this.confirmButton.click());
        this.confirmButton3D.object.position.set(1, -0.5, 0.5);

        this.cancelButton3D = new TextMeshButton(this.env, "confirmationDialogCancelButton", "No", newStyle(cancelButton3DStyle, fontFamily));
        this.cancelButton3D.addEventListener("click", () =>
            this.cancelButton.click());

        this.cancelButton3D.object.position.set(2, -0.5, 0.5);

        HtmlRender(this.container,
            maxWidth(`calc(${perc(100)} - ${em(2)})`),
            width("max-content")
        );

        HtmlRender(this.contentArea,
            fontSize(pt(18)),
            textAlign("center"),
            padding(em(1))
        );

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
    }

    get name() {
        return this.object.name;
    }

    get visible() {
        return elementIsDisplayed(this);
    }

    set visible(visible) {
        elementSetDisplay(this, visible, "inline-block");
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

    protected override async onShowing(): Promise<void> {
        await super.onShowing();

        if (this.use3D) {
            this.root.visible = true;
            await this.showHide(0, 1);
        }
    }

    override onShown(): void {
        if (this.use3D) {
            this.element.style.display = "none";
        }
    }

    protected override async onClosing(): Promise<void> {
        if (this.use3D) {
            await this.showHide(1, -1);
            this.root.visible = false;
        }

        await super.onClosing();
    }

    prompt(title: string, message: string): Promise<boolean> {
        this.title = title;
        elementSetText(this.contentArea, message);
        this.mesh.image.value = message;

        return this.showDialog();
    }
}