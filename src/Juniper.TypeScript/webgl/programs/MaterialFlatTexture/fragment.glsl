#version 300 es

precision mediump float;

in vec2 vUV;
  
uniform sampler2D uTexture;

out vec4 fragmentColor;
  
vec3 gammaCorrect(vec3 color){
    return pow(color, vec3(1.0/2.2));
}
  
void main() {
    vec4 color = texture(uTexture, vUV);
    fragmentColor = vec4(gammaCorrect(color.rgb), color.a);
}