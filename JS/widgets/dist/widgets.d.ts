import { ElementChild } from "@juniper-lib/dom";
interface input {
    countryCode: HTMLSelectElement;
    areaCode: HTMLInputElement;
    exchange: HTMLInputElement;
    extension: HTMLInputElement;
}
export declare function populatePhoneNumber(inputs: input, phoneNumber: string[]): void;
export declare function readPhoneNumber(inputs: input): string;
export declare function addScrollButton(parent?: Window | HTMLElement): void;
export declare function ButtonDark(title: string, icon: string, ...rest: ElementChild[]): HTMLButtonElement;
export declare function ButtonPrimary(title: string, text: string, ...rest: ElementChild[]): HTMLButtonElement;
export declare function ButtonSecondary(title: string, text: string, ...rest: ElementChild[]): HTMLButtonElement;
export declare function ButtonDanger(title: string, text: string, ...rest: ElementChild[]): HTMLButtonElement;
export declare function buildCards<T>(dataName: string, warningClassName: string, container: HTMLElement, template: HTMLTemplateElement, data: T[], makeCard: (template: DocumentFragment, item: T) => DocumentFragment): void;
export declare function validateForm(form: HTMLFormElement): boolean;
export declare function resetValidation(form: HTMLFormElement): void;
export declare function SelectPlaceholder(title: string, selectable?: boolean): HTMLOptionElement;
export declare function buildDropdown(select: HTMLSelectElement, values: readonly string[], placeholder: string): void;
export {};
//# sourceMappingURL=widgets.d.ts.map