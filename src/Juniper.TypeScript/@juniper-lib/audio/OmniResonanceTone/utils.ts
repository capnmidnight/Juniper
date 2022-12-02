/**
 * @license
 * Copyright 2017 Google Inc. All Rights Reserved.
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

/**
 * @file A set of defaults, constants and utility functions.
 * @author Andrew Allen <bitllama@google.com>
 * 
 * Conversion to TypeScript, modernization, and miscellaneous defect fixes by:
 * @author Sean T. McBeth <sean.mcbeth+gh@gmail.com>
 */

import { ReadonlyVec3, vec3 } from "gl-matrix";


/**
 * Default input gain (linear).
 */
export const DEFAULT_SOURCE_GAIN = 1;


/**
 * Maximum outside-the-room distance to attenuate far-field listener by.
 */
export const LISTENER_MAX_OUTSIDE_ROOM_DISTANCE = 1;


/**
 * Maximum outside-the-room distance to attenuate far-field sources by.
 */
export const SOURCE_MAX_OUTSIDE_ROOM_DISTANCE = 1;


/**
 * Default distance from listener when setting angle.
 */
export const DEFAULT_SOURCE_DISTANCE = 1;


export const DEFAULT_POSITION: ReadonlyVec3 = vec3.fromValues(0, 0, 0);


export const DEFAULT_FORWARD: ReadonlyVec3 = vec3.fromValues(0, 0, -1);


export const DEFAULT_UP: ReadonlyVec3 = vec3.fromValues(0, 1, 0);


export const DEFAULT_RIGHT: ReadonlyVec3 = vec3.fromValues(1, 0, 0);

/**
 * The speed of sound in air, in meters per second
 */
export const DEFAULT_SPEED_OF_SOUND = 343;

export type AttenuationRolloff =
    | "logarithmic"
    | "linear"
    | "none";

/** 
 * Rolloff models (e.g. "logarithmic", "linear", or "none").
 */
export const ATTENUATION_ROLLOFFS: AttenuationRolloff[] = [
    "logarithmic",
    "linear",
    "none"
];


/** 
 * Default rolloff model ("logarithmic").
 */
export const DEFAULT_ATTENUATION_ROLLOFF: AttenuationRolloff = "logarithmic";


export const DEFAULT_MIN_DISTANCE = 1;


export const DEFAULT_MAX_DISTANCE = 1000;


/**
 * The default alpha (i.e. microphone pattern).
 */
export const DEFAULT_DIRECTIVITY_ALPHA = 0;


/**
 * The default pattern sharpness (i.e. pattern exponent).
 */
export const DEFAULT_DIRECTIVITY_SHARPNESS = 1;


/**
 * Default azimuth (in degrees). Suitable range is 0 to 360.
 */
export const DEFAULT_AZIMUTH = 0;


/**
 * Default elevation (in degres).
 * Suitable range is from -90 (below) to 90 (above).
 */
export const DEFAULT_ELEVATION = 0;


/**
 * The default ambisonic order.
 */
export const DEFAULT_AMBISONIC_ORDER = 1;


/**
 * The default source width.
 */
export const DEFAULT_SOURCE_WIDTH = 0;


/**
 * The maximum delay (in seconds) of a single wall reflection.
 */
export const DEFAULT_REFLECTION_MAX_DURATION = 0.5;


/**
 * The -12dB cutoff frequency (in Hertz) for the lowpass filter applied to
 * all reflections.
 */
export const DEFAULT_REFLECTION_CUTOFF_FREQUENCY = 6400; // Uses -12dB cutoff.

export type PrimaryCubeSide =
    | "left"
    | "front"
    | "up";

export const PRIMARY_CUBE_SIDES: PrimaryCubeSide[] = [
    "left",
    "front",
    "up"
];

export type InverseCubeSide =
    | "right"
    | "back"
    | "down";

export const INVERSE_CUBE_SIDES: InverseCubeSide[] = [
    "right",
    "back",
    "down"
];

export type CubeSide = PrimaryCubeSide | InverseCubeSide;

