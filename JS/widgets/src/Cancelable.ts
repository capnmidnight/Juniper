import { HtmlProp } from "@juniper-lib/dom";

export interface ICancelable {
    cancelable: boolean;
}

export function Cancelable(cancelable: boolean) {
    return new HtmlProp("cancelable", cancelable);
}
