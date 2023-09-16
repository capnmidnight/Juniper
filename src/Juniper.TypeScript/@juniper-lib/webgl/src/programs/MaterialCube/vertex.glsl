#version 300 es

in vec3 aPosition;

uniform mat4 uProjection;
uniform mat4 uView;

out vec3 vDir;

void main() {
    gl_Position = uProjection * uView * vec4(aPosition, 1.0);
    vDir = normalize(aPosition);
}