export const CUBE_SIDES: CubeSide[] = [...PRIMARY_CUBE_SIDES, ...INVERSE_CUBE_SIDES];

export type ReflectionCube = Map<CubeSide, number>;

/**
 * The default reflection coefficients (where 0 = no reflection, 1 = perfect
 * reflection, -1 = mirrored reflection (180-degrees out of phase)).
 */
export const DEFAULT_REFLECTION_COEFFICIENTS: ReflectionCube = new Map([
    ["left", 0],
    ["right", 0],
    ["front", 0],
    ["back", 0],
    ["down", 0],
    ["up", 0],
]);


/**
 * The minimum distance we consider the listener to be to any given wall.
 */
export const DEFAULT_REFLECTION_MIN_DISTANCE = 1;

export type RoomDimensionsAxis =
    | "width"
    | "height"
    | "depth";

export const ROOM_DIMENSION_AXES: RoomDimensionsAxis[] = [
    "width",
    "height",
    "depth"
];

export type RoomDimensions = Map<RoomDimensionsAxis, number>;

/**
 * Default room dimensions (in meters).
 */
export const DEFAULT_ROOM_DIMENSIONS: RoomDimensions = new Map([
    ["width", 0],
    ["height", 0],
    ["depth", 0]
]);


/**
 * The multiplier to apply to distances from the listener to each wall.
 */
export const DEFAULT_REFLECTION_MULTIPLIER = 1;


/**
 * The default bandwidth (in octaves) of the center frequencies.
 */
export const DEFAULT_REVERB_BANDWIDTH = 1;


/**
 * The default multiplier applied when computing tail lengths.
 */
export const DEFAULT_REVERB_DURATION_MULTIPLIER = 1;


/**
 * The late reflections pre-delay (in milliseconds).
 */
export const DEFAULT_REVERB_PREDELAY = 1.5;


/**
 * The length of the beginning of the impulse response to apply a
 * half-Hann window to.
 */
export const DEFAULT_REVERB_TAIL_ONSET = 3.8;


/**
 * The default gain (linear).
 */
export const DEFAULT_REVERB_GAIN = 0.01;


/**
 * The maximum impulse response length (in seconds).
 */
export const DEFAULT_REVERB_MAX_DURATION = 3;


/**
 * Center frequencies of the multiband late reflections.
 * Nine bands are computed by: 31.25 * 2^(0:8).
 */
export const DEFAULT_REVERB_FREQUENCY_BANDS = [
    31.25, 62.5, 125, 250, 500, 1000, 2000, 4000, 8000,
];


/**
 * The number of frequency bands.
 */
export const NUMBER_REVERB_FREQUENCY_BANDS = DEFAULT_REVERB_FREQUENCY_BANDS.length;


export type AudioMaterialCoefficients = [number, number, number, number, number, number, number, number, number];

/**
 * The default multiband RT60 durations (in seconds).
 */
export const DEFAULT_REVERB_DURATIONS: AudioMaterialCoefficients =
    [0,0,0,0,0,0,0,0,0];


export type AudioMaterialName =
    | "transparent"
    | "acoustic-ceiling-tiles"
    | "brick-bare"
    | "brick-painted"
    | "concrete-block-coarse"
    | "concrete-block-painted"
    | "curtain-heavy"
    | "fiber-glass-insulation"
    | "glass-thin"
    | "glass-thick"
    | "grass"
    | "linoleum-on-concrete"
    | "marble"
    | "metal"
    | "parquet-on-concrete"
    | "plaster-rough"
    | "plaster-smooth"
    | "plywood-panel"
    | "polished-concrete-or-tile"
    | "sheet-rock"
    | "water-or-ice-surface"
    | "wood-ceiling"
    | "wood-panel"
    | "uniform";

