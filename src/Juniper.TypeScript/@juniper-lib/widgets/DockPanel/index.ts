import { Attr, classList, customData, draggable, id } from "@juniper-lib/dom/attrs";
import { gridColumn, gridRow, gridTemplateColumns, gridTemplateRows } from "@juniper-lib/dom/css";
import { onClick, onDragEnd, onDragOver, onDragStart } from "@juniper-lib/dom/evts";
import { ButtonSmall, Div, elementApply, ElementChild, elementGetCustomData, elementInsertBefore, elementIsDisplayed, elementSetText, elementToggleDisplay, H3, IElementAppliable } from "@juniper-lib/dom/tags";
import { blackMediumDownPointingTriangleCentered as closeIcon, blackMediumRightPointingTriangleCentered as openIcon } from "@juniper-lib/emoji";
import { TypedEvent } from "@juniper-lib/tslib/events/EventBase";
import { isBoolean, isDate, isDefined, isNumber, isString } from "@juniper-lib/tslib/typeChecks";
import { vec2 } from "gl-matrix";
import "./style";

type DockType = "cell" | "row" | "column" | "sep" | "panel";
type SepType = "r" | "c";

const SIZE_KEY = "proportion";
const INDEX_KEY = "index";

function Dock(type: DockType, ...rest: ElementChild[]) {
    return Div(
        classList("dock", type),
        ...rest
    )
}

function isProportion(r: ElementChild): boolean {
    return r instanceof Attr
        && r.key === SIZE_KEY;
}

