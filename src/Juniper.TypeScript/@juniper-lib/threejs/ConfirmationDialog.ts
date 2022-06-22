import {
    fontSize,
    maxWidth,
    padding,
    styles,
    textAlign,
    width
} from "@juniper-lib/dom/css";
import {
    elementApply,
    elementIsDisplayed,
    elementSetDisplay,
    elementSetText
} from "@juniper-lib/dom/tags";
import { Animator } from "@juniper-lib/graphics2d/animation/Animator";
import { jump } from "@juniper-lib/graphics2d/animation/tween";
import { TextImageOptions } from "@juniper-lib/graphics2d/TextImage";
import { DialogBox } from "@juniper-lib/widgets/DialogBox";
import type { Environment } from "./environment/Environment";
import { objectSetVisible, objGraph } from "./objects";
import { TextMesh } from "./widgets/TextMesh";
import { TextMeshButton } from "./widgets/TextMeshButton";
import type { Widget } from "./widgets/widgets";

const baseTextStyle: Partial<TextImageOptions> = {
    bgStrokeColor: "#000000",
    bgStrokeSize: 0.04,
    wrapWords: false,
    textFillColor: "#ffffff",
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
    bgFillColor: "#0078d7"
});

const cancelButton3DStyle: Partial<TextImageOptions> = Object.assign({}, textButtonStyle, {
    bgFillColor: "#780000"
});

const textLabelStyle: Partial<TextImageOptions> = Object.assign({}, baseTextStyle, {
    bgFillColor: "#ffffff",
    textFillColor: "#000000",
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
export class ConfirmationDialog extends DialogBox implements Widget {
    readonly object = new THREE.Object3D();
    readonly name = "ConfirmationDialog";

    private readonly root = new THREE.Object3D();
    readonly mesh: TextMesh;
    private readonly confirmButton3D: TextMeshButton;
    private readonly cancelButton3D: TextMeshButton;

    private readonly animator = new Animator();

    constructor(private readonly env: Environment, fontFamily: string) {
        super("Confirm action");

        this.confirmButton.innerText = "Yes";
        this.cancelButton.innerText = "No";

        this.mesh = new TextMesh(this.env, "confirmationDialogLabel", newStyle(textLabelStyle, fontFamily));

        this.confirmButton3D = new TextMeshButton(this.env.fetcher, this.env, "confirmationDialogConfirmButton", "Yes", newStyle(confirmButton3DStyle, fontFamily));
        this.confirmButton3D.target.addEventListener("click", () =>
            this.confirmButton.click());
        this.confirmButton3D.position.set(1, -0.5, 0.5);

        this.cancelButton3D = new TextMeshButton(this.env.fetcher, this.env, "confirmationDialogCancelButton", "No", newStyle(cancelButton3DStyle, fontFamily));
        this.cancelButton3D.target.addEventListener("click", () =>
            this.cancelButton.click());

        this.cancelButton3D.position.set(2, -0.5, 0.5);

        elementApply(this.container, styles(
            maxWidth("calc(100% - 2em)"),
            width("max-content")
        ));

        elementApply(this.contentArea, styles(
            fontSize("18pt"),
            textAlign("center"),
            padding("1em")
        ));

        objGraph(this,
            objGraph(this.root,
                this.mesh,
                this.confirmButton3D,
                this.cancelButton3D));

        objectSetVisible(this.root, false);
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
        await this.animator.start(0, 0.25, (t) => {
            const scale = jump(a + b * t, JUMP_FACTOR);
            this.root.scale.set(scale, scale, 0.01);
        });
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