export const AUDIO_MATERIAL_NAMES: AudioMaterialName[] = [
    "transparent",
    "acoustic-ceiling-tiles",
    "brick-bare",
    "brick-painted",
    "concrete-block-coarse",
    "concrete-block-painted",
    "curtain-heavy",
    "fiber-glass-insulation",
    "glass-thin",
    "glass-thick",
    "grass",
    "linoleum-on-concrete",
    "marble",
    "metal",
    "parquet-on-concrete",
    "plaster-rough",
    "plaster-smooth",
    "plywood-panel",
    "polished-concrete-or-tile",
    "sheet-rock",
    "water-or-ice-surface",
    "wood-ceiling",
    "wood-panel",
    "uniform",
];

export type RoomAudioMaterialNameCoefficients = Map<AudioMaterialName, AudioMaterialCoefficients>;
/**
 * Pre-defined frequency-dependent absorption coefficients for listed materials.
 * Currently supported materials are:
 * <ul>
 * <li>"transparent"</li>
 * <li>"acoustic-ceiling-tiles"</li>
 * <li>"brick-bare"</li>
 * <li>"brick-painted"</li>
 * <li>"concrete-block-coarse"</li>
 * <li>"concrete-block-painted"</li>
 * <li>"curtain-heavy"</li>
 * <li>"fiber-glass-insulation"</li>
 * <li>"glass-thin"</li>
 * <li>"glass-thick"</li>
 * <li>"grass"</li>
 * <li>"linoleum-on-concrete"</li>
 * <li>"marble"</li>
 * <li>"metal"</li>
 * <li>"parquet-on-concrete"</li>
 * <li>"plaster-smooth"</li>
 * <li>"plywood-panel"</li>
 * <li>"polished-concrete-or-tile"</li>
 * <li>"sheetrock"</li>
 * <li>"water-or-ice-surface"</li>
 * <li>"wood-ceiling"</li>
 * <li>"wood-panel"</li>
 * <li>"uniform"</li>
 * </ul>
 */
export const ROOM_MATERIAL_COEFFICIENTS: RoomAudioMaterialNameCoefficients = new Map([
    ["transparent", [1.000, 1.000, 1.000, 1.000, 1.000, 1.000, 1.000, 1.000, 1.000]],
    ["acoustic-ceiling-tiles", [0.672, 0.675, 0.700, 0.660, 0.720, 0.920, 0.880, 0.750, 1.000]],
    ["brick-bare", [0.030, 0.030, 0.030, 0.030, 0.030, 0.040, 0.050, 0.070, 0.140]],
    ["brick-painted", [0.006, 0.007, 0.010, 0.010, 0.020, 0.020, 0.020, 0.030, 0.060]],
    ["concrete-block-coarse", [0.360, 0.360, 0.360, 0.440, 0.310, 0.290, 0.390, 0.250, 0.500]],
    ["concrete-block-painted", [0.092, 0.090, 0.100, 0.050, 0.060, 0.070, 0.090, 0.080, 0.160]],
    ["curtain-heavy", [0.073, 0.106, 0.140, 0.350, 0.550, 0.720, 0.700, 0.650, 1.000]],
    ["fiber-glass-insulation", [0.193, 0.220, 0.220, 0.820, 0.990, 0.990, 0.990, 0.990, 1.000]],
    ["glass-thin", [0.180, 0.169, 0.180, 0.060, 0.040, 0.030, 0.020, 0.020, 0.040]],
    ["glass-thick", [0.350, 0.350, 0.350, 0.250, 0.180, 0.120, 0.070, 0.040, 0.080]],
    ["grass", [0.050, 0.050, 0.150, 0.250, 0.400, 0.550, 0.600, 0.600, 0.600]],
    ["linoleum-on-concrete", [0.020, 0.020, 0.020, 0.030, 0.030, 0.030, 0.030, 0.020, 0.040]],
    ["marble", [0.010, 0.010, 0.010, 0.010, 0.010, 0.010, 0.020, 0.020, 0.040]],
    ["metal", [0.030, 0.035, 0.040, 0.040, 0.050, 0.050, 0.050, 0.070, 0.090]],
    ["parquet-on-concrete", [0.028, 0.030, 0.040, 0.040, 0.070, 0.060, 0.060, 0.070, 0.140]],
    ["plaster-rough", [0.017, 0.018, 0.020, 0.030, 0.040, 0.050, 0.040, 0.030, 0.060]],
    ["plaster-smooth", [0.011, 0.012, 0.013, 0.015, 0.020, 0.030, 0.040, 0.050, 0.100]],
    ["plywood-panel", [0.400, 0.340, 0.280, 0.220, 0.170, 0.090, 0.100, 0.110, 0.220]],
    ["polished-concrete-or-tile", [0.008, 0.008, 0.010, 0.010, 0.015, 0.020, 0.020, 0.020, 0.040]],
    ["sheet-rock", [0.290, 0.279, 0.290, 0.100, 0.050, 0.040, 0.070, 0.090, 0.180]],
    ["water-or-ice-surface", [0.006, 0.006, 0.008, 0.008, 0.013, 0.015, 0.020, 0.025, 0.050]],
    ["wood-ceiling", [0.150, 0.147, 0.150, 0.110, 0.100, 0.070, 0.060, 0.070, 0.140]],
    ["wood-panel", [0.280, 0.280, 0.280, 0.220, 0.170, 0.090, 0.100, 0.110, 0.220]],
    ["uniform", [0.500, 0.500, 0.500, 0.500, 0.500, 0.500, 0.500, 0.500, 0.500]]
]);


