import { formatBytes, generate, isDefined, isNullOrUndefined, isString, singleton } from "@juniper-lib/util";
import { A, alignItems, Audio, backgroundColor, border, Button, ClassList, Controls, Disabled, display, Div, elementSetDisplay, FigCaption, flexDirection, flexGrow, gap, gridTemplateRows, height, I, Img, justifyContent, justifyItems, OnClick, OnInput, Option, overflow, padding, ReadOnly, registerFactory, rule, Select, SingletonStyleBlob, SpanTag, Src, Target, TextArea, textOverflow, TypedHTMLElement, Value, Video, whiteSpace, width } from "@juniper-lib/dom";
import { leftwardsArrow, rightwardsArrow, wastebasket } from "@juniper-lib/emoji";
import { sleep, Task } from "@juniper-lib/events";
import { createFetcher } from "@juniper-lib/fetcher";
import { RemovingEvent } from "../ArrayViewElement";
import { IndexChangedEvent } from "./IndexChangedEvent";
import { RequestInputEvent } from "./RequestInputEvent";
export class FileViewElement extends TypedHTMLElement {
    static { this.observedAttributes = [
        "disabled",
        "readonly"
    ]; }
    #titleBar;
    #title;
    #deleter;
    #caption;
    #type;
    #size;
    #preview;
    #controls;
    #indexSelector;
    #moveLeft;
    #moveRight;
    #downloadLink;
    constructor() {
        super();
        SingletonStyleBlob("Juniper::Cedrus::FileView", () => rule("file-view", display("grid"), gridTemplateRows("auto", "1fr", "auto", "auto"), alignItems("center"), justifyItems("center"), border("solid 1px #ccc"), padding("0.5em"), gap("0.25em"), whiteSpace("nowrap"), rule("[deleting]", backgroundColor("rgba(255, 0, 0, 0.1)")), rule(" textarea", width("100%"), height("12em")), rule(">div, >figcaption", display("flex"), justifyContent("center")), rule(">.controls", flexDirection("row"), justifyContent("space-between"), gap("0.25em"), overflow("hidden"), rule(">span:first-child", flexGrow(1), overflow("hidden"), textOverflow("ellipsis"))), rule(">div, >figcaption, >div>img", width("100%"))));
        this.#titleBar = Div(ClassList("controls"), this.#title = SpanTag(), this.#deleter = Button(ClassList("borderless"), I(wastebasket.value), OnClick(() => this.dispatchEvent(new RemovingEvent(this.file)))));
        this.#preview = Div("Loading preview...");
        this.#caption = FigCaption(ClassList("controls"), this.#type = SpanTag("No file selected"), this.#size = SpanTag(" 0.0 KB"));
        this.#downloadLink = A(Target("_blank"), "Download...");
        this.#controls = Div(ClassList("controls"), this.#moveLeft = Button(I(leftwardsArrow.value), OnClick(() => {
            if (this.index > 0) {
                this.dispatchEvent(new IndexChangedEvent(this.index - 1));
            }
        })), this.#indexSelector = Select(OnInput(() => this.dispatchEvent(new IndexChangedEvent(this.#indexSelector.selectedIndex)))), this.#moveRight = Button(I(rightwardsArrow.value), OnClick(() => {
            if (this.index < this.count - 1) {
                this.dispatchEvent(new IndexChangedEvent(this.index + 1));
            }
        })));
    }
    #ready = false;
    connectedCallback() {
        if (!this.#ready) {
            this.#ready = true;
            this.replaceChildren(this.#titleBar, this.#preview, this.#caption, this.#downloadLink, this.#controls);
            this.#render();
        }
    }
    attributeChangedCallback(name, oldValue, newValue) {
        if (oldValue === newValue)
            return;
        switch (name) {
            case "disabled":
                this.#deleter.disabled
                    = this.#indexSelector.disabled
                        = this.#moveLeft.disabled
                            = this.#moveRight.disabled
                                = this.disabled;
                break;
            case "readonly":
                elementSetDisplay(this.#deleter, !this.readOnly);
                elementSetDisplay(this.#controls, !this.readOnly);
                break;
        }
    }
    #file = null;
    get file() { return this.#file; }
    set file(v) {
        if (isString(v) && v === "") {
            v = null;
        }
        this.#file = v;
        this.#lastInfo = null;
        this.#render();
    }
    #index = 0;
    get index() { return this.#index; }
    get count() { return this.#indexSelector.options.length; }
    setPosition(index, count) {
        this.#indexSelector.replaceChildren(...generate(0, count).map(i => Option(i + 1)));
        this.#indexSelector.selectedIndex = this.#index = index;
        elementSetDisplay(this.#controls, count > 1);
    }
    get deleting() { return this.hasAttribute("deleting"); }
    set deleting(v) { this.toggleAttribute("deleting", v); }
    get disabled() { return this.hasAttribute("disabled"); }
    set disabled(v) { this.toggleAttribute("disabled", v); }
    get readOnly() { return this.hasAttribute("readonly"); }
    set readOnly(v) { this.toggleAttribute("readonly", v); }
    #lastInfo = null;
    async #render() {
        let title = "Unknown file...";
        let type = "unknown/unknown";
        let formattedSize = "N/A";
        let preview = Div("Preview unavailable...");
        ;
        const info = await this.#getFileInfo() ?? this.#lastInfo;
        if (isDefined(info)) {
            this.#lastInfo = info;
            const path = info.path;
            title = info.name;
            type = info.type;
            formattedSize = formatBytes(info.size);
            this.#downloadLink.href = path;
            if (type.startsWith("text/")) {
                const response = await fetch(path);
                const text = await response.text();
                preview = TextArea(ReadOnly(true), Disabled(true), Value(text));
            }
            else if (type.startsWith("image/")) {
                preview = Img(Src(path));
            }
            else if (type.startsWith("audio")) {
                preview = Audio(Src(path), Controls(true));
            }
            else if (type.startsWith("video")) {
                preview = Video(Src(path), Controls(true));
            }
        }
        this.#preview.replaceChildren(preview);
        this.#title.title = title;
        this.#title.replaceChildren(title);
        this.#type.title = type;
        this.#type.replaceChildren(`[${type}]`);
        this.#size.replaceChildren(formattedSize);
        elementSetDisplay(this.#downloadLink, isDefined(this.#file));
    }
    async #getFileInfo() {
        if (isNullOrUndefined(this.#file)) {
            return null;
        }
        if (isString(this.#file) || this.#file instanceof URL) {
            if (this.#file instanceof URL) {
                this.#file = this.#file.href;
            }
            const fetcher = createFetcher();
            const response = await fetcher
                .head(this.#file)
                .exec();
            if (response.status >= 400) {
                return null;
            }
            return {
                type: response.contentType,
                name: response.fileName ?? response.responsePath,
                size: response.contentLength,
                path: this.#file
            };
        }
        else if (this.#file instanceof File) {
            return {
                type: this.#file.type,
                name: this.#file.name,
                size: this.#file.size,
                path: URL.createObjectURL(this.#file)
            };
        }
        else {
            const task = new Task();
            sleep(3000).then(() => task.reject("Cancelled"));
            const request = new RequestInputEvent(this.#file, task);
            this.dispatchEvent(request);
            try {
                const path = await task;
                if (!path) {
                    return null;
                }
                return {
                    type: this.#file.type,
                    name: this.#file.name,
                    size: this.#file.size,
                    path
                };
            }
            catch (reason) {
                if (reason === "Cancelled") {
                    return null;
                }
                else {
                    throw reason;
                }
            }
        }
    }
    static install() {
        return singleton("Juniper::Widgets::FileViewElement", () => registerFactory("file-view", FileViewElement));
    }
}
export function FileView(...rest) {
    return FileViewElement.install()(...rest);
}
//# sourceMappingURL=FileViewElement.js.map