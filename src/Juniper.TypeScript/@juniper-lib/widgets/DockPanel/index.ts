import { classList, customData, draggable } from "@juniper-lib/dom/attrs";
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

function Dock(type: DockType, ...rest: ElementChild[]) {
    return Div(
        classList("dock", type),
        ...rest
    )
}

export function DockPanel(...rest: ElementChild[]) {
    let cell: HTMLElement = null;
    let target: HTMLElement = null;
    let sep: HTMLElement = null;
    let sepParent: HTMLElement = null;
    const start = vec2.create();
    const end = vec2.create();
    const delta = vec2.create();

    const panel = Dock(
        "panel",

        onDragStart((evt) => {
            const e = evt.target as HTMLElement;
            target = null;
            if (isCell(e)) {
                cell = e;
                cell.classList.toggle("dragging", true);
            }
            else if (isSep(e)) {
                sep = e;
                sepParent = sep.parentElement;
                sep.classList.toggle("targeting", true);
                vec2.set(start, evt.clientX, evt.clientY);
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
            else {
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
            if (evt.target === cell && isSep(target)) {
                let newParent = target.parentElement;
                const insert = isRow(newParent) && isRowSep(target)
                    || isColumn(newParent) && isColumnSep(target);
                const index = getInsertionIndex(target);
                if (!insert) {
                    const grandParent = isRow(newParent)
                        ? DockCol()
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
        panel.querySelectorAll(".sep")
            .forEach(sep => sep.remove());

        const groups = Array.from(panel.querySelectorAll(".row, .column"));
        groups.reverse();
        groups.forEach(parent => {
            if (parent.childElementCount === 1) {
                parent.replaceWith(parent.children[0]);
            }
            else {
                const inAxis: CSSGridTemplateTrackSizes[] = ["auto"];

                const gridCell = isRow(parent)
                    ? gridColumn
                    : gridRow;
                const gridCellAlt = isRow(parent)
                    ? gridRow
                    : gridColumn;

                parent.querySelectorAll(":scope > .dock")
                    .forEach((e, i) => {
                        const child = e as HTMLElement;
                        const start = 2 * i + 2;
                        gridCell(start, start + 1).applyToElement(child);
                        gridCellAlt(2, 3).applyToElement(child);

                        if (isClosed(child)) {
                            inAxis.push("auto");
                        }
                        else {
                            const size = getProportion(child);
                            inAxis.push(`${size}fr`);
                        }
                        inAxis.push("auto");
                    });

                const sepType = isRow(parent) ? "r" : "c";
                const sepTypeAlt = isRow(parent) ? "c" : "r";

                for (let l = parent.childElementCount, i = 0; i <= l; ++i) {
                    const start = 2 * i + 1;
                    elementApply(parent,
                        DockSep(
                            sepType,
                            i,
                            i === 0 || i === l,
                            gridCell(start, start + 1),
                            gridCellAlt(2, 3)
                        )
                    );
                }

                if (isPanel(parent.parentElement)) {
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
                const templateAlt = gridTemplateAlt("auto", "1fr", "auto");

                elementApply(parent,
                    template,
                    templateAlt);
            }
        });
    }

    panel.querySelectorAll(".dock")
        .forEach(child =>
            setProportion(child, 1));

    regrid();

    panel.querySelectorAll(".dock.cell")
        .forEach(child =>
            child.addEventListener("regrid", regrid));

    return panel;
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

function DockGroup(direction: "column" | "row", ...rest: ElementChild[]) {
    return Dock(direction, ...rest);
}

export function DockCol(...rest: ElementChild[]) {
    return DockGroup("column", ...rest);
}

export function DockRow(...rest: ElementChild[]) {
    return DockGroup("row", ...rest);
}

export function DockCell(header: Exclude<ElementChild, IElementAppliable>, ...rest: ElementChild[]) {
    if (isString(header)
        || isDate(header)
        || isNumber(header)
        || isBoolean(header)) {
        header = H3(header);
    }

    const content = Div(
        classList("content"),
        ...rest
    );

    const closer = ButtonSmall(
        classList("closer"),
        closeIcon.emojiStyle,
        onClick(() => {
            elementToggleDisplay(content);
            const isOpen = elementIsDisplayed(content);
            elementSetText(closer, isOpen ? closeIcon.emojiStyle : openIcon.emojiStyle);
            cell.classList.toggle("closed", !isOpen);
            cell.dispatchEvent(new TypedEvent("regrid"));
        })
    );

    const cell = Dock("cell",
        draggable(true),
        closer,
        elementApply(
            header,
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

const INDEX_KEY = "index";
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

const SIZE_KEY = "proportion";
function getProportion(v: Element): number {
    if (!isSep(v)) {
        return parseFloat(elementGetCustomData(v as HTMLElement, SIZE_KEY)) || 1;
    }

    return null;
}

function setProportion(v: Element, p: number) {
    if (!isSep(v)) {
        customData(SIZE_KEY, p.toString()).applyToElement(v as HTMLElement);
    }
}