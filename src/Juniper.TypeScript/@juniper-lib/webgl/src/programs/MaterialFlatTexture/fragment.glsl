#version 300 es

precision mediump float;
  
uniform sampler2D uTexture;
uniform float uGamma;

in vec2 vUV;
out vec4 fragmentColor;
  
vec3 gammaCorrect(vec3 color) {
    return pow(color, vec3(uGamma));
}
  
void main() {
    vec4 color = texture(uTexture, vUV);
    vec3 rgb = gammaCorrect(color.rgb);
    fragmentColor = vec4(rgb, color.a);
}