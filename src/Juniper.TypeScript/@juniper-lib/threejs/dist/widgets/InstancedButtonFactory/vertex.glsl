//MY VERTEX SHADER
#define THREEZ
#ifdef THREEZ
uniform mat4 projectionMatrix;
uniform mat4 modelViewMatrix;
attribute mat4 instanceMatrix;
attribute vec4 instanceColor;
attribute vec2 uv;
attribute vec3 position;
#endif

varying vec2 vUv;
void main(){
  vUv = instanceColor.rg + (uv * instanceColor.b);
  gl_Position = projectionMatrix * modelViewMatrix * instanceMatrix * vec4( position, 1.0);
}