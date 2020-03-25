#version 450
layout(location = 0) in vec2 fsin_texCoords;
layout(location = 0) out vec4 fsout_color;

layout(set = 0, binding = 1) uniform sampler DiffuseSampler;
layout(set = 1, binding = 0) uniform texture2D DiffuseTexture;

void main()
{
    fsout_color =  texture(sampler2D(DiffuseTexture, DiffuseSampler), fsin_texCoords);
}