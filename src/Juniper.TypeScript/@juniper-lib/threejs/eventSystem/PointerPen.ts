import { PointerID } from "@juniper-lib/tslib";
import type { BaseEnvironment } from "../environment/BaseEnvironment";
import { BaseScreenPointerSinglePoint } from "./BaseScreenPointerSinglePoint";

export class PointerPen extends BaseScreenPointerSinglePoint {
    constructor(env: BaseEnvironment) {
        super("pen", PointerID.Pen, env);
        Object.seal(this);
    }

    vibrate() {
        // do nothing
    }
}
