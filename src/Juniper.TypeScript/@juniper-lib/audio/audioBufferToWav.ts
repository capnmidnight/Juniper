import { Audio_Wav } from "@juniper-lib/mediatypes";
import { Exception } from "@juniper-lib/tslib/Exception";
import { audioBufferGetData } from "./audioBufferGetData";

export function audioBufferToWav(buffer: AudioBuffer, useFloat32 = false) {

    const format = useFloat32
        ? 3
        : 1;
    const bitDepth = format === 3
        ? 32
        : 16;

    if (buffer.numberOfChannels > 2) {
        throw new Exception("Wav files only support a max of 2 channels");
    }

    const result = audioBufferGetData(buffer);

    return encodeWAV(result, format, buffer.sampleRate, buffer.numberOfChannels, bitDepth);
}

function encodeWAV(samples: Float32Array, format: number, sampleRate: number, numChannels: number, bitDepth: number) {
    var bytesPerSample = bitDepth / 8;
    var blockAlign = numChannels * bytesPerSample;

    var buffer = new ArrayBuffer(44 + samples.length * bytesPerSample);
    var view = new DataView(buffer);

    writeString(view, 0, "RIFF"); // RIFF identifier
    view.setUint32(4, 36 + samples.length * bytesPerSample, true); // RIFF chunk length
    writeString(view, 8, "WAVE"); // RIFF type
    writeString(view, 12, "fmt "); // format chunk identifier
    view.setUint32(16, 16, true); // format chunk length
    view.setUint16(20, format, true); // sample format (raw)
    view.setUint16(22, numChannels, true); // channel count
    view.setUint32(24, sampleRate, true); // sample rate
    view.setUint32(28, sampleRate * blockAlign, true); // byte rate (sample rate * block align)
    view.setUint16(32, blockAlign, true); // block align (channel count * bytes per sample)
    view.setUint16(34, bitDepth, true); // bits per sample
    writeString(view, 36, "data"); // data chunk identifier
    view.setUint32(40, samples.length * bytesPerSample, true); // data chunk length


    // Raw PCM
    if (format === 1) {
        floatTo16BitPCM(view, 44, samples);
    } else {
        writeFloat32(view, 44, samples);
    }

    return new Blob([buffer], { type: Audio_Wav.value });
}

function writeFloat32(output: DataView, offset: number, input: Float32Array) {
    for (var i = 0; i < input.length; i++, offset += 4) {
        output.setFloat32(offset, input[i], true);
    }
}

function floatTo16BitPCM(output: DataView, offset: number, input: Float32Array) {
    for (var i = 0; i < input.length; i++, offset += 2) {
        var s = Math.max(-1, Math.min(1, input[i]));
        output.setInt16(offset, s < 0 ? s * 0x8000 : s * 0x7FFF, true);
    }
}

function writeString(view: DataView, offset: number, string: string) {
    for (var i = 0; i < string.length; i++) {
        view.setUint8(offset + i, string.charCodeAt(i));
    }
}
