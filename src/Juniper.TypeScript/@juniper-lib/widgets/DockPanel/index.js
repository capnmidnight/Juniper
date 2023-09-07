import { arrayInsertAt, arrayScanReverse } from "@juniper-lib/collections/arrays";
import { ClassList, CustomData, Draggable, HtmlAttr, ID, QueryAll } from "@juniper-lib/dom/attrs";
import { fr, gridColumn, gridRow, gridTemplateColumns, gridTemplateRows } from "@juniper-lib/dom/css";
import { onClick, onDragEnd, onDragOver, onDragStart } from "@juniper-lib/dom/evts";
import { ButtonSmall, Div, H3, HtmlRender, elementGetCustomData, elementInsertBefore, elementIsDisplayed, elementSetText, elementSwap, elementToggleDisplay } from "@juniper-lib/dom/tags";
import { blackMediumDownPointingTriangleCentered as closeIcon, blackMediumRightPointingTriangleCentered as openIcon } from "@juniper-lib/emoji";
import { isBoolean, isDate, isDefined, isNullOrUndefined, isNumber, isString } from "@juniper-lib/tslib/typeChecks";
import { vec2 } from "gl-matrix";
import "./styles.css";
const SIZE_KEY = "proportion";
const INDEX_KEY = "index";
function Dock(type, ...rest) {
    return Div(ClassList("dock", type), ...rest);
}
function isProportion(r) {
    return r instanceof HtmlAttr
        && r.key === SIZE_KEY;
}
class DockPanelAttr {
    constructor(type, value) {
        this.type = type;
        this.value = value;
    }
}
export function resizable(v) { return new DockPanelAttr("resizable", v); }
export function rearrangeable(v) { return new DockPanelAttr("rearrangeable", v); }
function isDockPanelAttr(type, obj) {
    return obj instanceof DockPanelAttr
        && obj.type === type;
}
function isResizableAttr(obj) {
    return isDockPanelAttr("resizable", obj);
}
function isRearrangeableAttr(obj) {
    return isDockPanelAttr("rearrangeable", obj);
}
function isRest(obj) {
    return !(obj instanceof DockPanelAttr);
}
export function DockPanel(name, ...rest) {
    const resizable = arrayScanReverse(rest, isResizableAttr);
    const rearrangeable = arrayScanReverse(rest, isRearrangeableAttr);
    const isResizable = resizable && resizable.value;
    const isRearrangeable = rearrangeable && rearrangeable.value;
    const sub = rest.filter(isRest);
    const classes = [];
    if (isResizable) {
        classes.push("resizable");
    }
    if (isRearrangeable) {
        classes.push("rearrangeable");
    }
    let dragged = null;
    let draggedParent = null;
    let dragType = null;
    let target = null;
    const panel = Dock("panel", ID(name), ClassList(...classes), onDragStart((evt) => {
        const obj = resolveDockObject(evt.target);
        if (isRearrangeable && isCell(obj)
            || isResizable && isSep(obj)) {
            setDraggedObject(obj, evt.clientX, evt.clientY);
        }
    }), onDragOver((evt) => {
        if (isDefined(dragged)) {
            const obj = resolveDockObject(evt.target);
            if (isResizable && dragType === "sep") {
                evt.preventDefault();
                resizeGroup(draggedParent, dragged, evt.clientX, evt.clientY);
            }
            else if (isRearrangeable
                && dragType === "cell"
                && isSep(obj)) {
                evt.preventDefault();
                setDropTarget(obj);
            }
        }
    }), onDragEnd((evt) => {
        const obj = resolveDockObject(evt.target);
        if (obj === dragged
            && isCell(dragged)
            && isSep(target)) {
            moveGroup(draggedParent, target);
        }
        if (dragged) {
            dragged.classList.remove("dragging");
            dragged = null;
            draggedParent = null;
            dragType = null;
        }
        if (target) {
            target.classList.remove("targeting");
            target = null;
        }
    }), ...sub);
    function resolveDockObject(e) {
        let obj = e;
        while (isDefined(obj) && !obj.classList.contains("dock")) {
            obj = obj.parentElement;
        }
        return obj;
    }
    const mouseStart = vec2.create();
    const mouseEnd = vec2.create();
    const mouseMove = vec2.create();
    function setDraggedObject(obj, startMouseX, startMouseY) {
        dragged = obj;
        draggedParent = dragged.parentElement;
        dragType = getDockType(dragged);
        dragged.classList.add("dragging");
        if (isSep(dragged)) {
            vec2.set(mouseStart, startMouseX, startMouseY);
        }
    }
    function setDropTarget(obj) {
        if (target) {
            target.classList.remove("targeting");
            target = null;
        }
        target = obj;
        target.classList.add("targeting");
    }
    function moveGroup(group, sep) {
        let newParent = sep.parentElement;
        const insert = getDirection(newParent) === getDirection(sep);
        const index = getInsertionIndex(sep);
        if (!insert) {
            const dir = getDirectionAlt(newParent);
            newParent = elementSwap(newParent, p => DockGroup(dir, p));
        }
        const next = newParent.children[index];
        if (next !== dragged.nextElementSibling) {
            elementInsertBefore(newParent, group, next);
            regrid(false);
        }
    }
    function resizeGroup(group, sep, mouseX, mouseY) {
        vec2.set(mouseEnd, mouseX, mouseY);
        vec2.sub(mouseMove, mouseEnd, mouseStart);
        const r = isRow(group);
        const dist = mouseMove[r ? 0 : 1];
        if (dist !== 0) {
            vec2.copy(mouseStart, mouseEnd);
            const dim = r ? "clientWidth" : "clientHeight";
            let size = 0;
            let count = 0;
            group.querySelectorAll(":scope > .dock.group")
                .forEach(child => {
                if (!isClosed(child)) {
                    ++count;
                    size += child[dim];
                }
            });
            const avg = size / count;
            const index = getInsertionIndex(sep);
            const left = findOpenGroup(group, index, -1);
            const right = findOpenGroup(group, index, 1);
            if (isDefined(left)
                && isDefined(right)) {
                const leftSize = left[dim] + dist;
                const rightSize = right[dim] - dist;
                const leftProp = leftSize / avg;
                const rightProp = rightSize / avg;
                setProportion(left, leftProp);
                setProportion(right, rightProp);
                regrid(true);
            }
        }
    }
    function findOpenGroup(obj, index, dir) {
        if (dir === -1) {
            --index;
        }
        let child = obj.children[index];
        while (isDefined(child)
            && !isSep(child)
            && isClosed(child)) {
            index += dir;
            child = obj.children[index];
        }
        if (isSep(child)) {
            child = null;
        }
        return child;
    }
    function getProportion(v) {
        if (!isSep(v)) {
            const str = localStorage.getItem(name + "." + v.id + ":" + SIZE_KEY)
                || elementGetCustomData(v, SIZE_KEY);
            return parseFloat(str) || 1;
        }
        return null;
    }
    function setProportion(v, p) {
        if (!isSep(v)) {
            const str = p.toString();
            localStorage.setItem(name + "." + v.id + ":" + SIZE_KEY, str);
            CustomData(SIZE_KEY, str).applyToElement(v);
        }
    }
    function regrid(resize) {
        if (!resize) {
            panel.querySelectorAll(".dock.sep")
                .forEach(sep => sep.remove());
            panel.querySelectorAll(".dock.cell")
                .forEach(cell => {
                if (cell.parentElement.childElementCount > 1) {
                    elementSwap(cell, p => DockGroupRow(p));
                }
            });
        }
        Array.from(panel.querySelectorAll(".dock.group"))
            .reverse()
            .forEach(group => {
            if (group.childElementCount === 0) {
                group.remove();
            }
            else if (group.childElementCount === 1
                && isGroup(group.parentElement)
                && group.parentElement.childElementCount === 1) {
                group.replaceWith(...group.children);
            }
            else {
                regridGroup(group, resize);
            }
        });
    }
    function regridGroup(group, resize) {
        if (group.childElementCount === 1 && isColumn(group)) {
            group.classList.remove("column");
            group.classList.add("row");
        }
        const gParentDir = getDirection(group.parentElement);
        const parentDirection = getDirection(group);
        const parentDirectionAlt = getDirectionAlt(group);
        const r = isRow(group);
        const gridCell = r
            ? gridColumn
            : gridRow;
        const gridCellAlt = r
            ? gridRow
            : gridColumn;
        const gridTemplate = r
            ? gridTemplateColumns
            : gridTemplateRows;
        const gridTemplateAlt = r
            ? gridTemplateRows
            : gridTemplateColumns;
        const offset = isRearrangeable ? 1 : 0;
        const center = gridCell(2, -2);
        const centerAlt = gridCellAlt(2, -2);
        const inAxis = QueryAll(group, ":scope > .dock:not(.sep)")
            .map((e, i) => {
            const child = e;
            const start = 2 * i + offset + 1;
            gridCell(start, start + 1).applyToElement(child);
            centerAlt.applyToElement(child);
            if (isClosed(child)) {
                return "auto";
            }
            else {
                return `${getProportion(child)}fr`;
            }
        });
        for (let i = inAxis.length + offset - 1; i >= 1 - offset; --i) {
            arrayInsertAt(inAxis, "min-content", i);
        }
        const template = gridTemplate(...inAxis);
        const templateAlt = isRearrangeable
            ? gridTemplateAlt("min-content", fr(1), "min-content")
            : gridTemplateAlt("auto");
        HtmlRender(group, template, templateAlt);
        if (!resize) {
            for (let l = group.childElementCount, i = 0; i <= l; ++i) {
                const start = 2 * i + offset;
                const isEdge = i === 0 || i === l;
                if (!isEdge || isRearrangeable && parentDirection !== gParentDir) {
                    HtmlRender(group, DockSep(parentDirection, i, isEdge, gridCell(start, start + 1), centerAlt));
                }
            }
            if (isRearrangeable && parentDirectionAlt !== gParentDir) {
                for (let i = 0; i < 2; ++i) {
                    const start = 2 * i + 1;
                    HtmlRender(group, DockSep(parentDirectionAlt, i, true, gridCellAlt(start, start + 1), center));
                }
            }
        }
    }
    let groupCounter = 0;
    panel.querySelectorAll(".dock.group")
        .forEach(child => {
        child.id = "G" + (++groupCounter);
        setProportion(child, getProportion(child));
    });
    regrid(false);
    panel.querySelectorAll(".dock.cell")
        .forEach(child => {
        child.querySelector(":scope > .header").draggable = isRearrangeable;
        child.addEventListener("regrid", () => regrid(false));
    });
    return panel;
}
function DockGroup(direction, ...rest) {
    return Dock("group", ClassList(direction), ...rest);
}
export function DockGroupColumn(...rest) {
    return DockGroup("column", ...rest);
}
export function DockGroupRow(...rest) {
    return DockGroup("row", ...rest);
}
function getInsertionIndex(v) {
    if (isSep(v)) {
        const str = elementGetCustomData(v, INDEX_KEY);
        if (isNullOrUndefined(str)) {
            return null;
        }
        else {
            return parseFloat(str);
        }
    }
    return null;
}
function setInsertionIndex(v, index) {
    if (isSep(v)) {
        CustomData(INDEX_KEY, index.toFixed(0)).applyToElement(v);
    }
}
function DockSep(type, index, isEdge, ...rest) {
    const classes = [type];
    if (isEdge) {
        classes.push("edge");
    }
    const part = Dock("sep", ClassList(...classes), Draggable(!isEdge), ...rest);
    setInsertionIndex(part, index);
    return part;
}
export function DockCell(header, ...rest) {
    if (isString(header)
        || isDate(header)
        || isNumber(header)
        || isBoolean(header)) {
        header = H3(header);
    }
    const proportion = rest.filter(isProportion);
    rest = rest.filter(e => !isProportion(e));
    const content = Div(ClassList("content"), ...rest);
    const closer = ButtonSmall(ClassList("closer"), closeIcon.emojiStyle, onClick(() => {
        elementToggleDisplay(content, "grid");
        const isOpen = elementIsDisplayed(content);
        elementSetText(closer, isOpen ? closeIcon.emojiStyle : openIcon.emojiStyle);
        cell.classList.toggle("closed", !isOpen);
        cell.dispatchEvent(new Event("regrid"));
    }));
    const cell = Dock("cell", ...proportion, closer, HtmlRender(header, Draggable(true), ClassList("header")), content);
    return DockGroupRow(cell);
}
function isDockType(type, v) {
    return isDefined(v) && v.classList.contains(type);
}
function isPanel(v) {
    return isDockType("panel", v);
}
function isSep(v) {
    return isDockType("sep", v);
}
function isCell(v) {
    return isDockType("cell", v);
}
function isGroup(v) {
    return isDockType("group", v);
}
function getDockType(v) {
    if (isPanel(v)) {
        return "panel";
    }
    else if (isSep(v)) {
        return "sep";
    }
    else if (isCell(v)) {
        return "cell";
    }
    else if (isGroup(v)) {
        return "group";
    }
    return null;
}
function isClosed(v) {
    if (isCell(v)) {
        return v.classList.contains("closed");
    }
    else if (isGroup(v)) {
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
function isDirection(type, v) {
    return isDefined(v) && v.classList.contains(type);
}
function isColumn(v) {
    return isDirection("column", v);
}
function isRow(v) {
    return isDirection("row", v);
}
function getDirection(v) {
    if (isRow(v)) {
        return "row";
    }
    else if (isColumn(v)) {
        return "column";
    }
    else {
        return null;
    }
}
function getDirectionAlt(v) {
    if (isRow(v)) {
        return "column";
    }
    else if (isColumn(v)) {
        return "row";
    }
    else {
        return null;
    }
}
//# sourceMappingURL=index.js.map