import { ClassList, display, Div, fr, gridTemplateColumns, rule, SingletonStyleBlob } from "@juniper-lib/dom";
export function GroupPanel(...rest) {
    SingletonStyleBlob("Juniper::Widgets::GroupPanel", () => rule(".group-panel", display("grid")));
    const elems = rest.filter(e => e instanceof Element);
    const colExpr = elems.map((_, i) => i === 0 ? fr(1) : "auto");
    const element = Div(ClassList("group-panel"), gridTemplateColumns(...colExpr), ...rest);
    return element;
}
//# sourceMappingURL=index.js.map