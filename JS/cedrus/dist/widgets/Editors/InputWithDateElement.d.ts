import { ElementChild } from "@juniper-lib/dom";
import { TimeSeries } from "../../models";
export declare class InputWithDateElement<T> extends HTMLElement {
    #private;
    constructor();
    connectedCallback(): void;
    get item(): TimeSeries<T>;
    set item(v: TimeSeries<T>);
    static install(): import("@juniper-lib/dom").ElementFactory<InputWithDateElement<unknown>>;
}
export declare function InputWithDate<T>(...rest: ElementChild[]): InputWithDateElement<T>;
//# sourceMappingURL=InputWithDateElement.d.ts.map