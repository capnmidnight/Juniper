import { ClassList } from "@juniper-lib/dom/attrs";
import { left, perc, px, top } from "@juniper-lib/dom/css";
import { onClick } from "@juniper-lib/dom/evts";
import { Button, Div, ErsatzElement, elementApply, elementClearChildren, elementSetDisplay } from "@juniper-lib/dom/tags";
import { Task } from "@juniper-lib/events/Task";
import { isDefined } from "@juniper-lib/tslib/typeChecks";

import "./styles.css";


export class ContextMenu implements ErsatzElement {

    readonly element: HTMLElement;

    private currentTask: Task<any>;
    private mouseX = 0;
    private mouseY = 0;

    constructor() {
        this.element = Div(
            ClassList("context-menu")
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
    
    public async show<T>(displayNames: Map<T, string>, ...options: (T | HTMLHRElement)[]): Promise<T | null>;
    public async show<T>(...options: (T | HTMLHRElement)[]): Promise<T | null>
    public async show<T>(displayNamesOrFirstOption: T | HTMLHRElement | Map<T, string>, ...options: (T | HTMLHRElement)[]): Promise<T | null> {
        let displayNames: Map<T, string>;
        if (displayNamesOrFirstOption instanceof Map) {
            displayNames = displayNamesOrFirstOption;
        }
        else {
            displayNames = new Map<T, string>();
            options.unshift(displayNamesOrFirstOption);
        }

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
                    option.style.width = perc(100);
                    return option;
                }
                else {
                    return Button(
                        displayNames.has(option) ? displayNames.get(option) : option.toString(),
                        onClick(this.currentTask.resolver(option), true)
                    );
                }
            })
        );
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