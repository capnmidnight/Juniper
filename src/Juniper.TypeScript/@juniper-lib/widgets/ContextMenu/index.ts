import { className } from "@juniper-lib/dom/attrs";
import { left, styles, top } from "@juniper-lib/dom/css";
import { onClick } from "@juniper-lib/dom/evts";
import { Button, Div, elementApply, elementClearChildren, elementSetDisplay, ErsatzElement } from "@juniper-lib/dom/tags";
import { isDefined, Task } from "@juniper-lib/tslib";

import "./styles";

export class ContextMenu<T> implements ErsatzElement {

    element: HTMLElement;

    private currentTask: Task<any>;
    private mouseX = 0;
    private mouseY = 0;

    constructor(private readonly displayNames: Map<T, string>) {
        this.element = Div(
            className("context-menu")
        );

        elementSetDisplay(this.element, false);

        window.addEventListener("mousemove", (evt) => {
            this.mouseX = evt.clientX;
            this.mouseY = evt.clientY;
        });
    }

    public async show(...options: readonly T[]): Promise<T | null> {
        const hasTask = isDefined(this.currentTask);
        if (hasTask) {
            this.currentTask.resolve("cancel");
            await this.currentTask;
            this.currentTask = null;
        }

        const task = new Task<T | null>();
        this.currentTask = task;

        elementClearChildren(this.element);
        elementApply(this.element,
            styles(
                left(`${this.mouseX}px`),
                top(`${this.mouseY}px`)
            ),
            ...options.map(option =>
                Button(
                    this.displayNames.get(option),
                    onClick(() => this.currentTask.resolve(option), true)
                )
            )
        );
        elementSetDisplay(this.element, true, "grid");
        this.mouseY = Math.min(this.mouseY, window.innerHeight - this.element.clientHeight - 50);
        this.element.style.top = `${this.mouseY}px`;

        const onSideClick = () => this.currentTask.resolve("cancel");
        addEventListener("click", onSideClick);
        this.currentTask.finally(() => {
            elementSetDisplay(this.element, false);
            removeEventListener("click", onSideClick);
        });

        return await task;
    }
}