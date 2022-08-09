import { BaseProgress, IProgress, isFunction } from "@juniper-lib/tslib";
import { elementApply, IElementAppliable } from "./tags";

type EventListenerOpts = boolean | AddEventListenerOptions;

export function isModifierless(evt: KeyboardEvent | MouseEvent | PointerEvent) {
    return !(evt.shiftKey || evt.altKey || evt.ctrlKey || evt.metaKey);
}

export function makeEnterKeyEventHandler(callback: (evt: KeyboardEvent) => void) {
    return (ev: Event) => {
        const evt = ev as KeyboardEvent;
        if (isModifierless(evt)
            && evt.key === "Enter") {
            callback(evt);
        }
    };
}


class HTMLProgressCallback extends BaseProgress {
    constructor(private readonly element: HTMLProgressElement) {
        super();
    }

    override report(soFar: number, total: number, message?: string, est?: number) {
        super.report(soFar, total, message, est);
        this.element.max = total;
        this.element.value = soFar;
    }
}

export function makeProgress(element: HTMLProgressElement): IProgress {
    return new HTMLProgressCallback(element);
}

/**
 * A setter functor for HTML element events.
 **/
export class HtmlEvt<T extends Event>
    implements IElementAppliable {
    opts?: EventListenerOpts;

    /**
     * Creates a new setter functor for an HTML element event.
     * @param name - the name of the event to attach to.
     * @param callback - the callback function to use with the event handler.
     * @param opts - additional attach options.
     */
    constructor(name: keyof DocumentEventMap, callback: (evt: T) => void, opts?: EventListenerOpts);
    constructor(name: keyof HTMLBodyElementEventMap, callback: (evt: T) => void, opts?: EventListenerOpts);
    constructor(name: keyof HTMLElementEventMap, callback: (evt: T) => void, opts?: EventListenerOpts);
    constructor(name: keyof GlobalEventHandlersEventMap, callback: (evt: T) => void, opts?: EventListenerOpts);
    constructor(public name: string, public callback: (evt: T) => void, opts?: EventListenerOpts) {
        if (!isFunction(callback)) {
            throw new Error("A function instance is required for this parameter");
        }

        this.opts = opts;
        Object.freeze(this);
    }

    applyToElement(elem: HTMLElement) {
        this.add(elem);
    }

    /**
     * Add the encapsulate callback as an event listener to the give HTMLElement
     */
    add(elem: HTMLElement) {
        elem.addEventListener(this.name, this.callback as EventListenerOrEventListenerObject, this.opts);
    }

    /**
     * Remove the encapsulate callback as an event listener from the give HTMLElement
     */
    remove(elem: HTMLElement) {
        elem.removeEventListener(this.name, this.callback as EventListenerOrEventListenerObject);
    }
}


