/**
 * Constants passed to WebGLRenderingContext.clear() to clear buffer masks.;
 **/
export declare enum ClearBits {
    DEPTH_BUFFER_BIT = 256,// Passed to clear to clear the current depth buffer.
    STENCIL_BUFFER_BIT = 1024,// Passed to clear to clear the current stencil buffer.
    COLOR_BUFFER_BIT = 16384
}
/**
 * Constants passed to WebGLRenderingContext.drawElements() or WebGLRenderingContext.drawArrays() to specify what kind of primitive to render.;
 **/
export declare enum DrawTypes {
    POINTS = 0,//Passed to drawElements or drawArrays to draw single points.
    LINES = 1,//Passed to drawElements or drawArrays to draw lines.Each vertex connects to the one after it.
    LINE_LOOP = 2,//Passed to drawElements or drawArrays to draw lines.Each set of two vertices is treated as a separate line segment.
    LINE_STRIP = 3,//Passed to drawElements or drawArrays to draw a connected group of line segments from the first vertex to the last.
    TRIANGLES = 4,//Passed to drawElements or drawArrays to draw triangles.Each set of three vertices creates a separate triangle.
    TRIANGLE_STRIP = 5,//Passed to drawElements or drawArrays to draw a connected group of triangles.
    TRIANGLE_FAN = 6
}
/**
 * Constants passed to WebGLRenderingContext.blendFunc() or WebGLRenderingContext.blendFuncSeparate() to specify the blending mode(for both, RBG and alpha, or separately).
 **/
export declare enum BlendFunctionMode {
    ZERO = 0,//Passed to blendFunc or blendFuncSeparate to turn off a component.
    ONE = 1,//Passed to blendFunc or blendFuncSeparate to turn on a component.
    SRC_COLOR = 768,//Passed to blendFunc or blendFuncSeparate to multiply a component by the source elements color.
    ONE_MINUS_SRC_COLOR = 769,//Passed to blendFunc or blendFuncSeparate to multiply a component by one minus the source elements color.
    SRC_ALPHA = 770,//Passed to blendFunc or blendFuncSeparate to multiply a component by the source's alpha.;
    ONE_MINUS_SRC_ALPHA = 771,//Passed to blendFunc or blendFuncSeparate to multiply a component by one minus the source's alpha.;
    DST_ALPHA = 772,//Passed to blendFunc or blendFuncSeparate to multiply a component by the destination's alpha.;
    ONE_MINUS_DST_ALPHA = 773,//Passed to blendFunc or blendFuncSeparate to multiply a component by one minus the destination's alpha.;
    DST_COLOR = 774,//Passed to blendFunc or blendFuncSeparate to multiply a component by the destination's color.;
    ONE_MINUS_DST_COLOR = 775,//Passed to blendFunc or blendFuncSeparate to multiply a component by one minus the destination's color.;
    SRC_ALPHA_SATURATE = 776,//Passed to blendFunc or blendFuncSeparate to multiply a component by the minimum of source's alpha or one minus the destination's alpha.
    CONSTANT_COLOR = 32769,//Passed to blendFunc or blendFuncSeparate to specify a constant color blend function.
    ONE_MINUS_CONSTANT_COLOR = 32770,//Passed to blendFunc or blendFuncSeparate to specify one minus a constant color blend function.
    CONSTANT_ALPHA = 32771,//Passed to blendFunc or blendFuncSeparate to specify a constant alpha blend function.
    ONE_MINUS_CONSTANT_ALPHA = 32772
}
/**
 * Constants passed to WebGLRenderingContext.blendEquation() or WebGLRenderingContext.blendEquationSeparate() to control how the blending is calculated(for both, RBG and alpha, or separately).
 **/
export declare enum BlendEquationMode {
    FUNC_ADD = 32774,//Passed to blendEquation or blendEquationSeparate to set an addition blend function.
    FUNC_SUBTRACT = 32778,//Passed to blendEquation or blendEquationSeparate to specify a subtraction blend function (source - destination).
    FUNC_REVERSE_SUBTRACT = 32779,//Passed to blendEquation or blendEquationSeparate to specify a reverse subtraction blend function (destination - source).
    MIN = 32775,
    MAX = 32776
}
/**
 * Constants passed to WebGLRenderingContext.getParameter() to specify what information to return.;
 **/
