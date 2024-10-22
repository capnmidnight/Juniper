export type PointerType = "mouse" | "pen" | "touch" | "gamepad" | "hand" | "nose" | "remote";
export declare enum PointerID {
    LocalUser = 0,
    Mouse = 1,
    Pen = 2,
    Touch = 3,
    Gamepad = 4,
    MotionController = 5,
    MotionControllerLeft = 6,
    MotionControllerRight = 7,
    Nose = 8,
    RemoteUser = 9
}
export declare function getPointerType(id: PointerID): PointerType;
//# sourceMappingURL=Pointers.d.ts.map