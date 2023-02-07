import { Color, Matrix4, Object3D, Quaternion, SkinnedMesh, Vector3, XRHandSpace } from 'three';
import { materialPhysicalToPhong, materialStandardToPhong } from '../../materials';
import { isMeshPhongMaterial, isMeshPhysicalMaterial, isMeshStandardMaterial } from '../../typeChecks';
import { GLTFLoader } from '../loaders/GLTFLoader';
import { IXRHandModel, jointNames } from './XRHandModelFactory';

const _oculusBrowserV14CorrectionRight = new Quaternion().identity();
const _oculusBrowserV14CorrectionLeft = new Quaternion().identity();

if (/OculusBrowser\/14\./.test(navigator.userAgent)) {
    _oculusBrowserV14CorrectionRight.setFromAxisAngle(new Vector3(0, 1, 0), Math.PI / 2);
    _oculusBrowserV14CorrectionLeft.setFromAxisAngle(new Vector3(1, 0, 0), Math.PI)
        .premultiply(_oculusBrowserV14CorrectionRight);
}

const DEFAULT_HAND_PROFILE_PATH = 'https://cdn.jsdelivr.net/npm/@webxr-input-profiles/assets@1.0/dist/profiles/generic-hand/';

export class XRHandMeshModel extends Object3D implements IXRHandModel {
    private readonly oculusBrowserV14Correction: Quaternion;
    private readonly bones = new Map<XRHandJoint, Object3D>();
    private root: Object3D = null;

    readonly instanceMatrix = { needsUpdate: false };

    constructor(
        public readonly controller: XRHandSpace,
        public readonly handedness: XRHandedness,
        color: Color) {
        super();

        this.oculusBrowserV14Correction = this.handedness === 'left'
            ? _oculusBrowserV14CorrectionLeft
            : _oculusBrowserV14CorrectionRight;

        const loader = new GLTFLoader();
        loader.setPath(DEFAULT_HAND_PROFILE_PATH);
        loader.load(`${this.handedness}.glb`, gltf => {

            this.root = gltf.scene.children[0];
            this.add(this.root);

            const mesh = this.root.getObjectByProperty('type', 'SkinnedMesh') as SkinnedMesh;

            mesh.frustumCulled = false;
            mesh.castShadow = true;
            mesh.receiveShadow = true;

            if (isMeshPhysicalMaterial(mesh.material)) {
                mesh.material = materialPhysicalToPhong(mesh.material);
            }
            else if (isMeshStandardMaterial(mesh.material)) {
                mesh.material = materialStandardToPhong(mesh.material);
            }
            if (isMeshPhongMaterial(mesh.material)) {
                mesh.material.color = color;
                mesh.material.shininess = 0.1;
            }

            jointNames.forEach(jointName =>
                this.addBone(jointName));

        });
    }

    private addBone(jointName: XRHandJoint) {
        if (this.root) {
            const bone = this.root.getObjectByName(jointName);
            if (bone) {
                this.bones.set(jointName, bone);
            }
        }
    }

    get count() {
        return this.bones.size;
    }

    set count(_v: number) {
        // no op
    }

    getMatrixAt(n: number, M: Matrix4) {
        if (0 <= n && n < jointNames.length) {
            const jointName = jointNames[n];
            if (this.bones.has(jointName)) {
                M.copy(this.bones
                    .get(jointName)
                    .matrix);
            }
        }
    }

    setMatrixAt(n: number, M: Matrix4) {
        if (0 <= n && n < jointNames.length) {
            const jointName = jointNames[n];
            if (this.bones.has(jointName)) {
                const bone = this.bones.get(jointName);
                bone.matrix.copy(M);
                bone.matrix.decompose(
                    bone.position,
                    bone.quaternion,
                    bone.scale);
            }
        }
    }

    updateMesh() {
        if (this.controller) {
            for (const [jointName, bone] of this.bones) {
                const joint = this.controller.joints[jointName];
                if (joint && joint.visible) {
                    bone.position.copy(joint.position);
                    bone.quaternion.copy(joint.quaternion)
                        .multiply(this.oculusBrowserV14Correction);
                }
            }
        }
    }
}
