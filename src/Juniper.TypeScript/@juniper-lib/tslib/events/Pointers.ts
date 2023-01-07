export type PointerType = "mouse"
    | "touch"
    | "gamepad"
    | "pen"
    | "hand"
    | "nose"
    | "remote";

export enum PointerID {
    LocalUser = 0,
    Mouse,
    Pen,
    Touch,
    MotionController,
    MotionControllerLeft,
    MotionControllerRight,
    Nose,
    RemoteUser
}