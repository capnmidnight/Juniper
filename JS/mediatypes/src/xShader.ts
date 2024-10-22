import { specialize } from "./util";

const xShader = /*@__PURE__*/ (function() { return specialize("x-shader"); })();

export const XShader_XVertex = /*@__PURE__*/ (function() { return xShader("x-vertex", "vert", "vs"); })();
export const XShader_XFragment = /*@__PURE__*/ (function() { return xShader("x-fragment", "frag", "fs"); })();
export const XShader_XGLSL = /*@__PURE__*/ (function() { return xShader("x-glsl", "glsl"); })();
