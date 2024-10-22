import { ISequenced } from "./ISequenced";
export interface INamed extends ISequenced {
    name: string;
}
export declare function isINamed(obj: unknown): obj is INamed;
export declare const getName: (v: INamed) => string;
//# sourceMappingURL=INamed.d.ts.map