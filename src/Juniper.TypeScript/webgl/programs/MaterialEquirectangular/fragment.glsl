#version 300 es

precision mediump float;

uniform mediump sampler2DArray uTexture;
uniform int uEye;
uniform float uGamma;

in vec3 vDir;
out vec4 fragmentColor;

const float pi = radians(180.0);
const float tau = radians(360.0);

vec3 gammaCorrect(vec3 color) {
    return pow(color, vec3(uGamma));
}

vec3 equirectUVW(vec3 dir) {
    float x = dir.x;
    float y = dir.y;
    float z = -dir.z;

    float r = sqrt(x * x + y * y + z * z);
    float lat = asin(y / r);
    float lon = atan(z, x);

    x = 0.5 + lon / tau;
    y = 0.5 - lat / pi;

    if (uEye != 0) {
        z = 0.0;
    }

    if (uEye < 0) {
        z = 1.0;
    }

    return vec3(x, y, z);

}

void main() {
    vec3 vUVW = equirectUVW(vDir);
    vec3 color = texture(uTexture, vUVW).rgb;
    vec3 rgb = gammaCorrect(color);
    fragmentColor = vec4(rgb, 1.0);
}