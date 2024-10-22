import { singleton } from "@juniper-lib/util";
import { Progress, SingletonStyleBlob, SpanTag, Value, display, flexDirection, registerFactory, rule, width } from "@juniper-lib/dom";
import { BaseProgress } from "@juniper-lib/progress";
export class TypedProgressBar extends HTMLElement {
    #progressBar;
    #statusOutput;
    #mixin;
    constructor() {
        super();
        SingletonStyleBlob("Juniper::Widgets::TypedProgressBar::Style", () => rule("progress-bar", display("flex"), flexDirection("column"), rule("> progress", width("100%"))));
        this.#progressBar = Progress(Value(0));
        this.#statusOutput = SpanTag();
        this.#mixin = new BaseProgress();
        this.#mixin.addEventListener("progress", evt => {
            this.#statusOutput.replaceChildren(evt.progress.basicMessage);
            this.#progressBar.value = evt.progress.p;
        });
    }
    connectedCallback() {
        if (!this.#progressBar.isConnected) {
            this.append(this.#statusOutput, this.#progressBar);
        }
    }
    report(soFar, total, message, est) {
        this.#mixin.report(soFar, total, message, est);
    }
    attach(prog) {
        this.#mixin.attach(prog);
    }
    clear() {
        this.#mixin.clear();
    }
    start(msg) {
        this.#mixin.start(msg);
    }
    end(msg) {
        this.#mixin.end(msg);
    }
    static install() {
        return singleton("Juniper::Widgets::TypedProgressBarElement", () => registerFactory("progress-bar", TypedProgressBar));
    }
}
export function ProgressBar(...rest) {
    return TypedProgressBar.install()(...rest);
}
//# sourceMappingURL=ProgressBar.js.map