export declare enum InfoParamType {
    BLEND_EQUATION = 32777,//Passed to getParameter to get the current RGB blend function.
    BLEND_EQUATION_RGB = 32777,//Passed to getParameter to get the current RGB blend function.Same as BLEND_EQUATION;
    BLEND_EQUATION_ALPHA = 34877,//Passed to getParameter to get the current alpha blend function.Same as BLEND_EQUATION;
    BLEND_DST_RGB = 32968,//Passed to getParameter to get the current destination RGB blend function.
    BLEND_SRC_RGB = 32969,//Passed to getParameter to get the current destination RGB blend function.
    BLEND_DST_ALPHA = 32970,//Passed to getParameter to get the current destination alpha blend function.
    BLEND_SRC_ALPHA = 32971,//Passed to getParameter to get the current source alpha blend function.
    BLEND_COLOR = 32773,//Passed to getParameter to return a the current blend color.
    ARRAY_BUFFER_BINDING = 34964,//Passed to getParameter to get the array buffer binding.
    ELEMENT_ARRAY_BUFFER_BINDING = 34965,//Passed to getParameter to get the current element array buffer.
    LINE_WIDTH = 2849,//Passed to getParameter to get the current lineWidth(set by the lineWidth method).
    ALIASED_POINT_SIZE_RANGE = 33901,//Passed to getParameter to get the current size of a point drawn with gl.POINTS
    ALIASED_LINE_WIDTH_RANGE = 33902,//Passed to getParameter to get the range of available widths for a line.Returns a length - 2 array with the lo value at 0, and hight at 1.;
    CULL_FACE_MODE = 2885,//Passed to getParameter to get the current value of cullFace.Should return FRONT, BACK, or FRONT_AND_BACK;
    FRONT_FACE = 2886,//Passed to getParameter to determine the current value of frontFace.Should return CW or CCW.
    DEPTH_RANGE = 2928,//Passed to getParameter to return a length - 2 array of floats giving the current depth range.
    DEPTH_WRITEMASK = 2930,//Passed to getParameter to determine if the depth write mask is enabled.
    DEPTH_CLEAR_VALUE = 2931,//Passed to getParameter to determine the current depth clear value.
    DEPTH_FUNC = 2932,//Passed to getParameter to get the current depth function.Returns NEVER, ALWAYS, LESS, EQUAL, LEQUAL, GREATER, GEQUAL, or NOTEQUAL.
    STENCIL_CLEAR_VALUE = 2961,//Passed to getParameter to get the value the stencil will be cleared to.
    STENCIL_FUNC = 2962,//Passed to getParameter to get the current stencil function.Returns NEVER, ALWAYS, LESS, EQUAL, LEQUAL, GREATER, GEQUAL, or NOTEQUAL.
    STENCIL_FAIL = 2964,//Passed to getParameter to get the current stencil fail function.Should return KEEP, REPLACE, INCR, DECR, INVERT, INCR_WRAP, or DECR_WRAP.
    STENCIL_PASS_DEPTH_FAIL = 2965,//Passed to getParameter to get the current stencil fail function should the depth buffer test fail.Should return KEEP, REPLACE, INCR, DECR, INVERT, INCR_WRAP, or DECR_WRAP.
    STENCIL_PASS_DEPTH_PASS = 2966,//Passed to getParameter to get the current stencil fail function should the depth buffer test pass.Should return KEEP, REPLACE, INCR, DECR, INVERT, INCR_WRAP, or DECR_WRAP.
    STENCIL_REF = 2967,//Passed to getParameter to get the reference value used for stencil tests.
    STENCIL_VALUE_MASK = 2963,//STENCIL_WRITEMASK  0x0B98;
    STENCIL_BACK_FUNC = 34816,
    STENCIL_BACK_FAIL = 34817,
    STENCIL_BACK_PASS_DEPTH_FAIL = 34818,
    STENCIL_BACK_PASS_DEPTH_PASS = 34819,
    STENCIL_BACK_REF = 36003,
    STENCIL_BACK_VALUE_MASK = 36004,
    STENCIL_BACK_WRITEMASK = 36005,
    VIEWPORT = 2978,//Returns an Int32Array with four elements for the current viewport dimensions.
    SCISSOR_BOX = 3088,//Returns an Int32Array with four elements for the current scissor box dimensions.
    COLOR_CLEAR_VALUE = 3106,
    COLOR_WRITEMASK = 3107,
    UNPACK_ALIGNMENT = 3317,
    PACK_ALIGNMENT = 3333,
    MAX_TEXTURE_SIZE = 3379,
    MAX_VIEWPORT_DIMS = 3386,
    SUBPIXEL_BITS = 3408,
    RED_BITS = 3410,
    GREEN_BITS = 3411,
    BLUE_BITS = 3412,
    ALPHA_BITS = 3413,
    DEPTH_BITS = 3414,
    STENCIL_BITS = 3415,
    POLYGON_OFFSET_UNITS = 10752,
    POLYGON_OFFSET_FACTOR = 32824,
    TEXTURE_BINDING_2D = 32873,
    SAMPLE_BUFFERS = 32936,
    SAMPLES = 32937,
    SAMPLE_COVERAGE_VALUE = 32938,
    SAMPLE_COVERAGE_INVERT = 32939,
    COMPRESSED_TEXTURE_FORMATS = 34467,
    VENDOR = 7936,
    RENDERER = 7937,
    VERSION = 7938,
    IMPLEMENTATION_COLOR_READ_TYPE = 35738,
    IMPLEMENTATION_COLOR_READ_FORMAT = 35739,
    BROWSER_DEFAULT_WEBGL = 37444,
    READ_BUFFER = 3074,
    UNPACK_ROW_LENGTH = 3314,
    UNPACK_SKIP_ROWS = 3315,
    UNPACK_SKIP_PIXELS = 3316,
    PACK_ROW_LENGTH = 3330,
    PACK_SKIP_ROWS = 3331,
    PACK_SKIP_PIXELS = 3332,
    TEXTURE_BINDING_3D = 32874,
    UNPACK_SKIP_IMAGES = 32877,
    UNPACK_IMAGE_HEIGHT = 32878,
    MAX_3D_TEXTURE_SIZE = 32883,
    MAX_ELEMENTS_VERTICES = 33000,
    MAX_ELEMENTS_INDICES = 33001,
    MAX_TEXTURE_LOD_BIAS = 34045,
    MAX_FRAGMENT_UNIFORM_COMPONENTS = 35657,
    MAX_VERTEX_UNIFORM_COMPONENTS = 35658,
    MAX_ARRAY_TEXTURE_LAYERS = 35071,
    MIN_PROGRAM_TEXEL_OFFSET = 35076,
    MAX_PROGRAM_TEXEL_OFFSET = 35077,
    MAX_VARYING_COMPONENTS = 35659,
    FRAGMENT_SHADER_DERIVATIVE_HINT = 35723,
    RASTERIZER_DISCARD = 35977,
    VERTEX_ARRAY_BINDING = 34229,
    MAX_VERTEX_OUTPUT_COMPONENTS = 37154,
    MAX_FRAGMENT_INPUT_COMPONENTS = 37157,
    MAX_SERVER_WAIT_TIMEOUT = 37137,
    MAX_ELEMENT_INDEX = 36203
}
/**
 * Constants passed to WebGLRenderingContext.bufferData(), WebGLRenderingContext.bufferSubData(), WebGLRenderingContext.bindBuffer(), or WebGLRenderingContext.getBufferParameter().;
 **/
export declare enum XBufferType {
    ARRAY_BUFFER = 34962,//Passed to bindBuffer or bufferData to specify the type of buffer being used.
    ELEMENT_ARRAY_BUFFER = 34963,//Passed to bindBuffer or bufferData to specify the type of buffer being used.
    PIXEL_PACK_BUFFER = 35051,
    PIXEL_UNPACK_BUFFER = 35052,
    PIXEL_PACK_BUFFER_BINDING = 35053,
    PIXEL_UNPACK_BUFFER_BINDING = 35055,
    COPY_READ_BUFFER = 36662,
    COPY_WRITE_BUFFER = 36663,
    COPY_READ_BUFFER_BINDING = 36662,
    COPY_WRITE_BUFFER_BINDING = 36663
}
export declare enum BufferUsage {
    STATIC_DRAW = 35044,//Passed to bufferData as a hint about whether the contents of the buffer are likely to be used often and not change often.
    STREAM_DRAW = 35040,//Passed to bufferData as a hint about whether the contents of the buffer are likely to not be used often.
    DYNAMIC_DRAW = 35048,//Passed to bufferData as a hint about whether the contents of the buffer are likely to be used often and change often.
    STREAM_READ = 35041,// The contents are intended to be specified once by reading data from WebGL, and queried at most a few times by the application
    STREAM_COPY = 35042,// The contents are intended to be specified once by reading data from WebGL, and used at most a few times as the source for WebGL drawing and image specification commands.
    STATIC_READ = 35045,// The contents are intended to be respecified repeatedly by reading data from WebGL, and queried many times by the application.
    STATIC_COPY = 35046,// The contents are intended to be specified once by reading data from WebGL, and used many times as the source for WebGL drawing and image specification commands.
    DYNAMIC_READ = 35049,// The contents are intended to be respecified repeatedly by reading data from WebGL, and queried many times by the application.
    DYNAMIC_COPY = 35050
}
export declare enum BufferTarget {
    ARRAY_BUFFER = 34962,//Passed to bindBuffer or bufferData to specify the type of buffer being used.
    ELEMENT_ARRAY_BUFFER = 34963,//Passed to bindBuffer or bufferData to specify the type of buffer being used.
    PIXEL_PACK_BUFFER = 35051,
    PIXEL_UNPACK_BUFFER = 35052,
    TRANSFORM_FEEDBACK_BUFFER = 35982,
    UNIFORM_BUFFER = 35345,
    COPY_READ_BUFFER = 36662,
    COPY_WRITE_BUFFER = 36663
}
export declare enum BufferParam {
    BUFFER_SIZE = 34660,//Passed to getBufferParameter to get a buffer's size.;
    BUFFER_USAGE = 34661
}
/**
 * Constants passed to WebGLRenderingContext.getVertexAttrib().;
 **/
