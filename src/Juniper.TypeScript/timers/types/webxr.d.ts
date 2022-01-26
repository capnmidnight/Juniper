interface Constructor<T = object> {
    new(...args: any[]): T;
    prototype: T;
}

interface Window {
    XRSession?: Constructor<XRSession> | undefined;
    XR?: Constructor<XRSystem> | undefined;
}

interface Navigator {
    xr?: XRSystem | undefined;
}

interface XRWebGLRenderingContext {
    makeXRCompatible(): Promise<void>;
}

interface WebGLRenderingContenxt extends XRWebGLRenderingContext { }
interface WebGL2RenderingContext extends XRWebGLRenderingContext { }

type XRSessionMode = 'inline' | 'immersive-vr' | 'immersive-ar';

type XRReferenceSpaceType = 'viewer' | 'local' | 'local-floor' | 'bounded-floor' | 'unbounded';

type XREnvironmentBlendMode = 'opaque' | 'additive' | 'alpha-blend';

type XRVisibilityState = 'visible' | 'visible-blurred' | 'hidden';

type XRHandedness = 'none' | 'left' | 'right';

type XRTargetRayMode = 'gaze' | 'tracked-pointer' | 'screen';

type XREye = 'none' | 'left' | 'right';

type XREventType =
    | 'end'
    | 'select'
    | 'selectstart'
    | 'selectend'
    | 'squeeze'
    | 'squeezestart'
    | 'squeezeend'
    | 'inputsourceschange';

type XREventType =
    | 'devicechange'
    | 'visibilitychange'
    | 'end'
    | 'inputsourceschange'
    | 'select'
    | 'selectstart'
    | 'selectend'
    | 'squeeze'
    | 'squeezestart'
    | 'squeezeend'
    | 'reset';

type XRPlaneSet = Set<XRPlane>;
type XRAnchorSet = Set<XRAnchor>;

type XREventHandler = EventHandlerNonNull;

interface XRLayer extends EventTarget { }

interface XRSessionEvent extends Event {
    readonly session: XRSession;
}

class XRWebGLLayer extends EventTarget implements XRLayer {
    static getNativeFramebufferScaleFactor(session: XRSession): number;
    constructor(session: XRSession, gl: WebGLRenderingContext | WebGL2RenderingContext | undefined, options?: XRWebGLLayerInit);
    readonly antialias: boolean;
    readonly ignoreDepthValues: boolean;
    framebuffer: WebGLFramebuffer;
    framebufferWidth: number;
    framebufferHeight: number;
    getViewport(view: XRView): XRViewport;
}

interface XRSpace extends EventTarget { }

interface XRRenderStateInit extends XRRenderState {
    baseLayer?: XRWebGLLayer;
    depthFar?: number;
    depthNear?: number;
    inlineVerticalFieldOfView?: number;
    layers?: XRLayer[];
}

interface XRBoundedReferenceSpace extends XRReferenceSpace {
    readonly boundsGeometry: DOMPointReadOnly[];
}

type XRInputSourceArray = XRInputSource[];

interface XRView {
    requestViewportScale(scale: number): void;
}

interface XRInputSourceChangeEvent extends Event {
    session: XRSession;
    removed: XRInputSource[];
    added: XRInputSource[];
}

// Experimental/Draft features

interface XRHand extends Iterable<XRJointSpace> {
    readonly length: number;

    [index: number]: XRJointSpace;

    readonly WRIST: number;

    readonly THUMB_METACARPAL: number;
    readonly THUMB_PHALANX_PROXIMAL: number;
    readonly THUMB_PHALANX_DISTAL: number;
    readonly THUMB_PHALANX_TIP: number;

    readonly INDEX_METACARPAL: number;
    readonly INDEX_PHALANX_PROXIMAL: number;
    readonly INDEX_PHALANX_INTERMEDIATE: number;
    readonly INDEX_PHALANX_DISTAL: number;
    readonly INDEX_PHALANX_TIP: number;

    readonly MIDDLE_METACARPAL: number;
    readonly MIDDLE_PHALANX_PROXIMAL: number;
    readonly MIDDLE_PHALANX_INTERMEDIATE: number;
    readonly MIDDLE_PHALANX_DISTAL: number;
    readonly MIDDLE_PHALANX_TIP: number;

    readonly RING_METACARPAL: number;
    readonly RING_PHALANX_PROXIMAL: number;
    readonly RING_PHALANX_INTERMEDIATE: number;
    readonly RING_PHALANX_DISTAL: number;
    readonly RING_PHALANX_TIP: number;

