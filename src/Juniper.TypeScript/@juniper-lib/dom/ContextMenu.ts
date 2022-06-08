import { isDefined, Task } from "@juniper-lib/tslib";
import { className } from "./attrs";
import { backgroundColor, border, left, padding, position, rule, styles, top } from "./css";
import { onClick } from "./evts";
import { Button, elementApply, elementClearChildren, elementSetDisplay, ErsatzElement, LI, Style, UL } from "./tags";

Style(
    rule(".context-menu",
        position("absolute"),
        backgroundColor("white"),
        padding("5px"),
        border("outset 2px #ccc")
    )
);

export class ContextMenu implements ErsatzElement {

    element: HTMLElement;

    private currentTask: Task<any>;

    constructor() {
        this.element = UL(
            className("context-menu")
        );

        elementSetDisplay(this.element, false);
    }

    public async show<T extends string>(clientX: number, clientY: number, ...options: T[]): Promise<T | null> {
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
                left(`${clientX}px`),
                top(`${clientY}px`)
            ),
            ...options.map(option =>
                LI(Button(
                    option,
                    onClick(() => this.currentTask.resolve(option), true)
                ))
            )
        );
        elementSetDisplay(this.element, true);
        clientY = Math.min(clientY, window.innerHeight - this.element.clientHeight - 50);
        this.element.style.top = `${clientY}px`;

        const onSideClick = () => this.currentTask.resolve("cancel");
        addEventListener("click", onSideClick);
        this.currentTask.finally(() => {
            elementSetDisplay(this.element, false);
            removeEventListener("click", onSideClick);
        });

        return await task;
    }
}