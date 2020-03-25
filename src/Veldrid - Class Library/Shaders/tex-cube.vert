#version 450

layout(set = 0, binding = 0) uniform ProjectionBuffer
{
    mat4 Projection;
    mat4 View;
    mat4 World;
};

layout(location = 0) in vec3 Position;
layout(location = 1) in vec3 Normal;
layout(location = 2) in vec2 TextureCoordinates;

layout(location = 0) out vec2 fsin_texCoords;

void main()
{
    gl_Position = Projection * View * World * vec4(Position, 1);
    fsin_texCoords = TextureCoordinates;
}