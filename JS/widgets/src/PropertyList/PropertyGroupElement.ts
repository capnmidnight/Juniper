import { isNullOrUndefined, singleton } from "@juniper-lib/util";
import { content, display, ElementChild, fontWeight, gridColumn, justifySelf, Name, registerFactory, rule, SingletonStyleBlob, textAlign, whiteSpace } from "@juniper-lib/dom";


export class PropertyGroupElement extends HTMLElement {

    constructor() {
        super();

        SingletonStyleBlob("Juniper::Widgets::PropertyGroupElement", () => [
            rule("property-list>property-group",
                display("contents"),

                rule(">*",
                    gridColumn(1, -1),
                    justifySelf("center")
                ),

                rule(">label, >property-editor>label",
                    gridColumn(1, 2),
                    justifySelf("end"),
                    whiteSpace("nowrap"),
                    fontWeight("bold"),

                    rule("::after",
                        content("':'")
                    )
                ),


                rule(">label, >label>input, >property-editor>label, >property-editor>label>input",
                    textAlign("end")
                ),

                rule(">label + *:not(label), >property-editor>label + *:not(label)",
                    gridColumn(2, 3),
                    justifySelf("start")
                )
            )
        ]);
    }

    get name() { return this.getAttribute("name"); }
    set name(v) {
        if (isNullOrUndefined(v)) {
            this.removeAttribute("name");
        }
        else {
            this.setAttribute("name", v);
        }
    }

    static install() {
        return singleton("Juniper::Widgets::PropertyGroupElement", () => registerFactory("property-group", PropertyGroupElement));
    }
}

export function PropertyGroup(name: string, ...rest: ElementChild<PropertyGroupElement>[]) {
    return PropertyGroupElement.install()(
        Name(name),
        ...rest
    );
}