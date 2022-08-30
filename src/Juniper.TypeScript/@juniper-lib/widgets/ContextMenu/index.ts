import { className } from "@juniper-lib/dom/attrs";
import { left, px, top } from "@juniper-lib/dom/css";
import { onClick } from "@juniper-lib/dom/evts";
import { Button, Div, elementApply, elementClearChildren, elementSetDisplay, ErsatzElement } from "@juniper-lib/dom/tags";
import { Task } from "@juniper-lib/tslib/events/Task";
import { isDefined } from "@juniper-lib/tslib/typeChecks";
import "./styles";


export class ContextMenu<T extends string = string> implements ErsatzElement {

    readonly element: HTMLElement;

    private readonly displayNames: Map<T, string>

    private currentTask: Task<any>;
    private mouseX = 0;
    private mouseY = 0;

    constructor(displayNames?: Map<T, string>) {

        this.displayNames = displayNames || new Map<T, string>();

        this.element = Div(
            className("context-menu")
        );

        elementSetDisplay(this.element, false);

        window.addEventListener("mousemove", (evt) => {
            this.mouseX = evt.clientX;
            this.mouseY = evt.clientY;
        });
    }

    public async cancel() {
        if (isDefined(this.currentTask)) {
            this.currentTask.resolve("cancel");
            await this.currentTask;
            this.currentTask = null;
        }
    }

    public async show(...options: readonly (T | HTMLHRElement)[]): Promise<T | null> {
        if (isDefined(this.currentTask)) {
            await this.cancel();
        }

        const task = new Task<T | null>();
        this.currentTask = task;

        elementClearChildren(this.element);
        elementApply(this.element,
            left(px(this.mouseX)),
            top(px(this.mouseY)),
            ...options.map(option => {
                if (option instanceof HTMLHRElement) {
                    option.style.width = "100%";
                    return option;
                }
                else {
                    return Button(
                        this.displayNames.has(option) ? this.displayNames.get(option) : option,
                        onClick(() => this.currentTask.resolve(option), true)
                    );
                }
            })
        );
        elementSetDisplay(this.element, true, "grid");
        this.mouseY = Math.min(this.mouseY, window.innerHeight - this.element.clientHeight - 50);
        this.element.style.top = px(this.mouseY);

        const onSideClick = () => this.currentTask.resolve("cancel");
        addEventListener("click", onSideClick);
        this.currentTask.finally(() => {
            elementSetDisplay(this.element, false);
            removeEventListener("click", onSideClick);
        });

        return await task;
    }
}