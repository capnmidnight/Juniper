import type { JuniperAudioContext } from "../context/JuniperAudioContext";
import { IAudioNode } from "../IAudioNode";
export type EffectFactory = (name: string, context: JuniperAudioContext) => IAudioNode;
export declare const effectStore: Map<string, EffectFactory>;
//# sourceMappingURL=index.d.ts.map