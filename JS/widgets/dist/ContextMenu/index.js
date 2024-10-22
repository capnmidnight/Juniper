import { isDefined, singleton } from "@juniper-lib/util";
import { backgroundColor, border, borderRadius, boxShadow, Button, Clear, display, elementSetDisplay, em, getSystemFamily, gridTemplateColumns, HtmlRender, left, margin, OnClick, padding, perc, position, px, registerFactory, rule, SingletonStyleBlob, textAlign, top, TypedHTMLElement } from "@juniper-lib/dom";
import { Task } from "@juniper-lib/events";
export class ContextMenuElement extends TypedHTMLElement {
    constructor() {
        super();
        this.mouseX = 0;
        this.mouseY = 0;
        SingletonStyleBlob("Juniper::Widgets::ContextMenuElement", () => rule("context-menu", position("absolute"), backgroundColor("white"), padding(px(5)), display("grid"), gridTemplateColumns("auto"), borderRadius(px(5)), boxShadow("rgb(0, 0, 0, 0.15) 2px 2px 17px"), rule(">button", border("none"), textAlign("left"), backgroundColor("transparent"), margin(px(2)), padding(0, em(2), 0, em(.5)), getSystemFamily(), rule(":hover", backgroundColor("lightgrey")))));
        elementSetDisplay(this, false);
        window.addEventListener("mousemove", (evt) => {
            this.mouseX = evt.clientX;
            this.mouseY = evt.clientY;
        });
    }
    async cancel() {
        if (isDefined(this.currentTask)) {
            this.currentTask.resolve("cancel");
            await this.currentTask;
            this.currentTask = null;
        }
    }
    async show(displayNamesOrFirstOption, ...options) {
        let displayNames;
        if (displayNamesOrFirstOption instanceof Map) {
            displayNames = displayNamesOrFirstOption;
        }
        else {
            displayNames = new Map();
            options.unshift(displayNamesOrFirstOption);
        }
        if (isDefined(this.currentTask)) {
            await this.cancel();
        }
        const task = new Task();
        this.currentTask = task;
        HtmlRender(this, Clear(), left(px(this.mouseX)), top(px(this.mouseY)), ...options.map(option => {
            if (option instanceof HTMLHRElement) {
                option.style.width = perc(100);
                return option;
            }
            else {
                return Button(displayNames.has(option) ? displayNames.get(option) : option.toString(), OnClick(this.currentTask.resolver(option), true));
            }
        }));
        elementSetDisplay(this, true, "grid");
        this.mouseY = Math.min(this.mouseY, window.innerHeight - this.clientHeight - 50);
        this.style.top = px(this.mouseY);
        const onSideClick = this.currentTask.resolver("cancel");
        addEventListener("click", onSideClick);
        this.currentTask.finally(() => {
            elementSetDisplay(this, false);
            removeEventListener("click", onSideClick);
        });
        return await task;
    }
    static install() { return singleton("Juniper::Widgets::ContextMenuElement", () => registerFactory("context-menu", ContextMenuElement)); }
}
export function ContextMenu() {
    return ContextMenuElement.install()();
}
//# sourceMappingURL=index.js.map