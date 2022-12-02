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
 * @file Omnitone FOARenderer. This is user-facing API for the first-order
 * ambisonic decoder and the optimized binaural renderer.
 * @author Andrew Allen <bitllama@google.com>
 * 
 * Conversion to TypeScript, modernization, and miscellaneous defect fixes by:
 * @author Sean T. McBeth <sean.mcbeth+gh@gmail.com>
 */


import { IFetcher } from "@juniper-lib/fetcher/IFetcher";
import { arrayReplace } from "@juniper-lib/tslib/collections/arrays";
import { isArray, isDefined } from "@juniper-lib/tslib/typeChecks";
import { ReadonlyMat3, ReadonlyMat4 } from "gl-matrix";
import { Gain } from "../nodes";
import { chain, connect, disconnect, isAudioContext, removeVertex } from "../util";
import { BufferList } from "./BufferList";
import { FOAConvolver } from "./FOAConvolver";
import { FOARotator } from "./FOARotator";
import { ChannelMapValues, FOARouter } from "./FOARouter";
import { IRenderer } from "./IRenderer";
import { RenderingMode } from "./RenderingMode";
import OmnitoneFOAHRIRBase64 from "./resources/OmnitoneFOAHRIRBase64";
import * as Utils from "./utils";


interface FOARendererBaseOptions {
    channelMap?: ChannelMapValues;
    renderingMode?: RenderingMode;
}

interface FOARendererURLOptions extends FOARendererBaseOptions {
    hrirPathList: string[];
    fetcher: IFetcher;
}

export type FOARendererOptions =
    | FOARendererBaseOptions
    | FOARendererURLOptions;

function isFOARendererURLOptions(obj: FOARendererOptions): obj is FOARendererURLOptions {
    return isDefined(obj)
        && ("hrirPathList" in obj
            || "fetcher" in obj);
}

/**
 * Omnitone FOA renderer class. Uses the optimized convolution technique.
 */
export class FOARenderer implements IRenderer {

    readonly input: GainNode;
    readonly output: GainNode;
    readonly _bypass: GainNode;
    readonly _foaRouter: FOARouter;
    readonly rotator: FOARotator;
    readonly _foaConvolver: FOAConvolver;
    private readonly _config: FOARendererURLOptions;
    private _isRendererReady = false;
    private get _context() { return this.input.context; }


    /**
     * Omnitone FOA renderer class. Uses the optimized convolution technique.
     * @constructor
     * @param name
     * @param context - Associated AudioContext.
     * @param options
     */
    constructor(name: string, context: BaseAudioContext, options?: FOARendererOptions) {

        if (!isAudioContext(context)) {
            throw Utils.formatError('FOARenderer: Invalid BaseAudioContext.');
        }

        this._config = {
            hrirPathList: null,
            fetcher: null,
            channelMap: FOARouter.ChannelMap.get("DEFAULT"),
            renderingMode: RenderingMode.AMBISONIC,
        };

        if (isDefined(options)) {
            if (isDefined(options.channelMap)) {
                if (Array.isArray(options.channelMap) && options.channelMap.length === 4) {
                    this._config.channelMap = options.channelMap;
                } else {
                    throw Utils.formatError(
                        'FOARenderer: Invalid channel map. (got ' + options.channelMap
                        + ')');
                }
            }

            if (isDefined(options.renderingMode)) {
                if (Object.values(RenderingMode).includes(options.renderingMode)) {
                    this._config.renderingMode = options.renderingMode;
                } else {
                    Utils.log(
                        'FOARenderer: Invalid rendering mode order. (got' +
                        options.renderingMode + ') Fallbacks to the mode "ambisonic".');
                }
            }

            if (isFOARendererURLOptions(options)) {
                if (isDefined(options.hrirPathList)) {
                    if (isArray(options.hrirPathList) &&
                        options.hrirPathList.length === 2) {
                        this._config.hrirPathList = options.hrirPathList;
                    } else {
                        throw Utils.formatError(
                            'FOARenderer: Invalid HRIR URLs. It must be an array with ' +
                            '2 URLs to HRIR files. (got ' + options.hrirPathList + ')');
                    }
                }

                if (isDefined(options.fetcher)) {
                    this._config.fetcher = options.fetcher;
                }
            }
        }

        this.input = Gain(`${name}-foa-renderer-input-gain`, context, {
            channelCount: 4,
            channelCountMode: "explicit",
            channelInterpretation: "discrete"
        });

        this.output = Gain(`${name}-foa-renderer-output-gain`, context);
        this._bypass = Gain(`${name}-foa-renderer-bypass-gain`, context);
        this._foaRouter = new FOARouter(`${name}-foa-renderer`, context, this._config.channelMap);
        this.rotator = new FOARotator(`${name}-foa-renderer`, context);
        this._foaConvolver = new FOAConvolver(`${name}-foa-renderer`, context);

        chain(this.input, this.rotator, this.rotator, this._foaConvolver, this.output);
        connect(this.input, this._bypass);
        
        this._isRendererReady = false;

        Object.seal(this);
    }

