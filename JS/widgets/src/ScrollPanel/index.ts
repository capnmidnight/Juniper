import { isDate, isNumber, isString } from "@juniper-lib/util";
import { ClassList, Div, ElementChild, em, height, overflow, padding, RawElementChild, rule, SingletonStyleBlob } from "@juniper-lib/dom";

function isElem(obj: ElementChild): obj is RawElementChild {
    return obj instanceof Node
        || isString(obj)
        || isDate(obj)
        || isNumber(obj);
}

function isRule(obj: ElementChild): obj is Exclude<ElementChild, RawElementChild> {
    return !isElem(obj);
}


export function ScrollPanel(...rest: ElementChild[]) {

    SingletonStyleBlob("Juniper::Widgets::ScrollPanel", () => rule(".scroll-panel",
        overflow("auto", "auto"),
        padding(em(.5)),

        rule(".scroll-panel-inner",
            height(0)
        )
    ));

    const rules = rest.filter(isRule);
    const elems = rest.filter(isElem);

    return Div(
        ClassList("scroll-panel"),
        ...rules,
        Div(
            ClassList("scroll-panel-inner"),
            ...elems
        )
    );
}
