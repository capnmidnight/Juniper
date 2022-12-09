import { isDefined } from "@juniper-lib/tslib/typeChecks";
import { IDisposable } from "@juniper-lib/tslib/using";
import type { JuniperAudioContext } from "./JuniperAudioContext";
import { AudioConnection, InputResolution, OutputResolution } from "./JuniperBaseNode";

export interface IAudioEndPoint extends IDisposable {
    name: string;
    readonly nodeType: string;
    _resolveInput(input?: number): InputResolution;
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

    _resolveOutput(output?: number): OutputResolution;
}

export interface IAudioParam extends AudioParam, IAudioEndPoint {
}

export function isEndpoint(obj: any): obj is IAudioEndPoint {
    return isDefined(obj)
        && "nodeType" in obj
        && "name" in obj;
}

export function isIAudioNode(obj: any): obj is IAudioNode {
    return isEndpoint(obj)
        && "connections" in obj;
}