import { isNumber } from "../typeChecks";

export function debounce<T extends (...args: any[]) => any>(action: T): (...args: Parameters<T>) => void
export function debounce<T extends (...args: any[]) => any>(time: number, action: T): (...args: Parameters<T>) => void;
export function debounce<T extends (...args: any[]) => any>(timeOrAction: number | T, action?: T): (...args: Parameters<T>) => void {
    let time = 0;
    if (isNumber(timeOrAction)) {
        time = timeOrAction;
    }
    else {
        action = timeOrAction;
    }

    let ready = true;
    return (...args: Parameters<T>) => {
        if (ready) {
            ready = false;
            setTimeout(() => {
                ready = true;
                action(...args);
            }, time);
        }
    }
}

