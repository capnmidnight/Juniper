import { Button, TitleAttr } from "@juniper-lib/dom";
import { obj, objectSetVisible, objGraph } from "../objects";
import { TextMeshButton } from "./TextMeshButton";
import { Widget } from "./widgets";
export class ButtonTextWidget extends Widget {
    constructor(env, name, text, textButtonStyle) {
        super(Button(TitleAttr(name), text), obj(`${name}-button`), "inline-block");
        this.env = env;
        this.mesh = new TextMeshButton(this.env, `${name}-button`, text, textButtonStyle);
        objGraph(this, this.mesh);
        this.mesh.addEventListener("click", () => {
            this.content.click();
        });
    }
    get visible() {
        return super.visible;
    }
    set visible(v) {
        super.visible = true;
        objectSetVisible(this.mesh, v);
    }
}
//# sourceMappingURL=ButtonTextWidget.js.map