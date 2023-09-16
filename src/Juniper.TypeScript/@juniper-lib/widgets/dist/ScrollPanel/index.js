import { HtmlAttr, ClassList } from "@juniper-lib/dom/dist/attrs";
import { CssElementStyleProp } from "@juniper-lib/dom/dist/css";
import { Div } from "@juniper-lib/dom/dist/tags";
import "./styles.css";
function isRule(obj) {
    return obj instanceof CssElementStyleProp
        || obj instanceof HtmlAttr;
}
function isElem(obj) {
    return !isRule(obj);
}
export class ScrollPanel {
    constructor(...rest) {
        const rules = rest.filter(isRule);
        const elems = rest.filter(isElem);
        this.element = Div(ClassList("scroll-panel"), ...rules, Div(ClassList("scroll-panel-inner"), ...elems));
    }
}
//# sourceMappingURL=index.js.map