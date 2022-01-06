import { isNullOrUndefined } from "./typeChecks";

export function singleton<T>(name: string, create: () => T): T {
    const box = globalThis as any;
    let value = box[name] as T;
    if (isNullOrUndefined(value)) {
        value = create();
        box[name] = value;
    }
    return value;
}