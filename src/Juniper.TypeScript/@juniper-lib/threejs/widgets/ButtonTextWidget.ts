import { title } from "@juniper-lib/dom/attrs";
import { ButtonPrimary } from "@juniper-lib/dom/tags";
import { TextImageOptions } from "@juniper-lib/graphics2d/TextImage";
import { BaseEnvironment } from "../environment/BaseEnvironment";
import { obj, objectSetVisible, objGraph } from "../objects";
import { TextMeshButton } from "./TextMeshButton";
import { Widget } from "./widgets";


export class ButtonTextWidget extends Widget<HTMLButtonElement> {

    readonly mesh: TextMeshButton;

    constructor(protected readonly env: BaseEnvironment, name: string, text: string, textButtonStyle: Partial<TextImageOptions>) {
        super(
            ButtonPrimary(
                title(name),
                text
            ),
            obj(`${name}-button`),
            "inline-block"
        );

        this.mesh = new TextMeshButton(this.env, `${name}-button`, text, textButtonStyle)
        objGraph(this, this.mesh);
        this.mesh.addEventListener("click", () => {
            this.element.click();
        });
    }

    override get visible(): boolean {
        return super.visible;
    }

    override set visible(v: boolean) {
        super.visible = true;
        objectSetVisible(this.mesh, v);
    }
}
