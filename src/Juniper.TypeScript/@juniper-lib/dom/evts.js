import { BaseProgress } from "@juniper-lib/progress/BaseProgress";
import { isFunction } from "@juniper-lib/tslib/typeChecks";
import { HtmlRender } from "./tags";
export function isModifierless(evt) {
    return !(evt.shiftKey || evt.altKey || evt.ctrlKey || evt.metaKey);
}
export function makeEnterKeyEventHandler(callback) {
    return (ev) => {
        const evt = ev;
        if (isModifierless(evt)
            && evt.key === "Enter") {
            callback(evt);
        }
    };
}
class HTMLProgressCallback extends BaseProgress {
    constructor(element) {
        super();
        this.element = element;
    }
    report(soFar, total, message, est) {
        super.report(soFar, total, message, est);
        this.element.max = total;
        this.element.value = soFar;
    }
}
export function makeProgress(element) {
    return new HTMLProgressCallback(element);
}
/**
 * A setter functor for HTML element events.
 **/
export class HtmlEvt {
    /**
     * Creates a new setter functor for an HTML element event.
     * @param name - the name of the event to attach to.
     * @param callback - the callback function to use with the event handler.
     * @param opts - additional attach options.
     */
    constructor(name, callback, opts) {
        this.name = name;
        this.callback = callback;
        if (!isFunction(callback)) {
            throw new Error("A function instance is required for this parameter");
        }
        this.opts = opts;
        Object.freeze(this);
    }
    applyToElement(elem) {
        this.add(elem);
    }
    /**
     * Add the encapsulate callback as an event listener to the give HTMLElement
     */
    add(elem) {
        elem.addEventListener(this.name, this.callback, this.opts);
    }
    /**
     * Remove the encapsulate callback as an event listener from the give HTMLElement
     */
    remove(elem) {
        elem.removeEventListener(this.name, this.callback);
    }
}
export function onEvent(eventName, callback, opts) { return new HtmlEvt(eventName, callback, opts); }
export function onAbort(callback, opts) { return onEvent("abort", callback, opts); }
export function onAfterPrint(callback, opts) { return onEvent("afterprint", callback, opts); }
export function onAnimationCancel(callback, opts) { return onEvent("animationcancel", callback, opts); }
export function onAnimationEnd(callback, opts) { return onEvent("animationend", callback, opts); }
export function onAnimationIteration(callback, opts) { return onEvent("animationiteration", callback, opts); }
export function onAnimationStart(callback, opts) { return onEvent("animationstart", callback, opts); }
export function onAuxClick(callback, opts) { return onEvent("auxclick", callback, opts); }
export function onBeforeInput(callback, opts) { return onEvent("beforeinput", callback, opts); }
export function onBeforePrint(callback, opts) { return onEvent("beforeprint", callback, opts); }
export function onBeforeUnload(callback, opts) { return onEvent("beforeunload", callback, opts); }
export function onBlur(callback, opts) { return onEvent("blur", callback, opts); }
export function onCancel(callback, opts) { return onEvent("cancel", callback, opts); }
export function onCanPlay(callback, opts) { return onEvent("canplay", callback, opts); }
export function onCanPlayThrough(callback, opts) { return onEvent("canplaythrough", callback, opts); }
export function onChange(callback, opts) { return onEvent("change", callback, opts); }
export function onClick(callback, opts) { return onEvent("click", callback, opts); }
export function onClose(callback, opts) { return onEvent("close", callback, opts); }
export function onCompositionEnd(callback, opts) { return onEvent("compositionend", callback, opts); }
export function onCompositionStart(callback, opts) { return onEvent("compositionstart", callback, opts); }
export function onCompositionUpdate(callback, opts) { return onEvent("compositionupdate", callback, opts); }
export function onContextMenu(callback, opts) { return onEvent("contextmenu", callback, opts); }
export function onCopy(callback, opts) { return onEvent("copy", callback, opts); }
export function onCut(callback, opts) { return onEvent("cut", callback, opts); }
export function onDblClick(callback, opts) { return onEvent("dblclick", callback, opts); }
export function onDrag(callback, opts) { return onEvent("drag", callback, opts); }
export function onDragEnd(callback, opts) { return onEvent("dragend", callback, opts); }
export function onDragEnter(callback, opts) { return onEvent("dragenter", callback, opts); }
export function onDragLeave(callback, opts) { return onEvent("dragleave", callback, opts); }
export function onDragOver(callback, opts) { return onEvent("dragover", callback, opts); }
export function onDragStart(callback, opts) { return onEvent("dragstart", callback, opts); }
export function onDrop(callback, opts) { return onEvent("drop", callback, opts); }
export function onDurationChange(callback, opts) { return onEvent("durationchange", callback, opts); }
export function onEmptied(callback, opts) { return onEvent("emptied", callback, opts); }
export function onEnded(callback, opts) { return onEvent("ended", callback, opts); }
export function onError(callback, opts) { return onEvent("error", callback, opts); }
export function onFocus(callback, opts) { return onEvent("focus", callback, opts); }
export function onFocusIn(callback, opts) { return onEvent("focusin", callback, opts); }
export function onFocusOut(callback, opts) { return onEvent("focusout", callback, opts); }
export function onFullScreenChange(callback, opts) { return onEvent("fullscreenchange", callback, opts); }
export function onFullScreenError(callback, opts) { return onEvent("fullscreenerror", callback, opts); }
export function onGamepadConnected(callback, opts) { return onEvent("gamepadconnected", callback, opts); }
export function onGamepadDisconnected(callback, opts) { return onEvent("gamepaddisconnected", callback, opts); }
export function onGotPointerCapture(callback, opts) { return onEvent("gotpointercapture", callback, opts); }
export function onHashChange(callback, opts) { return onEvent("hashchange", callback, opts); }
export function onLostPointerCapture(callback, opts) { return onEvent("lostpointercapture", callback, opts); }
export function onInput(callback, opts) { return onEvent("input", callback, opts); }
export function onInvalid(callback, opts) { return onEvent("invalid", callback, opts); }
export function onKeyDown(callback, opts) { return onEvent("keydown", callback, opts); }
export function onKeyPress(callback, opts) { return onEvent("keypress", callback, opts); }
export function onKeyUp(callback, opts) { return onEvent("keyup", callback, opts); }
export function onEnterKeyPressed(callback, opts) {
    return onKeyUp((evt) => {
        if (evt.key === "Enter") {
            callback(evt);
        }
    }, opts);
}
export function onLanguageChange(callback, opts) { return onEvent("languagechange", callback, opts); }
export function onLoad(callback, opts) { return onEvent("load", callback, opts); }
export function onLoadedData(callback, opts) { return onEvent("loadeddata", callback, opts); }
export function onLoadedMetadata(callback, opts) { return onEvent("loadedmetadata", callback, opts); }
export function onLoadStart(callback, opts) { return onEvent("loadstart", callback, opts); }
export function onMessage(callback, opts) { return onEvent("message", callback, opts); }
export function onMessageError(callback, opts) { return onEvent("messageerror", callback, opts); }
export function onMouseDown(callback, opts) { return onEvent("mousedown", callback, opts); }
export function onMouseEnter(callback, opts) { return onEvent("mouseenter", callback, opts); }
export function onMouseLeave(callback, opts) { return onEvent("mouseleave", callback, opts); }
export function onMouseMove(callback, opts) { return onEvent("mousemove", callback, opts); }
export function onMouseOut(callback, opts) { return onEvent("mouseout", callback, opts); }
export function onMouseOver(callback, opts) { return onEvent("mouseover", callback, opts); }
export function onMouseUp(callback, opts) { return onEvent("mouseup", callback, opts); }
export function onOffline(callback, opts) { return onEvent("offline", callback, opts); }
export function onOnline(callback, opts) { return onEvent("online", callback, opts); }
export function onPageHide(callback, opts) { return onEvent("pagehide", callback, opts); }
export function onPageShow(callback, opts) { return onEvent("pageshow", callback, opts); }
export function onPaste(callback, opts) { return onEvent("paste", callback, opts); }
export function onPause(callback, opts) { return onEvent("pause", callback, opts); }
export function onPointerCancel(callback, opts) { return onEvent("pointercancel", callback, opts); }
export function onPointerDown(callback, opts) { return onEvent("pointerdown", callback, opts); }
export function onPointerEnter(callback, opts) { return onEvent("pointerenter", callback, opts); }
export function onPointerLeave(callback, opts) { return onEvent("pointerleave", callback, opts); }
export function onPointerLockChange(callback, opts) { return onEvent("pointerlockchange", callback, opts); }
export function onPointerLockError(callback, opts) { return onEvent("pointerlockerror", callback, opts); }
export function onPointerMove(callback, opts) { return onEvent("pointermove", callback, opts); }
export function onPointerRawUpdate(callback, opts) { return onEvent("pointerrawupdate", callback, opts); }
export function onPointerOut(callback, opts) { return onEvent("pointerout", callback, opts); }
export function onPointerOver(callback, opts) { return onEvent("pointerover", callback, opts); }
export function onPointerUp(callback, opts) { return onEvent("pointerup", callback, opts); }
export function onPlay(callback, opts) { return onEvent("play", callback, opts); }
export function onPlaying(callback, opts) { return onEvent("playing", callback, opts); }
export function onPopstate(callback, opts) { return onEvent("popstate", callback, opts); }
export function onProgress(callback, opts) { return onEvent("progress", (evt) => callback(evt), opts); }
export function onProgressCallback(prog) {
    return onProgress((evt) => prog.report(evt.loaded || 0, evt.total || 1));
}
export function onRatechange(callback, opts) { return onEvent("ratechange", callback, opts); }
export function onReadystatechange(callback, opts) { return onEvent("readystatechange", callback, opts); }
export function onReleased(callback, opts) { return onEvent("released", callback, opts); }
export function onReset(callback, opts) { return onEvent("reset", callback, opts); }
export function onResize(callback, opts) { return onEvent("resize", callback, opts); }
export function onScroll(callback, opts) { return onEvent("scroll", callback, opts); }
export function onSeeked(callback, opts) { return onEvent("seeked", callback, opts); }
export function onSeeking(callback, opts) { return onEvent("seeking", callback, opts); }
export function onSelect(callback, opts) { return onEvent("select", callback, opts); }
export function onSelectStart(callback, opts) { return onEvent("selectstart", callback, opts); }
export function onSelectionChange(callback, opts) { return onEvent("selectionchange", callback, opts); }
export function onSlotChange(callback, opts) { return onEvent("slotchange", callback, opts); }
export function onStalled(callback, opts) { return onEvent("stalled", callback, opts); }
export function onStorage(callback, opts) { return onEvent("storage", callback, opts); }
export function onSubmit(callback, opts) { return onEvent("submit", callback, opts); }
export function onSuspend(callback, opts) { return onEvent("suspend", callback, opts); }
export function onTimeUpdate(callback, opts) { return onEvent("timeupdate", callback, opts); }
export function onToggle(callback, opts) { return onEvent("toggle", callback, opts); }
export function onTouchCancel(callback, opts) { return onEvent("touchcancel", callback, opts); }
export function onTouchEnd(callback, opts) { return onEvent("touchend", callback, opts); }
export function onTouchMove(callback, opts) { return onEvent("touchmove", callback, opts); }
export function onTouchStart(callback, opts) { return onEvent("touchstart", callback, opts); }
export function onTransitionEnd(callback, opts) { return onEvent("transitionend", callback, opts); }
export function onUnload(callback, opts) { return onEvent("unload", callback, opts); }
export function onVisibilityChange(callback, opts) { return onEvent("visibilitychange", callback, opts); }
export function onVolumeChange(callback, opts) { return onEvent("volumechange", callback, opts); }
export function onWaiting(callback, opts) { return onEvent("waiting", callback, opts); }
export function onWheel(callback, opts) { return onEvent("wheel", callback, opts); }
export function applyFakeProgress(file, elem, prog) {
    if (prog) {
        HtmlRender(elem, onProgressCallback(prog), onLoadStart(() => prog.start(`${file} loading`)), onLoad(() => prog.end(`${file} loaded`)));
    }
}
//# sourceMappingURL=evts.js.map