export type RoomAudioMaterialNames = Map<CubeSide, AudioMaterialName>;

/**
 * Default materials
 */
export const DEFAULT_ROOM_MATERIALS: RoomAudioMaterialNames = new Map([
    ["left", "transparent"],
    ["right", "transparent"],
    ["front", "transparent"],
    ["back", "transparent"],
    ["down", "transparent"],
    ["up", "transparent"]
]);

export type RoomAudioMaterialCoefficients = Map<CubeSide, AudioMaterialCoefficients>;

/**
 * The number of bands to average over when computing reflection coefficients.
 */
export const NUMBER_REFLECTION_AVERAGING_BANDS = 3;


/**
 * The starting band to average over when computing reflection coefficients.
 */
export const ROOM_STARTING_AVERAGING_BAND = 4;


/**
 * The minimum threshold for room volume.
 * Room model is disabled if volume is below this value.
 */
export const ROOM_MIN_VOLUME = 1e-4;


/**
 * Air absorption coefficients per frequency band.
 */
export const ROOM_AIR_ABSORPTION_COEFFICIENTS =
    [0.0006, 0.0006, 0.0007, 0.0008, 0.0010, 0.0015, 0.0026, 0.0060, 0.0207];


/**
 * A scalar correction value to ensure Sabine and Eyring produce the same RT60
 * value at the cross-over threshold.
 */
export const ROOM_EYRING_CORRECTION_COEFFICIENT = 1.38;


/**
 * Properties describing the geometry of a room.
 * @typedef {Object} Utils~RoomDimensions
 * @property {Number} width (in meters).
 * @property {Number} height (in meters).
 * @property {Number} depth (in meters).
 */

/**
 * Properties describing the wall materials (from
 * {@linkcode Utils.ROOM_MATERIAL_COEFFICIENTS ROOM_MATERIAL_COEFFICIENTS})
 * of a room.
 * @typedef {Object} Utils~RoomMaterials
 * @property {String} left Left-wall material name.
 * @property {String} right Right-wall material name.
 * @property {String} front Front-wall material name.
 * @property {String} back Back-wall material name.
 * @property {String} up Up-wall material name.
 * @property {String} down Down-wall material name.
 */

/**
 * library logging function.
 * @type {Function}
 * @param {any} Message to be printed out.
 * @private
 */
export function log(...args: any[]) {
    window.console.log.apply(window.console, [
        "%c[Juniper]%c "
        + args.join(" ") + " %c(@"
        + performance.now().toFixed(2) + "ms)",
        "background: #BBDEFB; color: #FF5722; font-weight: 700",
        "font-weight: 400",
        "color: #AAA",
    ]);
}


