import { IDisposable } from "@juniper-lib/tslib/using";

export interface IAudioNode extends AudioNode, IDisposable {
    name: string;
}
