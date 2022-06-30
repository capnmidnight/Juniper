export type PointerType = "mouse"
    | "touch"
    | "gamepad"
    | "pen"
    | "hand"
    | "remote";

export enum PointerID {
    LocalUser = 0,
    Mouse,
    Pen,
    Touch,
    Touches,
    MotionController,
    MotionControllerLeft,
    MotionControllerRight,
    RemoteUser
}