/**
 * Omnitone library error-throwing function.
 * @param {any} Message to be printed out.
 */
export function formatError(...args: any[]) {
    return `[Omnitone] \
${args.join(" ")} \
(${performance.now().toFixed(2)}ms)`;
}


/**
 * Perform channel-wise merge on multiple AudioBuffers. The sample rate and
 * the length of buffers to be merged must be identical.
 * @param context Associated BaseAudioContext.
 * @param bufferList An array of AudioBuffers to be merged
 * channel-wise.
 * @return A single merged AudioBuffer.
 */
export function mergeBufferListByChannel(context: BaseAudioContext, bufferList: AudioBuffer[]): AudioBuffer {
    const bufferLength = bufferList[0].length;
    const bufferSampleRate = bufferList[0].sampleRate;
    let bufferNumberOfChannel = 0;

    for (let i = 0; i < bufferList.length; ++i) {
        if (bufferNumberOfChannel > 32) {
            throw formatError("Utils.mergeBuffer: Number of channels cannot exceed 32." +
                "(got " + bufferNumberOfChannel + ")");
        }
        if (bufferLength !== bufferList[i].length) {
            throw formatError("Utils.mergeBuffer: AudioBuffer lengths are " +
                "inconsistent. (expected " + bufferLength + " but got " +
                bufferList[i].length + ")");
        }
        if (bufferSampleRate !== bufferList[i].sampleRate) {
            throw formatError("Utils.mergeBuffer: AudioBuffer sample rates are " +
                "inconsistent. (expected " + bufferSampleRate + " but got " +
                bufferList[i].sampleRate + ")");
        }
        bufferNumberOfChannel += bufferList[i].numberOfChannels;
    }

    const buffer = context.createBuffer(
        bufferNumberOfChannel, bufferLength, bufferSampleRate);
    let destinationChannelIndex = 0;
    for (let i = 0; i < bufferList.length; ++i) {
        for (let j = 0; j < bufferList[i].numberOfChannels; ++j) {
            buffer.getChannelData(destinationChannelIndex++).set(
                bufferList[i].getChannelData(j));
        }
    }

    return buffer;
}


/**
 * Perform channel-wise split by the given channel count. For example,
 * 1 x AudioBuffer(8) -> splitBuffer(context, buffer, 2) -> 4 x AudioBuffer(2).
 * @param context - Associated BaseAudioContext.
 * @param audioBuffer - An AudioBuffer to be splitted.
 * @param splitBy - Number of channels to be splitted.
 * @return An array of splitted AudioBuffers.
 */
export function splitBufferbyChannel(context: BaseAudioContext, audioBuffer: AudioBuffer, splitBy: number): AudioBuffer[] {
    if (audioBuffer.numberOfChannels <= splitBy) {
        throw formatError("Utils.splitBuffer: Insufficient number of channels. (" +
            audioBuffer.numberOfChannels + " splitted by " + splitBy + ")");
    }

    const bufferList: AudioBuffer[] = [];
    let sourceChannelIndex = 0;
    const numberOfSplittedBuffer =
        Math.ceil(audioBuffer.numberOfChannels / splitBy);
    for (let i = 0; i < numberOfSplittedBuffer; ++i) {
        const buffer = context.createBuffer(
            splitBy, audioBuffer.length, audioBuffer.sampleRate);
        for (let j = 0; j < splitBy; ++j) {
            if (sourceChannelIndex < audioBuffer.numberOfChannels) {
                buffer.getChannelData(j).set(
                    audioBuffer.getChannelData(sourceChannelIndex++));
            }
        }
        bufferList.push(buffer);
    }

    return bufferList;
}


/**
 * Converts Base64-encoded string to ArrayBuffer.
 */
export function getArrayBufferFromBase64String(base64String: string): ArrayBuffer {
    const binaryString = window.atob(base64String);
    const byteArray = new Uint8Array(binaryString.length);
    byteArray.forEach(
        (_, index) => byteArray[index] = binaryString.charCodeAt(index));
    return byteArray.buffer;
}
