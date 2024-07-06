import { Div, renderHTMLElement } from "@juniper-lib/dom";


alert("ALERT!");

const table = document.querySelector<HTMLElement>("#rankings");
document.body.append(Div(
    renderHTMLElement(table)
));