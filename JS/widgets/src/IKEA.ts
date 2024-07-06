/**
 * Find all of the elements with the "slot" *attribute* in the current element
 * and replace all of the "slot" *elements* with them.
 * 
 * "INSERT TAB A INTO SLOT B" - IKEA
 * 
 * @param element
 */
export function IKEA(element: Element) {
    const slots = new Map(
        Array.from(element.querySelectorAll("slot"))
            .map(slot => [slot.name, slot])
    );

    for (const tab of element.querySelectorAll("[slot]")) {
        const slot = slots.get(tab.slot);
        if (slot) {
            slot.replaceWith(tab);
        }
    }
}