    readonly LITTLE_METACARPAL: number;
    readonly LITTLE_PHALANX_PROXIMAL: number;
    readonly LITTLE_PHALANX_INTERMEDIATE: number;
    readonly LITTLE_PHALANX_DISTAL: number;
    readonly LITTLE_PHALANX_TIP: number;
}

type XRTextureType = "texture" | "texture-array";

type XRLayerLayout = "default" | "mono" | "stereo" | "stereo-left-right" | "stereo-top-bottom";

interface XRLayerEventInit extends EventInit {
    layer: XRLayer;
}

interface XRLayerEvent extends Event {
    constructor(type: string, eventInitDict: XRLayerEventInit);
    readonly layer: XRLayer;
}

interface XRRedrawEventMap {
    "redraw": XRLayerEvent;
}

interface XRCompositionLayer extends XRLayer {
    readonly layout: XRLayerLayout;
    blendTextureSourceAlpha: boolean;
    chromaticAberrationCorrection: boolean;
    readonly mipLevels: number;
    readonly needsRedraw: boolean;
    destroy(): void;
}

interface _XRCompositionLayer extends XRCompositionLayer, EventTarget {
    space: XRSpace;

    // Events
    onredraw: (evt: XRRedrawEventMap[K]) => any;
    addEventListener<K extends keyof XRRedrawEventMap>(type: K, callback: (evt: XRRedrawEventMap[K]) => any, options?: boolean | AddEventListenerOptions);
    removeEventListener<K extends keyof XRRedrawEventMap>(type: K, callback: (evt: XRRedrawEventMap[K]) => any);
}

interface XRProjectionLayerInit {
    textureType?: XRTextureType;
    colorFormat?: GLenum;
    depthFormat?: GLenum;
    scaleFactor?: number;
}

interface XRProjectionLayer extends XRCompositionLayer {
    readonly textureWidth: number;
    readonly textureHeight: number;
    readonly textureArrayLength: number;
    readonly ignoreDepthValues: number;
    fixedFoveation: number;
}


interface XRLayerInit {
    space: XRSpace;
    colorFormat?: GLenum;
    depthFormat?: GLenum;
    mipLevels?: number;
    viewPixelWidth: number;
    viewPixelHeight: number;
    layout?: XRLayerLayout;
    isStatic?: boolean;
}

interface XRCylinderLayerInit extends XRLayerInit {
    textureType?: XRTextureType;
    transform?: XRRigidTransform;
    radius?: number;
    centralAngle?: number;
    aspectRatio?: number;
}

interface XRCylinderLayer extends _XRCompositionLayer {
    transform: XRRigidTransform;
    radius: number;
    centralAngle: number;
    aspectRatio: number;
}

interface XRQuadLayerInit extends XRLayerInit {
    textureType?: XRTextureType;
    transform?: XRRigidTransform;
    width?: number;
    height?: number;
}

interface XRQuadLayer extends _XRCompositionLayer {
    transform: XRRigidTransform;

    width: number;
    height: number;
}

interface XREquirectLayerInit extends XRLayerInit {
    textureType?: XRTextureType;
    transform?: XRRigidTransform;
    radius?: number;
    centralHorizontalAngle?: number;
    upperVerticalAngle?: number;
    lowerVerticalAngle?: number;
}

interface XREquirectLayer extends _XRCompositionLayer {
    transform: XRRigidTransform;
    radius: number;
    centralHorizontalAngle: number;
    upperVerticalAngle: number;
    lowerVerticalAngle: number;
}

interface XRCubeLayerInit extends XRLayerInit {
    orientation?: DOMPointReadOnly;
}

interface XRCubeLayer extends _XRCompositionLayer {
    orientation: DOMPointReadOnly;
}

interface XRSubImage {
    readonly viewport: XRViewport;
}

interface XRWebGLSubImage extends XRSubImage {
    readonly colorTexture: WebGLTexture;
    readonly depthStencilTexture: WebGLTexture;
    readonly imageIndex: number;
    readonly textureWidth: number;
    readonly textureHeight: number;
}

class XRWebGLBinding {
    constructor(session: XRSession, context: WebGLRenderingContext);

    readonly nativeProjectionScaleFactor: number;

