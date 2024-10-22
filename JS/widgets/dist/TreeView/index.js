import { alwaysFalse, arrayClear, arrayRemove, isDefined, isFunction, isNullOrUndefined, singleton, stringRandom } from "@juniper-lib/util";
import { buildTree } from "@juniper-lib/collections";
import { AutoComplete, Button, ClassList, Div, For, HtmlRender, ID, InputText, Label, OnClick, OnContextMenu, OnDragEnd, OnDragOver, OnDragStart, OnDrop, OnInput, OnKeyDown, PlaceHolder, SingletonStyleBlob, TabIndex, TypedHTMLElement, backgroundColor, border, display, elementGetIndexInParent, elementSetDisplay, fr, getSystemFamily, gridAutoFlow, gridTemplateRows, height, isModifierless, overflow, overflowWrap, padding, registerFactory, rgb, rule, whiteSpace } from "@juniper-lib/dom";
import { TypedEvent } from "@juniper-lib/events";
import { DataAttr, LabelField, OnItemSelected, SortKeyField, ValueField } from "../FieldDef";
import { PropertyList } from "../PropertyList";
import { TypedSelect } from "../TypedSelectElement";
import { TreeViewNodeContextMenuEvent, TreeViewNodeElement, TreeViewNodeSelectedEvent } from './TreeViewNodeElement';
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
export class TreeViewElement extends TypedHTMLElement {
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
        SingletonStyleBlob("Juniper::Widgets::TreeView", () => rule(".tree-view", display("grid"), gridTemplateRows("auto", "auto", fr(1)), rule(" .btn-sm", padding(0)), rule(" .tree-view-inner", border("inset 2px"), backgroundColor(rgb(224, 224, 224)), whiteSpace("nowrap"), overflowWrap("normal"), overflow("auto", "scroll"), getSystemFamily()), rule(" .tree-view-controls", display("grid"), gridAutoFlow("column")), rule(" .tree-view-children", height(0))));
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
        this.addEventListener("select", (evt) => {
            for (const elem of this.elements) {
                elem._selected = elem.node === evt.target.node;
            }
        });
        this.addEventListener("create", (evt) => {
            this.createElement(evt.node);
        });
        this.element = Div(ClassList("tree-view"), ...styleProps, this.filters = PropertyList(ClassList("tree-view-controls"), ...this.options.additionalProperties), Div(ClassList("tree-view-controls"), this.collapseButton = Button(OnClick(() => this.collapseAll()), "Collapse all"), this.expandButton = Button(OnClick(() => this.expandAll()), "Expand all")), Div(ClassList("tree-view-inner"), TabIndex(0), OnContextMenu(async (evt) => {
            if (!this.disabled) {
                const rootElement = this.nodes2Elements.get(this.rootNode);
                await rootElement._launchMenu(evt, new TreeViewNodeContextMenuEvent());
            }
        }), OnClick((evt) => {
            if (!this.disabled && !evt.defaultPrevented) {
                for (const element of this.elements) {
                    if (element.selected) {
                        this.selectedNode = null;
                        this.dispatchEvent(new TreeViewNodeSelectedEvent());
                        return;
                    }
                }
            }
        }), OnKeyDown((evt) => {
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
                else if (this.treeViewNodes.children.length > 0) {
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
        }), this.treeViewNodes = Div(ClassList("tree-view-children"))));
        const id = stringRandom(12);
        if (this.options.showNameFilter) {
            this.filters.append(Label(For(id + "Name"), "Name"), this.filterNameInput = InputText(ID(id + "Name"), ClassList("form-control"), PlaceHolder("Filter by name"), AutoComplete("off"), OnInput(() => {
                this.nameFilter = this.filterNameInput.value.toLocaleLowerCase();
                if (this.nameFilter.length === 0) {
                    this.nameFilter = null;
                }
                this.refreshFilter();
            })));
        }
        if (isDefined(this.options.typeFilters)) {
            this.filters.append(Label(For(id + "Type"), "Type"), this.filterTypeInput = TypedSelect(ID(id + "Type"), ValueField(v => v), LabelField(this.options.typeFilters.getTypeLabel), SortKeyField(this.options.typeFilters.getTypeLabel), PlaceHolder("Filter by type"), DataAttr(this.options.typeFilters.getTypes()), OnItemSelected(evt => {
                this.typeFilter = evt.item;
                this.refreshFilter();
            })));
        }
        if (this.options.showNameFilter || isDefined(this.options.typeFilters)) {
            this.filters.append(Button("Clear filter", OnClick(() => this.clearFilter())));
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
            HtmlRender(this.treeViewNodes, OnDragStart((evt) => {
                clearTarget();
                draggedElement = this.findElement(evt.target);
                draggedElement.style.opacity = "0.5";
            }), OnDragOver((evt) => {
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
            }), OnDrop((evt) => {
                if (this.canReparent(dropElement, draggedElement, lastTarget)) {
                    evt.preventDefault();
                    this.reparentNode(draggedElement.node, dropElement.node, delta);
                }
                clearTarget();
            }), OnDragEnd(() => {
                if (draggedElement) {
                    draggedElement.style.opacity = "1";
                }
                clearTarget();
            }));
        }
        if (this.options.replaceElement) {
            this.options.replaceElement.replaceWith(this.element);
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
            this.treeViewNodes.innerHTML = "";
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
                HtmlRender(this.treeViewNodes, rootElement);
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
            this.dispatchEvent(new TreeViewNodeSelectedEvent());
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
            if (here === this.treeViewNodes) {
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
        const element = TreeViewNodeElement.create(node, this.options.defaultLabel, this.options.getLabel, this.options.getDescription, this.options.canReorder, this.options.getChildDescription, this.options.canHaveChildren);
        this.elements.push(element);
        this.nodes2Elements.set(node, element);
        this.htmlElements2Nodes.set(element, node);
        this.htmlElements2Elements.set(element, element);
        const parentNode = node.parent;
        if (parentNode) {
            const parentElement = this.nodes2Elements.get(parentNode);
            if (parentElement) {
                if (!this.canChangeOrder) {
                    parentElement.childTreeNodes.append(element);
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
                    parentElement.childTreeNodes.insertBefore(element, nextElement);
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
        nextParentElement.childTreeNodes.insertBefore(curElement, nextSiblingElement);
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
            this.dispatchEvent(new TreeViewNodeSelectedEvent());
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
    static install() { return singleton("Juniper::Widgets::TreeViewElement", () => registerFactory("tree-view", TreeViewElement)); }
}
export function TreeView(...rest) {
    return TreeViewElement.install()(...rest);
}
//# sourceMappingURL=index.js.map