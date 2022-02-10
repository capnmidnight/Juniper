import { isFunction, isOculusBrowser, oculusBrowserVersion } from "juniper-tslib";
import type { BaseEnvironment } from "./environment/BaseEnvironment";

export function hasWebXRLayers(env: BaseEnvironment<unknown>) {
    return env.renderer.getContextAttributes().alpha
        && "XRWebGLBinding" in globalThis
        && isFunction(XRWebGLBinding.prototype.createCubeLayer)
        && !(isOculusBrowser && oculusBrowserVersion.major <= 15);
}
