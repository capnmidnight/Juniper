import { isFunction, isObject } from "@juniper-lib/util";
import { Query } from "@juniper-lib/dom";
import { StandardDialogElement } from "./StandardDialogElement";
import { resetValidation } from "./widgets";


export class FormDialog<T, V = T> {

    #modal;
    #invalidValue;

    constructor(modal: StandardDialogElement, invalidValue: V = null) {
        this.#modal = modal;
        this.#invalidValue = invalidValue;
    }

    get body() { return this.#modal.body; }
    get form() { return this.#modal.form; }

    get title() { return this.#modal.title; }
    set title(v) { this.#modal.title = v; }

    get saveButtonText() { return this.#modal.saveButtonText; }
    set saveButtonText(v) { this.#modal.saveButtonText = v; }

    findElement(pattern: string) {
        return Query(this.#modal, pattern);
    }

    onShown(_record?: T): any | Promise<any> {
        throw new Error("Not implemented");
    }


    onClosing(): any | Promise<any> {
        return Promise.resolve();
    }

    onValidate(_record?: T) {
        return Promise.resolve(true);
    }

    onValid(_record?: T): V | Promise<V> {
        throw new Error("Not implemented");
    }

    cancel() {
        this.#modal.cancel();
    }

    get isOpen() {
        return this.#modal.isOpen;
    }

    async show(...args: any[]): Promise<V> {
        this.#modal.show();
        try {
            resetValidation(this.form);

            await this.onShown(...args);

            while (this.#modal.isOpen) {
                if (await this.#modal.cancelled()) {
                    break;
                }

                try {
                    if (await this.#validate(...args)) {
                        const value = await this.onValid(...args);
                        return value;
                    }
                }
                catch (err) {
                    console.error(err);
                    if (isObject(err)) {
                        if ("message" in err) {
                            this.errorMessage = err.message as string;
                        }
                        else if ("error" in err) {
                            this.errorMessage = err.error as string;
                        }
                        else if ("target" in err 
                            && isObject(err.target)
                            && "error" in err.target) {
                            this.errorMessage = err.target.error as string;
                        }
                        else if("toString" in err
                            && isFunction(err.toString)){
                            this.errorMessage = err.toString();
                        }
                    }
                    else {
                        this.errorMessage = "Unknown error";
                    }
                }
            }
        }
        finally {
            await this.onClosing();
            this.#modal.close();
        }

        return this.#invalidValue;
    }

    async #validate(record?: T) {
        return await this.onValidate(record)
            && this.#modal.validateForm();
    }

    get errorMessage() {
        return this.#modal.errorMessage;
    }

    set errorMessage(v) {
        this.#modal.errorMessage = v;
    }
}
