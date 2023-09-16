import { Group, Mesh, MeshBasicMaterial, Object3D, SphereGeometry, Texture, XRTargetRaySpace } from "three";
import { cleanup } from "../../cleanup";
import { isMesh, isMeshPhongMaterial, isMeshPhysicalMaterial, isMeshStandardMaterial } from "../../typeChecks";
import { GLTF, GLTFLoader } from "../loaders/GLTFLoader";

import {
    Constants as MotionControllerConstants,
    fetchProfile,
    MotionController,
    VisualResponse,
    VisualResponsePropertyType
} from "./motion-controllers.module";

const DEFAULT_PROFILES_PATH = "https://cdn.jsdelivr.net/npm/@webxr-input-profiles/assets@1.0/dist/profiles";
const DEFAULT_PROFILE = "generic-trigger";

export class XRControllerModel extends Object3D {

    envMap: Texture = null;
    motionController: MotionController = null;

    setEnvironmentMap(envMap: Texture) {

        if (this.envMap == envMap) {

            return this;

        }

        this.envMap = envMap;
        this.traverse((child) => {

            if (isMesh(child)
                && (isMeshStandardMaterial(child.material)
                    || isMeshPhongMaterial(child.material)
                    || isMeshPhysicalMaterial(child.material))) {

                child.material.envMap = this.envMap;
                child.material.needsUpdate = true;

            }

        });

        return this;

    }

    /**
     * Polls data from the XRInputSource and updates the model's components to match
     * the real world data
     */
    override updateMatrixWorld(force?: boolean) {

        super.updateMatrixWorld(force);

        if (!this.motionController) return;

        // Cause the MotionController to poll the Gamepad for data
        this.motionController.updateFromGamepad();

        // Update the 3D model to reflect the button, thumbstick, and touchpad state
        Object.values(this.motionController.components).forEach((component) => {

            // Update node data based on the visual responses' current states
            Object.values(component.visualResponses).forEach((visualResponse) => {

                const { valueNode, minNode, maxNode } = visualResponse;

                // Skip if the visual response node is not found. No error is needed,
                // because it will have been reported at load time.
                if (!valueNode) return;

                // Calculate the new properties based on the weight supplied
                if (isVisibility(visualResponse)) {

                    valueNode.visible = visualResponse.value;

                } else if (isTransform(visualResponse)) {

                    valueNode.quaternion.slerpQuaternions(
                        minNode.quaternion,
                        maxNode.quaternion,
                        visualResponse.value
                    );

                    valueNode.position.lerpVectors(
                        minNode.position,
                        maxNode.position,
                        visualResponse.value
                    );

                }

            });

        });

    }

}

function isVisualResponse<T extends VisualResponsePropertyType>(visualResponse: VisualResponse, type: T): visualResponse is VisualResponse<T> {
    return visualResponse && visualResponse.valueNodeProperty === type;
}

function isVisibility(visualResponse: VisualResponse): visualResponse is VisualResponse<"visibility"> {
    return isVisualResponse(visualResponse, "visibility");
}

function isTransform(visualResponse: VisualResponse): visualResponse is VisualResponse<"transform"> {
    return isVisualResponse(visualResponse, "transform");
}

/**
 * Walks the model's tree to find the nodes needed to animate the components and
 * saves them to the motionContoller components for use in the frame loop. When
 * touchpads are found, attaches a touch dot to them.
 */
