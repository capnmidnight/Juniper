import type { SolidMaterial } from "./materials";
import { solidTransparentBlack } from "./materials";
import { setGeometryUVsForCubemaps } from "./setGeometryUVsForCubemaps";

export const sphere = new THREE.SphereBufferGeometry(0.5);
sphere.name = "SphereGeom";

export const invSphere = sphere.clone() as THREE.SphereBufferGeometry;
invSphere.name = "InvertedSphereGeom";
setGeometryUVsForCubemaps(invSphere);

abstract class BaseSphere extends THREE.Mesh {
    constructor(size: number, material: SolidMaterial, public isCollider: boolean) {
        super(sphere, material);
        this.scale.setScalar(0.5 * size);
    }
}

export class Sphere extends BaseSphere {
    isDraggable = false;

    constructor(size: number, material: SolidMaterial) {
        super(size, material, false);
    }
}

export class SphereCollider extends BaseSphere {
    constructor(size: number) {
        super(size, solidTransparentBlack(0), true);
        this.visible = false;
    }
}