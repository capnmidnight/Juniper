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
 * @file Omnitone HOARenderer. This is user-facing API for the higher-order
 * ambisonic decoder and the optimized binaural renderer.
 * @author Andrew Allen <bitllama@google.com>
 * 
 * Conversion to TypeScript, modernization, and miscellaneous defect fixes by:
 * @author Sean T. McBeth <sean.mcbeth+gh@gmail.com>
 */

import { IFetcher } from "@juniper-lib/fetcher/IFetcher";
import { isArray, isDefined } from "@juniper-lib/tslib/typeChecks";
import { ReadonlyMat3, ReadonlyMat4 } from "gl-matrix";
import { Gain } from "../nodes";
import { chain, connect, disconnect, isAudioContext, removeVertex } from "../util";
import { BufferList } from "./BufferList";
import { HOAConvolver } from "./HOAConvolver";
import { HOARotator } from "./HOARotator";
import { IRenderer } from './IRenderer.js';
import { RenderingMode } from "./RenderingMode";
import OmnitoneSOAHRIRBase64 from "./resources/OmnitoneSOAHRIRBase64";
import OmnitoneTOAHRIRBase64 from "./resources/OmnitoneTOAHRIRBase64";
import * as Utils from "./utils";


// Currently SOA and TOA are only supported.
const SupportedAmbisonicOrder = [2, 3];

interface HOARendererBaseOptions {
    ambisonicOrder?: number;
    renderingMode?: RenderingMode;
    numberOfChannels?: number;
    numberOfStereoChannels?: number;
}

interface HOARendererURLOptions extends HOARendererBaseOptions {
    hrirPathList: string[];
    fetcher: IFetcher;
}

export type HOARendererOptions =
    | HOARendererBaseOptions
    | HOARendererURLOptions;

function isHOARendererURLOptions(obj: HOARendererOptions): obj is HOARendererURLOptions {
    return isDefined(obj)
        && ("hrirPathList" in obj
            || "fetcher" in obj);
}

export class HOARenderer implements IRenderer {

    readonly input: GainNode;
    readonly output: GainNode;
    private readonly _bypass: GainNode;
    readonly rotator: HOARotator;
    private readonly _hoaConvolver: HOAConvolver;

    private get _context() { return this.input.context; }

    private _config: HOARendererURLOptions;
    private _isRendererReady = false;

    /**
     * Omnitone HOA renderer class. Uses the optimized convolution technique.
     * @constructor
     * @param {AudioContext} context - Associated AudioContext.
     * @param {Object} config
     * @param {Number} [config.ambisonicOrder=3] - Ambisonic order.
     * @param {Array} [config.hrirPathList] - A list of paths to HRIR files. It
     * overrides the internal HRIR list if given.
     * @param {RenderingMode} [config.renderingMode='ambisonic'] - Rendering mode.
     */
    constructor(name: string, context: BaseAudioContext, config?: HOARendererOptions) {

        if (!isAudioContext(context)) {
            throw Utils.formatError('FOARenderer: Invalid BaseAudioContext.');
        }

        this._config = {
            ambisonicOrder: 3,
            renderingMode: RenderingMode.AMBISONIC,
            numberOfChannels: null,
            numberOfStereoChannels: null,
            hrirPathList: null,
            fetcher: null
        };

        if (isDefined(config) && isDefined(config.ambisonicOrder)) {
            if (SupportedAmbisonicOrder.includes(config.ambisonicOrder)) {
                this._config.ambisonicOrder = config.ambisonicOrder;
            } else {
                Utils.log(
                    'HOARenderer: Invalid ambisonic order. (got ' +
                    config.ambisonicOrder + ') Fallbacks to 3rd-order ambisonic.');
            }
        }

        this._config.numberOfChannels =
            (this._config.ambisonicOrder + 1) * (this._config.ambisonicOrder + 1);
        this._config.numberOfStereoChannels =
            Math.ceil(this._config.numberOfChannels / 2);

        if (isHOARendererURLOptions(config)) {
            if (isArray(config.hrirPathList) &&
                config.hrirPathList.length === this._config.numberOfStereoChannels) {
                this._config.hrirPathList = config.hrirPathList;
                this._config.fetcher = config.fetcher;
            } else {
                throw Utils.formatError(
                    'HOARenderer: Invalid HRIR URLs. It must be an array with ' +
                    this._config.numberOfStereoChannels + ' URLs to HRIR files.' +
                    ' (got ' + config.hrirPathList + ')');
            }
        }

        if (isDefined(config) && isDefined(config.renderingMode)) {
            if (Object.values(RenderingMode).includes(config.renderingMode)) {
                this._config.renderingMode = config.renderingMode;
            } else {
                Utils.log(
                    'HOARenderer: Invalid rendering mode. (got ' +
                    config.renderingMode + ') Fallbacks to "ambisonic".');
            }
        }

        this.input = Gain(`${name}-hoa-renderer-input-gain`, context, {
            channelCount: this._config.numberOfChannels,
            channelCountMode: "explicit",
            channelInterpretation: "discrete"
        });
        this.output = Gain(`${name}-hoa-renderer-output-gain`, context);
        this._bypass = Gain(`${name}-hoa-renderer-bypass-gain`, context);
        this.rotator = new HOARotator(`${name}-hoa-renderer`, context, this._config.ambisonicOrder);
        this._hoaConvolver = new HOAConvolver(`${name}-hoa-renderer`, context, this._config.ambisonicOrder);

        chain(this.input, this.rotator, this._hoaConvolver, this.output);
        connect(this.input, this._bypass);

        Object.seal(this);
    }