export declare enum VertexAttribType {
    CURRENT_VERTEX_ATTRIB = 34342,//Passed to getVertexAttrib to read back the current vertex attribute.
    VERTEX_ATTRIB_ARRAY_ENABLED = 34338,
    VERTEX_ATTRIB_ARRAY_SIZE = 34339,
    VERTEX_ATTRIB_ARRAY_STRIDE = 34340,
    VERTEX_ATTRIB_ARRAY_TYPE = 34341,
    VERTEX_ATTRIB_ARRAY_NORMALIZED = 34922,
    VERTEX_ATTRIB_ARRAY_POINTER = 34373,
    VERTEX_ATTRIB_ARRAY_BUFFER_BINDING = 34975,
    VERTEX_ATTRIB_ARRAY_INTEGER = 35069,
    VERTEX_ATTRIB_ARRAY_DIVISOR = 35070
}
/**
 * Constants passed to WebGLRenderingContext.cullFace().;
 **/
export declare enum CullingMode {
    CULL_FACE = 2884,//Passed to enable / disable to turn on / off culling.Can also be used with getParameter to find the current culling method.
    FRONT = 1028,//Passed to cullFace to specify that only front faces should be culled.
    BACK = 1029,//Passed to cullFace to specify that only back faces should be culled.
    FRONT_AND_BACK = 1032
}
/**
 * Constants passed to WebGLRenderingContext.enable() or WebGLRenderingContext.disable().;
 **/
export declare enum GLMode {
    BLEND = 3042,//Passed to enable / disable to turn on / off blending.Can also be used with getParameter to find the current blending method.
    DEPTH_TEST = 2929,//Passed to enable / disable to turn on / off the depth test.Can also be used with getParameter to query the depth test.
    DITHER = 3024,//Passed to enable / disable to turn on / off dithering.Can also be used with getParameter to find the current dithering method.
    POLYGON_OFFSET_FILL = 32823,//Passed to enable / disable to turn on / off the polygon offset.Useful for rendering hidden - line images, decals, and or solids with highlighted edges.Can also be used with getParameter to query the scissor test.
    SAMPLE_ALPHA_TO_COVERAGE = 32926,//Passed to enable / disable to turn on / off the alpha to coverage.Used in multi - sampling alpha channels.
    SAMPLE_COVERAGE = 32928,//Passed to enable / disable to turn on / off the sample coverage.Used in multi - sampling.
    SCISSOR_TEST = 3089,//Passed to enable / disable to turn on / off the scissor test.Can also be used with getParameter to query the scissor test.
    STENCIL_TEST = 2960
}
/**
 * Constants returned from WebGLRenderingContext.getError().;
 **/
export declare enum GLError {
    NO_ERROR = 0,//Returned from getError.
    INVALID_ENUM = 1280,//Returned from getError.
    INVALID_VALUE = 1281,//Returned from getError.
    INVALID_OPERATION = 1282,//Returned from getError.
    OUT_OF_MEMORY = 1285,//Returned from getError.
    CONTEXT_LOST_WEBGL = 37442
}
/**
 * Constants passed to WebGLRenderingContext.frontFace().;
 **/
export declare enum FrontFaceDirection {
    CW = 2304,//Passed to frontFace to specify the front face of a polygon is drawn in the clockwise direction;
    CCW = 2305
}
/**
 * Constants passed to WebGLRenderingContext.hint();
 **/
export declare enum Hint {
    DONT_CARE = 4352,//There is no preference for this behavior.
    FASTEST = 4353,//The most efficient behavior should be used.
    NICEST = 4354,//The most correct or the highest quality option should be used.
    GENERATE_MIPMAP_HINT = 33170
}
export declare enum DataType {
    BYTE = 5120,
    UNSIGNED_BYTE = 5121,
    SHORT = 5122,
    UNSIGNED_SHORT = 5123,
    INT = 5124,
    UNSIGNED_INT = 5125,
    FLOAT = 5126,
    FLOAT_MAT2x3 = 35685,
    FLOAT_MAT2x4 = 35686,
    FLOAT_MAT3x2 = 35687,
    FLOAT_MAT3x4 = 35688,
    FLOAT_MAT4x2 = 35689,
    FLOAT_MAT4x3 = 35690,
    UNSIGNED_INT_VEC2 = 36294,
    UNSIGNED_INT_VEC3 = 36295,
    UNSIGNED_INT_VEC4 = 36296,
    UNSIGNED_NORMALIZED = 35863,
    SIGNED_NORMALIZED = 36764
}
export declare enum PixelFormat {
    DEPTH_COMPONENT = 6402,
    ALPHA = 6406,
    RGB = 6407,
    RGBA = 6408,
    LUMINANCE = 6409,
    LUMINANCE_ALPHA = 6410
}
export declare enum PixelType {
    UNSIGNED_BYTE = 5121,
    UNSIGNED_SHORT_4_4_4_4 = 32819,
    UNSIGNED_SHORT_5_5_5_1 = 32820,
    UNSIGNED_SHORT_5_6_5 = 33635,
    UNSIGNED_INT_2_10_10_10_REV = 33640,
    UNSIGNED_INT_10F_11F_11F_REV = 35899,
    UNSIGNED_INT_5_9_9_9_REV = 35902,
    FLOAT_32_UNSIGNED_INT_24_8_REV = 36269,
    UNSIGNED_INT_24_8 = 34042,
    HALF_FLOAT = 5131,
    RG = 33319,
    RG_INTEGER = 33320,
    INT_2_10_10_10_REV = 36255
}
/**
 * Constants passed to WebGLRenderingContext.createShader() or WebGLRenderingContext.getShaderParameter();
 **/
