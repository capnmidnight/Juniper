import { IDisposable } from "@juniper-lib/tslib/using";
import type { JuniperAudioContext } from "./JuniperAudioContext";
import { AudioConnection } from "./JuniperBaseNode";

export interface IAudioEndPoint extends IDisposable {
    name: string;
    readonly nodeType: string;
}

export interface IAudioNode extends AudioNode, IAudioEndPoint {
    readonly context: JuniperAudioContext;
    readonly connected: boolean;
    readonly connections: ReadonlySet<AudioConnection>;

    connect(destinationParam: IAudioParam, output?: number): void;
    connect(destinationNode: IAudioNode, output?: number, input?: number): IAudioNode;

    disconnect(): void;
    disconnect(output: number): void;
    disconnect(destinationParam: IAudioParam, output?: number): void;
    disconnect(destinationNode: IAudioNode, output?: number, input?: number): void;
}

export interface IAudioParam extends AudioParam, IAudioEndPoint {
}