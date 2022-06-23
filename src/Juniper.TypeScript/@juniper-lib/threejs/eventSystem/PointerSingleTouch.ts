import { PointerName } from "@juniper-lib/tslib/events/PointerName";
import type { BaseEnvironment } from "../environment/BaseEnvironment";
import { BaseScreenPointerSinglePoint } from "./BaseScreenPointerSinglePoint";

export class PointerSingleTouch extends BaseScreenPointerSinglePoint {
    constructor(env: BaseEnvironment) {
        super("touch", PointerName.Touch0, env);
        Object.seal(this);
    }
}
