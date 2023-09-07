import { ClassList } from "@juniper-lib/dom/attrs";
import { fr, gridTemplateColumns } from "@juniper-lib/dom/css";
import { Div, isElements } from "@juniper-lib/dom/tags";
import "./style.css";
export class GroupPanel {
    constructor(...rest) {
        const elems = rest.filter(isElements);
        const colExpr = elems.map((_, i) => i === 0 ? fr(1) : "auto");
        this.element = Div(ClassList("group-panel"), gridTemplateColumns(...colExpr), ...rest);
        Object.seal(this);
    }
}
//# sourceMappingURL=index.js.map