    private disposed = false;
    dispose() {
        if (!this.disposed) {
            this.disposed = true;
            removeVertex(this.input);
            removeVertex(this.output);
            removeVertex(this._bypass);
            this.rotator.dispose();
            this._hoaConvolver.dispose();
        }
    }


    /**
     * Initializes and loads the resource for the renderer.
     * @return {Promise}
     */
    async initialize() {
        Utils.log(
            'HOARenderer: Initializing... (mode: ' + this._config.renderingMode +
            ', ambisonic order: ' + this._config.ambisonicOrder + ')');

        let bufferList;
        if (isDefined(this._config.hrirPathList)) {
            bufferList =
                new BufferList(this._context, this._config.hrirPathList, {
                    dataType: 'url',
                    fetcher: this._config.fetcher
                });
        } else {
            bufferList = this._config.ambisonicOrder === 2
                ? new BufferList(this._context, OmnitoneSOAHRIRBase64)
                : new BufferList(this._context, OmnitoneTOAHRIRBase64);
        }

        try {
            const hrirBufferList = await bufferList.load();
            this._hoaConvolver.setHRIRBufferList(hrirBufferList);
            this.setRenderingMode(this._config.renderingMode);
            this._isRendererReady = true;
            Utils.log('HOARenderer: HRIRs loaded successfully. Ready.');
        }
        catch (exp) {
            const errorMessage = 'HOARenderer: HRIR loading/decoding failed.';
            throw Utils.formatError(errorMessage);
        }
    }


    /**
     * Updates the rotation matrix with 3x3 matrix.
     * @param {Number[]} mat - A 3x3 rotation matrix. (column-major)
     */
    setRotationMatrix3(mat: ReadonlyMat3) {
        if (!this._isRendererReady) {
            return;
        }

        this.rotator.setRotationMatrix3(mat);
    }


    /**
     * Updates the rotation matrix with 4x4 matrix.
     * @param {Number[]} mat - A 4x4 rotation matrix. (column-major)
     */
    setRotationMatrix4(mat: ReadonlyMat4) {
        if (!this._isRendererReady) {
            return;
        }

        this.rotator.setRotationMatrix4(mat);
    }


    /**
     * Set the decoding mode.
     * @param {RenderingMode} mode - Decoding mode.
     *  - 'ambisonic': activates the ambisonic decoding/binaurl rendering.
     *  - 'bypass': bypasses the input stream directly to the output. No ambisonic
     *    decoding or encoding.
     *  - 'off': all the processing off saving the CPU power.
     */
    setRenderingMode(mode: RenderingMode) {
        if (mode === this._config.renderingMode) {
            return;
        }

        switch (mode) {
            case RenderingMode.AMBISONIC:
                this._hoaConvolver.enable();
                disconnect(this._bypass, this.output);
                break;
            case RenderingMode.BYPASS:
                this._hoaConvolver.disable();
                connect(this._bypass, this.output);
                break;
            case RenderingMode.OFF:
                this._hoaConvolver.disable();
                disconnect(this._bypass, this.output);
                break;
            default:
                Utils.log(
                    'HOARenderer: Rendering mode "' + mode + '" is not ' +
                    'supported.');
                return;
        }

        this._config.renderingMode = mode;
        Utils.log('HOARenderer: Rendering mode changed. (' + mode + ')');
    }
}
