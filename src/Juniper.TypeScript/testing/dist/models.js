import { isDefined } from "juniper-tslib";
export const KEY = "XXX_QUAKE_LOGGER_XXX";
export function isWorkerLoggerMessageData(data) {
    return isDefined(data)
        && "key" in data
        && data.key === KEY;
}
