import { isDefined, isNullOrUndefined } from "./typeChecks";

export function singleton<T>(name: string, create?: () => T extends void ? never : T): T {
    const box = globalThis as any;
    let value = box[name] as T;
    if (isNullOrUndefined(value)
        && isDefined(create)) {
        value = create();
        box[name] = value;
    }
    return value;
}