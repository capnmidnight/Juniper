import { backgroundColor, color, columnGap, display, Div, elementApply, elementClearChildren, elementToggleDisplay, getMonospaceFamily, gridAutoFlow, gridColumn, height, isModifierless, left, opacity, overflow, overflowY, padding, pointerEvents, position, styles, TextNode, top, width, zIndex } from "juniper-dom";
import { assertNever } from "juniper-tslib";
import { isWorkerLoggerMessageData } from "./models";
function track(a, b) {
    return styles(gridColumn(`${a}/${b}`), getMonospaceFamily());
}
export class WindowLogger {
    logs = new Map();
    rows = new Map();
    element;
    grid;
    workerCount = 0;
    constructor() {
        this.element = Div(styles(position("fixed"), display("none"), top("0"), left("0"), width("100%"), height("100%"), zIndex(9001), padding("1em"), opacity("0.5"), backgroundColor("black"), color("white"), overflow("hidden"), pointerEvents("none")), this.grid = Div(styles(display("grid"), overflowY("auto"), columnGap("0.5em"), gridAutoFlow("row"))));
        elementApply(document.body, this);
        window.addEventListener("keypress", (evt) => {
            if (isModifierless(evt) && evt.key === '`') {
                elementToggleDisplay(this);
            }
        });
    }
    render() {
        const toRemove = new Array();
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
        this.grid.style.gridTemplateColumns = `auto repeat(${maxWidth}, 1fr)`;
        for (const [id, values] of this.logs) {
            let row = this.rows.get(id);
            if (!row) {
                row = [
                    Div(id, track(1, 2)),
                    ...values.map((_, i) => {
                        const isLast = i === values.length - 1;
                        const endTrack = isLast ? -1 : i + 3;
                        const cell = Div(track(i + 2, endTrack));
                        return cell;
                    })
                ];
                this.rows.set(id, row);
                this.grid.append(...row);
            }
            for (let i = 0; i < values.length; ++i) {
                const value = values[i];
                const cell = row[i + 1];
                elementClearChildren(cell);
                cell.append(TextNode(JSON.stringify(value)));
            }
        }
    }
    log(id, ...values) {
        this.logs.set(id, values);
        this.render();
    }
    delete(id) {
        this.logs.delete(id);
        this.render();
    }
    clear() {
        this.logs.clear();
        this.render();
    }
    addWorker(name, worker) {
        worker.addEventListener("message", (evt) => {
            const slug = `worker:${name || this.workerCount.toFixed(0)}:`;
            if (isWorkerLoggerMessageData(evt.data)) {
                switch (evt.data.method) {
                    case "log":
                        this.log(slug + evt.data.id, ...evt.data.values);
                        break;
                    case "delete":
                        this.delete(slug + evt.data.id);
                        break;
                    case "clear":
                        for (const key of this.logs.keys()) {
                            if (key.startsWith(slug)) {
                                this.delete(key);
                            }
                        }
                        break;
                    default:
                        assertNever(evt.data.method);
                }
            }
        });
        ++this.workerCount;
    }
}
