import {
    BoxGeometry,
    BufferGeometry,
    CylinderGeometry,
    DynamicDrawUsage,
    InstancedMesh,
    Matrix4,
    MeshStandardMaterial, SphereGeometry, Vector3,
    XRHandSpace
} from "three";
import { XRHandModelPrimitiveProfileType } from "./XRHandModelFactory";

const _matrix = new Matrix4();
const _vector = new Vector3();
const _oculusBrowserV14CorrectionRight = new Matrix4().identity();
const _oculusBrowserV14CorrectionLeft = new Matrix4().identity();

if (/OculusBrowser\/14\./.test(navigator.userAgent)) {
    _oculusBrowserV14CorrectionRight.makeRotationY(Math.PI / 2);
    _oculusBrowserV14CorrectionLeft.makeRotationY(-Math.PI / 2);
}

export class XRHandPrimitiveModel extends InstancedMesh<BufferGeometry, MeshStandardMaterial> {
    private readonly oculusBrowserV14Correction: Matrix4;
    private readonly joints: ReadonlyArray<XRHandJoint>;

    constructor(
        public readonly controller: XRHandSpace,
        handedness: XRHandedness,
        primitive: XRHandModelPrimitiveProfileType) {

        let geometry: BufferGeometry;

        if (primitive === 'boxes') {
            geometry = new BoxGeometry(1, 1, 1);
        } else if (primitive === 'bones') {
            geometry = new CylinderGeometry(0.5, 0.75, 2.25, 10, 1).rotateX(-Math.PI / 2);
        } else {
            geometry = new SphereGeometry(1, 10, 10);
        }

        const material = new MeshStandardMaterial();

        super(geometry, material, 30);

        this.instanceMatrix.setUsage(DynamicDrawUsage); // will be updated every frame
        this.castShadow = true;
        this.receiveShadow = true;

        this.oculusBrowserV14Correction = handedness === 'left'
            ? _oculusBrowserV14CorrectionLeft
            : _oculusBrowserV14CorrectionRight;

        this.joints = [
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
            'pinky-finger-tip'
        ];

    }

    override updateMatrixWorld() {
        if (this.controller) {
            const defaultRadius = 0.008;
            const joints = this.controller.joints;

            let count = 0;

            for (let i = 0; i < this.joints.length; i++) {
                const joint = joints[this.joints[i]];
                if (joint.visible) {
                    _vector.setScalar(joint.jointRadius || defaultRadius);
                    _matrix.compose(joint.position, joint.quaternion, _vector);
                    _matrix.multiply(this.oculusBrowserV14Correction);
                    this.setMatrixAt(i, _matrix);
                    count++;
                }
            }

            this.count = count;
        }

        super.updateMatrixWorld();
    }

    voodoo() {
        this.instanceMatrix.needsUpdate = true;
    }
}
