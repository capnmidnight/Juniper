import { CustomElement } from "@juniper-lib/dom/CustomElement";
import {
    backgroundColor,
    color,
    columnGap,
    display,
    em,
    getMonospaceFamily,
    gridAutoFlow,
    gridColumn,
    gridTemplateColumns,
    height,
    left,
    opacity,
    overflow,
    overflowY,
    padding,
    perc,
    pointerEvents,
    position,
    rule,
    top,
    width,
    zIndex
} from "@juniper-lib/dom/css";
import { isModifierless } from "@juniper-lib/dom/evts";
import {
    Div,
    ElementChild,
    HtmlRender,
    HtmlTag,
    StyleBlob,
    elementSetDisplay,
    elementToggleDisplay
} from "@juniper-lib/dom/tags";
import { IDebugLogger, MessageType, isWorkerLoggerMessageData } from "./models";

function track(a: number, b: number) {
    return [
        gridColumn(a, b),
        getMonospaceFamily()
    ];
}

const style = StyleBlob(
    rule(":host",
        position("fixed"),
        top(0),
        left(0),
        width(perc(100)),
        height(perc(100)),
        zIndex(9001),
        padding(em(1)),
        opacity(0.5),
        backgroundColor("black"),
        color("white"),
        overflow("hidden"),
        pointerEvents("none")
    ),

    rule(":host > div",
        display("grid"),
        overflowY("auto"),
        columnGap("0.5em"),
        gridAutoFlow("row")
    )
);

export function WindowLogger(...rest: ElementChild[]) {
    const logger = HtmlTag<{ "window-logger": WindowLoggerElement }>("window-logger", ...rest);
    if (!logger.isConnected) {
        document.body.append(logger);
    }
    return logger;
}

@CustomElement("window-logger")
export class WindowLoggerElement extends HTMLElement implements IDebugLogger {
    private readonly workerFunctions = new Map<MessageType, (slug: string, evt: MessageEvent<any>) => void>();
    private readonly logs = new Map<string, Array<any>>();
    private readonly rows = new Map<string, HTMLElement[]>();

    private readonly onKeyPress: (evt: KeyboardEvent) => void;
    private readonly grid: HTMLElement;

    private workerCount = 0;

    constructor() {
        super();
        this.workerFunctions.set("log", this.workerLog.bind(this));
        this.workerFunctions.set("delete", this.workerDelete.bind(this));
        this.workerFunctions.set("clear", this.workerClear.bind(this));

        this.attachShadow({ mode: "closed" })
            .append(
                style,
                this.grid = Div()
            );

        this.onKeyPress = (evt) => {
            if (isModifierless(evt) && evt.key === "Escape") {
                this.toggle();
            }
        };
    }

    connectedCallback() {
        window.addEventListener("keydown", this.onKeyPress);
        this.log("instructions", "Press Esc to toggle Debug View");
        this.close();
    }

    disconnectedCallback() {
        window.removeEventListener("keydown", this.onKeyPress);
    }

    toggle() {
        elementToggleDisplay(this);
    }

    open() {
        elementSetDisplay(this, true);
    }

    close() {
        elementSetDisplay(this, false);
    }

    private render(): void {
        const toRemove = new Array<string>();
        for (const [id, row] of this.rows) {
            if (!this.logs.has(id)) {
                for (const cell of row) {
                    this.grid.removeChild(cell);
                }
                toRemove.push(id);
            }
        }

        for (const id of toRemove) {
            this.rows.delete(id);
        }

        let maxWidth = 0;
        for (const values of this.logs.values()) {
            maxWidth = Math.max(maxWidth, values.length);
        }

        gridTemplateColumns("auto", `repeat(${maxWidth}, 1fr)`)
            .applyToElement(this.grid);

        for (const [id, values] of this.logs) {
            const newRow = [
                Div(id),
                ...values.map((value) => Div(JSON.stringify(value)))
            ];

            for (let i = 0; i < newRow.length; ++i) {
                HtmlRender(newRow[i], ...track(i + 1, i + 2));
            }

            newRow[newRow.length - 1].style.gridColumnEnd = "-1";

            const oldRow = this.rows.get(id) || [];
            this.rows.set(id, newRow);
            let lastCell: HTMLElement = null;
            for (const newCell of newRow) {
                if (oldRow.length > 0) {
                    const oldCell = oldRow.shift();
                    oldCell.replaceWith(newCell);
                }
                else if (lastCell) {
                    lastCell.insertAdjacentElement("afterend", newCell);
                }
                else {
                    this.grid.append(newCell);
                }
                lastCell = newCell;
            }

            while (oldRow.length > 0) {
                oldRow.pop().remove();
            }

            while (oldRow.length > values.length + 1) {
                const cell = oldRow.pop();
                cell.remove();
            }
        }
    }

    log(id: string, ...values: any[]): void {
        this.logs.set(id, values);
        this.render();
    }

    delete(id: string): void {
        this.logs.delete(id);
        this.render();
    }

    clear(): void {
        this.logs.clear();
        this.render();
    }

    addWorker(name: string, worker: Worker) {
        worker.addEventListener("message", (evt): void => {
            const slug = `worker:${name || this.workerCount.toFixed(0)}:`;
            if (isWorkerLoggerMessageData(evt.data)
                && this.workerFunctions.has(evt.data.method)) {
                this.workerFunctions.get(evt.data.method)(slug, evt);
            }
        });

        ++this.workerCount;
    }

    private workerClear(slug: string) {
        for (const key of this.logs.keys()) {
            if (key.startsWith(slug)) {
                this.delete(key);
            }
        }
    }

    private workerDelete(slug: string, evt: MessageEvent<any>) {
        this.delete(slug + evt.data.id);
    }

    private workerLog(slug: string, evt: MessageEvent<any>) {
        this.log(slug + evt.data.id, ...evt.data.values);
    }
}
