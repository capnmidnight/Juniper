/**
* @webxr-input-profiles/motion-controllers 1.0.0 https://github.com/immersive-web/webxr-input-profiles
*/

import { Object3D, Quaternion, XRTargetRaySpace } from "three";

export namespace Constants {
    let Handedness: Readonly<{
        NONE: "none";
        LEFT: "left";
        RIGHT: "right";
    }>;
    let ComponentState: Readonly<{
        DEFAULT: "default";
        TOUCHED: "touched";
        PRESSED: "pressed";
    }>;
    let ComponentProperty: Readonly<{
        BUTTON: "button";
        X_AXIS: "xAxis";
        Y_AXIS: "yAxis";
        STATE: "state";
    }>;
    let ComponentType: Readonly<{
        TRIGGER: "trigger";
        SQUEEZE: "squeeze";
        TOUCHPAD: "touchpad";
        THUMBSTICK: "thumbstick";
        BUTTON: "button";
    }>;
    let ButtonTouchThreshold: number;
    let AxisTouchThreshold: number;
    let VisualResponseProperty: Readonly<{
        TRANSFORM: "transform";
        VISIBILITY: "visibility";
    }>;
}

export type VisualResponsePropertyType =
    | "transform"
    | "visibility";

interface VisualResponsePropertyTypes extends Record<VisualResponsePropertyType, any> {
    "transform": number;
    "visibility": boolean;
}

interface Profile {
    path: string;
    deprecated?: boolean;
    layouts: Record<XRHandedness, {
        assetPath: string;
    }>;
}

/**
 * @description Static helper function to fetch a JSON file and turn it into a JS object
 * @param {string} path - Path to JSON file to be fetched
 */
export function fetchProfilesList(basePath: string): Promise<Record<string, Profile>>;

export function fetchProfile(xrInputSource: XRInputSource, basePath: string, defaultProfile?: string, getAssetPath?: boolean): Promise<{ profile: Profile, assetPath: string }>;

/**
 * Contains the description of how the 3D model should visually respond to a specific user input.
 * This is accomplished by initializing the object with the name of a node in the 3D model and
 * property that need to be modified in response to user input, the name of the nodes representing
 * the allowable range of motion, and the name of the input which triggers the change. In response
 * to the named input changing, this object computes the appropriate weighting to use for
 * interpolating between the range of motion nodes.
 */
export class VisualResponse<T extends VisualResponsePropertyType = VisualResponsePropertyType> {
    valueNodeName: string;
    valueNode: Object3D;
    minNodeName: string;
    minNode: Object3D;
    maxNodeName: string;
    maxNode: Object3D;
    valueNodeProperty: T;
    value: VisualResponsePropertyTypes[T];

    constructor(visualResponseDescription: Record<string, VisualResponsePropertyType>);

    /**
     * Computes the visual response's interpolation weight based on component state
     * @param {Object} componentValues - The component from which to update
     * @param {number} xAxis - The reported X axis value of the component
     * @param {number} yAxis - The reported Y axis value of the component
     * @param {number} button - The reported value of the component's button
     * @param {string} state - The component's active state
     */
    updateFromComponent(obj: {
        xAxis: number, yAxis: number, button: number, state: string
    }): void;
}

export class Component {
    id: string;
    type: string;
    touchPointNodeName: string;
    touchPointNode: Object3D;
    visualResponses: Record<string, VisualResponse>;
}

/**
  * @description Builds a motion controller with components and visual responses based on the
  * supplied profile description. Data is polled from the xrInputSource's gamepad.
  * @author Nell Waliczek / https://github.com/NellWaliczek
*/
export class MotionController {

    components: Array<Component>;
    gripSpace(): XRTargetRaySpace;
    targetRaySpace(): XRTargetRaySpace;
    assetUrl: string;

    /**
     * @param {Object} xrInputSource - The XRInputSource to build the MotionController around
     * @param {Object} profile - The best matched profile description for the supplied xrInputSource
     * @param {Object} assetUrl
     */
    constructor(xrInputSource: XRInputSource, profile: Profile, assetUrl: string);

    /**
     * @description Poll for updated data based on current gamepad state
     */
    updateFromGamepad(): void;
}
