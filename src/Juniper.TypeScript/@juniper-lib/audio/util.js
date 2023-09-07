import { isFunction } from "@juniper-lib/tslib/typeChecks";
export const hasAudioContext = /*@__PURE__*/ "AudioContext" in globalThis;
export const hasAudioListener = /*@__PURE__*/ hasAudioContext && "AudioListener" in globalThis;
export const hasOldAudioListener = /*@__PURE__*/ hasAudioListener && "setPosition" in AudioListener.prototype;
export const hasNewAudioListener = /*@__PURE__*/ hasAudioListener && "positionX" in AudioListener.prototype;
export const hasStreamSources = /*@__PURE__*/ "createMediaStreamSource" in AudioContext.prototype;
export const canCaptureStream = /*@__PURE__*/ isFunction(HTMLMediaElement.prototype.captureStream)
    || isFunction(HTMLMediaElement.prototype.mozCaptureStream);
export function isAudioContext(context) {
    return context instanceof BaseAudioContext;
}
//# sourceMappingURL=util.js.map