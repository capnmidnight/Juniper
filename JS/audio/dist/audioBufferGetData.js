import { interleave } from "./interleave";
export function audioBufferGetData(buffer) {
    return buffer.numberOfChannels === 2
        ? interleave(buffer.getChannelData(0), buffer.getChannelData(1))
        : buffer.getChannelData(0);
}
//# sourceMappingURL=audioBufferGetData.js.map