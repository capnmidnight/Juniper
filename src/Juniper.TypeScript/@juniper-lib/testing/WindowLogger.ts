import {
    backgroundColor,
    color,
    columnGap,
    display,
    em,
    getMonospaceFamily,
    gridAutoFlow, gridColumn, gridTemplateColumns, height,
    left,
    opacity,
    overflow,
    overflowY,
    padding,
    perc,
    pointerEvents,
    position,
    top,
    width,
    zIndex
} from "@juniper-lib/dom/css";
import { isModifierless } from "@juniper-lib/dom/evts";
import {
    Div,
    elementApply,
    elementSetDisplay,
    elementToggleDisplay,
    ErsatzElement
} from "@juniper-lib/dom/tags";
import { IDebugLogger, isWorkerLoggerMessageData, MessageType } from "./models";

function track(a: number, b: number) {
    return [
        gridColumn(a, b),
        getMonospaceFamily()
    ];
}

export class WindowLogger implements IDebugLogger, ErsatzElement {
    private readonly workerFunctions = new Map<MessageType, (slug: string, evt: MessageEvent<any>) => void>();
    private readonly logs = new Map<string, Array<any>>();
    private readonly rows = new Map<string, HTMLElement[]>();
    private readonly grid: HTMLElement;

    private workerCount = 0;

    readonly element: HTMLElement;

    constructor() {
        this.workerFunctions.set("log", this.workerLog.bind(this));
        this.workerFunctions.set("delete", this.workerDelete.bind(this));
        this.workerFunctions.set("clear", this.workerClear.bind(this));

        this.element = Div(
            position("fixed"),
            display("none"),
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
            pointerEvents("none"),
            this.grid = Div(
                display("grid"),
                overflowY("auto"),
                columnGap("0.5em"),
                gridAutoFlow("row")
            )
        );

        elementApply(document.body, this);

        window.addEventListener("keypress", (evt) => {
            if (isModifierless(evt) && evt.key === '`') {
                this.toggle();
            }
        });
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
                elementApply(newRow[i], ...track(i + 1, i + 2));
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
