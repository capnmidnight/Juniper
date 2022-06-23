import { PointerName } from "@juniper-lib/tslib/events/PointerName";
import type { BaseEnvironment } from "../environment/BaseEnvironment";
import { BaseScreenPointerSinglePoint } from "./BaseScreenPointerSinglePoint";

export class PointerPen extends BaseScreenPointerSinglePoint {
    constructor(env: BaseEnvironment) {
        super("pen", PointerName.Pen, env);
        Object.seal(this);
    }
}
