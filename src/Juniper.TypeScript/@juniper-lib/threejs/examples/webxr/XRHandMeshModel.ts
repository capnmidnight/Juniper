import { Color, Matrix4, Object3D, Quaternion, SkinnedMesh, Vector3, XRHandSpace } from 'three';
import { cleanup } from '../../cleanup';
import { materialPhysicalToPhong, materialStandardToPhong } from '../../materials';
import { isMeshPhongMaterial, isMeshPhysicalMaterial, isMeshStandardMaterial } from '../../typeChecks';
import { GLTFLoader } from '../loaders/GLTFLoader';
import { IXRHandModel } from './XRHandModelFactory';

const _oculusBrowserV14CorrectionRight = new Quaternion().identity();
const _oculusBrowserV14CorrectionLeft = new Quaternion().identity();

if (/OculusBrowser\/14\./.test(navigator.userAgent)) {
    _oculusBrowserV14CorrectionRight.setFromAxisAngle(new Vector3(0, 1, 0), Math.PI / 2);
    _oculusBrowserV14CorrectionLeft.setFromAxisAngle(new Vector3(1, 0, 0), Math.PI)
        .premultiply(_oculusBrowserV14CorrectionRight);
}

const DEFAULT_HAND_PROFILE_PATH = 'https://cdn.jsdelivr.net/npm/@webxr-input-profiles/assets@1.0/dist/profiles/generic-hand/';

const joints: ReadonlyArray<XRHandJoint> = [
    'wrist',
    'thumb-metacarpal',
    'thumb-phalanx-proximal',
    'thumb-phalanx-distal',
    'thumb-tip',
    'index-finger-metacarpal',
    'index-finger-phalanx-proximal',
    'index-finger-phalanx-intermediate',
    'index-finger-phalanx-distal',
    'index-finger-tip',
    'middle-finger-metacarpal',
    'middle-finger-phalanx-proximal',
    'middle-finger-phalanx-intermediate',
    'middle-finger-phalanx-distal',
    'middle-finger-tip',
    'ring-finger-metacarpal',
    'ring-finger-phalanx-proximal',
    'ring-finger-phalanx-intermediate',
    'ring-finger-phalanx-distal',
    'ring-finger-tip',
    'pinky-finger-metacarpal',
    'pinky-finger-phalanx-proximal',
    'pinky-finger-phalanx-intermediate',
    'pinky-finger-phalanx-distal',
    'pinky-finger-tip',
];

export class XRHandMeshModel extends Object3D implements IXRHandModel {
    private readonly oculusBrowserV14Correction: Quaternion;
    private readonly bones: Object3D[];
    private root: Object3D = null;

    readonly instanceMatrix = { needsUpdate: false };

    constructor(
        public readonly controller: XRHandSpace,
        public readonly handedness: XRHandedness) {
        super();

        this.oculusBrowserV14Correction = this.handedness === 'left'
            ? _oculusBrowserV14CorrectionLeft
            : _oculusBrowserV14CorrectionRight;

        this.bones = [];

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
                mesh.material.color = new Color(0x48505D);
                mesh.material.shininess = 0.1;
            }
            joints.forEach(jointName =>
                this.addBone(jointName));

        });
    }

    private addBone(jointName: string) {
        const bone = this.root.getObjectByName(jointName);

        if (bone !== undefined) {

            bone.userData.jointName = jointName;

        } else {

            console.warn(`Couldn't find ${jointName} in ${this.handedness} hand mesh`);

        }

        this.bones.push(bone);
    }

    get count() {
        return this.bones.length;
    }

    set count(v: number) {
        while (v < this.count && this.count > 0) {
            cleanup(this.bones.pop());
        }
        while (v > this.count && this.count < joints.length) {
            const jointName = joints[this.count];
            this.addBone(jointName);
        }
    }

    getMatrixAt(n: number, M: Matrix4) {
        if (0 <= n && n < this.bones.length) {
            M.copy(this.bones[n].matrixWorld);
        }
    }

    setMatrixAt(n: number, M: Matrix4) {
        if (0 <= n && n < this.bones.length) {
            this.bones[n].matrixWorld.copy(M);
        }
    }

    override updateMatrixWorld(force?: boolean) {

        // XR Joints
        const XRJoints = this.controller.joints;

        for (let i = 0; i < this.bones.length; i++) {

            const bone = this.bones[i];

            if (bone) {

                const XRJoint = XRJoints[bone.userData.jointName as XRHandJoint];

                if (XRJoint.visible) {

                    bone.position.copy(XRJoint.position);
                    bone.quaternion.copy(XRJoint.quaternion)
                        .multiply(this.oculusBrowserV14Correction);
                    // bone.scale.setScalar( XRJoint.jointRadius || defaultRadius );

                }

            }

        }

        super.updateMatrixWorld(force);
    }

    voodoo() {
        //no voodoo
    }
}
