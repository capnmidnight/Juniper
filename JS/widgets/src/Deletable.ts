import { HtmlProp } from "@juniper-lib/dom";

export interface IDeletable extends Node {
    deletable: boolean;
}

export function Deletable(deletable: boolean) {
    return new HtmlProp("deletable", deletable);
}
