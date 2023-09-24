#version 300 es

precision mediump float;

uniform mediump samplerCube uTexture;
uniform float uGamma;

in vec3 vDir;
out vec4 fragmentColor;

vec3 gammaCorrect(vec3 color) {
    return pow(color, vec3(uGamma));
}

void main() {
    vec3 vUVW = normalize(vec3(-vDir.x, vDir.yz));
    vec3 color = texture(uTexture, vUVW).rgb;
    vec3 rgb = gammaCorrect(color);
    fragmentColor = vec4(rgb, 1.0);
}