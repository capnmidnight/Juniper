type XRFrameRequestCallback = (time: number, frame: XRFrame) => void;

interface XRSession extends EventTarget {
    requestAnimationFrame(callback: XRFrameRequestCallback): number;
    cancelAnimationFrame(id: number): void;
}

declare class XRSession {
    prototype: XRSession;
}

interface XRFrame {
}

declare class XRFrame {
    prototype: XRFrame;
}