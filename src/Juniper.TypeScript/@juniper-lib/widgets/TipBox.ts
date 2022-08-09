import { className, id, open } from "@juniper-lib/dom/attrs";
import { onToggle } from "@juniper-lib/dom/evts";
import { Details, ErsatzElement, LI, Summary, UL } from "@juniper-lib/dom/tags";

export class TipBox implements ErsatzElement {

    readonly element: HTMLDetailsElement;

    constructor(tipBoxID: string, ...tips: string[]) {
        const storageKey = `Juniper:Widgets:TipBox:${tipBoxID}`;

        this.element = Details(
            id(tipBoxID),
            className("tip"),
            Summary("Tips:"),
            open(localStorage.getItem(storageKey) !== "closed"),
            UL(
                ...tips.map(tip => LI(tip)),
            ),
            onToggle(() =>
                localStorage.setItem(
                    storageKey,
                    this.element.open ? "open" : "closed"))
        );
    }
}