export function DockPanel(name: string, ...rest: ("resizable" | "rearrangeable" | ElementChild)[]) {
    let cell: HTMLElement = null;
    let target: HTMLElement = null;
    let sep: HTMLElement = null;
    let sepParent: HTMLElement = null;
    let groupCounter = 0;
    const start = vec2.create();
    const end = vec2.create();
    const delta = vec2.create();
    const resizable = rest.filter(v => v === "resizable") as string[];
    const rearrangeable = rest.filter(v => v === "rearrangeable") as string[];
    const isResizable = resizable.length > 0;
    const isRearrangeable = rearrangeable.length > 0;

    rest = rest.filter(v => v !== "resizable" && v !== "rearrangeable");

    function getProportion(v: Element): number {
        if (!isSep(v)) {
            const str = localStorage.getItem(name + "." + v.id + ":" + SIZE_KEY)
                || elementGetCustomData(v as HTMLElement, SIZE_KEY);
            return parseFloat(str) || 1;
        }

        return null;
    }

    function setProportion(v: Element, p: number) {
        if (!isSep(v)) {
            const str = p.toString();
            localStorage.setItem(name + "." + v.id + ":" + SIZE_KEY, str);
            customData(SIZE_KEY, str).applyToElement(v as HTMLElement);
        }
    }

    const panel = Dock(
        "panel",
        id(name),
        classList(...resizable, ...rearrangeable),
        onDragStart((evt) => {
            let e = evt.target as HTMLElement;
            while (isDefined(e) && !e.classList.contains("dock")) {
                e = e.parentElement;
            }

            target = null;
            if (isCell(e) && isRearrangeable) {
                cell = e;
                cell.classList.toggle("dragging", true);
            }
            else if (isSep(e) && isResizable) {
                sep = e;
                sepParent = sep.parentElement;
                sep.classList.toggle("targeting", true);
                vec2.set(start, evt.clientX, evt.clientY);
            }
            else {
                evt.preventDefault();
            }
        }),

        onDragOver((evt) => {
            const e = evt.target as HTMLElement;

            if (isDefined(sep)) {
                evt.preventDefault();
                vec2.set(end, evt.clientX, evt.clientY);
                vec2.sub(delta, end, start);
                const r = isRow(sepParent);
                const dist = delta[r ? 0 : 1];
                if (dist !== 0) {
                    vec2.copy(start, end);
                    const dim = r ? "clientWidth" : "clientHeight";
                    let size = 0;
                    let count = 0;
                    for (let i = 0; i < sepParent.childElementCount; ++i) {
                        const child = sepParent.children[i];
                        if (!isSep(child)) {
                            ++count;
                            size += child[dim];
                        }
                    }

                    const avg = size / count;
                    const index = getInsertionIndex(sep);
                    const left = sepParent.children[index - 1];
                    const right = sepParent.children[index];
                    const leftSize = left[dim] + dist;
                    const rightSize = right[dim] - dist;
                    const leftProp = leftSize / avg;
                    const rightProp = rightSize / avg;
                    setProportion(left, leftProp);
                    setProportion(right, rightProp);
                    regrid();
                }
            }
            else if (isRearrangeable) {
                if (target) {
                    target.classList.toggle("targeting", false);
                    target = null;
                }

                if (isSep(e)) {
                    target = e;
                    target.classList.toggle("targeting", true);
                    evt.preventDefault();
                }
            }
        }),

        onDragEnd((evt) => {
            let e = evt.target as HTMLElement;
            while (isDefined(e) && !e.classList.contains("dock")) {
                e = e.parentElement;
            }

            if (e === cell && isSep(target)) {
                let newParent = target.parentElement;
                const insert = isRow(newParent) && isRowSep(target)
                    || isColumn(newParent) && isColumnSep(target);
                const index = getInsertionIndex(target);
                if (!insert) {
                    const grandParent = isRow(newParent)
                        ? DockColumn()
                        : DockRow();
                    newParent.replaceWith(grandParent);
                    grandParent.append(newParent);
                    newParent = grandParent;
                }
                const next = newParent.children[index];
                if (next !== cell.nextElementSibling) {
                    elementInsertBefore(newParent, cell, next);
                    regrid();
                }
            }

            if (cell) {
                cell.classList.toggle("dragging", false);
                cell = null;
            }

            if (target) {
                target.classList.toggle("targeting", false);
                target = null;
            }

            if (sep) {
                sep.classList.toggle("targeting", false);
                sep = null;
            }
        }),

        ...rest);

    function regrid() {
        panel.querySelectorAll(".dock.sep")
            .forEach(sep => sep.remove());

        const groups = Array.from(panel.querySelectorAll(".dock.row, .dock.column"));
        groups.reverse();
        groups.forEach(parent => {
            if (parent.childElementCount === 1) {
                parent.replaceWith(parent.children[0]);
            }
            else {
                const inAxis: CSSGridTemplateTrackSizes[] = [];

                if (isRearrangeable) {
                    inAxis.push("min-content");
                }

                const gridCell = isRow(parent)
                    ? gridColumn
                    : gridRow;
                const gridCellAlt = isRow(parent)
                    ? gridRow
                    : gridColumn;

                const offset = isRearrangeable ? 1 : 0;

                const altPosition = isRearrangeable
                    ? gridCellAlt(2, 3)
                    : gridCellAlt(1, -1);

                parent.querySelectorAll(":scope > .dock")
                    .forEach((e, i) => {
                        const child = e as HTMLElement;
                        const start = 2 * i + offset + 1;
                        gridCell(start, start + 1).applyToElement(child);
                        altPosition.applyToElement(child);

                        if (isClosed(child)) {
                            inAxis.push("min-content");
                        }
                        else {
                            const size = getProportion(child);
                            inAxis.push(`${size}fr`);
                        }
                        inAxis.push("min-content");
                    });

                if (!isRearrangeable) {
                    inAxis.pop();
                }

                const sepType = isRow(parent) ? "r" : "c";
                const sepTypeAlt = isRow(parent) ? "c" : "r";

                for (let l = parent.childElementCount, i = 0; i <= l; ++i) {
                    const start = 2 * i + offset;
                    const isEdge = i === 0 || i === l;
                    if (!isEdge || isRearrangeable) {
                        elementApply(parent,
                            DockSep(
                                sepType,
                                i,
                                isEdge,
                                gridCell(start, start + 1),
                                altPosition
                            )
                        );
                    }
                }

                if (isRearrangeable && isPanel(parent.parentElement)) {
                    for (let i = 0; i < 2; ++i) {
                        const start = 2 * i + 1
                        elementApply(parent,
                            DockSep(
                                sepTypeAlt,
                                i,
                                true,
                                gridCellAlt(start, start + 1),
                                gridCell(1, -1)
                            )
                        );
                    }
                }

                const gridTemplate = isRow(parent)
                    ? gridTemplateColumns
                    : gridTemplateRows;
                const gridTemplateAlt = isRow(parent)
                    ? gridTemplateRows
                    : gridTemplateColumns;

                const template = gridTemplate(...inAxis);
                const templateAlt = isRearrangeable
                    ? gridTemplateAlt("min-content", "1fr", "min-content")
                    : gridTemplateAlt("min-content");

                elementApply(parent,
                    template,
                    templateAlt);
            }
        });
    }

    panel.querySelectorAll(".dock")
        .forEach(child => {
            child.id = "G" + (++groupCounter);
            setProportion(child, getProportion(child));
        });

    regrid();

    panel.querySelectorAll(".dock.cell")
        .forEach(child => {
            (child.querySelector(":scope > .header") as HTMLElement).draggable = isRearrangeable;
            child.addEventListener("regrid", regrid);
        });

    return panel;
}

