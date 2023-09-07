import { elementSetDisplay } from "@juniper-lib/dom/tags";
import { Task } from "@juniper-lib/events/Task";
export class MessageBox {
    constructor(msgBox) {
        this.msgBox = msgBox;
        this.task = new Task(false);
        this.msgBoxConfirm = this.msgBox.querySelector("button.confirm");
        this.msgBoxCancel = this.msgBox.querySelector("button.cancel");
        this.msgBoxConfirm.addEventListener("click", this.task.resolver(true));
        this.msgBoxCancel.addEventListener("click", this.task.resolver(false));
    }
    async show() {
        this.task.restart();
        elementSetDisplay(this.msgBox, true, "block");
        await this.task;
        elementSetDisplay(this.msgBox, false);
        return this.task.result;
    }
}
//# sourceMappingURL=MessageBox.js.map