export function onAbort(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("abort", callback, opts); }
export function onAfterPrint(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("afterprint", callback, opts); }
export function onAnimationCancel(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("animationcancel", callback, opts); }
export function onAnimationEnd(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("animationend", callback, opts); }
export function onAnimationIteration(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("animationiteration", callback, opts); }
export function onAnimationStart(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("animationstart", callback, opts); }
export function onAuxClick(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("auxclick", callback, opts); }
export function onBeforeInput(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("beforeinput", callback, opts); }
export function onBeforePrint(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("beforeprint", callback, opts); }
export function onBeforeUnload(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("beforeunload", callback, opts); }
export function onBlur(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("blur", callback, opts); }
export function onCanPlay(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("canplay", callback, opts); }
export function onCanPlayThrough(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("canplaythrough", callback, opts); }
export function onChange(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("change", callback, opts); }
export function onClick(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("click", callback, opts); }
export function onClose(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("close", callback, opts); }
export function onCompositionEnd(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("compositionend", callback, opts); }
export function onCompositionStart(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("compositionstart", callback, opts); }
export function onCompositionUpdate(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("compositionupdate", callback, opts); }
export function onContextMenu(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("contextmenu", callback, opts); }
export function onCopy(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("copy", callback, opts); }
export function onCut(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("cut", callback, opts); }
export function onDblClick(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("dblclick", callback, opts); }
export function onDrag(callback: (evt: DragEvent) => void, opts?: EventListenerOpts) { return new HtmlEvt("drag", callback, opts); }
export function onDragEnd(callback: (evt: DragEvent) => void, opts?: EventListenerOpts) { return new HtmlEvt("dragend", callback, opts); }
export function onDragEnter(callback: (evt: DragEvent) => void, opts?: EventListenerOpts) { return new HtmlEvt("dragenter", callback, opts); }
export function onDragLeave(callback: (evt: DragEvent) => void, opts?: EventListenerOpts) { return new HtmlEvt("dragleave", callback, opts); }
export function onDragOver(callback: (evt: DragEvent) => void, opts?: EventListenerOpts) { return new HtmlEvt("dragover", callback, opts); }
export function onDragStart(callback: (evt: DragEvent) => void, opts?: EventListenerOpts) { return new HtmlEvt("dragstart", callback, opts); }
export function onDrop(callback: (evt: DragEvent) => void, opts?: EventListenerOpts) { return new HtmlEvt("drop", callback, opts); }
export function onDurationChange(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("durationchange", callback, opts); }
export function onEmptied(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("emptied", callback, opts); }
export function onEnded(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("ended", callback, opts); }
export function onError(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("error", callback, opts); }
export function onFocus(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("focus", callback, opts); }
export function onFocusIn(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("focusin", callback, opts); }
export function onFocusOut(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("focusout", callback, opts); }
export function onFullScreenChange(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("fullscreenchange", callback, opts); }
export function onFullScreenError(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("fullscreenerror", callback, opts); }
export function onGamepadConnected(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("gamepadconnected", callback, opts); }
export function onGamepadDisconnected(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("gamepaddisconnected", callback, opts); }
export function onGotPointerCapture(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("gotpointercapture", callback, opts); }
export function onHashChange(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("hashchange", callback, opts); }
export function onLostPointerCapture(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("lostpointercapture", callback, opts); }
export function onInput(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("input", callback, opts); }
export function onInvalid(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("invalid", callback, opts); }
export function onKeyDown(callback: (evt: KeyboardEvent) => void, opts?: EventListenerOpts) { return new HtmlEvt("keydown", callback, opts); }
export function onKeyPress(callback: (evt: KeyboardEvent) => void, opts?: EventListenerOpts) { return new HtmlEvt("keypress", callback, opts); }
export function onKeyUp(callback: (evt: KeyboardEvent) => void, opts?: EventListenerOpts) { return new HtmlEvt("keyup", callback, opts); }
export function onEnterKeyPressed(callback: (evt: KeyboardEvent) => void, opts?: EventListenerOpts) {
    return onKeyUp((evt) => {
        if (evt.key === "Enter") {
            callback(evt);
        }
    }, opts);
}
export function onLanguageChange(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("languagechange", callback, opts); }
export function onLoad(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("load", callback, opts); }
export function onLoadedData(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("loadeddata", callback, opts); }
export function onLoadedMetadata(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("loadedmetadata", callback, opts); }
export function onLoadStart(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("loadstart", callback, opts); }
export function onMessage(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("message", callback, opts); }
export function onMessageError(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("messageerror", callback, opts); }
export function onMouseDown(callback: (evt: MouseEvent) => void, opts?: EventListenerOpts) { return new HtmlEvt("mousedown", callback, opts); }
export function onMouseEnter(callback: (evt: MouseEvent) => void, opts?: EventListenerOpts) { return new HtmlEvt("mouseenter", callback, opts); }
export function onMouseLeave(callback: (evt: MouseEvent) => void, opts?: EventListenerOpts) { return new HtmlEvt("mouseleave", callback, opts); }
export function onMouseMove(callback: (evt: MouseEvent) => void, opts?: EventListenerOpts) { return new HtmlEvt("mousemove", callback, opts); }
export function onMouseOut(callback: (evt: MouseEvent) => void, opts?: EventListenerOpts) { return new HtmlEvt("mouseout", callback, opts); }
export function onMouseOver(callback: (evt: MouseEvent) => void, opts?: EventListenerOpts) { return new HtmlEvt("mouseover", callback, opts); }
export function onMouseUp(callback: (evt: MouseEvent) => void, opts?: EventListenerOpts) { return new HtmlEvt("mouseup", callback, opts); }
export function onOffline(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("offline", callback, opts); }
export function onOnline(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("online", callback, opts); }
export function onOrientationChange(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("orientationchange", callback, opts); }
export function onPageHide(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("pagehide", callback, opts); }
export function onPageShow(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("pageshow", callback, opts); }
export function onPaste(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("paste", callback, opts); }
export function onPause(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("pause", callback, opts); }
export function onPointerCancel(callback: (evt: PointerEvent) => void, opts?: EventListenerOpts) { return new HtmlEvt("pointercancel", callback, opts); }
export function onPointerDown(callback: (evt: PointerEvent) => void, opts?: EventListenerOpts) { return new HtmlEvt("pointerdown", callback, opts); }
export function onPointerEnter(callback: (evt: PointerEvent) => void, opts?: EventListenerOpts) { return new HtmlEvt("pointerenter", callback, opts); }
export function onPointerLeave(callback: (evt: PointerEvent) => void, opts?: EventListenerOpts) { return new HtmlEvt("pointerleave", callback, opts); }
export function onPointerLockChange(callback: (evt: PointerEvent) => void, opts?: EventListenerOpts) { return new HtmlEvt("pointerlockchange", callback, opts); }
export function onPointerLockError(callback: (evt: PointerEvent) => void, opts?: EventListenerOpts) { return new HtmlEvt("pointerlockerror", callback, opts); }
export function onPointerMove(callback: (evt: PointerEvent) => void, opts?: EventListenerOpts) { return new HtmlEvt("pointermove", callback, opts); }
export function onPointerRawUpdate(callback: (evt: PointerEvent) => void, opts?: EventListenerOpts) { return new HtmlEvt("pointerrawupdate" as any, callback, opts); }
export function onPointerOut(callback: (evt: PointerEvent) => void, opts?: EventListenerOpts) { return new HtmlEvt("pointerout", callback, opts); }
export function onPointerOver(callback: (evt: PointerEvent) => void, opts?: EventListenerOpts) { return new HtmlEvt("pointerover", callback, opts); }
export function onPointerUp(callback: (evt: PointerEvent) => void, opts?: EventListenerOpts) { return new HtmlEvt("pointerup", callback, opts); }
export function onPlay(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("play", callback, opts); }
export function onPlaying(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("playing", callback, opts); }
export function onPopstate(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("popstate", callback, opts); }
export function onProgress(callback: (evt: ProgressEvent) => void, opts?: EventListenerOpts) { return new HtmlEvt("progress", (evt) => callback(evt as ProgressEvent), opts); }
export function onProgressCallback(prog: IProgress) {
    return onProgress((evt) => prog.report(evt.loaded || 0, evt.total || 1));
}
export function onRatechange(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("ratechange", callback, opts); }
export function onReadystatechange(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("readystatechange", callback, opts); }
export function onReset(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("reset", callback, opts); }
export function onResize(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("resize", callback, opts); }
export function onScroll(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("scroll", callback, opts); }
export function onSeeked(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("seeked", callback, opts); }
export function onSeeking(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("seeking", callback, opts); }
export function onSelect(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("select", callback, opts); }
export function onSelectStart(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("selectstart", callback, opts); }
export function onSelectionChange(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("selectionchange", callback, opts); }
export function onSlotChange(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("slotchange", callback, opts); }
export function onStalled(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("stalled", callback, opts); }
export function onStorage(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("storage", callback, opts); }
export function onSubmit(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("submit", callback, opts); }
export function onSuspend(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("suspend", callback, opts); }
export function onTimeUpdate(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("timeupdate", callback, opts); }
export function onToggle(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("toggle", callback, opts); }
export function onTouchCancel(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("touchcancel", callback, opts); }
export function onTouchEnd(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("touchend", callback, opts); }
export function onTouchMove(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("touchmove", callback, opts); }
export function onTouchStart(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("touchstart", callback, opts); }
export function onTransitionEnd(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("transitionend", callback, opts); }
export function onUnload(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("unload", callback, opts); }
export function onVisibilityChange(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("visibilitychange", callback, opts); }
export function onVolumeChange(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("volumechange", callback, opts); }
export function onWaiting(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("waiting", callback, opts); }
export function onWheel(callback: (evt: Event) => void, opts?: EventListenerOpts) { return new HtmlEvt("wheel", callback, opts); }

export function applyFakeProgress(file: string, elem: HTMLElement, prog: IProgress): void {
    if (prog) {
        elementApply(elem,
            onProgressCallback(prog),
            onLoadStart(() => prog.start(`${file} loading`)),
            onLoad(() => prog.end(`${file} loaded`))
        );
    }
}