import { groupBy } from "@juniper-lib/util";
/**
 * Find all of the elements with the "slot" *attribute* in the current element
 * and replace all of the "slot" *elements* with them.
 *
 * "INSERT TAB A INTO SLOT B" - IKEA
 *
 * @param element
 */
export function IKEA(element) {
    const slots = new Map(Array.from(element.querySelectorAll("slot"))
        .map(slot => [slot.name, slot]));
    const tabs = Array.from(element.querySelectorAll("[slot]"));
    const tabGroups = groupBy(tabs, tab => tab.slot);
    for (const [tabName, tabs] of tabGroups) {
        const slot = slots.get(tabName);
        if (slot) {
            slot.replaceChildren(...tabs);
        }
    }
}
//# sourceMappingURL=IKEA.js.map