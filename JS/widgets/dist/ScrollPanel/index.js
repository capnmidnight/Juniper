import { isDate, isNumber, isString } from "@juniper-lib/util";
import { ClassList, Div, em, height, overflow, padding, rule, SingletonStyleBlob } from "@juniper-lib/dom";
function isElem(obj) {
    return obj instanceof Node
        || isString(obj)
        || isDate(obj)
        || isNumber(obj);
}
function isRule(obj) {
    return !isElem(obj);
}
export function ScrollPanel(...rest) {
    SingletonStyleBlob("Juniper::Widgets::ScrollPanel", () => rule(".scroll-panel", overflow("auto", "auto"), padding(em(.5)), rule(".scroll-panel-inner", height(0))));
    const rules = rest.filter(isRule);
    const elems = rest.filter(isElem);
    return Div(ClassList("scroll-panel"), ...rules, Div(ClassList("scroll-panel-inner"), ...elems));
}
//# sourceMappingURL=index.js.map