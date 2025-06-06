import { isDefined } from "@juniper-lib/util";
export const KEY = /*@__PURE__*/ "XXX_QUAKE_LOGGER_XXX";
export function isWorkerLoggerMessageData(data) {
    return isDefined(data)
        && "key" in data
        && data.key === KEY;
}
//# sourceMappingURL=models.js.map