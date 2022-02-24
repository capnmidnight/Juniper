#version 300 es

in vec3 aPosition;

uniform mat4 uProjection;
uniform mat4 uView;

out vec3 vDir;

void main() {
    vec4 viewPos = uView * vec4(aPosition, 0.0);
    gl_Position = uProjection * vec4(viewPos.xyz, 1.0);
    gl_Position = gl_Position.xyww;
    vDir = normalize(aPosition);
}