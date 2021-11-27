import { MessageBox } from "./MessageBox";
import { getButton, getElement } from "./tags";
export function makeAction(buttonID, msgBoxID, action) {
    const button = getButton(buttonID);
    const msgBoxElement = getElement(msgBoxID);
    if (button && msgBoxElement) {
        const msgBox = new MessageBox(msgBoxElement);
        if (msgBox) {
            button.addEventListener("click", async () => {
                button.form.action = button.form.action.replace(/handler=\w+/, `handler=${action}`);
                const confirmed = await msgBox.show();
                if (confirmed) {
                    button.form.submit();
                }
            }, true);
        }
    }
}
