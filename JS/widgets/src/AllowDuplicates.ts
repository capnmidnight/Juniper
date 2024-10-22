import { HtmlProp } from "@juniper-lib/dom";


export function AllowDuplicates(allow: boolean) {
    return new HtmlProp("allowDuplicates", allow);
}
