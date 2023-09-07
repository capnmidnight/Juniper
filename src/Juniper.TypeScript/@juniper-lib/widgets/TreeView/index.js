import { buildTree } from "@juniper-lib/collections/TreeNode";
import { arrayClear, arrayRemove } from "@juniper-lib/collections/arrays";
import { AutoComplete, ClassList, PlaceHolder, TabIndex } from "@juniper-lib/dom/attrs";
import { isModifierless, onClick, onContextMenu, onDragEnd, onDragOver, onDragStart, onDrop, onInput, onKeyDown } from "@juniper-lib/dom/evts";
import { ButtonPrimarySmall, ButtonSecondarySmall, Div, HtmlRender, InputText, elementClearChildren, elementGetIndexInParent, elementInsertBefore, elementReplace, elementSetDisplay } from "@juniper-lib/dom/tags";
import { TypedEvent, TypedEventTarget } from "@juniper-lib/events/TypedEventTarget";
import { alwaysFalse } from "@juniper-lib/tslib/identity";
import { isDefined, isFunction, isNullOrUndefined } from "@juniper-lib/tslib/typeChecks";
import { TreeViewNodeContextMenuEvent, TreeViewNodeElement, TreeViewNodeSelectedEvent } from "./TreeViewNodeElement";
import { PropertyList } from "../PropertyList";
import { LabelField, SelectList, SortKeyField, ValueField } from "../SelectList";
import "./styles.css";
class TreeViewNodeEvent extends TypedEvent {
    constructor(type, node) {
        super(type);
        this.node = node;
    }
}
export class TreeViewNodeDeleteEvent extends TreeViewNodeEvent {
    constructor(node) {
        super("delete", node);
    }
}
export class TreeViewNodeMovedEvent extends TreeViewNodeEvent {
    constructor(node, newIndex) {
        super("moved", node);
        this.newIndex = newIndex;
    }
}
export class TreeViewNodeReparentedEvent extends TreeViewNodeEvent {
    constructor(node, newParent) {
        super("reparented", node);
        this.newParent = newParent;
    }
}
export class TreeView extends TypedEventTarget {
    constructor(options, ...styleProps) {
        super();
        this.filterTypeInput = null;
        this.elements = new Array();
        this.nodes2Elements = new Map();
        this.htmlElements2Nodes = new Map();
        this.htmlElements2Elements = new Map();
        this._rootNode = null;
        this.locked = false;
        this._disabled = false;
        this.typeFilter = null;
        this.nameFilter = null;
        this.readonly = false;
        this.createElement = this.createElement.bind(this);
        this.options = Object.assign({
            defaultLabel: null,
            getOrder: null,
            replaceElement: null,
            canReorder: alwaysFalse,
            additionalProperties: []
        }, options);
        const canReorder = this.options.canReorder;
        if (isDefined(canReorder)) {
            this.options.canReorder = (value) => !this.readonly && canReorder(value);
        }
        if (isNullOrUndefined(this.options.canParent)) {
            this.options.canParent = this.options.canHaveChildren;
        }
        this._canChangeOrder = isFunction(this.options.getOrder);
        this.element = Div(ClassList("tree-view"), ...styleProps, this.filters = PropertyList.create(ClassList("tree-view-controls"), ...this.options.additionalProperties), Div(ClassList("tree-view-controls"), this.collapseButton = ButtonPrimarySmall(onClick(() => this.collapseAll()), "Collapse all"), this.expandButton = ButtonPrimarySmall(onClick(() => this.expandAll()), "Expand all")), Div(ClassList("tree-view-inner"), TabIndex(0), onContextMenu(async (evt) => {
            if (!this.disabled) {
                const rootElement = this.nodes2Elements.get(this.rootNode);
                await rootElement._launchMenu(evt, new TreeViewNodeContextMenuEvent(rootElement));
            }
        }), onClick((evt) => {
            if (!this.disabled && !evt.defaultPrevented) {
                for (const element of this.elements) {
                    if (element.selected) {
                        this.selectedNode = null;
                        this.dispatchEvent(new TreeViewNodeSelectedEvent(null));
                        return;
                    }
                }
            }
        }), onKeyDown((evt) => {
            if (isModifierless(evt)) {
                const sel = this.selectedElement;
                if (sel) {
                    if (evt.key === "Delete") {
                        this.dispatchEvent(new TreeViewNodeDeleteEvent(sel.node));
                    }
                    else if (evt.key === "ArrowUp") {
                        const index = elementGetIndexInParent(sel);
                        if (index > 0) {
                            const nextHTMLElement = sel.parentElement.children[index - 1];
                            const nextElement = this.htmlElements2Elements.get(nextHTMLElement);
                            nextElement._select(true);
                        }
                        else if (sel.node.isChild && sel.node.parent.isChild) {
                            const nextNode = sel.node.parent;
                            const nextElement = this.nodes2Elements.get(nextNode);
                            nextElement._select(true);
                        }
                    }
                    else if (evt.key === "ArrowDown") {
                        const index = elementGetIndexInParent(sel);
                        if (index < sel.parentElement.childElementCount - 1) {
                            const nextHTMLElement = sel.parentElement.children[index + 1];
                            const nextElement = this.htmlElements2Elements.get(nextHTMLElement);
                            nextElement._select(true);
                        }
                        else if (sel.node.isChild) {
                            const parentNode = sel.node.parent;
                            const parentElement = this.nodes2Elements.get(parentNode);
                            const parentIndex = elementGetIndexInParent(parentElement);
                            const nextHTMLElement = parentElement.parentElement.children[parentIndex + 1];
                            if (nextHTMLElement) {
                                const nextElement = this.htmlElements2Elements.get(nextHTMLElement);
                                nextElement._select(true);
                            }
                        }
                    }
                    else if (evt.key === "ArrowRight") {
                        if (sel.node.hasChildren) {
                            if (sel.open) {
                                const nextHTMLElem = sel.childTreeNodes.children[0];
                                if (nextHTMLElem) {
                                    const elem = this.htmlElements2Elements.get(nextHTMLElem);
                                    elem._select(true);
                                }
                            }
                            else {
                                sel.open = true;
                            }
                        }
                    }
                    else if (evt.key === "ArrowLeft") {
                        const sel = this.selectedElement;
                        if (sel.open) {
                            sel.open = false;
                        }
                        else if (sel.node.isChild && sel.node.parent.isChild) {
                            const parentElem = this.nodes2Elements.get(sel.node.parent);
                            parentElem._select(true);
                        }
                    }
                }
                else if (this.children.children.length > 0) {
                    const rootElem = this.nodes2Elements.get(this.rootNode);
                    let htmlElem = null;
                    if (evt.key === "ArrowUp") {
                        htmlElem = rootElem.childTreeNodes.children[rootElem.childTreeNodes.children.length - 1];
                    }
                    else if (evt.key === "ArrowDown") {
                        htmlElem = rootElem.childTreeNodes.children[0];
                    }
                    if (htmlElem) {
                        const elem = this.htmlElements2Elements.get(htmlElem);
                        elem._select(true);
                    }
                }
            }
        }), this.children = Div(ClassList("tree-view-children"))));
        if (this.options.showNameFilter || isDefined(this.options.typeFilters)) {
            if (this.options.showNameFilter) {
                this.filters.append(["Name", this.filterNameInput = InputText(ClassList("form-control"), PlaceHolder("Filter by name"), AutoComplete("off"), onInput(() => {
                        this.nameFilter = this.filterNameInput.value.toLocaleLowerCase();
                        if (this.nameFilter.length === 0) {
                            this.nameFilter = null;
                        }
                        this.refreshFilter();
                    }))]);
            }
            if (isDefined(this.options.typeFilters)) {
                this.filters.append(["Type", this.filterTypeInput = SelectList(ValueField(v => v), LabelField(this.options.typeFilters.getTypeLabel), SortKeyField(this.options.typeFilters.getTypeLabel), PlaceHolder("Filter by type"))]);
                this.filterTypeInput.data = this.options.typeFilters.getTypes();
                this.filterTypeInput.addEventListener("itemselected", (evt) => {
                    this.typeFilter = evt.item;
                    this.refreshFilter();
                });
            }
            this.filters.append(ButtonSecondarySmall("Clear filter", onClick(() => this.clearFilter())));
        }
        if (this.canChangeOrder) {
            let draggedElement = null;
            let dropElement = null;
            let hoverTimer = null;
            let delta = 0;
            let lastTarget = null;
            const clearTarget = () => {
                if (hoverTimer) {
                    clearTimeout(hoverTimer);
                }
                lastTarget = null;
                if (dropElement) {
                    dropElement.highlighted = null;
                    dropElement = null;
                }
                delta = 0;
            };
            HtmlRender(this.children, onDragStart((evt) => {
                clearTarget();
                draggedElement = this.findElement(evt.target);
                draggedElement.style.opacity = "0.5";
            }), onDragOver((evt) => {
                const target = evt.target;
                const targetChanged = target !== lastTarget;
                const elem = this.findElement(target);
                if (targetChanged) {
                    clearTarget();
                    lastTarget = target;
                }
                if (elem && this.canReparent(elem, draggedElement, target)) {
                    dropElement = elem;
                    if (target === elem.upper) {
                        delta = -1;
                    }
                    else if (target === elem.lower) {
                        delta = 1;
                    }
                    else {
                        delta = 0;
                    }
                    dropElement.highlighted = delta;
                    if (targetChanged && delta === 0 && !elem.open) {
                        hoverTimer = setTimeout(() => elem.open = true, 1000);
                    }
                    evt.preventDefault();
                }
            }), onDrop((evt) => {
                if (this.canReparent(dropElement, draggedElement, lastTarget)) {
                    evt.preventDefault();
                    this.reparentNode(draggedElement.node, dropElement.node, delta);
                }
                clearTarget();
            }), onDragEnd(() => {
                if (draggedElement) {
                    draggedElement.style.opacity = "1";
                }
                clearTarget();
            }));
        }
        if (this.options.replaceElement) {
            elementReplace(this.options.replaceElement, this);
        }
    }
    clearFilter() {
        if (isDefined(this.filterNameInput)) {
            this.filterNameInput.value = "";
            this.nameFilter = null;
        }
        if (isDefined(this.filterTypeInput)) {
            this.filterTypeInput.selectedItem = null;
            this.typeFilter = null;
        }
        this.refreshFilter();
    }
    refreshFilter() {
        const included = new Set();
        for (const node of this.rootNode.depthFirst()) {
            const nameMatch = isNullOrUndefined(this.nameFilter)
                || isDefined(node.value) && this.options.getLabel(node.value).toLocaleLowerCase().indexOf(this.nameFilter) >= 0;
            const typeMatch = isNullOrUndefined(this.typeFilter)
                || isDefined(node.value) && this.options.typeFilters.getTypeFor(node.value) === this.typeFilter;
            const isMatch = nameMatch && typeMatch;
            const show = isMatch || included.has(node);
            const elem = this.nodes2Elements.get(node);
            elementSetDisplay(elem, show, "block");
            elem._filtered = included.has(node) && !isMatch;
            elem.open = true;
            if (show && isDefined(node.parent)) {
                included.add(node.parent);
            }
        }
        if (isDefined(this.selectedElement)) {
            this.selectedElement.scrollIntoView();
        }
    }
    canReparent(parent, child, target) {
        return !this.readonly
            && isDefined(parent)
            && isDefined(child)
            && (parent.canHaveChildren
                && this.options.canParent(parent.node, child.node)
                || target === parent.upper
                || target === parent.lower)
            && !child.node.contains(parent.node)
            && child.node.parent !== parent.node;
    }
    get canChangeOrder() {
        return !this.readonly
            && this._canChangeOrder;
    }
    get enabled() {
        return !this.disabled;
    }
    set enabled(v) {
        this.disabled = !v;
    }
    get disabled() {
        return this._disabled;
    }
    set disabled(v) {
        this.expandButton.disabled
            = this.collapseButton.disabled
                = this.filters.disabled
                    = this._disabled
                        = v;
        for (const element of this.elements) {
            element.disabled = v;
        }
    }
    async withLock(action) {
        const isEnabled = this.enabled;
        if (this.enabled) {
            try {
                await action();
            }
            finally {
                this.enabled = isEnabled;
            }
        }
    }
    findAll(predicate) {
        return Array.from(this.rootNode.searchAll((node) => predicate(node.value)));
    }
    enableOnlyValues(values) {
        for (const element of this.elements) {
            element.disabled = values.indexOf(element.node.value) === -1;
            element.specialSelectMode = true;
        }
    }
    enableAllElements() {
        for (const element of this.elements) {
            element.disabled = false;
            element.specialSelectMode = false;
        }
    }
    get values() {
        return this.rootNode.children
            .flatMap(f => Array.from(f.breadthFirst()))
            .map(n => n.value);
    }
    set values(arr) {
        this.rootNode = buildTree(arr, this.options.getParent, this.options.getOrder);
    }
    get rootNode() {
        return this._rootNode;
    }
    set rootNode(v) {
        if (v !== this.rootNode) {
            this.locked = true;
            for (const element of this.elements) {
                element.removeScope(this);
                element.removeBubbler(this);
            }
            elementClearChildren(this.children);
            arrayClear(this.elements);
            this.nodes2Elements.clear();
            this.htmlElements2Nodes.clear();
            this.htmlElements2Elements.clear();
            this._rootNode = v;
            if (this.rootNode) {
                for (const node of this.rootNode.breadthFirst()) {
                    this.createElement(node);
                }
                const rootElement = this.nodes2Elements.get(this.rootNode);
                HtmlRender(this.children, rootElement);
            }
            this.locked = false;
            for (const elem of this.elements) {
                if (elem.canHaveChildren) {
                    this.reorderChildren(elem);
                }
            }
            if (this.rootNode) {
                this.nodes2Elements.get(this.rootNode).refresh();
            }
        }
    }
    clear() {
        this.rootNode = null;
    }
    get selectedValue() {
        const node = this.selectedNode;
        if (node) {
            return node.value;
        }
        return null;
    }
    findNode(data) {
        return this.rootNode.find(data);
    }
    set selectedValue(v) {
        if (v !== this.selectedValue) {
            this.selectedNode = this.rootNode.find(v);
        }
    }
    get selectedNode() {
        const elem = this.selectedElement;
        if (elem) {
            return elem.node;
        }
        return null;
    }
    set selectedNode(v) {
        if (v !== this.selectedNode) {
            this.selectedElement = this.nodes2Elements.get(v);
        }
    }
    get selectedElement() {
        for (const elem of this.elements) {
            if (elem.selected) {
                return elem;
            }
        }
        return null;
    }
    set selectedElement(e) {
        if (isDefined(e)) {
            e._select(false);
            let here = e;
            while (isDefined(here)) {
                here.open = true;
                here = this.nodes2Elements.get(here.node.parent);
            }
            e.scrollIntoView();
        }
        else {
            e = this.selectedElement;
            if (isDefined(e)) {
                e._selected = false;
            }
            this.dispatchEvent(new TreeViewNodeSelectedEvent(null));
        }
    }
    findElement(target) {
        let here = target;
        while (here !== null) {
            const node = this.htmlElements2Elements.get(here);
            if (isDefined(node)) {
                return node;
            }
            here = here.parentElement;
            if (here === this.children) {
                return null;
            }
        }
        return null;
    }
    reorderChildren(parentElement) {
        if (!this.locked && this.canChangeOrder) {
            const numChildren = parentElement.childTreeNodes.children.length;
            for (let i = 0; i < numChildren; ++i) {
                const htmlElem = parentElement.childTreeNodes.children[i];
                const node = this.htmlElements2Nodes.get(htmlElem);
                const elem = this.nodes2Elements.get(node);
                const index = this.options.getOrder(node.value);
                if (index !== i) {
                    this.dispatchEvent(new TreeViewNodeMovedEvent(node, i));
                }
                elem.refresh();
            }
            parentElement.refresh();
        }
    }
    createElement(node) {
        const element = TreeViewNodeElement.create(node, this.options.defaultLabel, this.options.getLabel, this.options.getDescription, this.options.canReorder, this.options.getChildDescription, this.options.canHaveChildren, this.createElement);
        element.addScopedEventListener(this, "select", (evt) => {
            for (const elem of this.elements) {
                elem._selected = elem.node === evt.node;
            }
        });
        element.addBubbler(this);
        this.elements.push(element);
        this.nodes2Elements.set(node, element);
        this.htmlElements2Nodes.set(element, node);
        this.htmlElements2Elements.set(element, element);
        const parentNode = node.parent;
        if (parentNode) {
            const parentElement = this.nodes2Elements.get(parentNode);
            if (parentElement) {
                if (!this.canChangeOrder) {
                    HtmlRender(parentElement.childTreeNodes, element);
                }
                else {
                    const index = this.options.getOrder(node.value);
                    let nextNodeIndex = Number.MAX_SAFE_INTEGER;
                    let nextNode = null;
                    for (const sibling of parentNode.children) {
                        if (sibling !== node) {
                            const sibIndex = this.options.getOrder(sibling.value);
                            if (sibIndex > index && sibIndex < nextNodeIndex) {
                                nextNode = sibling;
                                nextNodeIndex = sibIndex;
                            }
                        }
                    }
                    const nextElement = this.nodes2Elements.has(nextNode)
                        && this.nodes2Elements.get(nextNode)
                        || null;
                    elementInsertBefore(parentElement.childTreeNodes, element, nextElement);
                    this.reorderChildren(parentElement);
                }
            }
        }
        return element;
    }
    addValue(value) {
        const parent = this.options.getParent(value);
        const parentNode = isNullOrUndefined(parent)
            ? this.rootNode
            : this.rootNode.find(parent);
        const parentElement = this.nodes2Elements.get(parentNode);
        parentElement.add(value);
    }
    updateNode(node) {
        const element = this.nodes2Elements.get(node);
        if (element) {
            element.refresh();
        }
    }
    reparentNode(node, newParentNode, delta) {
        const curParent = node.parent;
        const curElement = this.nodes2Elements.get(node);
        const curParentElement = this.nodes2Elements.get(curParent);
        let nextParentElement = this.nodes2Elements.get(newParentNode);
        let nextSiblingElement = null;
        if (delta !== 0) {
            nextSiblingElement = nextParentElement;
            newParentNode = newParentNode.parent;
            nextParentElement = this.nodes2Elements.get(newParentNode);
            if (delta === 1) {
                const index = elementGetIndexInParent(nextSiblingElement) + 1;
                const nextSiblingHTMLElement = nextParentElement.childTreeNodes.children[index];
                nextSiblingElement = this.htmlElements2Elements.get(nextSiblingHTMLElement);
            }
        }
        else if (this.canChangeOrder) {
            const curOrder = this.options.getOrder(node.value);
            for (let i = 0; i < newParentNode.children.length && nextSiblingElement === null; ++i) {
                const nextSiblingNode = newParentNode.children[i];
                const nextOrder = this.options.getOrder(nextSiblingNode.value);
                if (nextOrder > curOrder) {
                    nextSiblingElement = this.nodes2Elements.get(nextSiblingNode);
                }
            }
        }
        elementInsertBefore(nextParentElement.childTreeNodes, curElement, nextSiblingElement);
        if (nextParentElement !== curParentElement) {
            newParentNode.connectTo(node);
            this.reorderChildren(curParentElement);
            this.reorderChildren(nextParentElement);
            this.dispatchEvent(new TreeViewNodeReparentedEvent(node, newParentNode));
        }
        else {
            this.reorderChildren(curParentElement);
        }
        nextParentElement.open = true;
    }
    removeValue(value) {
        this.removeNode(this.rootNode.find(value));
    }
    removeNode(node) {
        const element = this.nodes2Elements.get(node);
        if (element.selected) {
            this.dispatchEvent(new TreeViewNodeSelectedEvent(null));
        }
        const parentElement = this.nodes2Elements.get(node.parent);
        element.removeScope(this);
        element.removeBubbler(this);
        element.remove();
        node.removeFromParent();
        arrayRemove(this.elements, element);
        this.nodes2Elements.delete(node);
        this.htmlElements2Nodes.delete(element);
        this.htmlElements2Elements.delete(element);
        this.reorderChildren(parentElement);
    }
    collapseAll() {
        for (const element of this.elements) {
            if (element.node.isChild
                && element.canHaveChildren) {
                element.open = false;
            }
        }
    }
    expandAll(maxDepth = null) {
        for (const element of this.elements) {
            if (element.node.isChild
                && element.canHaveChildren
                && (isNullOrUndefined(maxDepth)
                    || element.node.depth <= maxDepth)) {
                element.open = true;
            }
        }
    }
}
//# sourceMappingURL=index.js.map