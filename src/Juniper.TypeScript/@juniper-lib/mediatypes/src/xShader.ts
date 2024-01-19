import { specialize } from "./util";

const xShader = /*@__PURE__*/ specialize("x-shader");

export const XShader_XVertex = /*@__PURE__*/ xShader("x-vertex", "vert", "vs");
export const XShader_XFragment = /*@__PURE__*/ xShader("x-fragment", "frag", "fs");
export const XShader_XGLSL = /*@__PURE__*/ xShader("x-glsl", "glsl");
export const XShader_XWGSL = /*@__PURE__*/ xShader("x-wgsl", "wgsl");