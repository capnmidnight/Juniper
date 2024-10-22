import { eventHandler, isDefined, once, targetValidateEvent } from "@juniper-lib/util";
import { TypedEventMap } from "@juniper-lib/events";
import { AbstractAppliable } from "./AbstractAppliable";
import { IProgress } from "@juniper-lib/progress";


/**
 * A setter functor for HTML element events.
 **/
export class HtmlEvent<EventNameT extends string = string, EventTypeT extends Event = Event> extends AbstractAppliable<EventTarget> {

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
    constructor(name: EventNameT, callback: eventHandler<EventTypeT>, opts?: (boolean | AddEventListenerOptions), validate = true) {
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
    override apply(elem: EventTarget) {
        if (this.#validate) {
            targetValidateEvent(elem, this.#name);
        }

        elem.addEventListener(this.#name, this.#callback as eventHandler, this.#opts);
    }

    /**
     * Remove the encapsulate callback as an event listener from the give HTMLElement
     */
    remove(elem: EventTarget) {
        elem.removeEventListener(this.#name, this.#callback as eventHandler);
    }
}

type EventType<T> = T extends TypedEventMap<infer EventTypeT> ? EventTypeT extends keyof T ? string & EventTypeT : never : never;

export function HtmlEvt<EventMapT extends TypedEventMap<EventTypeT>, EventTypeT extends string = EventType<EventMapT>>(type: EventTypeT, callback: eventHandler<EventMapT[EventTypeT]>, options ?: boolean | AddEventListenerOptions, validate?: boolean): HtmlEvent;
export function HtmlEvt<K extends keyof HTMLElementEventMap>(type: string & K, listener: (this: HTMLElement, ev: HTMLElementEventMap[K]) => any, options?: boolean | AddEventListenerOptions, validate?: boolean): HtmlEvent;
export function HtmlEvt(type: string, listener: eventHandler, options?: boolean | AddEventListenerOptions, validate?: boolean): HtmlEvent;
/**
 * Creates a new setter functor for an HTML element event.
 * @param name - the name of the event to attach to.
 * @param callback - the callback function to use with the event handler.
 * @param opts - additional attach options.
 */
export function HtmlEvt(name: string, callback: eventHandler, opts?: (boolean | AddEventListenerOptions), validate = true) {
    return new HtmlEvent(name, callback, opts, validate);
}

/**
 * Checks an Event to see if all of the modifier keys are not pressed
 */
export function isModifierless(evt: KeyboardEvent | MouseEvent | PointerEvent) {
    return !(evt.shiftKey || evt.altKey || evt.ctrlKey || evt.metaKey);
}

/**
 * Creates an event handling callback that checks to make sure the 
 * Enter key without any modifiers was pressed before executing the
 * supplied callback
 */
export function onKeyPressedCallback(key: string, callback: eventHandler<KeyboardEvent>): eventHandler<KeyboardEvent> {
    return (evt: Event) => {
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
export function onEnterKeyCallback(callback: eventHandler<KeyboardEvent>) {
    return onKeyPressedCallback("Enter", callback);
}

/**
 * Creates an event handling callback that checks to make sure the 
 * Enter key without any modifiers was pressed before executing the
 * supplied callback
 */
export function onEscapeKeyCallback(callback: eventHandler<KeyboardEvent>) {
    return onKeyPressedCallback("Escape", callback);
}

/**********************************
 * EVENTS
 *********************************/
/**
 * Creates an event handler for the Abort event.
 **/
export function OnAbort(callback: eventHandler<UIEvent>, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("abort", callback, opts); }

/**
 * Creates an event handler for the AfterPrint event.
 **/
export function OnAfterPrint(callback: eventHandler, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("afterprint", callback, opts); }

/**
 * Creates an event handler for the AnimationCancel event.
 **/
export function OnAnimationCancel(callback: eventHandler<AnimationEvent>, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("animationcancel", callback, opts); }

/**
 * Creates an event handler for the AnimationEnd event.
 **/
export function OnAnimationEnd(callback: eventHandler<AnimationEvent>, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("animationend", callback, opts); }

/**
 * Creates an event handler for the AnimationIteration event.
 **/
export function OnAnimationIteration(callback: eventHandler<AnimationEvent>, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("animationiteration", callback, opts); }

/**
 * Creates an event handler for the AnimationStart event.
 **/
export function OnAnimationStart(callback: eventHandler<AnimationEvent>, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("animationstart", callback, opts); }

/**
 * Creates an event handler for the AuxClick event.
 **/
export function OnAuxClick(callback: eventHandler<MouseEvent>, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("auxclick", callback, opts); }

/**
 * Creates an event handler for the BeforeInput event.
 **/
export function OnBeforeInput(callback: eventHandler<InputEvent>, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("beforeinput", callback, opts); }

/**
 * Creates an event handler for the BeforePrint event.
 **/
export function OnBeforePrint(callback: eventHandler, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("beforeprint", callback, opts); }

/**
 * Creates an event handler for the BeforeUnload event.
 **/
export function OnBeforeUnload(callback: eventHandler, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("beforeunload", callback, opts); }

/**
 * Creates an event handler for the Blur event.
 **/
export function OnBlur(callback: eventHandler<FocusEvent>, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("blur", callback, opts); }

/**
 * Creates an event handler for the Cancel event.
 **/
export function OnChancel(callback: eventHandler, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("cancel", callback, opts); }

/**
 * Creates an event handler for the CanPlay event.
 **/
export function OnCanPlay(callback: eventHandler, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("canplay", callback, opts); }

/**
 * Creates an event handler for the CanPlayThrough event.
 **/
export function OnCanPlayThrough(callback: eventHandler, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("canplaythrough", callback, opts); }

/**
 * Creates an event handler for the Change event.
 **/
export function OnChange(callback: eventHandler, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("change", callback, opts); }

/**
 * Creates an event handler for the Click event.
 **/
export function OnClick(callback: eventHandler<MouseEvent>, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("click", callback, opts); }

/**
 * Creates an event handler for the Close event.
 **/
export function OnClose(callback: eventHandler, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("close", callback, opts); }

/**
 * Creates an event handler for the CompositionEnd event.
 **/
export function OnCompositionEnd(callback: eventHandler<CompositionEvent>, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("compositionend", callback, opts); }

/**
 * Creates an event handler for the CompositionStart event.
 **/
export function OnCompositionStart(callback: eventHandler<CompositionEvent>, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("compositionstart", callback, opts); }

/**
 * Creates an event handler for the CompositionUpdate event.
 **/
export function OnCompositionUpdate(callback: eventHandler<CompositionEvent>, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("compositionupdate", callback, opts); }

/**
 * Creates an event handler for the ContextMenu event.
 **/
export function OnContextMenu(callback: eventHandler<MouseEvent>, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("contextmenu", callback, opts); }

/**
 * Creates an event handler for the Copy event.
 **/
export function OnCopy(callback: eventHandler<ClipboardEvent>, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("copy", callback, opts); }

/**
 * Creates an event handler for the Cut event.
 **/
export function OnCut(callback: eventHandler<ClipboardEvent>, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("cut", callback, opts); }

/**
 * Creates an event handler for the DblClick event.
 **/
export function OnDblClick(callback: eventHandler<MouseEvent>, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("dblclick", callback, opts); }

/**
 * Creates an event handler for the Drag event.
 **/
export function OnDrag(callback: eventHandler<DragEvent>, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("drag", callback, opts); }

/**
 * Creates an event handler for the DragEnd event.
 **/
export function OnDragEnd(callback: eventHandler<DragEvent>, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("dragend", callback, opts); }

/**
 * Creates an event handler for the DragEnter event.
 **/
export function OnDragEnter(callback: eventHandler<DragEvent>, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("dragenter", callback, opts); }

/**
 * Creates an event handler for the DragLeave event.
 **/
export function OnDragLeave(callback: eventHandler<DragEvent>, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("dragleave", callback, opts); }

/**
 * Creates an event handler for the DragOver event.
 **/
export function OnDragOver(callback: eventHandler<DragEvent>, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("dragover", callback, opts); }

/**
 * Creates an event handler for the DragStart event.
 **/
export function OnDragStart(callback: eventHandler<DragEvent>, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("dragstart", callback, opts); }

/**
 * Creates an event handler for the Drop event.
 **/
export function OnDrop(callback: eventHandler<DragEvent>, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("drop", callback, opts); }

/**
 * Creates an event handler for the DurationChange event.
 **/
export function OnDurationChange(callback: eventHandler, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("durationchange", callback, opts); }

/**
 * Creates an event handler for the Emptied event.
 **/
export function OnEmptied(callback: eventHandler, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("emptied", callback, opts); }

/**
 * Creates an event handler for the Ended event.
 **/
export function OnEnded(callback: eventHandler, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("ended", callback, opts); }

/**
 * Creates an event handler for the Error event.
 **/
export function OnError(callback: eventHandler<ErrorEvent>, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("error", callback, opts); }

/**
 * Creates an event handler for the Focus event.
 **/
export function OnFocus(callback: eventHandler<FocusEvent>, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("focus", callback, opts); }

/**
 * Creates an event handler for the FocusIn event.
 **/
export function OnFocusIn(callback: eventHandler<FocusEvent>, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("focusin", callback, opts); }

/**
 * Creates an event handler for the FocusOut event.
 **/
export function OnFocusOut(callback: eventHandler<FocusEvent>, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("focusout", callback, opts); }

/**
 * Creates an event handler for the FullScreenChange event.
 **/
export function OnFullScreenChange(callback: eventHandler, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("fullscreenchange", callback, opts); }

/**
 * Creates an event handler for the FullScreenError event.
 **/
export function OnFullScreenError(callback: eventHandler, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("fullscreenerror", callback, opts); }

/**
 * Creates an event handler for the GamepadConnected event.
 **/
export function OnGamepadConnected(callback: eventHandler, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("gamepadconnected", callback, opts); }

/**
 * Creates an event handler for the GamepadDisconnected event.
 **/
export function OnGamepadDisconnected(callback: eventHandler, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("gamepaddisconnected", callback, opts); }

/**
 * Creates an event handler for the GotPointerCapture event.
 **/
export function OnGotPointerCapture(callback: eventHandler<PointerEvent>, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("gotpointercapture", callback, opts); }

/**
 * Creates an event handler for the HashChange event.
 **/
export function OnHashChange(callback: eventHandler, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("hashchange", callback, opts); }

/**
 * Creates an event handler for the LostPointerCapture event.
 **/
export function OnLostPointerCapture(callback: eventHandler<PointerEvent>, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("lostpointercapture", callback, opts); }

/**
 * Creates an event handler for the Input event.
 **/
export function OnInput(callback: eventHandler, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("input", callback, opts); }

/**
 * Creates an event handler for the Invalid event.
 **/
export function OnInvalid(callback: eventHandler, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("invalid", callback, opts); }

/**
 * Creates an event handler for the KeyDown event.
 **/
export function OnKeyDown(callback: eventHandler<KeyboardEvent>, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("keydown", callback, opts); }

/**
 * Creates an event handler for the KeyPress event.
 **/
export function OnKeyPress(callback: eventHandler<KeyboardEvent>, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("keypress", callback, opts); }

/**
 * Creates an event handler for the KeyUp event.
 **/
export function OnKeyUp(callback: eventHandler<KeyboardEvent>, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("keyup", callback, opts); }

/**
 * Creates an event handler to detect the "Enter" key being hit without any modifiers in the KeyUp event.
 */
export function OnEnterKeyPressed(callback: eventHandler<KeyboardEvent>, opts?: boolean | AddEventListenerOptions) { return OnKeyUp(onEnterKeyCallback(callback), opts); }

/**
 * Creates an event handler to detect the "Escape" key being hit without any modifiers in the KeyUp event.
 */
export function OnEscapeKeyPressed(callback: eventHandler<KeyboardEvent>, opts?: boolean | AddEventListenerOptions) { return OnKeyUp(onEscapeKeyCallback(callback), opts); }

/**
 * Creates an event handler for the LanguageChange event.
 **/
export function OnLanguageChange(callback: eventHandler, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("languagechange", callback, opts); }

/**
 * Creates an event handler for the Load event.
 **/
export function OnLoad(callback: eventHandler, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("load", callback, opts); }

/**
 * Creates an event handler for the LoadedData event.
 **/
export function OnLoadedData(callback: eventHandler, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("loadeddata", callback, opts); }

/**
 * Creates an event handler for the LoadedMetadata event.
 **/
export function OnLoadedMetadata(callback: eventHandler, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("loadedmetadata", callback, opts); }

/**
 * Creates an event handler for the LoadStart event.
 **/
export function OnLoadStart(callback: eventHandler, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("loadstart", callback, opts); }

/**
 * Creates an event handler for the Message event.
 **/
export function OnMessage(callback: eventHandler, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("message", callback, opts); }

/**
 * Creates an event handler for the MessageError event.
 **/
export function OnMessageError(callback: eventHandler, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("messageerror", callback, opts); }

/**
 * Creates an event handler for the MouseDown event.
 **/
export function OnMouseDown(callback: eventHandler<MouseEvent>, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("mousedown", callback, opts); }

/**
 * Creates an event handler for the MouseEnter event.
 **/
export function OnMouseEnter(callback: eventHandler<MouseEvent>, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("mouseenter", callback, opts); }

/**
 * Creates an event handler for the MouseLeave event.
 **/
export function OnMouseLeave(callback: eventHandler<MouseEvent>, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("mouseleave", callback, opts); }

/**
 * Creates an event handler for the MouseMove event.
 **/
export function OnMouseMove(callback: eventHandler<MouseEvent>, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("mousemove", callback, opts); }

/**
 * Creates an event handler for the MouseOut event.
 **/
export function OnMouseOut(callback: eventHandler<MouseEvent>, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("mouseout", callback, opts); }

/**
 * Creates an event handler for the MouseOver event.
 **/
export function OnMouseOver(callback: eventHandler<MouseEvent>, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("mouseover", callback, opts); }

/**
 * Creates an event handler for the MouseUp event.
 **/
export function OnMouseUp(callback: eventHandler<MouseEvent>, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("mouseup", callback, opts); }

/**
 * Creates an event handler for the Offline event.
 **/
export function OnOffline(callback: eventHandler, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("offline", callback, opts); }

/**
 * Creates an event handler for the Online event.
 **/
export function OnOnline(callback: eventHandler, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("online", callback, opts); }

/**
 * Creates an event handler for the PageHide event.
 **/
export function OnPageHide(callback: eventHandler, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("pagehide", callback, opts); }

/**
 * Creates an event handler for the PageShow event.
 **/
export function OnPageShow(callback: eventHandler, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("pageshow", callback, opts); }

/**
 * Creates an event handler for the Paste event.
 **/
export function OnPaste(callback: eventHandler<ClipboardEvent>, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("paste", callback, opts); }

/**
 * Creates an event handler for the Pause event.
 **/
export function OnPause(callback: eventHandler, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("pause", callback, opts); }

/**
 * Creates an event handler for the PointerCancel event.
 **/
export function OnPointerCancel(callback: eventHandler<PointerEvent>, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("pointercancel", callback, opts); }

/**
 * Creates an event handler for the PointerDown event.
 **/
export function OnPointerDown(callback: eventHandler<PointerEvent>, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("pointerdown", callback, opts); }

/**
 * Creates an event handler for the PointerEnter event.
 **/
export function OnPointerEnter(callback: eventHandler<PointerEvent>, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("pointerenter", callback, opts); }

/**
 * Creates an event handler for the PointerLeave event.
 **/
export function OnPointerLeave(callback: eventHandler<PointerEvent>, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("pointerleave", callback, opts); }

/**
 * Creates an event handler for the PointerLockChange event.
 **/
export function OnPointerLockChange(callback: eventHandler, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("pointerlockchange", callback, opts); }

/**
 * Creates an event handler for the PointerLockError event.
 **/
export function OnPointerLockError(callback: eventHandler, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("pointerlockerror", callback, opts); }

/**
 * Creates an event handler for the PointerMove event.
 **/
export function OnPointerMove(callback: eventHandler<PointerEvent>, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("pointermove", callback, opts); }

/**
 * Creates an event handler for the PointerRawUpdate event.
 **/
export function OnPointerRawUpdate(callback: eventHandler<PointerEvent>, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("pointerrawupdate", callback as eventHandler, opts); }

/**
 * Creates an event handler for the PointerOut event.
 **/
export function OnPointerOut(callback: eventHandler<PointerEvent>, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("pointerout", callback, opts); }

/**
 * Creates an event handler for the PointerOver event.
 **/
export function OnPointerOver(callback: eventHandler<PointerEvent>, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("pointerover", callback, opts); }

/**
 * Creates an event handler for the PointerUp event.
 **/
export function OnPointerUp(callback: eventHandler<PointerEvent>, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("pointerup", callback, opts); }

/**
 * Creates an event handler for the Play event.
 **/
export function OnPlay(callback: eventHandler, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("play", callback, opts); }

/**
 * Creates an event handler for the Playing event.
 **/
export function OnPlaying(callback: eventHandler, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("playing", callback, opts); }

/**
 * Creates an event handler for the Popstate event.
 **/
export function OnPopstate(callback: eventHandler, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("popstate", callback, opts); }

/**
 * Creates an event handler for the Progress event.
 **/
export function OnProgress(callback: eventHandler<ProgressEvent>, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("progress", callback, opts); }

/**
 * Creates an event handler for the Ratechange event.
 **/
export function OnRatechange(callback: eventHandler, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("ratechange", callback, opts); }

/**
 * Creates an event handler for the Readystatechange event.
 **/
export function OnReadystatechange(callback: eventHandler, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("readystatechange", callback, opts); }

/**
 * Creates an event handler for the released event.
 */
export function OnReleased(callback: eventHandler, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("released", callback, opts); }

/**
 * Creates an event handler for the Reset event.
 **/
export function OnReset(callback: eventHandler, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("reset", callback, opts); }

/**
 * Creates an event handler for the Resize event.
 **/
export function OnResize(callback: eventHandler<UIEvent>, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("resize", callback, opts); }

/**
 * Creates an event handler for the Scroll event.
 **/
export function OnScroll(callback: eventHandler, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("scroll", callback, opts); }

/**
 * Creates an event handler for the Search event.
 **/
export function OnSearch(callback: eventHandler, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("search", callback, opts); }

/**
 * Creates an event handler for the Seeked event.
 **/
export function OnSeeked(callback: eventHandler, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("seeked", callback, opts); }

/**
 * Creates an event handler for the Seeking event.
 **/
export function OnSeeking(callback: eventHandler, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("seeking", callback, opts); }

/**
 * Creates an event handler for the Select event.
 **/
export function OnSelect(callback: eventHandler, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("select", callback, opts); }

/**
 * Creates an event handler for the SelectStart event.
 **/
export function OnSelectStart(callback: eventHandler, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("selectstart", callback, opts); }

/**
 * Creates an event handler for the SelectionChange event.
 **/
export function OnSelectionChange(callback: eventHandler, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("selectionchange", callback, opts); }

/**
 * Creates an event handler for the SlotChange event.
 **/
export function OnSlotChange(callback: eventHandler, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("slotchange", callback, opts); }

/**
 * Creates an event handler for the Stalled event.
 **/
export function OnStalled(callback: eventHandler, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("stalled", callback, opts); }

/**
 * Creates an event handler for the Storage event.
 **/
export function OnStorage(callback: eventHandler, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("storage", callback, opts); }

/**
 * Creates an event handler for the Submit event.
 **/
export function OnSubmit(callback: eventHandler<SubmitEvent>, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("submit", callback, opts); }

/**
 * Creates an event handler for the Suspend event.
 **/
export function OnSuspend(callback: eventHandler, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("suspend", callback, opts); }

/**
 * Creates an event handler for the TimeUpdate event.
 **/
export function OnTimeUpdate(callback: eventHandler, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("timeupdate", callback, opts); }

/**
 * Creates an event handler for the Toggle event.
 **/
export function OnToggle(callback: eventHandler, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("toggle", callback, opts); }

/**
 * Creates an event handler for the TouchCancel event.
 **/
export function OnTouchCancel(callback: eventHandler<TouchEvent>, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("touchcancel", callback, opts); }

/**
 * Creates an event handler for the TouchEnd event.
 **/
export function OnTouchEnd(callback: eventHandler<TouchEvent>, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("touchend", callback, opts); }

/**
 * Creates an event handler for the TouchMove event.
 **/
export function OnTouchMove(callback: eventHandler<TouchEvent>, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("touchmove", callback, opts); }

/**
 * Creates an event handler for the TouchStart event.
 **/
export function OnTouchStart(callback: eventHandler<TouchEvent>, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("touchstart", callback, opts); }

/**
 * Creates an event handler for the TransitionEnd event.
 **/
export function OnTransitionEnd(callback: eventHandler<TransitionEvent>, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("transitionend", callback, opts); }

/**
 * Creates an event handler for the Unload event.
 **/
export function OnUnload(callback: eventHandler, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("unload", callback, opts); }

/**
 * Creates an event handler for the VisibilityChange event.
 **/
export function OnVisibilityChange(callback: eventHandler, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("visibilitychange", callback, opts); }

/**
 * Creates an event handler for the VolumeChange event.
 **/
export function OnVolumeChange(callback: eventHandler, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("volumechange", callback, opts); }

/**
 * Creates an event handler for the Waiting event.
 **/
export function OnWaiting(callback: eventHandler, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("waiting", callback, opts); }

/**
 * Creates an event handler for the Wheel event.
 **/
export function OnWheel(callback: eventHandler<WheelEvent>, opts?: boolean | AddEventListenerOptions) { return HtmlEvt("wheel", callback, opts); }




async function mediaElementCan(type: "canplay" | "canplaythrough", elem: HTMLMediaElement, prog?: IProgress): Promise<boolean> {
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

export function mediaElementCanPlay(elem: HTMLMediaElement, prog?: IProgress): Promise<boolean> {
    return mediaElementCan("canplay", elem, prog);
}

export function mediaElementCanPlayThrough(elem: HTMLMediaElement, prog?: IProgress): Promise<boolean> {
    return mediaElementCan("canplaythrough", elem, prog);
}