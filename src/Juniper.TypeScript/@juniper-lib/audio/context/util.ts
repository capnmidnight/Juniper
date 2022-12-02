import { Exception } from "@juniper-lib/tslib/Exception";
import { isArray } from "@juniper-lib/tslib/typeChecks";
import { IAudioNode } from "./IAudioNode";

export type AudioConnectionWithOutputChannel = [AudioNode | AudioParam, number];
export type AudioConnectionWithInputOutputChannels = [AudioNode, number, number];

export type AudioConnection =
    | AudioNode
    | AudioParam
    | AudioConnectionWithOutputChannel
    | AudioConnectionWithInputOutputChannels;


export function init<T extends IAudioNode>(name: string, node: T): T {
    node.name = name;
    return node;
}

export function fan<T extends IAudioNode>(node: T, ...rest: AudioConnection[]): T {
    for (const right of rest) {
        let dest: AudioNode | AudioParam;
        let output: number = undefined;
        let input: number = undefined;
        if (isArray(right)) {
            [dest, output, input] = right;
        }
        else {
            dest = right;
        }

        node.connect(dest as any, output, input);
    }
    return node;
}

export function chain<T extends IAudioNode>(node: T, ...rest: AudioConnection[]): T {
    for (let i = 0; i < rest.length - 1; ++i) {
        const right = rest[i];
        const dest = isArray(right)
            ? right[0]
            : right;

        if (dest instanceof AudioParam) {
            throw new Exception("Audio graph chains cannot have an AudioParam in the middle of them. If an AudioParam is included, it must come at the end.")
        }
    }

    let here = node;
    for (let i = 0; i < rest.length; ++i) {
        const right = rest[i];
        let dest: AudioNode | AudioParam;
        let output: number = undefined;
        let input: number = undefined;
        if (isArray(right)) {
            [dest, output, input] = right;
        }
        else {
            dest = right;
        }

        here.connect(dest as any, output, input);
    }

    return node;
}