    createProjectionLayer(init?: XRProjectionLayerInit): XRProjectionLayer;
    createQuadLayer(init?: XRQuadLayerInit): XRQuadLayer;
    createCylinderLayer(init?: XRCylinderLayerInit): XRCylinderLayer;
    createEquirectLayer(init?: XREquirectLayerInit): XREquirectLayer;
    createCubeLayer(init?: XRCubeLayerInit): XRCubeLayer;

    getSubImage(layer: XRCompositionLayer, frame: XRFrame, eye: XREye = "none"): XRWebGLSubImage;
    getViewSubImage(layer: XRProjectionLayer, view: XRView): XRWebGLSubImage;
}

type XRAnimationLoopCallback = (time: number, frame?: XRFrame) => void;

type XRFrameRequestCallback = (time: number, frame: XRFrame) => void;

interface XRSystem extends EventTarget {
    requestSession(mode: XRSessionMode, options?: XRSessionInit): Promise<XRSession>;
    isSessionSupported(mode: XRSessionMode): Promise<boolean>;
}

interface XRReferenceSpace extends EventTarget {
    getOffsetReferenceSpace(originOffset: XRRigidTransform): XRReferenceSpace;
}
interface XRHitTestOptionsInit {
    space: EventTarget;
    offsetRay?: XRRay | undefined;
}

interface XRTransientInputHitTestOptionsInit {
    profile: string;
    offsetRay?: XRRay | undefined;
}

interface XRViewport {
    readonly x: number;
    readonly y: number;
    readonly width: number;
    readonly height: number;
}

interface XRRenderState {
    readonly depthNear: number;
    readonly depthFar: number;
    readonly inlineVerticalFieldOfView?: number | undefined;
    readonly baseLayer?: XRWebGLLayer | undefined;
}

interface XRRenderStateInit {
    depthNear?: number | undefined;
    depthFar?: number | undefined;
    inlineVerticalFieldOfView?: number | undefined;
    baseLayer?: XRWebGLLayer | undefined;
}

interface XRInputSource {
    readonly handedness: XRHandedness;
    readonly targetRayMode: XRTargetRayMode;
    readonly targetRaySpace: EventTarget;
    readonly gripSpace?: EventTarget | undefined;
    readonly profiles: string[];
    readonly gamepad: Gamepad;
    readonly hand?: XRHand | undefined;
}

interface XRSessionInit {
    optionalFeatures?: string[] | undefined;
    requiredFeatures?: string[] | undefined;
}

interface XRSession extends EventTarget {
    /**
     * Returns a list of this session's XRInputSources, each representing an input device
     * used to control the camera and/or scene.
     */
    readonly inputSources: XRInputSource[];
    /**
     * object which contains options affecting how the imagery is rendered.
     * This includes things such as the near and far clipping planes
     */
    readonly renderState: XRRenderState;
    readonly environmentBlendMode: XREnvironmentBlendMode;
    readonly visibilityState: XRVisibilityState;
    readonly frameRate?: number;
    readonly supportedFrameRates?: Float32Array;

    updateTargetFrameRate(rate: number): Promise<void>;

    onend: XREventHandler;
    oninputsourceschange: XREventHandler;
    onselect: XREventHandler;
    onselectstart: XREventHandler;
    onselectend: XREventHandler;
    onsqueeze: XREventHandler;
    onsqueezestart: XREventHandler;
    onsqueezeend: XREventHandler;
    onvisibilitychange: XREventHandler;
    onframeratechange: XREventHandler;

    requestReferenceSpace(type: XRReferenceSpaceType): Promise<XRReferenceSpace>;
    updateRenderState(renderStateInit: XRRenderStateInit): Promise<void>;
    requestAnimationFrame(callback: XRFrameRequestCallback): number;
    cancelAnimationFrame(id: number): void;
    end(): Promise<void>;
    renderState: XRRenderState;
    inputSources: XRInputSource[];
    environmentBlendMode: XREnvironmentBlendMode;
    visibilityState: XRVisibilityState;

    // hit test
    requestHitTestSource(options: XRHitTestOptionsInit): Promise<XRHitTestSource>;
    requestHitTestSourceForTransientInput(
        options: XRTransientInputHitTestOptionsInit,
    ): Promise<XRTransientInputHitTestSource>;

    // legacy AR hit test
    requestHitTest(ray: XRRay, referenceSpace: XRReferenceSpace): Promise<XRHitResult[]>;

    // legacy plane detection
    updateWorldTrackingState(options: { planeDetectionState?: { enabled: boolean } | undefined }): void;
}

interface XRReferenceSpace extends EventTarget {
    getOffsetReferenceSpace(originOffset: XRRigidTransform): XRReferenceSpace;
    onreset: any;
}

