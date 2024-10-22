export var PointerID;
(function (PointerID) {
    PointerID[PointerID["LocalUser"] = 0] = "LocalUser";
    PointerID[PointerID["Mouse"] = 1] = "Mouse";
    PointerID[PointerID["Pen"] = 2] = "Pen";
    PointerID[PointerID["Touch"] = 3] = "Touch";
    PointerID[PointerID["Gamepad"] = 4] = "Gamepad";
    PointerID[PointerID["MotionController"] = 5] = "MotionController";
    PointerID[PointerID["MotionControllerLeft"] = 6] = "MotionControllerLeft";
    PointerID[PointerID["MotionControllerRight"] = 7] = "MotionControllerRight";
    PointerID[PointerID["Nose"] = 8] = "Nose";
    PointerID[PointerID["RemoteUser"] = 9] = "RemoteUser";
})(PointerID || (PointerID = {}));
export function getPointerType(id) {
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
//# sourceMappingURL=Pointers.js.map