import { Model_Gltf_Binary } from "@juniper-lib/mediatypes";
import { HalfPi, Pi } from "@juniper-lib/tslib/math";
import { isDefined, isNullOrUndefined } from "@juniper-lib/tslib/typeChecks";
import { IDisposable } from "@juniper-lib/tslib/using";
import { Group, Mesh, MeshPhongMaterial, MeshStandardMaterial } from "three";
import { AssetGltfModel } from "./AssetGltfModel";
import { cleanup } from "./cleanup";
import type { Environment } from "./environment/Environment";
import type { PointerHand } from "./eventSystem/devices/PointerHand";
import { convertMaterials, materialStandardToPhong } from "./materials";
import { ErsatzObject, objGraph } from "./objects";
import { objectScan } from "./objectScan";
import { isMesh } from "./typeChecks";

export class Watch implements ErsatzObject, IDisposable {
    readonly asset: AssetGltfModel;

    private _model: Mesh;
    get object() {
        return this._model;
    }

    constructor(env: Environment, modelPath: string) {
        this.asset = new AssetGltfModel(env, modelPath, Model_Gltf_Binary, !env.DEBUG);
        this.asset.then(() => {
            this._model = objectScan(this.asset.result.scene, isMesh);
            const roughnessMap = (this._model.material as MeshStandardMaterial).roughnessMap;
            convertMaterials(this._model, materialStandardToPhong);
            (this._model.material as MeshPhongMaterial).specularMap = roughnessMap;
            env.timer.addTickHandler(update);
        });

        function isGood(hand: PointerHand) {
            return hand.enabled
                && (hand.grip.visible
                    || hand.hand.visible
                    && hand.hand.joints.wrist
                    && (hand.hand.joints.wrist as unknown as Group).visible);
        }

        let wasDebug = env.DEBUG;
        let hadSession = env.xrUI.visible;
        let lastHandCount = 0;

        const update = () => {
            const hasSession = env.xrUI.visible;
            let handCount = 0;
            for (const hand of env.eventSys.hands) {
                if (isGood(hand)) {
                    ++handCount;
                }
            }

            if (hasSession !== hadSession
                || handCount !== lastHandCount
                || wasDebug !== env.DEBUG) {

                env.clockImage.isVisible = hasSession || env.DEBUG;

                if (hasSession && handCount > 0) {
                    let bestHand: PointerHand = null;
                    for (const hand of env.eventSys.hands) {
                        if (isGood(hand)
                            && (isNullOrUndefined(bestHand) ||
                                hand.handedness === "left")) {
                            bestHand = hand;
                        }
                    }

                    const parent = bestHand.grip.visible
                        ? bestHand.grip
                        : (bestHand.hand.joints.wrist as unknown as Group);

                    if (parent !== env.clockImage.object.parent) {
                        objGraph(parent,
                            objGraph(this,
                                env.clockImage,
                                env.batteryImage
                            )
                        );

                        const rotate = bestHand.handedness === "left" ? 1 : 0;

                        if (parent === bestHand.grip) {
                            this.object.rotation.set(0, rotate * Pi, -HalfPi, "XYZ");
                            this.object.position.set(0, 0, 0.07);
                        }
                        else {
                            this.object.rotation.set(0, rotate * Pi, 0, "XYZ");
                            this.object.position.set(0, 0, 0);
                        }

                        env.clockImage.scale.setScalar(0.0175);
                        env.clockImage.position.set(0, 0.029, 0);
                        env.clockImage.rotation.set(-HalfPi, 0, -HalfPi);

                        if (isDefined(env.batteryImage)) {
                            env.batteryImage.scale
                                .set(2, 1, 1)
                                .multiplyScalar(0.008);
                            env.batteryImage.position.set(0.0075, 0.029, 0);
                            env.batteryImage.rotation.set(-HalfPi, 0, -HalfPi);
                        }
                    }
                }
                else {
                    env.xrUI.addItem(env.clockImage, { x: -1, y: 1, height: 0.1 });
                    if (isDefined(env.batteryImage)) {
                        env.xrUI.addItem(env.batteryImage, { x: 0.75, y: -1, width: 0.2, height: 0.1 });
                    }
                }

                hadSession = hasSession;
                lastHandCount = handCount;
                wasDebug = env.DEBUG;
            }
        };
    }

    private disposed = false;
    dispose(): void {
        if (!this.disposed) {
            if (isDefined(this.object)) {
                this.object.removeFromParent();
                cleanup(this.object);
                this._model = null;
            }
            this.disposed = true;
        }
    }
}
