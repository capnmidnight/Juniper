import { PointerID } from "../Pointers";
import { BaseScreenPointerSinglePoint } from "./BaseScreenPointerSinglePoint";
export class PointerPen extends BaseScreenPointerSinglePoint {
    constructor(env) {
        super("pen", PointerID.Pen, env);
        Object.seal(this);
    }
    vibrate() {
        // do nothing
    }
}
//# sourceMappingURL=PointerPen.js.map