import { TypedHtmlProp } from "@juniper-lib/dom";

export interface IHasTemplateName extends HTMLElement {
    templateName: string;
}

export function TemplateName(name: string) {
    return TypedHtmlProp<IHasTemplateName>("templateName", name);
}