export declare enum ShaderType {
    FRAGMENT_SHADER = 35632,//Passed to createShader to define a fragment shader.
    VERTEX_SHADER = 35633
}
export declare enum ShaderParamType {
    COMPILE_STATUS = 35713,//Passed to getShaderParameter to get the status of the compilation.Returns false if the shader was not compiled.You can then query getShaderInfoLog to find the exact error;
    DELETE_STATUS = 35712,//Passed to getShaderParameter to determine if a shader was deleted via deleteShader.Returns true if it was, false otherwise.
    SHADER_TYPE = 35663
}
export declare enum ProgramParamType {
    DELETE_STATUS = 35712,//Passed to getProgramParameter to determine if a shader was deleted via deleteShader.Returns true if it was, false otherwise.
    LINK_STATUS = 35714,//Passed to getProgramParameter after calling linkProgram to determine if a program was linked correctly.Returns false if there were errors.Use getProgramInfoLog to find the exact error.
    VALIDATE_STATUS = 35715,//Passed to getProgramParameter after calling validateProgram to determine if it is valid.Returns false if errors were found.
    ATTACHED_SHADERS = 35717,//Passed to getProgramParameter after calling attachShader to determine if the shader was attached correctly.Returns false if errors occurred.
    ACTIVE_ATTRIBUTES = 35721,//Passed to getProgramParameter to get the number of attributes active in a program.
    ACTIVE_UNIFORMS = 35718,//Passed to getProgramParameter to get the number of uniforms active in a program.
    MAX_VERTEX_ATTRIBS = 34921,//The maximum number of entries possible in the vertex attribute list.
    MAX_VERTEX_UNIFORM_VECTORS = 36347,
    MAX_VARYING_VECTORS = 36348,
    MAX_COMBINED_TEXTURE_IMAGE_UNITS = 35661,
    MAX_VERTEX_TEXTURE_IMAGE_UNITS = 35660,
    MAX_TEXTURE_IMAGE_UNITS = 34930,//Implementation dependent number of maximum texture units.At least 8.;
    MAX_FRAGMENT_UNIFORM_VECTORS = 36349,
    SHADER_TYPE = 35663,
    SHADING_LANGUAGE_VERSION = 35724,
    CURRENT_PROGRAM = 35725
}
/**
 * Constants passed to WebGLRenderingContext.depthFunc() or WebGLRenderingContext.stencilFunc().;
 **/
export declare enum DepthAndStencilTest {
    NEVER = 512,//Passed to depthFunction or stencilFunction to specify depth or stencil tests will never pass.i.e.Nothing will be drawn.
    LESS = 513,//Passed to depthFunction or stencilFunction to specify depth or stencil tests will pass if the new depth value is less than the stored value.
    EQUAL = 514,//Passed to depthFunction or stencilFunction to specify depth or stencil tests will pass if the new depth value is equals to the stored value.
    LEQUAL = 515,//Passed to depthFunction or stencilFunction to specify depth or stencil tests will pass if the new depth value is less than or equal to the stored value.
    GREATER = 516,//Passed to depthFunction or stencilFunction to specify depth or stencil tests will pass if the new depth value is greater than the stored value.
    NOTEQUAL = 517,//Passed to depthFunction or stencilFunction to specify depth or stencil tests will pass if the new depth value is not equal to the stored value.
    GEQUAL = 518,//Passed to depthFunction or stencilFunction to specify depth or stencil tests will pass if the new depth value is greater than or equal to the stored value.
    ALWAYS = 519
}
/**
 * Constants passed to WebGLRenderingContext.stencilOp().;
 **/
export declare enum StencilAction {
    KEEP = 7680,
    REPLACE = 7681,
    INCR = 7682,
    DECR = 7683,
    INVERT = 5386,
    INCR_WRAP = 34055,
    DECR_WRAP = 34056
}
/**
 * Constants passed to WebGLRenderingContext.texParameteri(), WebGLRenderingContext.texParameterf(), WebGLRenderingContext.bindTexture(), WebGLRenderingContext.texImage2D(), and others.;
 **/
