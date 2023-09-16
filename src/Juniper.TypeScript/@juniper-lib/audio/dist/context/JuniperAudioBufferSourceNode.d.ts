import { TypedEvent } from "@juniper-lib/events/dist/TypedEventTarget";
import { IAudioParam } from "../IAudioNode";
import type { JuniperAudioContext } from "./JuniperAudioContext";
import { JuniperAudioNode } from "./JuniperAudioNode";
export declare class JuniperAudioBufferSourceNode extends JuniperAudioNode<AudioBufferSourceNode, {
    ended: TypedEvent<"ended">;
}> implements AudioBufferSourceNode {
    readonly playbackRate: IAudioParam;
    readonly detune: IAudioParam;
    constructor(context: JuniperAudioContext, options?: AudioBufferSourceOptions);
    get buffer(): AudioBuffer;
    set buffer(v: AudioBuffer);
    get loop(): boolean;
    set loop(v: boolean);
    get loopEnd(): number;
    set loopEnd(v: number);
    get loopStart(): number;
    set loopStart(v: number);
    get onended(): (this: AudioScheduledSourceNode, ev: Event) => any;
    set onended(v: (this: AudioScheduledSourceNode, ev: Event) => any);
    start(when?: number, offset?: number, duration?: number): void;
    stop(when?: number): void;
}
//# sourceMappingURL=JuniperAudioBufferSourceNode.d.ts.map