    private disposed = false;
    dispose() {
        if (!this.disposed) {
            this.disposed = true;
            removeVertex(this.input);
            removeVertex(this.output);
            removeVertex(this._bypass);
            this._foaRouter.dispose();
            this.rotator.dispose();
            this._foaConvolver.dispose();
        }
    }


    /**
     * Initializes and loads the resource for the renderer.
     * @return {Promise}
     */
    async initialize() {
        Utils.log(
            'FOARenderer: Initializing... (mode: ' + this._config.renderingMode +
            ')');

        const bufferList = this._config.hrirPathList
            ? new BufferList(this._context, this._config.hrirPathList, {
                fetcher: this._config.fetcher,
                dataType: 'url'
            })
            : new BufferList(this._context, OmnitoneFOAHRIRBase64);
        try {
            const hrirBufferList = await bufferList.load();
            this._foaConvolver.setHRIRBufferList(hrirBufferList);
            this.setRenderingMode(this._config.renderingMode);
            this._isRendererReady = true;
            Utils.log('FOARenderer: HRIRs loaded successfully. Ready.');

        }
        catch (exp) {
            const errorMessage = 'FOARenderer: HRIR loading/decoding failed.';
            throw Utils.formatError(errorMessage);
        }
    }


    /**
     * Set the channel map.
     * @param channelMap - Custom channel routing for FOA stream.
     */
    setChannelMap(channelMap: ChannelMapValues) {
        if (!this._isRendererReady) {
            return;
        }

        if (channelMap.toString() !== this._config.channelMap.toString()) {
            Utils.log(
                'Remapping channels ([' + this._config.channelMap.toString() +
                '] -> [' + channelMap.toString() + ']).');
            arrayReplace(this._config.channelMap, ...channelMap);
            this._foaRouter.setChannelMap(this._config.channelMap);
        }
    }


    /**
     * Updates the rotation matrix with 3x3 matrix.
     * @param {Number[]} rotationMatrix3 - A 3x3 rotation matrix. (column-major)
     */
    setRotationMatrix3(rotationMatrix3: ReadonlyMat3) {
        if (!this._isRendererReady) {
            return;
        }

        this.rotator.setRotationMatrix3(rotationMatrix3);
    }


    /**
     * Updates the rotation matrix with 4x4 matrix.
     * @param {Number[]} rotationMatrix4 - A 4x4 rotation matrix. (column-major)
     */
    setRotationMatrix4(rotationMatrix4: ReadonlyMat4) {
        if (!this._isRendererReady) {
            return;
        }

        this.rotator.setRotationMatrix4(rotationMatrix4);
    }


    /**
     * Set the rendering mode.
     * @param {RenderingMode} mode - Rendering mode.
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
                this._foaConvolver.enable();
                disconnect(this._bypass, this);
                break;
            case RenderingMode.BYPASS:
                this._foaConvolver.disable();
                connect(this._bypass, this);
                break;
            case RenderingMode.OFF:
                this._foaConvolver.disable();
                disconnect(this._bypass, this);
                break;
            default:
                Utils.log(
                    'FOARenderer: Rendering mode "' + mode + '" is not ' +
                    'supported.');
                return;
        }

        this._config.renderingMode = mode;
        Utils.log('FOARenderer: Rendering mode changed. (' + mode + ')');
    }
}
