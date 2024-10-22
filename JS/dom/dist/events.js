import { isDefined, once, targetValidateEvent } from "@juniper-lib/util";
import { AbstractAppliable } from "./AbstractAppliable";
/**
 * A setter functor for HTML element events.
 **/
export class HtmlEvent extends AbstractAppliable {
    #name;
    #callback;
    #opts;
    #validate;
    /**
     * Creates a new setter functor for an HTML element event.
     * @param name - the name of the event to attach to.
     * @param callback - the callback function to use with the event handler.
     * @param opts - additional attach options.
     */
    constructor(name, callback, opts, validate = true) {
        super();
        if (typeof callback !== "function") {
            throw new Error("A function instance is required for this parameter");
        }
        this.#name = name;
        this.#callback = callback;
        this.#opts = opts;
        this.#validate = validate;
        Object.freeze(this);
    }
    /**
     * Add the encapsulate callback as an event listener to the give HTMLElement
     */
    apply(elem) {
        if (this.#validate) {
            targetValidateEvent(elem, this.#name);
        }
        elem.addEventListener(this.#name, this.#callback, this.#opts);
    }
    /**
     * Remove the encapsulate callback as an event listener from the give HTMLElement
     */
    remove(elem) {
        elem.removeEventListener(this.#name, this.#callback);
    }
}
/**
 * Creates a new setter functor for an HTML element event.
 * @param name - the name of the event to attach to.
 * @param callback - the callback function to use with the event handler.
 * @param opts - additional attach options.
 */
export function HtmlEvt(name, callback, opts, validate = true) {
    return new HtmlEvent(name, callback, opts, validate);
}
/**
 * Checks an Event to see if all of the modifier keys are not pressed
 */
export function isModifierless(evt) {
    return !(evt.shiftKey || evt.altKey || evt.ctrlKey || evt.metaKey);
}
/**
 * Creates an event handling callback that checks to make sure the
 * Enter key without any modifiers was pressed before executing the
 * supplied callback
 */
export function onKeyPressedCallback(key, callback) {
    return (evt) => {
        if (evt instanceof KeyboardEvent
            && isModifierless(evt)
            && evt.key === key) {
            callback(evt);
        }
    };
}
/**
 * Creates an event handling callback that checks to make sure the
 * Enter key without any modifiers was pressed before executing the
 * supplied callback
 */
export function onEnterKeyCallback(callback) {
    return onKeyPressedCallback("Enter", callback);
}
/**
 * Creates an event handling callback that checks to make sure the
 * Enter key without any modifiers was pressed before executing the
 * supplied callback
 */
export function onEscapeKeyCallback(callback) {
    return onKeyPressedCallback("Escape", callback);
}
/**********************************
 * EVENTS
 *********************************/
/**
 * Creates an event handler for the Abort event.
 **/
export function OnAbort(callback, opts) { return HtmlEvt("abort", callback, opts); }
/**
 * Creates an event handler for the AfterPrint event.
 **/
export function OnAfterPrint(callback, opts) { return HtmlEvt("afterprint", callback, opts); }
/**
 * Creates an event handler for the AnimationCancel event.
 **/
export function OnAnimationCancel(callback, opts) { return HtmlEvt("animationcancel", callback, opts); }
/**
 * Creates an event handler for the AnimationEnd event.
 **/
export function OnAnimationEnd(callback, opts) { return HtmlEvt("animationend", callback, opts); }
/**
 * Creates an event handler for the AnimationIteration event.
 **/
export function OnAnimationIteration(callback, opts) { return HtmlEvt("animationiteration", callback, opts); }
/**
 * Creates an event handler for the AnimationStart event.
 **/
export function OnAnimationStart(callback, opts) { return HtmlEvt("animationstart", callback, opts); }
/**
 * Creates an event handler for the AuxClick event.
 **/
export function OnAuxClick(callback, opts) { return HtmlEvt("auxclick", callback, opts); }
/**
 * Creates an event handler for the BeforeInput event.
 **/
export function OnBeforeInput(callback, opts) { return HtmlEvt("beforeinput", callback, opts); }
/**
 * Creates an event handler for the BeforePrint event.
 **/
export function OnBeforePrint(callback, opts) { return HtmlEvt("beforeprint", callback, opts); }
/**
 * Creates an event handler for the BeforeUnload event.
 **/
export function OnBeforeUnload(callback, opts) { return HtmlEvt("beforeunload", callback, opts); }
/**
 * Creates an event handler for the Blur event.
 **/
export function OnBlur(callback, opts) { return HtmlEvt("blur", callback, opts); }
/**
 * Creates an event handler for the Cancel event.
 **/
export function OnChancel(callback, opts) { return HtmlEvt("cancel", callback, opts); }
/**
 * Creates an event handler for the CanPlay event.
 **/
export function OnCanPlay(callback, opts) { return HtmlEvt("canplay", callback, opts); }
/**
 * Creates an event handler for the CanPlayThrough event.
 **/
export function OnCanPlayThrough(callback, opts) { return HtmlEvt("canplaythrough", callback, opts); }
/**
 * Creates an event handler for the Change event.
 **/
export function OnChange(callback, opts) { return HtmlEvt("change", callback, opts); }
/**
 * Creates an event handler for the Click event.
 **/
export function OnClick(callback, opts) { return HtmlEvt("click", callback, opts); }
/**
 * Creates an event handler for the Close event.
 **/
export function OnClose(callback, opts) { return HtmlEvt("close", callback, opts); }
/**
 * Creates an event handler for the CompositionEnd event.
 **/
export function OnCompositionEnd(callback, opts) { return HtmlEvt("compositionend", callback, opts); }
/**
 * Creates an event handler for the CompositionStart event.
 **/
export function OnCompositionStart(callback, opts) { return HtmlEvt("compositionstart", callback, opts); }
/**
 * Creates an event handler for the CompositionUpdate event.
 **/
export function OnCompositionUpdate(callback, opts) { return HtmlEvt("compositionupdate", callback, opts); }
/**
 * Creates an event handler for the ContextMenu event.
 **/
export function OnContextMenu(callback, opts) { return HtmlEvt("contextmenu", callback, opts); }
/**
 * Creates an event handler for the Copy event.
 **/
export function OnCopy(callback, opts) { return HtmlEvt("copy", callback, opts); }
/**
 * Creates an event handler for the Cut event.
 **/
export function OnCut(callback, opts) { return HtmlEvt("cut", callback, opts); }
/**
 * Creates an event handler for the DblClick event.
 **/
export function OnDblClick(callback, opts) { return HtmlEvt("dblclick", callback, opts); }
/**
 * Creates an event handler for the Drag event.
 **/
export function OnDrag(callback, opts) { return HtmlEvt("drag", callback, opts); }
/**
 * Creates an event handler for the DragEnd event.
 **/
export function OnDragEnd(callback, opts) { return HtmlEvt("dragend", callback, opts); }
/**
 * Creates an event handler for the DragEnter event.
 **/
export function OnDragEnter(callback, opts) { return HtmlEvt("dragenter", callback, opts); }
/**
 * Creates an event handler for the DragLeave event.
 **/
export function OnDragLeave(callback, opts) { return HtmlEvt("dragleave", callback, opts); }
/**
 * Creates an event handler for the DragOver event.
 **/
export function OnDragOver(callback, opts) { return HtmlEvt("dragover", callback, opts); }
/**
 * Creates an event handler for the DragStart event.
 **/
export function OnDragStart(callback, opts) { return HtmlEvt("dragstart", callback, opts); }
/**
 * Creates an event handler for the Drop event.
 **/
export function OnDrop(callback, opts) { return HtmlEvt("drop", callback, opts); }
/**
 * Creates an event handler for the DurationChange event.
 **/
export function OnDurationChange(callback, opts) { return HtmlEvt("durationchange", callback, opts); }
/**
 * Creates an event handler for the Emptied event.
 **/
export function OnEmptied(callback, opts) { return HtmlEvt("emptied", callback, opts); }
/**
 * Creates an event handler for the Ended event.
 **/
export function OnEnded(callback, opts) { return HtmlEvt("ended", callback, opts); }
/**
 * Creates an event handler for the Error event.
 **/
export function OnError(callback, opts) { return HtmlEvt("error", callback, opts); }
/**
 * Creates an event handler for the Focus event.
 **/
export function OnFocus(callback, opts) { return HtmlEvt("focus", callback, opts); }
/**
 * Creates an event handler for the FocusIn event.
 **/
export function OnFocusIn(callback, opts) { return HtmlEvt("focusin", callback, opts); }
/**
 * Creates an event handler for the FocusOut event.
 **/
export function OnFocusOut(callback, opts) { return HtmlEvt("focusout", callback, opts); }
/**
 * Creates an event handler for the FullScreenChange event.
 **/
export function OnFullScreenChange(callback, opts) { return HtmlEvt("fullscreenchange", callback, opts); }
/**
 * Creates an event handler for the FullScreenError event.
 **/
export function OnFullScreenError(callback, opts) { return HtmlEvt("fullscreenerror", callback, opts); }
/**
 * Creates an event handler for the GamepadConnected event.
 **/
export function OnGamepadConnected(callback, opts) { return HtmlEvt("gamepadconnected", callback, opts); }
/**
 * Creates an event handler for the GamepadDisconnected event.
 **/
export function OnGamepadDisconnected(callback, opts) { return HtmlEvt("gamepaddisconnected", callback, opts); }
/**
 * Creates an event handler for the GotPointerCapture event.
 **/
export function OnGotPointerCapture(callback, opts) { return HtmlEvt("gotpointercapture", callback, opts); }
/**
 * Creates an event handler for the HashChange event.
 **/
export function OnHashChange(callback, opts) { return HtmlEvt("hashchange", callback, opts); }
/**
 * Creates an event handler for the LostPointerCapture event.
 **/
export function OnLostPointerCapture(callback, opts) { return HtmlEvt("lostpointercapture", callback, opts); }
/**
 * Creates an event handler for the Input event.
 **/
export function OnInput(callback, opts) { return HtmlEvt("input", callback, opts); }
/**
 * Creates an event handler for the Invalid event.
 **/
export function OnInvalid(callback, opts) { return HtmlEvt("invalid", callback, opts); }
/**
 * Creates an event handler for the KeyDown event.
 **/
export function OnKeyDown(callback, opts) { return HtmlEvt("keydown", callback, opts); }
/**
 * Creates an event handler for the KeyPress event.
 **/
export function OnKeyPress(callback, opts) { return HtmlEvt("keypress", callback, opts); }
/**
 * Creates an event handler for the KeyUp event.
 **/
export function OnKeyUp(callback, opts) { return HtmlEvt("keyup", callback, opts); }
/**
 * Creates an event handler to detect the "Enter" key being hit without any modifiers in the KeyUp event.
 */
export function OnEnterKeyPressed(callback, opts) { return OnKeyUp(onEnterKeyCallback(callback), opts); }
/**
 * Creates an event handler to detect the "Escape" key being hit without any modifiers in the KeyUp event.
 */
export function OnEscapeKeyPressed(callback, opts) { return OnKeyUp(onEscapeKeyCallback(callback), opts); }
/**
 * Creates an event handler for the LanguageChange event.
 **/
export function OnLanguageChange(callback, opts) { return HtmlEvt("languagechange", callback, opts); }
/**
 * Creates an event handler for the Load event.
 **/
export function OnLoad(callback, opts) { return HtmlEvt("load", callback, opts); }
/**
 * Creates an event handler for the LoadedData event.
 **/
export function OnLoadedData(callback, opts) { return HtmlEvt("loadeddata", callback, opts); }
/**
 * Creates an event handler for the LoadedMetadata event.
 **/
export function OnLoadedMetadata(callback, opts) { return HtmlEvt("loadedmetadata", callback, opts); }
/**
 * Creates an event handler for the LoadStart event.
 **/
export function OnLoadStart(callback, opts) { return HtmlEvt("loadstart", callback, opts); }
/**
 * Creates an event handler for the Message event.
 **/
export function OnMessage(callback, opts) { return HtmlEvt("message", callback, opts); }
/**
 * Creates an event handler for the MessageError event.
 **/
export function OnMessageError(callback, opts) { return HtmlEvt("messageerror", callback, opts); }
/**
 * Creates an event handler for the MouseDown event.
 **/
export function OnMouseDown(callback, opts) { return HtmlEvt("mousedown", callback, opts); }
/**
 * Creates an event handler for the MouseEnter event.
 **/
export function OnMouseEnter(callback, opts) { return HtmlEvt("mouseenter", callback, opts); }
/**
 * Creates an event handler for the MouseLeave event.
 **/
export function OnMouseLeave(callback, opts) { return HtmlEvt("mouseleave", callback, opts); }
/**
 * Creates an event handler for the MouseMove event.
 **/
export function OnMouseMove(callback, opts) { return HtmlEvt("mousemove", callback, opts); }
/**
 * Creates an event handler for the MouseOut event.
 **/
export function OnMouseOut(callback, opts) { return HtmlEvt("mouseout", callback, opts); }
/**
 * Creates an event handler for the MouseOver event.
 **/
export function OnMouseOver(callback, opts) { return HtmlEvt("mouseover", callback, opts); }
/**
 * Creates an event handler for the MouseUp event.
 **/
export function OnMouseUp(callback, opts) { return HtmlEvt("mouseup", callback, opts); }
/**
 * Creates an event handler for the Offline event.
 **/
export function OnOffline(callback, opts) { return HtmlEvt("offline", callback, opts); }
/**
 * Creates an event handler for the Online event.
 **/
export function OnOnline(callback, opts) { return HtmlEvt("online", callback, opts); }
/**
 * Creates an event handler for the PageHide event.
 **/
export function OnPageHide(callback, opts) { return HtmlEvt("pagehide", callback, opts); }
/**
 * Creates an event handler for the PageShow event.
 **/
export function OnPageShow(callback, opts) { return HtmlEvt("pageshow", callback, opts); }
/**
 * Creates an event handler for the Paste event.
 **/
export function OnPaste(callback, opts) { return HtmlEvt("paste", callback, opts); }
/**
 * Creates an event handler for the Pause event.
 **/
export function OnPause(callback, opts) { return HtmlEvt("pause", callback, opts); }
/**
 * Creates an event handler for the PointerCancel event.
 **/
export function OnPointerCancel(callback, opts) { return HtmlEvt("pointercancel", callback, opts); }
/**
 * Creates an event handler for the PointerDown event.
 **/
export function OnPointerDown(callback, opts) { return HtmlEvt("pointerdown", callback, opts); }
/**
 * Creates an event handler for the PointerEnter event.
 **/
export function OnPointerEnter(callback, opts) { return HtmlEvt("pointerenter", callback, opts); }
/**
 * Creates an event handler for the PointerLeave event.
 **/
export function OnPointerLeave(callback, opts) { return HtmlEvt("pointerleave", callback, opts); }
/**
 * Creates an event handler for the PointerLockChange event.
 **/
export function OnPointerLockChange(callback, opts) { return HtmlEvt("pointerlockchange", callback, opts); }
/**
 * Creates an event handler for the PointerLockError event.
 **/
export function OnPointerLockError(callback, opts) { return HtmlEvt("pointerlockerror", callback, opts); }
/**
 * Creates an event handler for the PointerMove event.
 **/
export function OnPointerMove(callback, opts) { return HtmlEvt("pointermove", callback, opts); }
/**
 * Creates an event handler for the PointerRawUpdate event.
 **/
export function OnPointerRawUpdate(callback, opts) { return HtmlEvt("pointerrawupdate", callback, opts); }
/**
 * Creates an event handler for the PointerOut event.
 **/
export function OnPointerOut(callback, opts) { return HtmlEvt("pointerout", callback, opts); }
/**
 * Creates an event handler for the PointerOver event.
 **/
export function OnPointerOver(callback, opts) { return HtmlEvt("pointerover", callback, opts); }
/**
 * Creates an event handler for the PointerUp event.
 **/
export function OnPointerUp(callback, opts) { return HtmlEvt("pointerup", callback, opts); }
/**
 * Creates an event handler for the Play event.
 **/
export function OnPlay(callback, opts) { return HtmlEvt("play", callback, opts); }
/**
 * Creates an event handler for the Playing event.
 **/
export function OnPlaying(callback, opts) { return HtmlEvt("playing", callback, opts); }
/**
 * Creates an event handler for the Popstate event.
 **/
export function OnPopstate(callback, opts) { return HtmlEvt("popstate", callback, opts); }
/**
 * Creates an event handler for the Progress event.
 **/
export function OnProgress(callback, opts) { return HtmlEvt("progress", callback, opts); }
/**
 * Creates an event handler for the Ratechange event.
 **/
export function OnRatechange(callback, opts) { return HtmlEvt("ratechange", callback, opts); }
/**
 * Creates an event handler for the Readystatechange event.
 **/
export function OnReadystatechange(callback, opts) { return HtmlEvt("readystatechange", callback, opts); }
/**
 * Creates an event handler for the released event.
 */
export function OnReleased(callback, opts) { return HtmlEvt("released", callback, opts); }
/**
 * Creates an event handler for the Reset event.
 **/
export function OnReset(callback, opts) { return HtmlEvt("reset", callback, opts); }
/**
 * Creates an event handler for the Resize event.
 **/
export function OnResize(callback, opts) { return HtmlEvt("resize", callback, opts); }
/**
 * Creates an event handler for the Scroll event.
 **/
export function OnScroll(callback, opts) { return HtmlEvt("scroll", callback, opts); }
/**
 * Creates an event handler for the Search event.
 **/
export function OnSearch(callback, opts) { return HtmlEvt("search", callback, opts); }
/**
 * Creates an event handler for the Seeked event.
 **/
export function OnSeeked(callback, opts) { return HtmlEvt("seeked", callback, opts); }
/**
 * Creates an event handler for the Seeking event.
 **/
export function OnSeeking(callback, opts) { return HtmlEvt("seeking", callback, opts); }
/**
 * Creates an event handler for the Select event.
 **/
export function OnSelect(callback, opts) { return HtmlEvt("select", callback, opts); }
/**
 * Creates an event handler for the SelectStart event.
 **/
export function OnSelectStart(callback, opts) { return HtmlEvt("selectstart", callback, opts); }
/**
 * Creates an event handler for the SelectionChange event.
 **/
export function OnSelectionChange(callback, opts) { return HtmlEvt("selectionchange", callback, opts); }
/**
 * Creates an event handler for the SlotChange event.
 **/
export function OnSlotChange(callback, opts) { return HtmlEvt("slotchange", callback, opts); }
/**
 * Creates an event handler for the Stalled event.
 **/
export function OnStalled(callback, opts) { return HtmlEvt("stalled", callback, opts); }
/**
 * Creates an event handler for the Storage event.
 **/
export function OnStorage(callback, opts) { return HtmlEvt("storage", callback, opts); }
/**
 * Creates an event handler for the Submit event.
 **/
export function OnSubmit(callback, opts) { return HtmlEvt("submit", callback, opts); }
/**
 * Creates an event handler for the Suspend event.
 **/
export function OnSuspend(callback, opts) { return HtmlEvt("suspend", callback, opts); }
/**
 * Creates an event handler for the TimeUpdate event.
 **/
export function OnTimeUpdate(callback, opts) { return HtmlEvt("timeupdate", callback, opts); }
/**
 * Creates an event handler for the Toggle event.
 **/
export function OnToggle(callback, opts) { return HtmlEvt("toggle", callback, opts); }
/**
 * Creates an event handler for the TouchCancel event.
 **/
export function OnTouchCancel(callback, opts) { return HtmlEvt("touchcancel", callback, opts); }
/**
 * Creates an event handler for the TouchEnd event.
 **/
export function OnTouchEnd(callback, opts) { return HtmlEvt("touchend", callback, opts); }
/**
 * Creates an event handler for the TouchMove event.
 **/
export function OnTouchMove(callback, opts) { return HtmlEvt("touchmove", callback, opts); }
/**
 * Creates an event handler for the TouchStart event.
 **/
export function OnTouchStart(callback, opts) { return HtmlEvt("touchstart", callback, opts); }
/**
 * Creates an event handler for the TransitionEnd event.
 **/
export function OnTransitionEnd(callback, opts) { return HtmlEvt("transitionend", callback, opts); }
/**
 * Creates an event handler for the Unload event.
 **/
export function OnUnload(callback, opts) { return HtmlEvt("unload", callback, opts); }
/**
 * Creates an event handler for the VisibilityChange event.
 **/
export function OnVisibilityChange(callback, opts) { return HtmlEvt("visibilitychange", callback, opts); }
/**
 * Creates an event handler for the VolumeChange event.
 **/
export function OnVolumeChange(callback, opts) { return HtmlEvt("volumechange", callback, opts); }
/**
 * Creates an event handler for the Waiting event.
 **/
export function OnWaiting(callback, opts) { return HtmlEvt("waiting", callback, opts); }
/**
 * Creates an event handler for the Wheel event.
 **/
export function OnWheel(callback, opts) { return HtmlEvt("wheel", callback, opts); }
async function mediaElementCan(type, elem, prog) {
    if (isDefined(prog)) {
        prog.start();
    }
    const expectedState = type === "canplay"
        ? elem.HAVE_CURRENT_DATA
        : elem.HAVE_ENOUGH_DATA;
    if (elem.readyState >= expectedState) {
        return true;
    }
    try {
        await once(elem, type, "error");
        return true;
    }
    catch (err) {
        console.warn(elem.error, err);
        return false;
    }
    finally {
        if (isDefined(prog)) {
            prog.end();
        }
    }
}
export function mediaElementCanPlay(elem, prog) {
    return mediaElementCan("canplay", elem, prog);
}
export function mediaElementCanPlayThrough(elem, prog) {
    return mediaElementCan("canplaythrough", elem, prog);
}
//# sourceMappingURL=events.js.map