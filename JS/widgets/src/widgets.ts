import { formatPhoneNumber, isBoolean, isFunction, isObject } from "@juniper-lib/util";
import {
    Button,
    ClassList,
    CustomData,
    Disabled,
    Div,
    ElementChild,
    FAIcon,
    Hidden,
    HtmlRender,
    ID,
    OnClick,
    Option,
    Role,
    Selected,
    TitleAttr,
    Value,
    scrollWindowToTop
} from "@juniper-lib/dom";

interface input {
    countryCode: HTMLSelectElement, areaCode: HTMLInputElement, exchange: HTMLInputElement, extension: HTMLInputElement
}

export function populatePhoneNumber(
    inputs: input,
    phoneNumber: string[]
): void {
    if (!phoneNumber) {
        inputs.countryCode.value = "";
        inputs.areaCode.value = "";
        inputs.exchange.value = "";
        inputs.extension.value = "";
    } else {
        if (phoneNumber[0]) {
            inputs.countryCode.value = phoneNumber[0];
        }
        inputs.areaCode.value = phoneNumber[1];
        inputs.exchange.value = phoneNumber[2];
        inputs.extension.value = phoneNumber[3];
    }
}

export function readPhoneNumber(inputs: input): string {
    return formatPhoneNumber(
        inputs.countryCode.value,
        inputs.areaCode.value,
        inputs.exchange.value,
        inputs.extension.value
    );
}

/* Setup a button for scrolling back to the top of the screen. */
export function addScrollButton(parent: Window | HTMLElement = window): void {
    const scrollButton: HTMLButtonElement = ButtonDark(
        "Go to top",
        "arrow-up",
        ID("scrollButton"),
        OnClick(() => parent.scrollTo({
            top: 0,
            behavior: "smooth"
        })));

    const target: HTMLElement | Window = parent instanceof Window ? document.documentElement : parent;
    parent.addEventListener("scroll", () => {
        scrollButton.style.display =
            target.scrollTop > 20
                ? "block"
                : "none";
    });

    document.body.append(scrollButton);
}


export function ButtonDark(title: string, icon: string, ...rest: ElementChild[]): HTMLButtonElement {
    return Button(
        ClassList("btn", "btn-dark"),
        TitleAttr(title),
        CustomData("tooltip", "tooltip"),
        FAIcon(icon),
        ...rest
    );
}

export function ButtonPrimary(title: string, text: string, ...rest: ElementChild[]): HTMLButtonElement {
    return Button(
        ClassList("btn", "btn-primary"),
        TitleAttr(title),
        CustomData("tooltip", "tooltip"),
        text,
        ...rest
    );
}

export function ButtonSecondary(title: string, text: string, ...rest: ElementChild[]): HTMLButtonElement {
    return Button(
        ClassList("btn", "btn-secondary"),
        TitleAttr(title),
        CustomData("tooltip", "tooltip"),
        text,
        ...rest
    );
}

export function ButtonDanger(title: string, text: string, ...rest: ElementChild[]): HTMLButtonElement {
    return Button(
        ClassList("btn", "btn-danger"),
        TitleAttr(title),
        CustomData("tooltip", "tooltip"),
        text,
        ...rest
    );
}

export function buildCards<T>(dataName: string, warningClassName: string, container: HTMLElement, template: HTMLTemplateElement, data: T[], makeCard: (template: DocumentFragment, item: T) => DocumentFragment): void {
    container.innerHTML = "";
    if (data.length === 0) {
        HtmlRender(container, Div(
            ClassList(
                "alert",
                "alert-warning",
                "d-flex",
                "align-items-center",
                warningClassName
            ),
            Role("alert"),
            FAIcon("triangle-exclamation"),
            Div(`There are no ${dataName} to display based on your filters`)
        ));
    }
    else {
        HtmlRender(container,
            ...data
                .map(item =>
                    makeCard(template.content.cloneNode(true) as DocumentFragment, item)
                )
                .flatMap(frag => Array.from(frag.children))
        );
    }

    scrollWindowToTop();
}

export function validateForm(form: HTMLFormElement): boolean {
    resetValidation(form);

    let valid = true;

    for (const input of form) {
        if (isObject(input)
            && /INPUT|SELECT|TEXTAREA/.test(input.tagName)
            && "form" in input
            && input.form instanceof HTMLFormElement
            && input.form === form
            && "required" in input
            && isBoolean(input.required)
            && input.required
            && "reportValidity" in input
            && isFunction(input.reportValidity)
            && !input.reportValidity()) {
            input.classList.add("invalid");
            valid = false;
        }
    }

    if (valid) {
        form.classList.add("was-validated");
    }

    return valid;
}

export function resetValidation(form: HTMLFormElement): void {
    form.classList.remove("was-validated");
    for (const input of form) {
        if (isObject(input)
            && "form" in input
            && input.form instanceof HTMLFormElement
            && input.form === form
            && "required" in input
            && isBoolean(input.required)
            && input.required) {
            input.classList.remove("invalid");
        }
    }
}

/* Creates a placeholder Option for a select box */
export function SelectPlaceholder(title: string, selectable?: boolean): HTMLOptionElement {
    return Option(
        Selected(true),
        Hidden(!selectable),
        Disabled(!selectable),
        Value(""),
        title
    );
}

export function buildDropdown(select: HTMLSelectElement, values: readonly string[], placeholder: string): void {
    select.innerHTML = "";

    if (placeholder) {
        select.append(SelectPlaceholder(placeholder));
    }

    HtmlRender(
        select,
        ...values.map((v) =>
            Option(
                Value(v),
                Selected(values.length === 1),
                v
            )
        )
    );
}
