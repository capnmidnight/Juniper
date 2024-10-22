import { IDisposable, isDefined } from "@juniper-lib/util";
import type { InputResolution, JuniperAudioContext, OutputResolution } from "./context/JuniperAudioContext";

export interface IAudioEndPoint extends IDisposable {
    name: string;
    readonly nodeType: string;
    resolveInput(input?: number): InputResolution;
    _resolveInput(input?: number): InputResolution;
}

export interface IAudioNode extends AudioNode, IAudioEndPoint {
    readonly context: JuniperAudioContext;

    isConnected(dest?: IAudioNode | IAudioParam, output?: number, input?: number): boolean;

    connect(destinationParam: IAudioParam, output?: number): void;
    connect(destinationNode: IAudioNode, output?: number, input?: number): IAudioNode;

    disconnect(): void;
    disconnect(output: number): void;
    disconnect(destinationParam: IAudioParam, output?: number): void;
    disconnect(destinationNode: IAudioNode, output?: number, input?: number): void;

    resolveOutput(output?: number): OutputResolution;
    _resolveOutput(output?: number): OutputResolution;
}

export interface IAudioParam extends AudioParam, IAudioEndPoint {
}

export function isEndpoint(obj: any): obj is IAudioEndPoint {
    return isDefined(obj)
        && "_resolveInput" in obj;
}

export function isIAudioNode(obj: any): obj is IAudioNode {
    return isEndpoint(obj)
        && "_resolveOutput" in obj;
}