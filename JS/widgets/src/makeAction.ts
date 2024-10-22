import { Button, ID } from "@juniper-lib/dom";
import { MessageBox } from "./MessageBox";

export function makeAction(buttonID: string, msgBoxID: string, action: string): void {
    const button = Button(ID(buttonID));
    const msgBoxElement = document.getElementById(msgBoxID);
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
