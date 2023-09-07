const USER_GESTURE_EVENTS = [
    "change",
    "click",
    "contextmenu",
    "dblclick",
    "mouseup",
    "pointerup",
    "reset",
    "submit",
    "touchend"
];
/**
 * This is not an event handler that you can add to an element. It's a global event that
 * waits for the user to perform some sort of interaction with the website.
  */
export function onUserGesture(callback, perpetual = false) {
    const check = async (evt) => {
        if (evt.isTrusted) {
            if (!perpetual) {
                for (const gesture of USER_GESTURE_EVENTS) {
                    window.removeEventListener(gesture, check);
                }
            }
            callback();
        }
    };
    for (const gesture of USER_GESTURE_EVENTS) {
        window.addEventListener(gesture, check);
    }
}
//# sourceMappingURL=onUserGesture.js.map