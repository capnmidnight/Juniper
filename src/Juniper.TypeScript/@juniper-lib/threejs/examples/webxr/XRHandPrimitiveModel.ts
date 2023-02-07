import {
    BoxGeometry,
    BufferGeometry,
    Color,
    CylinderGeometry,
    DynamicDrawUsage,
    InstancedMesh,
    Matrix4,
    MeshPhongMaterial,
    SphereGeometry, Vector3, XRHandSpace
} from "three";
import { jointNames, XRHandModelPrimitiveProfileType } from "./XRHandModelFactory";

const defaultRadius = 0.008;
const _matrix = new Matrix4();
const _vector = new Vector3();
const _oculusBrowserV14CorrectionRight = new Matrix4().identity();
const _oculusBrowserV14CorrectionLeft = new Matrix4().identity();

if (/OculusBrowser\/14\./.test(navigator.userAgent)) {
    _oculusBrowserV14CorrectionRight.makeRotationY(Math.PI / 2);
    _oculusBrowserV14CorrectionLeft.makeRotationY(-Math.PI / 2);
}

export class XRHandPrimitiveModel extends InstancedMesh<BufferGeometry, MeshPhongMaterial> {
    private readonly oculusBrowserV14Correction: Matrix4;

    constructor(
        public readonly controller: XRHandSpace,
        handedness: XRHandedness,
        color: Color,
        primitive: XRHandModelPrimitiveProfileType) {

        let geometry: BufferGeometry;

        if (primitive === 'boxes') {
            geometry = new BoxGeometry(1, 1, 1);
        } else if (primitive === 'bones') {
            geometry = new CylinderGeometry(0.5, 0.75, 2.25, 10, 1).rotateX(-Math.PI / 2);
        } else {
            geometry = new SphereGeometry(1, 10, 10);
        }

        const material = new MeshPhongMaterial({
            color,
            shininess: 0.1
        });

        super(geometry, material, 30);

        this.instanceMatrix.setUsage(DynamicDrawUsage); // will be updated every frame
        this.castShadow = true;
        this.receiveShadow = true;

        this.oculusBrowserV14Correction = handedness === 'left'
            ? _oculusBrowserV14CorrectionLeft
            : _oculusBrowserV14CorrectionRight;
    }

    updateMesh() {
        if (this.controller) {
            let count = 0;

            for (const jointName of jointNames) {
                const joint = this.controller.joints[jointName];
                if (joint && joint.visible) {
                    _vector.setScalar(joint.jointRadius || defaultRadius);
                    _matrix.compose(joint.position, joint.quaternion, _vector);
                    _matrix.multiply(this.oculusBrowserV14Correction);
                    this.setMatrixAt(count, _matrix);
                    count++;
                }
            }

            this.count = count;
        }

        this.instanceMatrix.needsUpdate = true;
    }
}
