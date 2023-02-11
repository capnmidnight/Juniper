import { interleave } from "./interleave";


export function audioBufferGetData(buffer: AudioBuffer): Float32Array {
    return buffer.numberOfChannels === 2
        ? interleave(buffer.getChannelData(0), buffer.getChannelData(1))
        : buffer.getChannelData(0);
}
