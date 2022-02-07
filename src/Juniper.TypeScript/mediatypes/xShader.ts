import { specialize } from "./util";

const xShader = specialize("x-shader");

export const anyXShader = xShader("*");
export const XShader_XVertex = xShader("x-vertex", "vert", "vs", "glsl");
export const XShader_XFragment = xShader("x-fragment", "frag", "fs", "glsl");
export const allShader = [
    XShader_XVertex,
    XShader_XFragment
];