export declare enum TextureParam {
    NEAREST = 9728,
    LINEAR = 9729,
    NEAREST_MIPMAP_NEAREST = 9984,
    LINEAR_MIPMAP_NEAREST = 9985,
    NEAREST_MIPMAP_LINEAR = 9986,
    LINEAR_MIPMAP_LINEAR = 9987,
    TEXTURE_MAG_FILTER = 10240,
    TEXTURE_MIN_FILTER = 10241,
    TEXTURE_WRAP_S = 10242,
    TEXTURE_WRAP_T = 10243,
    TEXTURE = 5890,
    TEXTURE_BINDING_CUBE_MAP = 34068,
    TEXTURE_CUBE_MAP_POSITIVE_X = 34069,
    TEXTURE_CUBE_MAP_NEGATIVE_X = 34070,
    TEXTURE_CUBE_MAP_POSITIVE_Y = 34071,
    TEXTURE_CUBE_MAP_NEGATIVE_Y = 34072,
    TEXTURE_CUBE_MAP_POSITIVE_Z = 34073,
    TEXTURE_CUBE_MAP_NEGATIVE_Z = 34074,
    MAX_CUBE_MAP_TEXTURE_SIZE = 34076,
    ACTIVE_TEXTURE = 34016,//The current active texture unit.
    REPEAT = 10497,
    CLAMP_TO_EDGE = 33071,
    MIRRORED_REPEAT = 33648,
    RED = 6403,
    RGB8 = 32849,
    RGBA8 = 32856,
    RGB10_A2 = 32857,
    TEXTURE_WRAP_R = 32882,
    TEXTURE_MIN_LOD = 33082,
    TEXTURE_MAX_LOD = 33083,
    TEXTURE_BASE_LEVEL = 33084,
    TEXTURE_MAX_LEVEL = 33085,
    TEXTURE_COMPARE_MODE = 34892,
    TEXTURE_COMPARE_FUNC = 34893,
    SRGB = 35904,
    SRGB8 = 35905,
    SRGB8_ALPHA8 = 35907,
    COMPARE_REF_TO_TEXTURE = 34894,
    RGBA32F = 34836,
    RGB32F = 34837,
    RGBA16F = 34842,
    RGB16F = 34843,
    TEXTURE_BINDING_2D_ARRAY = 35869,
    R11F_G11F_B10F = 35898,
    RGB9_E5 = 35901,
    RGBA32UI = 36208,
    RGB32UI = 36209,
    RGBA16UI = 36214,
    RGB16UI = 36215,
    RGBA8UI = 36220,
    RGB8UI = 36221,
    RGBA32I = 36226,
    RGB32I = 36227,
    RGBA16I = 36232,
    RGB16I = 36233,
    RGBA8I = 36238,
    RGB8I = 36239,
    RED_INTEGER = 36244,
    RGB_INTEGER = 36248,
    RGBA_INTEGER = 36249,
    R8 = 33321,
    RG8 = 33323,
    R16F = 33325,
    R32F = 33326,
    RG16F = 33327,
    RG32F = 33328,
    R8I = 33329,
    R8UI = 33330,
    R16I = 33331,
    R16UI = 33332,
    R32I = 33333,
    R32UI = 33334,
    RG8I = 33335,
    RG8UI = 33336,
    RG16I = 33337,
    RG16UI = 33338,
    RG32I = 33339,
    RG32UI = 33340,
    R8_SNORM = 36756,
    RG8_SNORM = 36757,
    RGB8_SNORM = 36758,
    RGBA8_SNORM = 36759,
    RGB10_A2UI = 36975,
    TEXTURE_IMMUTABLE_FORMAT = 37167,
    TEXTURE_IMMUTABLE_LEVELS = 33503
}
export declare enum TextureTarget {
    TEXTURE_2D = 3553,
    TEXTURE_3D = 32879,
    TEXTURE_CUBE_MAP = 34067,
    TEXTURE_2D_ARRAY = 35866
}
export declare enum TextureUnit {
    TEXTURE0 = 33984,// A texture unit.
    TEXTURE1 = 33985,// A texture unit.
    TEXTURE2 = 33986,// A texture unit.
    TEXTURE3 = 33987,// A texture unit.
    TEXTURE4 = 33988,// A texture unit.
    TEXTURE5 = 33989,// A texture unit.
    TEXTURE6 = 33990,// A texture unit.
    TEXTURE7 = 33991,// A texture unit.
    TEXTURE8 = 33992,// A texture unit.
    TEXTURE9 = 33993,// A texture unit.
    TEXTURE10 = 33994,// A texture unit.
    TEXTURE11 = 33995,// A texture unit.
    TEXTURE12 = 33996,// A texture unit.
    TEXTURE13 = 33997,// A texture unit.
    TEXTURE14 = 33998,// A texture unit.
    TEXTURE15 = 33999,// A texture unit.
    TEXTURE16 = 34000,// A texture unit.
    TEXTURE17 = 34001,// A texture unit.
    TEXTURE18 = 34002,// A texture unit.
    TEXTURE19 = 34003,// A texture unit.
    TEXTURE20 = 34004,// A texture unit.
    TEXTURE21 = 34005,// A texture unit.
    TEXTURE22 = 34006,// A texture unit.
    TEXTURE23 = 34007,// A texture unit.
    TEXTURE24 = 34008,// A texture unit.
    TEXTURE25 = 34009,// A texture unit.
    TEXTURE26 = 34010,// A texture unit.
    TEXTURE27 = 34011,// A texture unit.
    TEXTURE28 = 34012,// A texture unit.
    TEXTURE29 = 34013,// A texture unit.
    TEXTURE30 = 34014,// A texture unit.
    TEXTURE31 = 34015
}
export declare enum UniformType {
    FLOAT_VEC2 = 35664,
    FLOAT_VEC3 = 35665,
    FLOAT_VEC4 = 35666,
    INT_VEC2 = 35667,
    INT_VEC3 = 35668,
    INT_VEC4 = 35669,
    BOOL = 35670,
    BOOL_VEC2 = 35671,
    BOOL_VEC3 = 35672,
    BOOL_VEC4 = 35673,
    FLOAT_MAT2 = 35674,
    FLOAT_MAT3 = 35675,
    FLOAT_MAT4 = 35676,
    SAMPLER_2D = 35678,
    SAMPLER_CUBE = 35680,
    UNIFORM_BUFFER = 35345,
    UNIFORM_BUFFER_BINDING = 35368,
    UNIFORM_BUFFER_START = 35369,
    UNIFORM_BUFFER_SIZE = 35370,
    MAX_VERTEX_UNIFORM_BLOCKS = 35371,
    MAX_FRAGMENT_UNIFORM_BLOCKS = 35373,
    MAX_COMBINED_UNIFORM_BLOCKS = 35374,
    MAX_UNIFORM_BUFFER_BINDINGS = 35375,
    MAX_UNIFORM_BLOCK_SIZE = 35376,
    MAX_COMBINED_VERTEX_UNIFORM_COMPONENTS = 35377,
    MAX_COMBINED_FRAGMENT_UNIFORM_COMPONENTS = 35379,
    UNIFORM_BUFFER_OFFSET_ALIGNMENT = 35380,
    ACTIVE_UNIFORM_BLOCKS = 35382,
    UNIFORM_TYPE = 35383,
    UNIFORM_SIZE = 35384,
    UNIFORM_BLOCK_INDEX = 35386,
    UNIFORM_OFFSET = 35387,
    UNIFORM_ARRAY_STRIDE = 35388,
    UNIFORM_MATRIX_STRIDE = 35389,
    UNIFORM_IS_ROW_MAJOR = 35390,
    UNIFORM_BLOCK_BINDING = 35391,
    UNIFORM_BLOCK_DATA_SIZE = 35392,
    UNIFORM_BLOCK_ACTIVE_UNIFORMS = 35394,
    UNIFORM_BLOCK_ACTIVE_UNIFORM_INDICES = 35395,
    UNIFORM_BLOCK_REFERENCED_BY_VERTEX_SHADER = 35396,
    UNIFORM_BLOCK_REFERENCED_BY_FRAGMENT_SHADER = 35398
}
export declare enum ShaderPrecision {
    LOW_FLOAT = 36336,
    MEDIUM_FLOAT = 36337,
    HIGH_FLOAT = 36338,
    LOW_INT = 36339,
    MEDIUM_INT = 36340,
    HIGH_INT = 36341
}
export declare enum FrameAndRenderBuffers {
    BACK = 1029,
    RGBA4 = 32854,
    RGB5_A1 = 32855,
    RGB565 = 36194,
    RGBA8 = 32856,
    DEPTH_COMPONENT16 = 33189,
    STENCIL_INDEX8 = 36168,
    DEPTH_STENCIL = 34041,
    RENDERBUFFER_WIDTH = 36162,
    RENDERBUFFER_HEIGHT = 36163,
    RENDERBUFFER_INTERNAL_FORMAT = 36164,
    RENDERBUFFER_RED_SIZE = 36176,
    RENDERBUFFER_GREEN_SIZE = 36177,
    RENDERBUFFER_BLUE_SIZE = 36178,
    RENDERBUFFER_ALPHA_SIZE = 36179,
    RENDERBUFFER_DEPTH_SIZE = 36180,
    RENDERBUFFER_STENCIL_SIZE = 36181,
    FRAMEBUFFER_ATTACHMENT_OBJECT_TYPE = 36048,
    FRAMEBUFFER_ATTACHMENT_OBJECT_NAME = 36049,
    FRAMEBUFFER_ATTACHMENT_TEXTURE_LEVEL = 36050,
    FRAMEBUFFER_ATTACHMENT_TEXTURE_CUBE_MAP_FACE = 36051,
    COLOR_ATTACHMENT0 = 36064,
    COLOR_ATTACHMENT1 = 36065,
    COLOR_ATTACHMENT2 = 36066,
    COLOR_ATTACHMENT3 = 36067,
    COLOR_ATTACHMENT4 = 36068,
    COLOR_ATTACHMENT5 = 36069,
    COLOR_ATTACHMENT6 = 36070,
    COLOR_ATTACHMENT7 = 36071,
    COLOR_ATTACHMENT8 = 36072,
    COLOR_ATTACHMENT9 = 36073,
    COLOR_ATTACHMENT10 = 36074,
    COLOR_ATTACHMENT11 = 36075,
    COLOR_ATTACHMENT12 = 36076,
    COLOR_ATTACHMENT13 = 36077,
    COLOR_ATTACHMENT14 = 36078,
    COLOR_ATTACHMENT15 = 36079,
    DEPTH_ATTACHMENT = 36096,
    STENCIL_ATTACHMENT = 36128,
    DEPTH_STENCIL_ATTACHMENT = 33306,
    NONE = 0,
    FRAMEBUFFER_COMPLETE = 36053,
    FRAMEBUFFER_INCOMPLETE_ATTACHMENT = 36054,
    FRAMEBUFFER_INCOMPLETE_MISSING_ATTACHMENT = 36055,
    FRAMEBUFFER_INCOMPLETE_DIMENSIONS = 36057,
    FRAMEBUFFER_UNSUPPORTED = 36061,
    FRAMEBUFFER_BINDING = 36006,
    RENDERBUFFER_BINDING = 36007,
    MAX_RENDERBUFFER_SIZE = 34024,
    INVALID_FRAMEBUFFER_OPERATION = 1286,
    FRAMEBUFFER_ATTACHMENT_COLOR_ENCODING = 33296,
    FRAMEBUFFER_ATTACHMENT_COMPONENT_TYPE = 33297,
    FRAMEBUFFER_ATTACHMENT_RED_SIZE = 33298,
    FRAMEBUFFER_ATTACHMENT_GREEN_SIZE = 33299,
    FRAMEBUFFER_ATTACHMENT_BLUE_SIZE = 33300,
    FRAMEBUFFER_ATTACHMENT_ALPHA_SIZE = 33301,
    FRAMEBUFFER_ATTACHMENT_DEPTH_SIZE = 33302,
    FRAMEBUFFER_ATTACHMENT_STENCIL_SIZE = 33303,
    FRAMEBUFFER_DEFAULT = 33304,
    DEPTH24_STENCIL8 = 35056,
    DRAW_FRAMEBUFFER_BINDING = 36006,
    READ_FRAMEBUFFER_BINDING = 36010,
    RENDERBUFFER_SAMPLES = 36011,
    FRAMEBUFFER_ATTACHMENT_TEXTURE_LAYER = 36052,
    FRAMEBUFFER_INCOMPLETE_MULTISAMPLE = 36182
}
export declare enum RenderbufferType {
    RENDERBUFFER = 36161
}
export declare enum FramebufferType {
    FRAMEBUFFER = 36160,
    READ_FRAMEBUFFER = 36008,
    DRAW_FRAMEBUFFER = 36009
}
export type FramebufferTypes = 0x8D40 | 0x8CA8 | 0x8CA9;
/**
 * Constants passed to WebGLRenderingContext.pixelStorei().;
 **/
