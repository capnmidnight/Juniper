import { eventHandler } from "@juniper-lib/util";
import { TypedEventMap } from "@juniper-lib/events";
import { AbstractAppliable } from "./AbstractAppliable";
import { IProgress } from "@juniper-lib/progress";
/**
 * A setter functor for HTML element events.
 **/
export declare class HtmlEvent<EventNameT extends string = string, EventTypeT extends Event = Event> extends AbstractAppliable<EventTarget> {
    #private;
    /**
     * Creates a new setter functor for an HTML element event.
     * @param name - the name of the event to attach to.
     * @param callback - the callback function to use with the event handler.
     * @param opts - additional attach options.
     */
    constructor(name: EventNameT, callback: eventHandler<EventTypeT>, opts?: (boolean | AddEventListenerOptions), validate?: boolean);
    /**
     * Add the encapsulate callback as an event listener to the give HTMLElement
     */
    apply(elem: EventTarget): void;
    /**
     * Remove the encapsulate callback as an event listener from the give HTMLElement
     */
    remove(elem: EventTarget): void;
}
type EventType<T> = T extends TypedEventMap<infer EventTypeT> ? EventTypeT extends keyof T ? string & EventTypeT : never : never;
export declare function HtmlEvt<EventMapT extends TypedEventMap<EventTypeT>, EventTypeT extends string = EventType<EventMapT>>(type: EventTypeT, callback: eventHandler<EventMapT[EventTypeT]>, options?: boolean | AddEventListenerOptions, validate?: boolean): HtmlEvent;
export declare function HtmlEvt<K extends keyof HTMLElementEventMap>(type: string & K, listener: (this: HTMLElement, ev: HTMLElementEventMap[K]) => any, options?: boolean | AddEventListenerOptions, validate?: boolean): HtmlEvent;
export declare function HtmlEvt(type: string, listener: eventHandler, options?: boolean | AddEventListenerOptions, validate?: boolean): HtmlEvent;
/**
 * Checks an Event to see if all of the modifier keys are not pressed
 */
export declare function isModifierless(evt: KeyboardEvent | MouseEvent | PointerEvent): boolean;
/**
 * Creates an event handling callback that checks to make sure the
 * Enter key without any modifiers was pressed before executing the
 * supplied callback
 */
export declare function onKeyPressedCallback(key: string, callback: eventHandler<KeyboardEvent>): eventHandler<KeyboardEvent>;
/**
 * Creates an event handling callback that checks to make sure the
 * Enter key without any modifiers was pressed before executing the
 * supplied callback
 */
export declare function onEnterKeyCallback(callback: eventHandler<KeyboardEvent>): eventHandler<KeyboardEvent>;
/**
 * Creates an event handling callback that checks to make sure the
 * Enter key without any modifiers was pressed before executing the
 * supplied callback
 */
export declare function onEscapeKeyCallback(callback: eventHandler<KeyboardEvent>): eventHandler<KeyboardEvent>;
/**********************************
 * EVENTS
 *********************************/
/**
 * Creates an event handler for the Abort event.
 **/
