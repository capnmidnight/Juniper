import { IProgress } from "@juniper-lib/progress/IProgress";
import { IElementAppliable } from "./tags";
export type EventListenerOpts = boolean | AddEventListenerOptions;
export declare function isModifierless(evt: KeyboardEvent | MouseEvent | PointerEvent): boolean;
export declare function makeEnterKeyEventHandler(callback: (evt: KeyboardEvent) => void): (ev: Event) => void;
export declare function makeProgress(element: HTMLProgressElement): IProgress;
/**
 * A setter functor for HTML element events.
 **/
export declare class HtmlEvt<T extends Event> implements IElementAppliable {
    name: string;
    callback: (evt: T) => void;
    opts?: EventListenerOpts;
    /**
     * Creates a new setter functor for an HTML element event.
     * @param name - the name of the event to attach to.
     * @param callback - the callback function to use with the event handler.
     * @param opts - additional attach options.
     */
    constructor(name: string, callback: (evt: T) => void, opts?: EventListenerOpts);
    applyToElement(elem: HTMLElement): void;
    /**
     * Add the encapsulate callback as an event listener to the give HTMLElement
     */
    add(elem: HTMLElement): void;
    /**
     * Remove the encapsulate callback as an event listener from the give HTMLElement
     */
    remove(elem: HTMLElement): void;
}
export declare function onEvent<T extends Event>(eventName: string, callback: (evt: T) => void, opts?: EventListenerOpts): HtmlEvt<T>;
export declare function onAbort(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onAfterPrint(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onAnimationCancel(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onAnimationEnd(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onAnimationIteration(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onAnimationStart(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onAuxClick(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onBeforeInput(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onBeforePrint(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onBeforeUnload(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onBlur(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onCancel(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onCanPlay(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onCanPlayThrough(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onChange(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onClick(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onClose(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onCompositionEnd(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onCompositionStart(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onCompositionUpdate(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onContextMenu(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onCopy(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onCut(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onDblClick(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onDrag(callback: (evt: DragEvent) => void, opts?: EventListenerOpts): HtmlEvt<DragEvent>;
export declare function onDragEnd(callback: (evt: DragEvent) => void, opts?: EventListenerOpts): HtmlEvt<DragEvent>;
export declare function onDragEnter(callback: (evt: DragEvent) => void, opts?: EventListenerOpts): HtmlEvt<DragEvent>;
export declare function onDragLeave(callback: (evt: DragEvent) => void, opts?: EventListenerOpts): HtmlEvt<DragEvent>;
export declare function onDragOver(callback: (evt: DragEvent) => void, opts?: EventListenerOpts): HtmlEvt<DragEvent>;
export declare function onDragStart(callback: (evt: DragEvent) => void, opts?: EventListenerOpts): HtmlEvt<DragEvent>;
export declare function onDrop(callback: (evt: DragEvent) => void, opts?: EventListenerOpts): HtmlEvt<DragEvent>;
export declare function onDurationChange(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onEmptied(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onEnded(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onError(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onFocus(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onFocusIn(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onFocusOut(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onFullScreenChange(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onFullScreenError(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onGamepadConnected(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onGamepadDisconnected(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onGotPointerCapture(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onHashChange(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onLostPointerCapture(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onInput(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onInvalid(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onKeyDown(callback: (evt: KeyboardEvent) => void, opts?: EventListenerOpts): HtmlEvt<KeyboardEvent>;
export declare function onKeyPress(callback: (evt: KeyboardEvent) => void, opts?: EventListenerOpts): HtmlEvt<KeyboardEvent>;
export declare function onKeyUp(callback: (evt: KeyboardEvent) => void, opts?: EventListenerOpts): HtmlEvt<KeyboardEvent>;
export declare function onEnterKeyPressed(callback: (evt: KeyboardEvent) => void, opts?: EventListenerOpts): HtmlEvt<KeyboardEvent>;
export declare function onLanguageChange(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onLoad(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onLoadedData(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onLoadedMetadata(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onLoadStart(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onMessage(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onMessageError(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onMouseDown(callback: (evt: MouseEvent) => void, opts?: EventListenerOpts): HtmlEvt<MouseEvent>;
export declare function onMouseEnter(callback: (evt: MouseEvent) => void, opts?: EventListenerOpts): HtmlEvt<MouseEvent>;
export declare function onMouseLeave(callback: (evt: MouseEvent) => void, opts?: EventListenerOpts): HtmlEvt<MouseEvent>;
export declare function onMouseMove(callback: (evt: MouseEvent) => void, opts?: EventListenerOpts): HtmlEvt<MouseEvent>;
export declare function onMouseOut(callback: (evt: MouseEvent) => void, opts?: EventListenerOpts): HtmlEvt<MouseEvent>;
export declare function onMouseOver(callback: (evt: MouseEvent) => void, opts?: EventListenerOpts): HtmlEvt<MouseEvent>;
export declare function onMouseUp(callback: (evt: MouseEvent) => void, opts?: EventListenerOpts): HtmlEvt<MouseEvent>;
export declare function onOffline(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onOnline(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onPageHide(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onPageShow(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onPaste(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onPause(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onPointerCancel(callback: (evt: PointerEvent) => void, opts?: EventListenerOpts): HtmlEvt<PointerEvent>;
export declare function onPointerDown(callback: (evt: PointerEvent) => void, opts?: EventListenerOpts): HtmlEvt<PointerEvent>;
export declare function onPointerEnter(callback: (evt: PointerEvent) => void, opts?: EventListenerOpts): HtmlEvt<PointerEvent>;
export declare function onPointerLeave(callback: (evt: PointerEvent) => void, opts?: EventListenerOpts): HtmlEvt<PointerEvent>;
export declare function onPointerLockChange(callback: (evt: PointerEvent) => void, opts?: EventListenerOpts): HtmlEvt<PointerEvent>;
export declare function onPointerLockError(callback: (evt: PointerEvent) => void, opts?: EventListenerOpts): HtmlEvt<PointerEvent>;
export declare function onPointerMove(callback: (evt: PointerEvent) => void, opts?: EventListenerOpts): HtmlEvt<PointerEvent>;
export declare function onPointerRawUpdate(callback: (evt: PointerEvent) => void, opts?: EventListenerOpts): HtmlEvt<PointerEvent>;
export declare function onPointerOut(callback: (evt: PointerEvent) => void, opts?: EventListenerOpts): HtmlEvt<PointerEvent>;
export declare function onPointerOver(callback: (evt: PointerEvent) => void, opts?: EventListenerOpts): HtmlEvt<PointerEvent>;
export declare function onPointerUp(callback: (evt: PointerEvent) => void, opts?: EventListenerOpts): HtmlEvt<PointerEvent>;
export declare function onPlay(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onPlaying(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onPopstate(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onProgress(callback: (evt: ProgressEvent) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onProgressCallback(prog: IProgress): HtmlEvt<Event>;
export declare function onRatechange(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onReadystatechange(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onReleased(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onReset(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onResize(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onScroll(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onSeeked(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onSeeking(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onSelect(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onSelectStart(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onSelectionChange(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onSlotChange(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onStalled(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onStorage(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onSubmit(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onSuspend(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onTimeUpdate(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onToggle(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onTouchCancel(callback: (evt: TouchEvent) => void, opts?: EventListenerOpts): HtmlEvt<TouchEvent>;
export declare function onTouchEnd(callback: (evt: TouchEvent) => void, opts?: EventListenerOpts): HtmlEvt<TouchEvent>;
export declare function onTouchMove(callback: (evt: TouchEvent) => void, opts?: EventListenerOpts): HtmlEvt<TouchEvent>;
export declare function onTouchStart(callback: (evt: TouchEvent) => void, opts?: EventListenerOpts): HtmlEvt<TouchEvent>;
export declare function onTransitionEnd(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onUnload(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onVisibilityChange(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onVolumeChange(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onWaiting(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function onWheel(callback: (evt: Event) => void, opts?: EventListenerOpts): HtmlEvt<Event>;
export declare function applyFakeProgress(file: string, elem: HTMLElement, prog: IProgress): void;
//# sourceMappingURL=evts.d.ts.map