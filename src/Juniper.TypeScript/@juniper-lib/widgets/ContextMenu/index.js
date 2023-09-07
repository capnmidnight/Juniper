import { ClassList } from "@juniper-lib/dom/attrs";
import { left, perc, px, top } from "@juniper-lib/dom/css";
import { onClick } from "@juniper-lib/dom/evts";
import { Button, Div, HtmlRender, elementClearChildren, elementSetDisplay } from "@juniper-lib/dom/tags";
import { Task } from "@juniper-lib/events/Task";
import { isDefined } from "@juniper-lib/tslib/typeChecks";
import "./styles.css";
export class ContextMenu {
    constructor() {
        this.mouseX = 0;
        this.mouseY = 0;
        this.element = Div(ClassList("context-menu"));
        elementSetDisplay(this.element, false);
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
        elementClearChildren(this.element);
        HtmlRender(this.element, left(px(this.mouseX)), top(px(this.mouseY)), ...options.map(option => {
            if (option instanceof HTMLHRElement) {
                option.style.width = perc(100);
                return option;
            }
            else {
                return Button(displayNames.has(option) ? displayNames.get(option) : option.toString(), onClick(this.currentTask.resolver(option), true));
            }
        }));
        elementSetDisplay(this.element, true, "grid");
        this.mouseY = Math.min(this.mouseY, window.innerHeight - this.element.clientHeight - 50);
        this.element.style.top = px(this.mouseY);
        const onSideClick = this.currentTask.resolver("cancel");
        addEventListener("click", onSideClick);
        this.currentTask.finally(() => {
            elementSetDisplay(this.element, false);
            removeEventListener("click", onSideClick);
        });
        return await task;
    }
}
//# sourceMappingURL=index.js.map