function DockGroup(direction: "column" | "row", ...rest: ElementChild[]) {
    return Dock(direction, ...rest);
}

export function DockColumn(...rest: ElementChild[]) {
    return DockGroup("column", ...rest);
}

export function DockRow(...rest: ElementChild[]) {
    return DockGroup("row", ...rest);
}

function getInsertionIndex(v: Element) {
    if (isSep(v)) {
        return parseFloat(elementGetCustomData(v as HTMLElement, INDEX_KEY)) || null;
    }

    return null;
}

function setInsertionIndex(v: Element, index: number) {
    if (isSep(v)) {
        customData(INDEX_KEY, index.toFixed(0)).applyToElement(v as HTMLElement);
    }
}

function DockSep(type: SepType, index: number, isEdge: boolean, ...rest: ElementChild[]) {
    const classes = [type as string];
    if (isEdge) {
        classes.push("edge");
    }

    const part = Dock("sep",
        classList(...classes),
        draggable(!isEdge),
        ...rest);

    setInsertionIndex(part, index);

    return part;
}

export function DockCell(header: Exclude<ElementChild, IElementAppliable>, ...rest: ElementChild[]) {
    if (isString(header)
        || isDate(header)
        || isNumber(header)
        || isBoolean(header)) {
        header = H3(header);
    }

    const proportion = rest.filter(isProportion);
    rest = rest.filter(e => !isProportion(e));

    const content = Div(
        classList("content"),
        ...rest
    );

    const closer = ButtonSmall(
        classList("closer"),
        closeIcon.emojiStyle,
        onClick(() => {
            elementToggleDisplay(content, "grid");
            const isOpen = elementIsDisplayed(content);
            elementSetText(closer, isOpen ? closeIcon.emojiStyle : openIcon.emojiStyle);
            cell.classList.toggle("closed", !isOpen);
            cell.dispatchEvent(new TypedEvent("regrid"));
        })
    );

    const cell = Dock("cell",
        ...proportion,
        closer,
        elementApply(
            header,
            draggable(true),
            classList("header")
        ),
        content);

    return cell;
}

function isDockType(type: DockType, v: Element) {
    return isDefined(v) && v.classList.contains(type);
}

function isPanel(v: Element): boolean {
    return isDockType("panel", v);
}

function isSep(v: Element): boolean {
    return isDockType("sep", v);
}

function isCell(v: Element): boolean {
    return isDockType("cell", v);
}

function isRow(v: Element): boolean {
    return isDockType("row", v);
}

function isColumn(v: Element): boolean {
    return isDockType("column", v);
}

function isClosed(v: Element): boolean {
    if (isCell(v)) {
        return v.classList.contains("closed");
    }
    else if (isRow(v) || isColumn(v)) {
        for (let i = 0; i < v.childElementCount; ++i) {
            if (!isClosed(v.children[i])) {
                return false;
            }
        }

        return true;
    }
    else if (isSep(v)) {
        return true;
    }
    else {
        return false;
    }
}

function isSepType(type: SepType, v: Element): boolean {
    return isDefined(v) && v.classList.contains(type);
}

function isRowSep(v: Element) {
    return isSepType("r", v);
}

function isColumnSep(v: Element) {
    return isSepType("c", v);
}