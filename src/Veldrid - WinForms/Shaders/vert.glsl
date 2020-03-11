#version 450

layout(set = 0, binding = 0) uniform ProjectionBuffer
{
    mat4 Projection;
};

layout(set = 0, binding = 1) uniform ViewBuffer
{
    mat4 View;
};

layout(set = 1, binding = 0) uniform WorldBuffer
{
    mat4 World;
};

layout(location = 0) in vec3 Position;
layout(location = 1) in vec2 UV;

layout(location = 0) out vec2 fsin_texCoords;

void main()
{
    gl_Position = Projection * View * World * vec4(Position, 1);
    fsin_texCoords = UV;
}