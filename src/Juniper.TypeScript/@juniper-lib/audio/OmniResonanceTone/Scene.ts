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
 * @file ResonanceAudio library name space and common utilities.
 * @author Andrew Allen <bitllama@google.com>
 * 
 * Conversion to TypeScript, modernization, and miscellaneous defect fixes by:
 * @author Sean T. McBeth <sean.mcbeth+gh@gmail.com>
 */


import { arrayRemove } from "@juniper-lib/tslib/collections/arrays";
import { isBadNumber, isNullOrUndefined } from "@juniper-lib/tslib/typeChecks";
import { ReadonlyMat4, ReadonlyVec3 } from "gl-matrix";
import { BaseNodeCluster } from "../BaseNodeCluster";
import { JuniperAudioContext } from "../context/JuniperAudioContext";
import { JuniperGainNode } from "../context/JuniperGainNode";
import { Listener } from "./Listener";
import { Room } from "./Room";
import { Source, SourceOptions } from "./Source";
import * as Utils from "./utils";


/**
 * Options for constructing a new ResonanceAudio scene.
 * @typedef {Object} ResonanceAudio~ResonanceAudioOptions
 * @property {Number} ambisonicOrder
 * Desired ambisonic Order. Defaults to
 * {@linkcode Utils.DEFAULT_AMBISONIC_ORDER DEFAULT_AMBISONIC_ORDER}.
 * @property {Float32Array} listenerPosition
 * The listener's initial position (in meters), where origin is the center of
 * the room. Defaults to {@linkcode Utils.DEFAULT_POSITION DEFAULT_POSITION}.
 * @property {Float32Array} listenerForward
 * The listener's initial forward vector.
 * Defaults to {@linkcode Utils.DEFAULT_FORWARD DEFAULT_FORWARD}.
 * @property {Float32Array} listenerUp
 * The listener's initial up vector.
 * Defaults to {@linkcode Utils.DEFAULT_UP DEFAULT_UP}.
 * @property {Utils~RoomDimensions} dimensions Room dimensions (in meters). Defaults to
 * {@linkcode Utils.DEFAULT_ROOM_DIMENSIONS DEFAULT_ROOM_DIMENSIONS}.
 * @property {Utils~RoomMaterials} materials Named acoustic materials per wall.
 * Defaults to {@linkcode Utils.DEFAULT_ROOM_MATERIALS DEFAULT_ROOM_MATERIALS}.
 * @property {Number} speedOfSound
 * (in meters/second). Defaults to
 * {@linkcode Utils.DEFAULT_SPEED_OF_SOUND DEFAULT_SPEED_OF_SOUND}.
 */

export interface SceneOptions {
    ambisonicOrder: number;
    listenerPosition: ReadonlyVec3;
    listenerForward: ReadonlyVec3;
    listenerUp: ReadonlyVec3;
    dimensions: Utils.RoomDimensions;
    materials: Utils.RoomAudioMaterialNames;
    speedOfSound: number;
}

/**
 * @class ResonanceAudio
 * @description Main class for managing sources, room and listener models.
 * @param {AudioContext} context
 * Associated {@link
https://developer.mozilla.org/en-US/docs/Web/API/AudioContext AudioContext}.
 * @param {Scene~ResonanceAudioOptions} options
 * Options for constructing a new ResonanceAudio scene.
 */
export class Scene extends BaseNodeCluster {

    readonly room: Room;

    private readonly _sources = new Array<Source>();
    readonly output: JuniperGainNode;
    readonly ambisonicOutput: JuniperGainNode;

    readonly listener: Listener;

