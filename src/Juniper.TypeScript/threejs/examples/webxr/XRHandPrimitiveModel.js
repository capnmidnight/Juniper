const _matrix = new THREE.Matrix4();
const _vector = new THREE.Vector3();
const _oculusBrowserV14CorrectionRight = new THREE.Matrix4().identity();
const _oculusBrowserV14CorrectionLeft = new THREE.Matrix4().identity();

if (/OculusBrowser\/14\./.test(navigator.userAgent)) {
    _oculusBrowserV14CorrectionRight.makeRotationY(Math.PI / 2);
    _oculusBrowserV14CorrectionLeft.makeRotationY(-Math.PI / 2);
}

class XRHandPrimitiveModel {

    constructor(handModel, controller, path, handedness, options) {

        this.controller = controller;
        this.handModel = handModel;
        this.envMap = null;

        this.oculusBrowserV14Correction = handedness === 'left' ? _oculusBrowserV14CorrectionLeft : _oculusBrowserV14CorrectionRight;

        let geometry;

        if (!options || !options.primitive || options.primitive === 'sphere') {

            geometry = new THREE.SphereGeometry(1, 10, 10);

        } else if (options.primitive === 'box') {

            geometry = new THREE.BoxGeometry(1, 1, 1);

        } else if (options.primitive === 'bone') {

            geometry = new THREE.CylinderGeometry(0.5, 0.75, 2.25, 10, 1).rotateX(-Math.PI / 2);

        }

        const material = new THREE.MeshStandardMaterial();

        this.handMesh = new THREE.InstancedMesh(geometry, material, 30);
        this.handMesh.instanceMatrix.setUsage(THREE.DynamicDrawUsage); // will be updated every frame
        this.handMesh.castShadow = true;
        this.handMesh.receiveShadow = true;
        this.handModel.add(this.handMesh);

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

    updateMesh() {

        const defaultRadius = 0.008;
        const joints = this.controller.joints;

        let count = 0;

        for (let i = 0; i < this.joints.length; i++) {

            const joint = joints[this.joints[i]];

            if (joint.visible) {

                _vector.setScalar(joint.jointRadius || defaultRadius);
                _matrix.compose(joint.position, joint.quaternion, _vector);
                _matrix.multiply(this.oculusBrowserV14Correction);
                this.handMesh.setMatrixAt(i, _matrix);

                count++;

            }

        }

        this.handMesh.count = count;
        this.handMesh.instanceMatrix.needsUpdate = true;

    }

}

export { XRHandPrimitiveModel };