type XRPlaneSet = Set<XRPlane>;
type XRAnchorSet = Set<XRAnchor>;

interface XRFrame {
    readonly session: XRSession;
    getViewerPose(referenceSpace: XRReferenceSpace): XRViewerPose | undefined;
    getPose(space: EventTarget, baseSpace: EventTarget): XRPose | undefined;

    // AR
    getHitTestResults(hitTestSource: XRHitTestSource): XRHitTestResult[];
    getHitTestResultsForTransientInput(hitTestSource: XRTransientInputHitTestSource): XRTransientInputHitTestResult[];
    // Anchors
    trackedAnchors?: XRAnchorSet | undefined;
    createAnchor(pose: XRRigidTransform, space: EventTarget): Promise<XRAnchor>;
    // Planes
    worldInformation: {
        detectedPlanes?: XRPlaneSet | undefined;
    };
    // Hand tracking
    getJointPose(joint: XRJointSpace, baseSpace: EventTarget): XRJointPose;
}

interface XRViewerPose {
    readonly transform: XRRigidTransform;
    readonly views: XRView[];
}

interface XRPose {
    readonly emulatedPosition: boolean;
    readonly transform: XRRigidTransform;
}

interface XRWebGLLayerInit {
    antialias?: boolean | undefined;
    depth?: boolean | undefined;
    stencil?: boolean | undefined;
    alpha?: boolean | undefined;
    ignoreDepthValues?: boolean | undefined;
    framebufferScaleFactor?: number | undefined;
}

interface DOMPointInit {
    w?: number | undefined;
    x?: number | undefined;
    y?: number | undefined;
    z?: number | undefined;
}

class XRRigidTransform {
    constructor(matrix: Float32Array | DOMPointInit, direction?: DOMPointInit);
    position: DOMPointReadOnly;
    orientation: DOMPointReadOnly;
    matrix: Float32Array;
    inverse: XRRigidTransform;
}

interface XRView {
    readonly eye: XREye;
    readonly projectionMatrix: Float32Array;
    readonly viewMatrix: Float32Array;
    readonly transform: XRRigidTransform;
}

interface XRRayDirectionInit {
    x?: number | undefined;
    y?: number | undefined;
    z?: number | undefined;
    w?: number | undefined;
}

class XRRay {
    readonly origin: DOMPointReadOnly;
    readonly direction: XRRayDirectionInit;
    matrix: Float32Array;

    constructor(transformOrOrigin: XRRigidTransform | DOMPointInit, direction?: XRRayDirectionInit);
}

enum XRHitTestTrackableType {
    'point',
    'plane',
    'mesh',
}

interface XRHitResult {
    hitMatrix: Float32Array;
}

interface XRTransientInputHitTestResult {
    readonly inputSource: XRInputSource;
    readonly results: XRHitTestResult[];
}

interface XRHitTestResult {
    getPose(baseSpace: EventTarget): XRPose | undefined | null;
    // When anchor system is enabled
    createAnchor?(pose: XRRigidTransform): Promise<XRAnchor>;
}

interface XRHitTestSource {
    cancel(): void;
}

interface XRTransientInputHitTestSource {
    cancel(): void;
}

interface XRHitTestOptionsInit {
    space: EventTarget;
    entityTypes?: XRHitTestTrackableType[] | undefined;
    offsetRay?: XRRay | undefined;
}

interface XRTransientInputHitTestOptionsInit {
    profile: string;
    entityTypes?: XRHitTestTrackableType[] | undefined;
    offsetRay?: XRRay | undefined;
}

interface XRAnchor {
    anchorSpace: EventTarget;
    delete(): void;
}

interface XRPlane {
    orientation: 'Horizontal' | 'Vertical';
    planeSpace: EventTarget;
    polygon: DOMPointReadOnly[];
    lastChangedTime: number;
}

enum XRHandJoint {
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
}

interface XRJointSpace extends EventTarget {
    readonly jointName: XRHandJoint;
}

interface XRJointPose extends XRPose {
    readonly radius: number | undefined;
}

interface XRHand extends Map<XRHandJoint, XRJointSpace> {
    readonly size: number;
}

interface XRInputSourceChangeEvent {
    session: XRSession;
    removed: XRInputSource[];
    added: XRInputSource[];
}

interface XRInputSourceEvent extends Event {
    readonly frame: XRFrame;
    readonly inputSource: XRInputSource;
}