function findNodes(motionController: MotionController, scene: Object3D) {

    // Loop through the components and find the nodes needed for each components' visual responses
    Object.values(motionController.components).forEach((component) => {

        const { type, touchPointNodeName, visualResponses } = component;

        if (type === MotionControllerConstants.ComponentType.TOUCHPAD) {

            component.touchPointNode = scene.getObjectByName(touchPointNodeName);
            if (component.touchPointNode) {

                // Attach a touch dot to the touchpad.
                const sphereGeometry = new SphereGeometry(0.001);
                const material = new MeshBasicMaterial({ color: 0x0000FF });
                const sphere = new Mesh(sphereGeometry, material);
                component.touchPointNode.add(sphere);

            } else {

                console.warn(`Could not find touch dot, ${component.touchPointNodeName}, in touchpad component ${component.id}`);

            }

        }

        // Loop through all the visual responses to be applied to this component
        Object.values(visualResponses).forEach((visualResponse) => {

            const { valueNodeName, minNodeName, maxNodeName, valueNodeProperty } = visualResponse;

            // If animating a transform, find the two nodes to be interpolated between.
            if (valueNodeProperty === MotionControllerConstants.VisualResponseProperty.TRANSFORM) {

                visualResponse.minNode = scene.getObjectByName(minNodeName);
                visualResponse.maxNode = scene.getObjectByName(maxNodeName);

                // If the extents cannot be found, skip this animation
                if (!visualResponse.minNode) {

                    console.warn(`Could not find ${minNodeName} in the model`);
                    return;

                }

                if (!visualResponse.maxNode) {

                    console.warn(`Could not find ${maxNodeName} in the model`);
                    return;

                }

            }

            // If the target node cannot be found, skip this animation
            visualResponse.valueNode = scene.getObjectByName(valueNodeName);
            if (!visualResponse.valueNode) {

                console.warn(`Could not find ${valueNodeName} in the model`);

            }

        });

    });

}

function addAssetSceneToControllerModel(controllerModel: XRControllerModel, scene: Object3D) {

    // Find the nodes needed for animation and cache them on the motionController.
    findNodes(controllerModel.motionController, scene);

    // Apply any environment map that the mesh already has set.
    if (controllerModel.envMap) {

        scene.traverse((child) => {

            if (isMesh(child)
                && (isMeshStandardMaterial(child.material)
                    || isMeshPhongMaterial(child.material)
                    || isMeshPhysicalMaterial(child.material))) {

                child.material.envMap = controllerModel.envMap;
                child.material.needsUpdate = true;

            }

        });

    }

    // Add the glTF scene to the controllerModel.
    controllerModel.add(scene);

}

export class XRControllerModelFactory {

    private readonly gltfLoader: GLTFLoader;
    private readonly path: string;
    private readonly _assetCache: Record<string, GLTF>;

    constructor(gltfLoader: GLTFLoader = null) {
        this.gltfLoader = gltfLoader || new GLTFLoader();
        this.path = DEFAULT_PROFILES_PATH;
        this._assetCache = {};
    }

    createControllerModel(controller: XRTargetRaySpace, profileName: string = null) {

        const controllerModel = new XRControllerModel();
        let scene: Group = null;

        controller.addEventListener("connected", async (event) => {

            const xrInputSource = event.data as XRInputSource;

            if (xrInputSource.targetRayMode !== "tracked-pointer" || !xrInputSource.gamepad) return;

            try {
                const { profile, assetPath } = await fetchProfile(xrInputSource, this.path, profileName || DEFAULT_PROFILE);

                controllerModel.motionController = new MotionController(
                    xrInputSource,
                    profile,
                    assetPath
                );

                const cachedAsset = this._assetCache[controllerModel.motionController.assetUrl];
                if (cachedAsset) {

                    scene = cachedAsset.scene.clone();

                    addAssetSceneToControllerModel(controllerModel, scene);

                } else {

                    if (!this.gltfLoader) {

                        throw new Error("GLTFLoader not set.");

                    }

                    this.gltfLoader.setPath("");
                    this.gltfLoader.load(controllerModel.motionController.assetUrl, (asset) => {

                        this._assetCache[controllerModel.motionController.assetUrl] = asset;

                        scene = asset.scene.clone();

                        addAssetSceneToControllerModel(controllerModel, scene);

                    },
                    null,
                    () => {

                        throw new Error(`Asset ${controllerModel.motionController.assetUrl} missing or malformed.`);

                    });

                }

            }
            catch (err) {

                console.warn(err);

            }

        });

        controller.addEventListener("disconnected", () => {
            cleanup(controllerModel.motionController);
            controllerModel.motionController = null;
            cleanup(scene);
            scene = null;
        });

        return controllerModel;

    }

}
