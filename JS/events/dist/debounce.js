import { isNumber } from "@juniper-lib/util";
export function debounce(timeOrAction, action) {
    let time = 0;
    if (isNumber(timeOrAction)) {
        time = timeOrAction;
    }
    else {
        action = timeOrAction;
    }
    let ready = true;
    return (...args) => {
        if (ready) {
            ready = false;
            setTimeout(() => {
                ready = true;
                action(...args);
            }, time);
        }
    };
}
//# sourceMappingURL=debounce.js.map