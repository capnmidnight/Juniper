export type SourcePointerEventTypes =
    | "down"
    | "move"
    | "up"
    | "click"
    | "drag"
    | "dragcancel"
    | "dragend"
    | "dragstart";

export type PointerEventTypes =
    | SourcePointerEventTypes
    | "enter"
    | "exit";
