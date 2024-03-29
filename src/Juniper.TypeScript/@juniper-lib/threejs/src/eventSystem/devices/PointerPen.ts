import type { BaseEnvironment } from "../../environment/BaseEnvironment";
import { PointerID } from "../Pointers";
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
