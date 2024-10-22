export type SourcePointerEventTypes =
    | "down"
    | "move"
    | "up"
    | "click";

export type PointerEventTypes =
    | SourcePointerEventTypes
    | "enter"
    | "exit";
