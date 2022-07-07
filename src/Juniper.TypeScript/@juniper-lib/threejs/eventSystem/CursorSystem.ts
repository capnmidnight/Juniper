import { BaseEnvironment } from "../environment/BaseEnvironment";
import { BaseCursor } from "./BaseCursor";

export class CursorSystem extends BaseCursor {
    private _hidden = false;

    constructor(env: BaseEnvironment, public element: HTMLElement) {
        super(env);

        this.visible = true;
        this.style = "default";

        document.addEventListener("pointerlockchange", () => {
            this._hidden = !!document.pointerLockElement;
            this.refresh();
        });
    }

    get position(): THREE.Vector3 {
        throw new Error("BaseCursor::get_Position(): Method not implemented.");
    }

    override get style() {
        return super.style;
    }

    override set style(v) {
        super.style = v;
        this.refresh();
    }

    override get visible() {
        return super.visible
            && !this._hidden;
    }

    override set visible(v) {
        super.visible = v;
        this.refresh();
    }

    private refresh() {
        this.element.style.cursor = this.visible
            ? this.style
            : "none";
    }
}