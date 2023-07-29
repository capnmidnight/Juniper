import { BaseProgress } from "@juniper-lib/progress/BaseProgress";
import { IProgress } from "@juniper-lib/progress/IProgress";
import { isFunction } from "@juniper-lib/tslib/typeChecks";
import { HtmlRender, IElementAppliable } from "./tags";

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


export function onEvent<T extends Event>(eventName: string, callback: (evt: T) => void, opts?: EventListenerOpts): HtmlEvt<T> { return new HtmlEvt<T>(eventName as any, callback, opts); }

export function onAbort(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("abort", callback, opts); }
export function onAfterPrint(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("afterprint", callback, opts); }
export function onAnimationCancel(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("animationcancel", callback, opts); }
export function onAnimationEnd(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("animationend", callback, opts); }
export function onAnimationIteration(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("animationiteration", callback, opts); }
export function onAnimationStart(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("animationstart", callback, opts); }
export function onAuxClick(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("auxclick", callback, opts); }
export function onBeforeInput(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("beforeinput", callback, opts); }
export function onBeforePrint(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("beforeprint", callback, opts); }
export function onBeforeUnload(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("beforeunload", callback, opts); }
export function onBlur(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("blur", callback, opts); }
export function onCanPlay(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("canplay", callback, opts); }
export function onCanPlayThrough(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("canplaythrough", callback, opts); }
export function onChange(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("change", callback, opts); }
export function onClick(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("click", callback, opts); }
export function onClose(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("close", callback, opts); }
export function onCompositionEnd(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("compositionend", callback, opts); }
export function onCompositionStart(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("compositionstart", callback, opts); }
export function onCompositionUpdate(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("compositionupdate", callback, opts); }
export function onContextMenu(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("contextmenu", callback, opts); }
export function onCopy(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("copy", callback, opts); }
export function onCut(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("cut", callback, opts); }
export function onDblClick(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("dblclick", callback, opts); }
export function onDrag(callback: (evt: DragEvent) => void, opts?: EventListenerOpts) { return onEvent("drag", callback, opts); }
export function onDragEnd(callback: (evt: DragEvent) => void, opts?: EventListenerOpts) { return onEvent("dragend", callback, opts); }
export function onDragEnter(callback: (evt: DragEvent) => void, opts?: EventListenerOpts) { return onEvent("dragenter", callback, opts); }
export function onDragLeave(callback: (evt: DragEvent) => void, opts?: EventListenerOpts) { return onEvent("dragleave", callback, opts); }
export function onDragOver(callback: (evt: DragEvent) => void, opts?: EventListenerOpts) { return onEvent("dragover", callback, opts); }
export function onDragStart(callback: (evt: DragEvent) => void, opts?: EventListenerOpts) { return onEvent("dragstart", callback, opts); }
export function onDrop(callback: (evt: DragEvent) => void, opts?: EventListenerOpts) { return onEvent("drop", callback, opts); }
export function onDurationChange(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("durationchange", callback, opts); }
export function onEmptied(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("emptied", callback, opts); }
export function onEnded(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("ended", callback, opts); }
export function onError(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("error", callback, opts); }
export function onFocus(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("focus", callback, opts); }
export function onFocusIn(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("focusin", callback, opts); }
export function onFocusOut(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("focusout", callback, opts); }
export function onFullScreenChange(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("fullscreenchange", callback, opts); }
export function onFullScreenError(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("fullscreenerror", callback, opts); }
export function onGamepadConnected(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("gamepadconnected", callback, opts); }
export function onGamepadDisconnected(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("gamepaddisconnected", callback, opts); }
export function onGotPointerCapture(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("gotpointercapture", callback, opts); }
export function onHashChange(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("hashchange", callback, opts); }
export function onLostPointerCapture(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("lostpointercapture", callback, opts); }
export function onInput(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("input", callback, opts); }
export function onInvalid(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("invalid", callback, opts); }
export function onKeyDown(callback: (evt: KeyboardEvent) => void, opts?: EventListenerOpts) { return onEvent("keydown", callback, opts); }
export function onKeyPress(callback: (evt: KeyboardEvent) => void, opts?: EventListenerOpts) { return onEvent("keypress", callback, opts); }
export function onKeyUp(callback: (evt: KeyboardEvent) => void, opts?: EventListenerOpts) { return onEvent("keyup", callback, opts); }
export function onEnterKeyPressed(callback: (evt: KeyboardEvent) => void, opts?: EventListenerOpts) {
    return onKeyUp((evt) => {
        if (evt.key === "Enter") {
            callback(evt);
        }
    }, opts);
}
export function onLanguageChange(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("languagechange", callback, opts); }
export function onLoad(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("load", callback, opts); }
export function onLoadedData(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("loadeddata", callback, opts); }
export function onLoadedMetadata(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("loadedmetadata", callback, opts); }
export function onLoadStart(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("loadstart", callback, opts); }
export function onMessage(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("message", callback, opts); }
export function onMessageError(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("messageerror", callback, opts); }
export function onMouseDown(callback: (evt: MouseEvent) => void, opts?: EventListenerOpts) { return onEvent("mousedown", callback, opts); }
export function onMouseEnter(callback: (evt: MouseEvent) => void, opts?: EventListenerOpts) { return onEvent("mouseenter", callback, opts); }
export function onMouseLeave(callback: (evt: MouseEvent) => void, opts?: EventListenerOpts) { return onEvent("mouseleave", callback, opts); }
export function onMouseMove(callback: (evt: MouseEvent) => void, opts?: EventListenerOpts) { return onEvent("mousemove", callback, opts); }
export function onMouseOut(callback: (evt: MouseEvent) => void, opts?: EventListenerOpts) { return onEvent("mouseout", callback, opts); }
export function onMouseOver(callback: (evt: MouseEvent) => void, opts?: EventListenerOpts) { return onEvent("mouseover", callback, opts); }
export function onMouseUp(callback: (evt: MouseEvent) => void, opts?: EventListenerOpts) { return onEvent("mouseup", callback, opts); }
export function onOffline(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("offline", callback, opts); }
export function onOnline(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("online", callback, opts); }
export function onPageHide(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("pagehide", callback, opts); }
export function onPageShow(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("pageshow", callback, opts); }
export function onPaste(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("paste", callback, opts); }
export function onPause(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("pause", callback, opts); }
export function onPointerCancel(callback: (evt: PointerEvent) => void, opts?: EventListenerOpts) { return onEvent("pointercancel", callback, opts); }
export function onPointerDown(callback: (evt: PointerEvent) => void, opts?: EventListenerOpts) { return onEvent("pointerdown", callback, opts); }
export function onPointerEnter(callback: (evt: PointerEvent) => void, opts?: EventListenerOpts) { return onEvent("pointerenter", callback, opts); }
export function onPointerLeave(callback: (evt: PointerEvent) => void, opts?: EventListenerOpts) { return onEvent("pointerleave", callback, opts); }
export function onPointerLockChange(callback: (evt: PointerEvent) => void, opts?: EventListenerOpts) { return onEvent("pointerlockchange", callback, opts); }
export function onPointerLockError(callback: (evt: PointerEvent) => void, opts?: EventListenerOpts) { return onEvent("pointerlockerror", callback, opts); }
export function onPointerMove(callback: (evt: PointerEvent) => void, opts?: EventListenerOpts) { return onEvent("pointermove", callback, opts); }
export function onPointerRawUpdate(callback: (evt: PointerEvent) => void, opts?: EventListenerOpts) { return onEvent("pointerrawupdate" as any, callback, opts); }
export function onPointerOut(callback: (evt: PointerEvent) => void, opts?: EventListenerOpts) { return onEvent("pointerout", callback, opts); }
export function onPointerOver(callback: (evt: PointerEvent) => void, opts?: EventListenerOpts) { return onEvent("pointerover", callback, opts); }
export function onPointerUp(callback: (evt: PointerEvent) => void, opts?: EventListenerOpts) { return onEvent("pointerup", callback, opts); }
export function onPlay(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("play", callback, opts); }
export function onPlaying(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("playing", callback, opts); }
export function onPopstate(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("popstate", callback, opts); }
export function onProgress(callback: (evt: ProgressEvent) => void, opts?: EventListenerOpts) { return onEvent("progress", (evt) => callback(evt as ProgressEvent), opts); }
export function onProgressCallback(prog: IProgress) {
    return onProgress((evt) => prog.report(evt.loaded || 0, evt.total || 1));
}
export function onRatechange(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("ratechange", callback, opts); }
export function onReadystatechange(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("readystatechange", callback, opts); }
export function onReset(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("reset", callback, opts); }
export function onResize(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("resize", callback, opts); }
export function onScroll(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("scroll", callback, opts); }
export function onSeeked(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("seeked", callback, opts); }
export function onSeeking(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("seeking", callback, opts); }
export function onSelect(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("select", callback, opts); }
export function onSelectStart(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("selectstart", callback, opts); }
export function onSelectionChange(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("selectionchange", callback, opts); }
export function onSlotChange(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("slotchange", callback, opts); }
export function onStalled(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("stalled", callback, opts); }
export function onStorage(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("storage", callback, opts); }
export function onSubmit(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("submit", callback, opts); }
export function onSuspend(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("suspend", callback, opts); }
export function onTimeUpdate(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("timeupdate", callback, opts); }
export function onToggle(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("toggle", callback, opts); }
export function onTouchCancel(callback: (evt: TouchEvent) => void, opts?: EventListenerOpts) { return onEvent("touchcancel", callback, opts); }
export function onTouchEnd(callback: (evt: TouchEvent) => void, opts?: EventListenerOpts) { return onEvent("touchend", callback, opts); }
export function onTouchMove(callback: (evt: TouchEvent) => void, opts?: EventListenerOpts) { return onEvent("touchmove", callback, opts); }
export function onTouchStart(callback: (evt: TouchEvent) => void, opts?: EventListenerOpts) { return onEvent("touchstart", callback, opts); }
export function onTransitionEnd(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("transitionend", callback, opts); }
export function onUnload(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("unload", callback, opts); }
export function onVisibilityChange(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("visibilitychange", callback, opts); }
export function onVolumeChange(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("volumechange", callback, opts); }
export function onWaiting(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("waiting", callback, opts); }
export function onWheel(callback: (evt: Event) => void, opts?: EventListenerOpts) { return onEvent("wheel", callback, opts); }

export function applyFakeProgress(file: string, elem: HTMLElement, prog: IProgress): void {
    if (prog) {
        HtmlRender(elem,
            onProgressCallback(prog),
            onLoadStart(() => prog.start(`${file} loading`)),
            onLoad(() => prog.end(`${file} loaded`))
        );
    }
}