export declare function OnAbort(callback: eventHandler<UIEvent>, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the AfterPrint event.
 **/
export declare function OnAfterPrint(callback: eventHandler, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the AnimationCancel event.
 **/
export declare function OnAnimationCancel(callback: eventHandler<AnimationEvent>, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the AnimationEnd event.
 **/
export declare function OnAnimationEnd(callback: eventHandler<AnimationEvent>, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the AnimationIteration event.
 **/
export declare function OnAnimationIteration(callback: eventHandler<AnimationEvent>, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the AnimationStart event.
 **/
export declare function OnAnimationStart(callback: eventHandler<AnimationEvent>, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the AuxClick event.
 **/
export declare function OnAuxClick(callback: eventHandler<MouseEvent>, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the BeforeInput event.
 **/
export declare function OnBeforeInput(callback: eventHandler<InputEvent>, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the BeforePrint event.
 **/
export declare function OnBeforePrint(callback: eventHandler, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the BeforeUnload event.
 **/
export declare function OnBeforeUnload(callback: eventHandler, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the Blur event.
 **/
export declare function OnBlur(callback: eventHandler<FocusEvent>, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the Cancel event.
 **/
export declare function OnChancel(callback: eventHandler, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the CanPlay event.
 **/
export declare function OnCanPlay(callback: eventHandler, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the CanPlayThrough event.
 **/
export declare function OnCanPlayThrough(callback: eventHandler, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the Change event.
 **/
export declare function OnChange(callback: eventHandler, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the Click event.
 **/
export declare function OnClick(callback: eventHandler<MouseEvent>, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the Close event.
 **/
export declare function OnClose(callback: eventHandler, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the CompositionEnd event.
 **/
export declare function OnCompositionEnd(callback: eventHandler<CompositionEvent>, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the CompositionStart event.
 **/
export declare function OnCompositionStart(callback: eventHandler<CompositionEvent>, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the CompositionUpdate event.
 **/
export declare function OnCompositionUpdate(callback: eventHandler<CompositionEvent>, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the ContextMenu event.
 **/
export declare function OnContextMenu(callback: eventHandler<MouseEvent>, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the Copy event.
 **/
export declare function OnCopy(callback: eventHandler<ClipboardEvent>, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the Cut event.
 **/
export declare function OnCut(callback: eventHandler<ClipboardEvent>, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the DblClick event.
 **/
export declare function OnDblClick(callback: eventHandler<MouseEvent>, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the Drag event.
 **/
export declare function OnDrag(callback: eventHandler<DragEvent>, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the DragEnd event.
 **/
export declare function OnDragEnd(callback: eventHandler<DragEvent>, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the DragEnter event.
 **/
export declare function OnDragEnter(callback: eventHandler<DragEvent>, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the DragLeave event.
 **/
export declare function OnDragLeave(callback: eventHandler<DragEvent>, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the DragOver event.
 **/
export declare function OnDragOver(callback: eventHandler<DragEvent>, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the DragStart event.
 **/
export declare function OnDragStart(callback: eventHandler<DragEvent>, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the Drop event.
 **/
export declare function OnDrop(callback: eventHandler<DragEvent>, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the DurationChange event.
 **/
export declare function OnDurationChange(callback: eventHandler, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the Emptied event.
 **/
export declare function OnEmptied(callback: eventHandler, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the Ended event.
 **/
export declare function OnEnded(callback: eventHandler, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the Error event.
 **/
export declare function OnError(callback: eventHandler<ErrorEvent>, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the Focus event.
 **/
export declare function OnFocus(callback: eventHandler<FocusEvent>, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the FocusIn event.
 **/
export declare function OnFocusIn(callback: eventHandler<FocusEvent>, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the FocusOut event.
 **/
export declare function OnFocusOut(callback: eventHandler<FocusEvent>, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the FullScreenChange event.
 **/
export declare function OnFullScreenChange(callback: eventHandler, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the FullScreenError event.
 **/
export declare function OnFullScreenError(callback: eventHandler, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the GamepadConnected event.
 **/
export declare function OnGamepadConnected(callback: eventHandler, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the GamepadDisconnected event.
 **/
export declare function OnGamepadDisconnected(callback: eventHandler, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the GotPointerCapture event.
 **/
export declare function OnGotPointerCapture(callback: eventHandler<PointerEvent>, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the HashChange event.
 **/
export declare function OnHashChange(callback: eventHandler, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the LostPointerCapture event.
 **/
export declare function OnLostPointerCapture(callback: eventHandler<PointerEvent>, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the Input event.
 **/
export declare function OnInput(callback: eventHandler, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the Invalid event.
 **/
export declare function OnInvalid(callback: eventHandler, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the KeyDown event.
 **/
export declare function OnKeyDown(callback: eventHandler<KeyboardEvent>, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the KeyPress event.
 **/
export declare function OnKeyPress(callback: eventHandler<KeyboardEvent>, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the KeyUp event.
 **/
export declare function OnKeyUp(callback: eventHandler<KeyboardEvent>, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler to detect the "Enter" key being hit without any modifiers in the KeyUp event.
 */
export declare function OnEnterKeyPressed(callback: eventHandler<KeyboardEvent>, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler to detect the "Escape" key being hit without any modifiers in the KeyUp event.
 */
export declare function OnEscapeKeyPressed(callback: eventHandler<KeyboardEvent>, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the LanguageChange event.
 **/
export declare function OnLanguageChange(callback: eventHandler, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the Load event.
 **/
export declare function OnLoad(callback: eventHandler, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the LoadedData event.
 **/
export declare function OnLoadedData(callback: eventHandler, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the LoadedMetadata event.
 **/
export declare function OnLoadedMetadata(callback: eventHandler, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the LoadStart event.
 **/
export declare function OnLoadStart(callback: eventHandler, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the Message event.
 **/
export declare function OnMessage(callback: eventHandler, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the MessageError event.
 **/
export declare function OnMessageError(callback: eventHandler, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the MouseDown event.
 **/
export declare function OnMouseDown(callback: eventHandler<MouseEvent>, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the MouseEnter event.
 **/
export declare function OnMouseEnter(callback: eventHandler<MouseEvent>, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the MouseLeave event.
 **/
export declare function OnMouseLeave(callback: eventHandler<MouseEvent>, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the MouseMove event.
 **/
export declare function OnMouseMove(callback: eventHandler<MouseEvent>, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the MouseOut event.
 **/
export declare function OnMouseOut(callback: eventHandler<MouseEvent>, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the MouseOver event.
 **/
export declare function OnMouseOver(callback: eventHandler<MouseEvent>, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the MouseUp event.
 **/
export declare function OnMouseUp(callback: eventHandler<MouseEvent>, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the Offline event.
 **/
export declare function OnOffline(callback: eventHandler, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the Online event.
 **/
export declare function OnOnline(callback: eventHandler, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the PageHide event.
 **/
export declare function OnPageHide(callback: eventHandler, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the PageShow event.
 **/
export declare function OnPageShow(callback: eventHandler, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the Paste event.
 **/
export declare function OnPaste(callback: eventHandler<ClipboardEvent>, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the Pause event.
 **/
export declare function OnPause(callback: eventHandler, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the PointerCancel event.
 **/
export declare function OnPointerCancel(callback: eventHandler<PointerEvent>, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the PointerDown event.
 **/
export declare function OnPointerDown(callback: eventHandler<PointerEvent>, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the PointerEnter event.
 **/
export declare function OnPointerEnter(callback: eventHandler<PointerEvent>, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the PointerLeave event.
 **/
export declare function OnPointerLeave(callback: eventHandler<PointerEvent>, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the PointerLockChange event.
 **/
export declare function OnPointerLockChange(callback: eventHandler, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the PointerLockError event.
 **/
export declare function OnPointerLockError(callback: eventHandler, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the PointerMove event.
 **/
export declare function OnPointerMove(callback: eventHandler<PointerEvent>, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the PointerRawUpdate event.
 **/
export declare function OnPointerRawUpdate(callback: eventHandler<PointerEvent>, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the PointerOut event.
 **/
export declare function OnPointerOut(callback: eventHandler<PointerEvent>, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the PointerOver event.
 **/
export declare function OnPointerOver(callback: eventHandler<PointerEvent>, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the PointerUp event.
 **/
export declare function OnPointerUp(callback: eventHandler<PointerEvent>, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the Play event.
 **/
export declare function OnPlay(callback: eventHandler, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the Playing event.
 **/
export declare function OnPlaying(callback: eventHandler, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the Popstate event.
 **/
export declare function OnPopstate(callback: eventHandler, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the Progress event.
 **/
export declare function OnProgress(callback: eventHandler<ProgressEvent>, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the Ratechange event.
 **/
export declare function OnRatechange(callback: eventHandler, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the Readystatechange event.
 **/
export declare function OnReadystatechange(callback: eventHandler, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the released event.
 */
export declare function OnReleased(callback: eventHandler, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the Reset event.
 **/
export declare function OnReset(callback: eventHandler, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the Resize event.
 **/
export declare function OnResize(callback: eventHandler<UIEvent>, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the Scroll event.
 **/
export declare function OnScroll(callback: eventHandler, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the Search event.
 **/
export declare function OnSearch(callback: eventHandler, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the Seeked event.
 **/
export declare function OnSeeked(callback: eventHandler, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the Seeking event.
 **/
export declare function OnSeeking(callback: eventHandler, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the Select event.
 **/
export declare function OnSelect(callback: eventHandler, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the SelectStart event.
 **/
export declare function OnSelectStart(callback: eventHandler, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the SelectionChange event.
 **/
export declare function OnSelectionChange(callback: eventHandler, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the SlotChange event.
 **/
export declare function OnSlotChange(callback: eventHandler, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the Stalled event.
 **/
export declare function OnStalled(callback: eventHandler, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the Storage event.
 **/
export declare function OnStorage(callback: eventHandler, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the Submit event.
 **/
export declare function OnSubmit(callback: eventHandler<SubmitEvent>, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the Suspend event.
 **/
export declare function OnSuspend(callback: eventHandler, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the TimeUpdate event.
 **/
export declare function OnTimeUpdate(callback: eventHandler, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the Toggle event.
 **/
export declare function OnToggle(callback: eventHandler, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the TouchCancel event.
 **/
export declare function OnTouchCancel(callback: eventHandler<TouchEvent>, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the TouchEnd event.
 **/
export declare function OnTouchEnd(callback: eventHandler<TouchEvent>, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the TouchMove event.
 **/
export declare function OnTouchMove(callback: eventHandler<TouchEvent>, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the TouchStart event.
 **/
export declare function OnTouchStart(callback: eventHandler<TouchEvent>, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the TransitionEnd event.
 **/
export declare function OnTransitionEnd(callback: eventHandler<TransitionEvent>, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the Unload event.
 **/
export declare function OnUnload(callback: eventHandler, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the VisibilityChange event.
 **/
export declare function OnVisibilityChange(callback: eventHandler, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the VolumeChange event.
 **/
export declare function OnVolumeChange(callback: eventHandler, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the Waiting event.
 **/
export declare function OnWaiting(callback: eventHandler, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
/**
 * Creates an event handler for the Wheel event.
 **/
export declare function OnWheel(callback: eventHandler<WheelEvent>, opts?: boolean | AddEventListenerOptions): HtmlEvent<string, Event>;
export declare function mediaElementCanPlay(elem: HTMLMediaElement, prog?: IProgress): Promise<boolean>;
export declare function mediaElementCanPlayThrough(elem: HTMLMediaElement, prog?: IProgress): Promise<boolean>;
export {};
//# sourceMappingURL=events.d.ts.map