export declare enum PixelStorageMode {
    UNPACK_FLIP_Y_WEBGL = 37440,
    UNPACK_PREMULTIPLY_ALPHA_WEBGL = 37441,
    UNPACK_COLORSPACE_CONVERSION_WEBGL = 37443
}
export declare enum QueryType {
    CURRENT_QUERY = 34917,
    QUERY_RESULT = 34918,
    QUERY_RESULT_AVAILABLE = 34919,
    ANY_SAMPLES_PASSED = 35887,
    ANY_SAMPLES_PASSED_CONSERVATIVE = 36202
}
export declare enum DrawBufferType {
    MAX_DRAW_BUFFERS = 34852,
    DRAW_BUFFER0 = 34853,
    DRAW_BUFFER1 = 34854,
    DRAW_BUFFER2 = 34855,
    DRAW_BUFFER3 = 34856,
    DRAW_BUFFER4 = 34857,
    DRAW_BUFFER5 = 34858,
    DRAW_BUFFER6 = 34859,
    DRAW_BUFFER7 = 34860,
    DRAW_BUFFER8 = 34861,
    DRAW_BUFFER9 = 34862,
    DRAW_BUFFER10 = 34863,
    DRAW_BUFFER11 = 34864,
    DRAW_BUFFER12 = 34865,
    DRAW_BUFFER13 = 34866,
    DRAW_BUFFER14 = 34867,
    DRAW_BUFFER15 = 34868,
    MAX_COLOR_ATTACHMENTS = 36063,
    COLOR_ATTACHMENT1 = 36065,
    COLOR_ATTACHMENT2 = 36066,
    COLOR_ATTACHMENT3 = 36067,
    COLOR_ATTACHMENT4 = 36068,
    COLOR_ATTACHMENT5 = 36069,
    COLOR_ATTACHMENT6 = 36070,
    COLOR_ATTACHMENT7 = 36071,
    COLOR_ATTACHMENT8 = 36072,
    COLOR_ATTACHMENT9 = 36073,
    COLOR_ATTACHMENT10 = 36074,
    COLOR_ATTACHMENT11 = 36075,
    COLOR_ATTACHMENT12 = 36076,
    COLOR_ATTACHMENT13 = 36077,
    COLOR_ATTACHMENT14 = 36078,
    COLOR_ATTACHMENT15 = 36079
}
export declare enum SamplerType {
    SAMPLER_3D = 35679,
    SAMPLER_2D_SHADOW = 35682,
    SAMPLER_2D_ARRAY = 36289,
    SAMPLER_2D_ARRAY_SHADOW = 36292,
    SAMPLER_CUBE_SHADOW = 36293,
    INT_SAMPLER_2D = 36298,
    INT_SAMPLER_3D = 36299,
    INT_SAMPLER_CUBE = 36300,
    INT_SAMPLER_2D_ARRAY = 36303,
    UNSIGNED_INT_SAMPLER_2D = 36306,
    UNSIGNED_INT_SAMPLER_3D = 36307,
    UNSIGNED_INT_SAMPLER_CUBE = 36308,
    UNSIGNED_INT_SAMPLER_2D_ARRAY = 36311,
    MAX_SAMPLES = 36183,
    SAMPLER_BINDING = 35097
}
export declare enum TransformFeedbackType {
    TRANSFORM_FEEDBACK_BUFFER_MODE = 35967,
    MAX_TRANSFORM_FEEDBACK_SEPARATE_COMPONENTS = 35968,
    TRANSFORM_FEEDBACK_VARYINGS = 35971,
    TRANSFORM_FEEDBACK_BUFFER_START = 35972,
    TRANSFORM_FEEDBACK_BUFFER_SIZE = 35973,
    TRANSFORM_FEEDBACK_PRIMITIVES_WRITTEN = 35976,
    MAX_TRANSFORM_FEEDBACK_INTERLEAVED_COMPONENTS = 35978,
    MAX_TRANSFORM_FEEDBACK_SEPARATE_ATTRIBS = 35979,
    INTERLEAVED_ATTRIBS = 35980,
    SEPARATE_ATTRIBS = 35981,
    TRANSFORM_FEEDBACK_BUFFER = 35982,
    TRANSFORM_FEEDBACK_BUFFER_BINDING = 35983,
    TRANSFORM_FEEDBACK = 36386,
    TRANSFORM_FEEDBACK_PAUSED = 36387,
    TRANSFORM_FEEDBACK_ACTIVE = 36388,
    TRANSFORM_FEEDBACK_BINDING = 36389
}
export declare enum SyncObjectType {
    OBJECT_TYPE = 37138,
    SYNC_CONDITION = 37139,
    SYNC_STATUS = 37140,
    SYNC_FLAGS = 37141,
    SYNC_FENCE = 37142,
    SYNC_GPU_COMMANDS_COMPLETE = 37143,
    UNSIGNALED = 37144,
    SIGNALED = 37145,
    ALREADY_SIGNALED = 37146,
    TIMEOUT_EXPIRED = 37147,
    CONDITION_SATISFIED = 37148,
    WAIT_FAILED = 37149,
    SYNC_FLUSH_COMMANDS_BIT = 1
}
export declare enum WebGL2Misc {
    COLOR = 6144,
    DEPTH = 6145,
    STENCIL = 6146,
    MIN = 32775,
    MAX = 32776,
    DEPTH_COMPONENT24 = 33190,
    DEPTH_COMPONENT32F = 36012,
    DEPTH32F_STENCIL8 = 36013,
    INVALID_INDEX = 4294967295,
    TIMEOUT_IGNORED = -1,
    MAX_CLIENT_WAIT_TIMEOUT_WEBGL = 37447
}
export declare enum XANGLE_instanced_arrays {
    VERTEX_ATTRIB_ARRAY_DIVISOR_ANGLE = 35070
}
export declare enum XWEBGL_debug_renderer_info {
    UNMASKED_VENDOR_WEBGL = 37445,//Passed to getParameter to get the vendor string of the graphics driver.
    UNMASKED_RENDERER_WEBGL = 37446
}
export declare enum XEXT_texture_filter_anisotropic {
    MAX_TEXTURE_MAX_ANISOTROPY_EXT = 34047,//Returns the maximum available anisotropy.
    TEXTURE_MAX_ANISOTROPY_EXT = 34046
}
export declare enum XWEBGL_compressed_texture_s3tc {
    COMPRESSED_RGB_S3TC_DXT1_EXT = 33776,//A DXT1 - compressed image in an RGB image format.
    COMPRESSED_RGBA_S3TC_DXT1_EXT = 33777,//A DXT1 - compressed image in an RGB image format with a simple on / off alpha value.
    COMPRESSED_RGBA_S3TC_DXT3_EXT = 33778,//A DXT3 - compressed image in an RGBA image format.Compared to a 32 - bit RGBA texture, it offers 4: 1 compression.
    COMPRESSED_RGBA_S3TC_DXT5_EXT = 33779
}
export declare enum XWEBGL_compressed_texture_etc {
    COMPRESSED_R11_EAC = 37488,//One - channel(red) unsigned format compression.
    COMPRESSED_SIGNED_R11_EAC = 37489,//One - channel(red) signed format compression.
    COMPRESSED_RG11_EAC = 37490,//Two - channel(red and green) unsigned format compression.
    COMPRESSED_SIGNED_RG11_EAC = 37491,//Two - channel(red and green) signed format compression.
    COMPRESSED_RGB8_ETC2 = 37492,//Compresses RBG8 data with no alpha channel.
    COMPRESSED_RGBA8_ETC2_EAC = 37493,//Compresses RGBA8 data.The RGB part is encoded the same as RGB_ETC2, but the alpha part is encoded separately.
    COMPRESSED_SRGB8_ETC2 = 37494,//Compresses sRBG8 data with no alpha channel.
    COMPRESSED_SRGB8_ALPHA8_ETC2_EAC = 37495,//Compresses sRGBA8 data.The sRGB part is encoded the same as SRGB_ETC2, but the alpha part is encoded separately.
    COMPRESSED_RGB8_PUNCHTHROUGH_ALPHA1_ETC2 = 37496,//Similar to RGB8_ETC, but with ability to punch through the alpha channel, which means to make it completely opaque or transparent.
    COMPRESSED_SRGB8_PUNCHTHROUGH_ALPHA1_ETC2 = 37497
}
export declare enum XWEBGL_compressed_texture_pvrtc {
    COMPRESSED_RGB_PVRTC_4BPPV1_IMG = 35840,//RGB compression in 4 - bit mode.One block for each 4×4 pixels.
    COMPRESSED_RGBA_PVRTC_4BPPV1_IMG = 35842,//RGBA compression in 4 - bit mode.One block for each 4×4 pixels.
    COMPRESSED_RGB_PVRTC_2BPPV1_IMG = 35841,//RGB compression in 2 - bit mode.One block for each 8×4 pixels.
    COMPRESSED_RGBA_PVRTC_2BPPV1_IMG = 35843
}
export declare enum XWEBGL_compressed_texture_etc1 {
    COMPRESSED_RGB_ETC1_WEBGL = 36196
}
export declare enum XWEBGL_compressed_texture_atc {
    COMPRESSED_RGB_ATC_WEBGL = 35986,//Compresses RGB textures with no alpha channel.
    COMPRESSED_RGBA_ATC_EXPLICIT_ALPHA_WEBGL = 35986,//Compresses RGBA textures using explicit alpha encoding(useful when alpha transitions are sharp).
    COMPRESSED_RGBA_ATC_INTERPOLATED_ALPHA_WEBGL = 34798
}
export declare enum XWEBGL_depth_texture {
    UNSIGNED_INT_24_8_WEBGL = 34042
}
export declare enum XOES_texture_half_float {
    HALF_FLOAT_OES = 36193
}
export declare enum XWEBGL_color_buffer_float {
    RGBA32F_EXT = 34836,//RGBA 32 - bit floating - point color - renderable format.
    RGB32F_EXT = 34837,//RGB 32 - bit floating - point color - renderable format.
    FRAMEBUFFER_ATTACHMENT_COMPONENT_TYPE_EXT = 33297,
    UNSIGNED_NORMALIZED_EXT = 35863
}
export declare enum XEXT_blend_minmax {
    MIN_EXT = 32775,//Produces the minimum color components of the source and destination colors.
    MAX_EXT = 32776
}
export declare enum XEXT_sRGB {
    SRGB_EXT = 35904,//Unsized sRGB format that leaves the precision up to the driver.
    SRGB_ALPHA_EXT = 35906,//Unsized sRGB format with unsized alpha component.
    SRGB8_ALPHA8_EXT = 35907,//Sized(8 - bit) sRGB and alpha formats.
    FRAMEBUFFER_ATTACHMENT_COLOR_ENCODING_EXT = 33296
}
export declare enum XOES_standard_derivatives {
    FRAGMENT_SHADER_DERIVATIVE_HINT_OES = 35723
}
export declare enum XWEBGL_draw_buffers {
    COLOR_ATTACHMENT0_WEBGL = 36064,//Framebuffer color attachment point;
    COLOR_ATTACHMENT1_WEBGL = 36065,//Framebuffer color attachment point;
    COLOR_ATTACHMENT2_WEBGL = 36066,//Framebuffer color attachment point;
    COLOR_ATTACHMENT3_WEBGL = 36067,//Framebuffer color attachment point;
    COLOR_ATTACHMENT4_WEBGL = 36068,//Framebuffer color attachment point;
    COLOR_ATTACHMENT5_WEBGL = 36069,//Framebuffer color attachment point;
    COLOR_ATTACHMENT6_WEBGL = 36070,//Framebuffer color attachment point;
    COLOR_ATTACHMENT7_WEBGL = 36071,//Framebuffer color attachment point;
    COLOR_ATTACHMENT8_WEBGL = 36072,//Framebuffer color attachment point;
    COLOR_ATTACHMENT9_WEBGL = 36073,//Framebuffer color attachment point;
    COLOR_ATTACHMENT10_WEBGL = 36074,//Framebuffer color attachment point;
    COLOR_ATTACHMENT11_WEBGL = 36075,//Framebuffer color attachment point;
    COLOR_ATTACHMENT12_WEBGL = 36076,//Framebuffer color attachment point;
    COLOR_ATTACHMENT13_WEBGL = 36077,//Framebuffer color attachment point;
    COLOR_ATTACHMENT14_WEBGL = 36078,//Framebuffer color attachment point;
    COLOR_ATTACHMENT15_WEBGL = 36079,//Framebuffer color attachment point;
    DRAW_BUFFER0_WEBGL = 34853,//Draw buffer;
    DRAW_BUFFER1_WEBGL = 34854,//Draw buffer;
    DRAW_BUFFER2_WEBGL = 34855,//Draw buffer;
    DRAW_BUFFER3_WEBGL = 34856,//Draw buffer;
    DRAW_BUFFER4_WEBGL = 34857,//Draw buffer;
    DRAW_BUFFER5_WEBGL = 34858,//Draw buffer;
    DRAW_BUFFER6_WEBGL = 34859,//Draw buffer;
    DRAW_BUFFER7_WEBGL = 34860,//Draw buffer;
    DRAW_BUFFER8_WEBGL = 34861,//Draw buffer;
    DRAW_BUFFER9_WEBGL = 34862,//Draw buffer;
    DRAW_BUFFER10_WEBGL = 34863,//Draw buffer;
    DRAW_BUFFER11_WEBGL = 34864,//Draw buffer;
    DRAW_BUFFER12_WEBGL = 34865,//Draw buffer;
    DRAW_BUFFER13_WEBGL = 34866,//Draw buffer;
    DRAW_BUFFER14_WEBGL = 34867,//Draw buffer;
    DRAW_BUFFER15_WEBGL = 34868,//Draw buffer;
    MAX_COLOR_ATTACHMENTS_WEBGL = 36063,//Maximum number of framebuffer color attachment points;
    MAX_DRAW_BUFFERS_WEBGL = 34852
}
export declare enum XOES_vertex_array_object {
    VERTEX_ARRAY_BINDING_OES = 34229
}
export declare enum XEXT_disjoint_timer_query {
    QUERY_COUNTER_BITS_EXT = 34916,//The number of bits used to hold the query result for the given target.
    CURRENT_QUERY_EXT = 34917,//The currently active query.
    QUERY_RESULT_EXT = 34918,//The query result.
    QUERY_RESULT_AVAILABLE_EXT = 34919,//A Boolean indicating whether or not a query result is available.
    TIME_ELAPSED_EXT = 35007,//Elapsed time(in nanoseconds).
    TIMESTAMP_EXT = 36392,//The current time.
    GPU_DISJOINT_EXT = 36795
}
export declare enum XOVR_multiview2 {
    FRAMEBUFFER_ATTACHMENT_TEXTURE_NUM_VIEWS_OVR = 38448,
    FRAMEBUFFER_ATTACHMENT_TEXTURE_BASE_VIEW_INDEX_OVR = 38450,
    MAX_VIEWS_OVR = 38449,
    FRAMEBUFFER_INCOMPLETE_VIEW_TARGETS_OVR = 38451
}
export interface OVR_multiview2 {
    FRAMEBUFFER_ATTACHMENT_TEXTURE_NUM_VIEWS_OVR: XOVR_multiview2;
    FRAMEBUFFER_ATTACHMENT_TEXTURE_BASE_VIEW_INDEX_OVR: XOVR_multiview2;
    MAX_VIEWS_OVR: XOVR_multiview2;
    FRAMEBUFFER_INCOMPLETE_VIEW_TARGETS_OVR: XOVR_multiview2;
    framebufferTextureMultiviewOVR(target: GLenum, attachment: GLenum, texture: WebGLTexture, level: number, baseViewIndex: number, numViews: number): WebGLRenderbuffer;
}
export interface OCULUS_multiview {
    FRAMEBUFFER_ATTACHMENT_TEXTURE_NUM_VIEWS_OVR: XOVR_multiview2;
    FRAMEBUFFER_ATTACHMENT_TEXTURE_BASE_VIEW_INDEX_OVR: XOVR_multiview2;
    MAX_VIEWS_OVR: XOVR_multiview2;
    FRAMEBUFFER_INCOMPLETE_VIEW_TARGETS_OVR: XOVR_multiview2;
    framebufferTextureMultisampleMultiviewOVR(target: GLenum, attachment: GLenum, texture: WebGLTexture, level: number, samples: number, baseViewIndex: number, numViews: number): WebGLRenderbuffer;
}
export interface WEBGL_multi_draw {
    multiDrawArraysWEBGL(mode: GLenum, firstsList: Int32Array, firstsOffset: number, countsList: Int32Array, countsOffset: number, drawCount: number): void;
    multiDrawElementsWEBGL(mode: GLenum, countsList: Int32Array, countsOffset: number, type: GLenum, firstsList: Int32Array, firstsOffset: number, drawCount: number): void;
    multiDrawArraysInstancedWEBGL(mmode: GLenum, firstsList: Int32Array, firstsOffset: number, countsList: Int32Array, countsOffset: number, instanceCountsList: Int32Array, instanceCountsOffset: number, drawCount: number): void;
    multiDrawElementsInstancedWEBGL(mode: GLenum, countsList: Int32Array, countsOffset: number, type: GLenum, firstsList: Int32Array, firstsOffset: number, instanceCountsList: Int32Array, instanceCountsOffset: number, drawCount: number): void;
}
//# sourceMappingURL=GLEnum.d.ts.map