export interface IWebXRLayerManager {
    hasXRCompositionLayers: boolean;
    xrBinding: XRWebGLBinding;
    xrMediaBinding: XRMediaBinding;
    stage: THREE.Object3D;
    gl: WebGLRenderingContext;
    referenceSpace: XRReferenceSpace;

    addWebXRLayer(layer: XRLayer, order: number): void;
    removeWebXRLayer(layer: XRLayer): void;
}
