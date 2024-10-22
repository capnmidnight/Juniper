import { elementSetDisplay } from "@juniper-lib/dom/dist/tags";
import { Task } from "@juniper-lib/events/dist/Task";


export class MessageBox {
    private task = new Task<boolean>(false);
    private msgBoxConfirm: HTMLButtonElement;
    private msgBoxCancel: HTMLButtonElement;

    constructor(private msgBox: HTMLElement) {
        this.msgBoxConfirm = this.msgBox.querySelector<HTMLButtonElement>("button.confirm");
        this.msgBoxCancel = this.msgBox.querySelector<HTMLButtonElement>("button.cancel");

        this.msgBoxConfirm.addEventListener("click", this.task.resolver(true));
        this.msgBoxCancel.addEventListener("click", this.task.resolver(false));
    }

    async show(): Promise<boolean> {
        this.task.restart();
        elementSetDisplay(this.msgBox, true, "block");
        await this.task;
        elementSetDisplay(this.msgBox, false);
        return this.task.result;
    }
}