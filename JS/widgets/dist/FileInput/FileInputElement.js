import { arrayClear, arrayReplace, arrayScan, isNullOrUndefined, singleton } from "@juniper-lib/util";
import { alignItems, Disabled, display, Div, elementSetDisplay, flexDirection, flexGrow, gap, gridAutoRows, gridTemplateColumns, HtmlRender, InputFile, OnDragEnd, OnDragOver, OnDrop, OnInput, ReadOnly, registerFactory, repeat, rule, SingletonStyleBlob, TypedHTMLElement } from "@juniper-lib/dom";
import { OnRemoving } from "../ArrayViewElement";
import { FileAttr } from "./FileAttr";
import { FileView, FileViewElement } from "./FileViewElement";
import { OnIndexChanged } from "./IndexChangedEvent";
export class FileInputElement extends TypedHTMLElement {
    static { this.observedAttributes = [
        "accept",
        "disabled",
        "multiple",
        "readonly",
        "required"
    ]; }
    #input;
    #views;
    #tempFiles = new Array();
    constructor() {
        super();
        SingletonStyleBlob("Juniper::Cedrus::FileInput", () => rule("file-input", display("flex"), flexDirection("column"), gap("1em"), alignItems("stretch"), rule(">div", display("grid"), flexGrow(1), gridTemplateColumns(repeat("auto-fill", "minmax(200px, 1fr)")), gridAutoRows("1fr"), gap("1em"))));
        this.#input = InputFile(OnInput(() => {
            this.addFiles(Array.from(this.#input.files));
        }));
        HtmlRender(this, OnDragOver(evt => {
            evt.preventDefault();
            if (this.#tempFiles.length === 0) {
                this.#addFiles(Array.from(evt.dataTransfer.items)
                    .filter(item => item.kind === "file")
                    .map(item => item.getAsFile()), true);
            }
        }), OnDrop(evt => {
            evt.preventDefault();
            this.addFiles(Array.from(evt.dataTransfer.files));
        }), OnDragEnd(evt => {
            evt.preventDefault();
            this.addFiles([]);
        }));
        this.#views = Div(OnIndexChanged(evt => {
            const views = Array.from(this.fileViews);
            const moving = arrayScan(views, view => view === evt.target);
            const offset = moving.index > evt.index ? 0 : 1;
            this.#views.insertBefore(moving, views[evt.index + offset]);
            this.#reindex();
        }), OnRemoving(evt => {
            if (evt.target instanceof FileViewElement) {
                if (evt.target.file instanceof File) {
                    evt.target.remove();
                    this.#reindex();
                }
                else {
                    evt.target.deleting = !evt.target.deleting;
                }
            }
        }));
    }
    #ready = false;
    connectedCallback() {
        if (!this.#ready) {
            this.#ready = true;
            this.append(this.#input, this.#views);
        }
    }
    attributeChangedCallback(name, oldValue, newValue) {
        if (oldValue === newValue)
            return;
        switch (name) {
            case "accept":
                this.#input.accept = this.accept;
                break;
            case "required":
                this.#input.required = this.required;
                break;
            case "multiple":
                this.#input.multiple = this.multiple;
                break;
            case "disabled":
                this.#input.disabled = this.disabled;
                for (const view of this.fileViews) {
                    view.disabled = this.disabled;
                }
                break;
            case "readonly":
                elementSetDisplay(this.#input, !this.readOnly);
                this.#input.readOnly = this.readOnly;
                for (const view of this.fileViews) {
                    view.readOnly = this.readOnly;
                }
                break;
        }
    }
    get fileViews() { return this.#views.querySelectorAll("file-view"); }
    get multiple() { return this.hasAttribute("multiple"); }
    set multiple(v) { this.toggleAttribute("multiple", v); }
    get disabled() { return this.hasAttribute("disabled"); }
    set disabled(v) { this.toggleAttribute("disabled", v); }
    get required() { return this.hasAttribute("required"); }
    set required(v) { this.toggleAttribute("required", v); }
    get readOnly() { return this.hasAttribute("readonly"); }
    set readOnly(v) { this.toggleAttribute("readonly", v); }
    get placeholder() { return null; }
    set placeholder(_v) { }
    get accept() { return this.getAttribute("accept"); }
    set accept(v) {
        if (isNullOrUndefined(v)) {
            this.removeAttribute("accept");
        }
        else {
            this.setAttribute("accept", v);
        }
    }
    clear() {
        this.#views.innerHTML = "";
        this.#input.value = "";
    }
    addFiles(files) {
        this.#addFiles(files, false);
    }
    #addFiles(files, temp) {
        for (const view of this.#tempFiles) {
            view.remove();
        }
        arrayClear(this.#tempFiles);
        if (files.length > 0) {
            if (!this.multiple) {
                files = [files[0]];
            }
            const views = files.map(f => FileView(FileAttr(f), Disabled(this.disabled), ReadOnly(this.readOnly)));
            if (temp) {
                arrayReplace(this.#tempFiles, views);
            }
            if (this.multiple) {
                this.#views.append(...views);
            }
            else {
                this.#views.replaceChildren(...views);
            }
            this.#reindex();
        }
    }
    getFiles() {
        return Array.from(this.fileViews)
            .filter(v => !v.deleting)
            .map(v => v.file);
    }
    #reindex() {
        const views = this.fileViews;
        for (let i = 0; i < views.length; ++i) {
            views[i].setPosition(i, views.length);
        }
    }
    static install() {
        FileViewElement.install();
        return singleton("Juniper::Widgets::FileInputElement", () => registerFactory("file-input", FileInputElement));
    }
}
export function FileInput(...rest) {
    return FileInputElement.install()(...rest);
}
//# sourceMappingURL=FileInputElement.js.map