    constructor(context: JuniperAudioContext, options?: Partial<SceneOptions>) {
        // Public variables.
        /**
         * Binaurally-rendered stereo (2-channel) output {@link
         * https://developer.mozilla.org/en-US/docs/Web/API/AudioNode AudioNode}.
         * @member {AudioNode} output
         * @memberof ResonanceAudio
         * @instance
         */
        /**
         * Ambisonic (multichannel) input {@link
         * https://developer.mozilla.org/en-US/docs/Web/API/AudioNode AudioNode}
         * (For rendering input soundfields).
         * @member {AudioNode} ambisonicInput
         * @memberof ResonanceAudio
         * @instance
         */
        /**
         * Ambisonic (multichannel) output {@link
         * https://developer.mozilla.org/en-US/docs/Web/API/AudioNode AudioNode}
         * (For allowing external rendering / post-processing).
         * @member {AudioNode} ambisonicOutput
         * @memberof ResonanceAudio
         * @instance
         */

        // Use defaults for undefined arguments.
        if (options == undefined) {
            options = {};
        }
        if (isBadNumber(options.ambisonicOrder)) {
            options.ambisonicOrder = Utils.DEFAULT_AMBISONIC_ORDER;
        }
        if (isNullOrUndefined(options.listenerPosition)) {
            options.listenerPosition = Utils.DEFAULT_POSITION;
        }
        if (isNullOrUndefined(options.listenerForward)) {
            options.listenerForward = Utils.DEFAULT_FORWARD;
        }
        if (isNullOrUndefined(options.listenerUp)) {
            options.listenerUp = Utils.DEFAULT_UP;
        }
        if (isNullOrUndefined(options.dimensions)) {
            options.dimensions = Utils.DEFAULT_ROOM_DIMENSIONS;
        }
        if (isNullOrUndefined(options.materials)) {
            options.materials = Utils.DEFAULT_ROOM_MATERIALS;
        }
        if (isBadNumber(options.speedOfSound)) {
            options.speedOfSound = Utils.DEFAULT_SPEED_OF_SOUND;
        }

        // Create auxillary audio nodes.
        const output = new JuniperGainNode(context);
        const ambisonicOutput = new JuniperGainNode(context);

        const listener = new Listener(context, {
            ambisonicOrder: options.ambisonicOrder,
            position: options.listenerPosition,
            forward: options.listenerForward,
            up: options.listenerUp,
        });

        const room = new Room(context, {
            listenerPosition: listener.position,
            dimensions: options.dimensions,
            materials: options.materials,
            speedOfSound: options.speedOfSound,
        });

        // Connect audio graph.
        room.connect(listener);
        listener.connect(output);
        listener.ambisonicOutput.connect(ambisonicOutput);

        super("ort-scene", context, [], [output, ambisonicOutput], [listener, room]);

        this.output = output;
        this.ambisonicOutput = ambisonicOutput;
        this.listener = listener;
        this.room = room;

        Object.seal(this);
    }

    get ambisonicOrder() {
        return this.listener.ambisonicOrder;
    }

    set ambisonicOrder(v) {
        this.listener.ambisonicOrder = v;
        for (const source of this._sources) {
            source.ambisonicOrder = v;
        }
    }

    get ambisonicInput() {
        return this.listener;
    }

    /**
     * Create a new source for the scene.
     * @param {Source~SourceOptions} options
     * Options for constructing a new Source.
     * @return {Source}
     */
    createSource(options?: Partial<SourceOptions>) {
        // Create a source and push it to the internal sources array, returning
        // the object's reference to the user.
        const source = new Source(this, options);
        this.add(source);
        this._sources.push(source);
        return source;
    }

    removeSource(source: Source) {
        arrayRemove(this._sources, source);
        this.remove(source);
    }


    /**
     * Set the room's dimensions and wall materials.
     * @param {Object} dimensions Room dimensions (in meters).
     * @param {Object} materials Named acoustic materials per wall.
     */
    setRoomProperties(dimensions: Utils.RoomDimensions, materials: Utils.RoomAudioMaterialNames) {
        this.room.setProperties(dimensions, materials);
    }


    /**
     * Set the listener's position (in meters), where origin is the center of
     * the room.
     * @param {Number} x
     * @param {Number} y
     * @param {Number} z
     */
    setListenerPosition(v: ReadonlyVec3) {
        // Update listener position.
        this.listener.setPosition(v)
        this.room.setListenerPosition(this.listener.position);

        // Update sources with new listener position.
        for (const source of this._sources) {
            source.update();
        }
    }


    /**
     * Set the source's orientation using forward and up vectors.
     * @param {Number} forwardX
     * @param {Number} forwardY
     * @param {Number} forwardZ
     * @param {Number} upX
     * @param {Number} upY
     * @param {Number} upZ
     */
    setListenerOrientation(fwd: ReadonlyVec3, up: ReadonlyVec3) {
        this.listener.setOrientation(fwd, up);
    }


    /**
     * Set the listener's position and orientation using a Three.js Matrix4 object.
     * @param {Object} matrix
     * The Three.js Matrix4 object representing the listener's world transform.
     */
    setListenerFromMatrix(matrix: ReadonlyMat4) {
        this.listener.setFromMatrix(matrix);

        // Update the rest of the scene using new listener position.
        this.setListenerPosition(this.listener.position);
    }

    get speedOfSound() {
        return this.room.speedOfSound;
    }

    /**
     * Set the speed of sound.
     * @param {Number} speedOfSound
     */
    set speedOfSound(speedOfSound: number) {
        this.room.speedOfSound = speedOfSound;
    }
}