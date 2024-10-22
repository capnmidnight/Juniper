import { HtmlProp } from "@juniper-lib/dom";
export interface IDeletable extends Node {
    deletable: boolean;
}
export declare function Deletable(deletable: boolean): HtmlProp<"deletable", boolean, Node & Record<"deletable", boolean>>;
//# sourceMappingURL=Deletable.d.ts.map