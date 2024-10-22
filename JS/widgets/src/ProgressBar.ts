import { singleton } from "@juniper-lib/util";
import { ElementChild, Progress, SingletonStyleBlob, SpanTag, Value, display, flexDirection, registerFactory, rule, width } from "@juniper-lib/dom";
import { BaseProgress, IProgress } from "@juniper-lib/progress";

export class TypedProgressBar extends HTMLElement implements IProgress {

    #progressBar: HTMLProgressElement;
    #statusOutput: HTMLElement;
    #mixin: BaseProgress;

    constructor() {
        super();

        SingletonStyleBlob("Juniper::Widgets::TypedProgressBar::Style", () =>
            rule("progress-bar",
                display("flex"),
                flexDirection("column"),
                rule("> progress",
                    width("100%")
                )
            )
        );

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
            this.append(
                this.#statusOutput,
                this.#progressBar
            );
        }
    }

    report(soFar: number, total: number, message?: string, est?: number): void {
        this.#mixin.report(soFar, total, message, est);
    }

    attach(prog: IProgress): void {
        this.#mixin.attach(prog);
    }

    clear(): void {
        this.#mixin.clear();
    }

    start(msg?: string): void {
        this.#mixin.start(msg);
    }

    end(msg?: string): void {
        this.#mixin.end(msg);
    }

    static install() {
        return singleton("Juniper::Widgets::TypedProgressBarElement", () => registerFactory("progress-bar", TypedProgressBar));
    }
}

export function ProgressBar(...rest: ElementChild<TypedProgressBar>[]) {
    return TypedProgressBar.install()(...rest);
}
