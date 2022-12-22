const gestures = [
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
export function onUserGesture(callback: () => any): void {
    const check = async (evt: Event) => {
        if (evt.isTrusted) {
            for (const gesture of gestures) {
                window.removeEventListener(gesture, check);
            }

            callback();
        }
    };

    for (const gesture of gestures) {
        window.addEventListener(gesture, check);
    }
}