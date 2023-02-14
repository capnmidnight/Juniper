export type PointerType = "mouse"
    | "pen"
    | "touch"
    | "gamepad"
    | "hand"
    | "nose"
    | "remote";

export enum PointerID {
    LocalUser = 0,
    Mouse,
    Pen,
    Touch,
    Gamepad,
    MotionController,
    MotionControllerLeft,
    MotionControllerRight,
    Nose,
    RemoteUser
}

export function getPointerType(id: PointerID): PointerType {
    switch (id) {
        case PointerID.Mouse: return "mouse";
        case PointerID.Pen: return "pen";
        case PointerID.Touch: return "touch";
        case PointerID.Gamepad: return "gamepad";
        case PointerID.MotionController:
        case PointerID.MotionControllerLeft:
        case PointerID.MotionControllerRight: return "hand";
        case PointerID.Nose: return "nose";
        case PointerID.RemoteUser: return "remote";
        default: return null;
    }
}