/**
 * Constants passed to WebGLRenderingContext.clear() to clear buffer masks.;
 **/
export var ClearBits;
(function (ClearBits) {
    ClearBits[ClearBits["DEPTH_BUFFER_BIT"] = 256] = "DEPTH_BUFFER_BIT";
    ClearBits[ClearBits["STENCIL_BUFFER_BIT"] = 1024] = "STENCIL_BUFFER_BIT";
    ClearBits[ClearBits["COLOR_BUFFER_BIT"] = 16384] = "COLOR_BUFFER_BIT"; // Passed to clear to clear the current color buffer.
})(ClearBits || (ClearBits = {}));
/**
 * Constants passed to WebGLRenderingContext.drawElements() or WebGLRenderingContext.drawArrays() to specify what kind of primitive to render.;
 **/
export var DrawTypes;
(function (DrawTypes) {
    DrawTypes[DrawTypes["POINTS"] = 0] = "POINTS";
    DrawTypes[DrawTypes["LINES"] = 1] = "LINES";
    DrawTypes[DrawTypes["LINE_LOOP"] = 2] = "LINE_LOOP";
    DrawTypes[DrawTypes["LINE_STRIP"] = 3] = "LINE_STRIP";
    DrawTypes[DrawTypes["TRIANGLES"] = 4] = "TRIANGLES";
    DrawTypes[DrawTypes["TRIANGLE_STRIP"] = 5] = "TRIANGLE_STRIP";
    DrawTypes[DrawTypes["TRIANGLE_FAN"] = 6] = "TRIANGLE_FAN";
})(DrawTypes || (DrawTypes = {}));
/**
 * Constants passed to WebGLRenderingContext.blendFunc() or WebGLRenderingContext.blendFuncSeparate() to specify the blending mode(for both, RBG and alpha, or separately).
 **/
export var BlendFunctionMode;
(function (BlendFunctionMode) {
    BlendFunctionMode[BlendFunctionMode["ZERO"] = 0] = "ZERO";
    BlendFunctionMode[BlendFunctionMode["ONE"] = 1] = "ONE";
    BlendFunctionMode[BlendFunctionMode["SRC_COLOR"] = 768] = "SRC_COLOR";
    BlendFunctionMode[BlendFunctionMode["ONE_MINUS_SRC_COLOR"] = 769] = "ONE_MINUS_SRC_COLOR";
    BlendFunctionMode[BlendFunctionMode["SRC_ALPHA"] = 770] = "SRC_ALPHA";
    BlendFunctionMode[BlendFunctionMode["ONE_MINUS_SRC_ALPHA"] = 771] = "ONE_MINUS_SRC_ALPHA";
    BlendFunctionMode[BlendFunctionMode["DST_ALPHA"] = 772] = "DST_ALPHA";
    BlendFunctionMode[BlendFunctionMode["ONE_MINUS_DST_ALPHA"] = 773] = "ONE_MINUS_DST_ALPHA";
    BlendFunctionMode[BlendFunctionMode["DST_COLOR"] = 774] = "DST_COLOR";
    BlendFunctionMode[BlendFunctionMode["ONE_MINUS_DST_COLOR"] = 775] = "ONE_MINUS_DST_COLOR";
    BlendFunctionMode[BlendFunctionMode["SRC_ALPHA_SATURATE"] = 776] = "SRC_ALPHA_SATURATE";
    BlendFunctionMode[BlendFunctionMode["CONSTANT_COLOR"] = 32769] = "CONSTANT_COLOR";
    BlendFunctionMode[BlendFunctionMode["ONE_MINUS_CONSTANT_COLOR"] = 32770] = "ONE_MINUS_CONSTANT_COLOR";
    BlendFunctionMode[BlendFunctionMode["CONSTANT_ALPHA"] = 32771] = "CONSTANT_ALPHA";
    BlendFunctionMode[BlendFunctionMode["ONE_MINUS_CONSTANT_ALPHA"] = 32772] = "ONE_MINUS_CONSTANT_ALPHA";
})(BlendFunctionMode || (BlendFunctionMode = {}));
/**
 * Constants passed to WebGLRenderingContext.blendEquation() or WebGLRenderingContext.blendEquationSeparate() to control how the blending is calculated(for both, RBG and alpha, or separately).
 **/
export var BlendEquationMode;
(function (BlendEquationMode) {
    BlendEquationMode[BlendEquationMode["FUNC_ADD"] = 32774] = "FUNC_ADD";
    BlendEquationMode[BlendEquationMode["FUNC_SUBTRACT"] = 32778] = "FUNC_SUBTRACT";
    BlendEquationMode[BlendEquationMode["FUNC_REVERSE_SUBTRACT"] = 32779] = "FUNC_REVERSE_SUBTRACT";
    // WebGL2
    BlendEquationMode[BlendEquationMode["MIN"] = 32775] = "MIN";
    BlendEquationMode[BlendEquationMode["MAX"] = 32776] = "MAX";
})(BlendEquationMode || (BlendEquationMode = {}));
/**
 * Constants passed to WebGLRenderingContext.getParameter() to specify what information to return.;
 **/
export var InfoParamType;
(function (InfoParamType) {
    InfoParamType[InfoParamType["BLEND_EQUATION"] = 32777] = "BLEND_EQUATION";
    InfoParamType[InfoParamType["BLEND_EQUATION_RGB"] = 32777] = "BLEND_EQUATION_RGB";
    InfoParamType[InfoParamType["BLEND_EQUATION_ALPHA"] = 34877] = "BLEND_EQUATION_ALPHA";
    InfoParamType[InfoParamType["BLEND_DST_RGB"] = 32968] = "BLEND_DST_RGB";
    InfoParamType[InfoParamType["BLEND_SRC_RGB"] = 32969] = "BLEND_SRC_RGB";
    InfoParamType[InfoParamType["BLEND_DST_ALPHA"] = 32970] = "BLEND_DST_ALPHA";
    InfoParamType[InfoParamType["BLEND_SRC_ALPHA"] = 32971] = "BLEND_SRC_ALPHA";
    InfoParamType[InfoParamType["BLEND_COLOR"] = 32773] = "BLEND_COLOR";
    InfoParamType[InfoParamType["ARRAY_BUFFER_BINDING"] = 34964] = "ARRAY_BUFFER_BINDING";
    InfoParamType[InfoParamType["ELEMENT_ARRAY_BUFFER_BINDING"] = 34965] = "ELEMENT_ARRAY_BUFFER_BINDING";
    InfoParamType[InfoParamType["LINE_WIDTH"] = 2849] = "LINE_WIDTH";
    InfoParamType[InfoParamType["ALIASED_POINT_SIZE_RANGE"] = 33901] = "ALIASED_POINT_SIZE_RANGE";
    InfoParamType[InfoParamType["ALIASED_LINE_WIDTH_RANGE"] = 33902] = "ALIASED_LINE_WIDTH_RANGE";
    InfoParamType[InfoParamType["CULL_FACE_MODE"] = 2885] = "CULL_FACE_MODE";
    InfoParamType[InfoParamType["FRONT_FACE"] = 2886] = "FRONT_FACE";
    InfoParamType[InfoParamType["DEPTH_RANGE"] = 2928] = "DEPTH_RANGE";
    InfoParamType[InfoParamType["DEPTH_WRITEMASK"] = 2930] = "DEPTH_WRITEMASK";
    InfoParamType[InfoParamType["DEPTH_CLEAR_VALUE"] = 2931] = "DEPTH_CLEAR_VALUE";
    InfoParamType[InfoParamType["DEPTH_FUNC"] = 2932] = "DEPTH_FUNC";
    InfoParamType[InfoParamType["STENCIL_CLEAR_VALUE"] = 2961] = "STENCIL_CLEAR_VALUE";
    InfoParamType[InfoParamType["STENCIL_FUNC"] = 2962] = "STENCIL_FUNC";
    InfoParamType[InfoParamType["STENCIL_FAIL"] = 2964] = "STENCIL_FAIL";
    InfoParamType[InfoParamType["STENCIL_PASS_DEPTH_FAIL"] = 2965] = "STENCIL_PASS_DEPTH_FAIL";
    InfoParamType[InfoParamType["STENCIL_PASS_DEPTH_PASS"] = 2966] = "STENCIL_PASS_DEPTH_PASS";
    InfoParamType[InfoParamType["STENCIL_REF"] = 2967] = "STENCIL_REF";
    InfoParamType[InfoParamType["STENCIL_VALUE_MASK"] = 2963] = "STENCIL_VALUE_MASK";
    InfoParamType[InfoParamType["STENCIL_BACK_FUNC"] = 34816] = "STENCIL_BACK_FUNC";
    InfoParamType[InfoParamType["STENCIL_BACK_FAIL"] = 34817] = "STENCIL_BACK_FAIL";
    InfoParamType[InfoParamType["STENCIL_BACK_PASS_DEPTH_FAIL"] = 34818] = "STENCIL_BACK_PASS_DEPTH_FAIL";
    InfoParamType[InfoParamType["STENCIL_BACK_PASS_DEPTH_PASS"] = 34819] = "STENCIL_BACK_PASS_DEPTH_PASS";
    InfoParamType[InfoParamType["STENCIL_BACK_REF"] = 36003] = "STENCIL_BACK_REF";
    InfoParamType[InfoParamType["STENCIL_BACK_VALUE_MASK"] = 36004] = "STENCIL_BACK_VALUE_MASK";
    InfoParamType[InfoParamType["STENCIL_BACK_WRITEMASK"] = 36005] = "STENCIL_BACK_WRITEMASK";
    InfoParamType[InfoParamType["VIEWPORT"] = 2978] = "VIEWPORT";
    InfoParamType[InfoParamType["SCISSOR_BOX"] = 3088] = "SCISSOR_BOX";
    InfoParamType[InfoParamType["COLOR_CLEAR_VALUE"] = 3106] = "COLOR_CLEAR_VALUE";
    InfoParamType[InfoParamType["COLOR_WRITEMASK"] = 3107] = "COLOR_WRITEMASK";
    InfoParamType[InfoParamType["UNPACK_ALIGNMENT"] = 3317] = "UNPACK_ALIGNMENT";
    InfoParamType[InfoParamType["PACK_ALIGNMENT"] = 3333] = "PACK_ALIGNMENT";
    InfoParamType[InfoParamType["MAX_TEXTURE_SIZE"] = 3379] = "MAX_TEXTURE_SIZE";
    InfoParamType[InfoParamType["MAX_VIEWPORT_DIMS"] = 3386] = "MAX_VIEWPORT_DIMS";
    InfoParamType[InfoParamType["SUBPIXEL_BITS"] = 3408] = "SUBPIXEL_BITS";
    InfoParamType[InfoParamType["RED_BITS"] = 3410] = "RED_BITS";
    InfoParamType[InfoParamType["GREEN_BITS"] = 3411] = "GREEN_BITS";
    InfoParamType[InfoParamType["BLUE_BITS"] = 3412] = "BLUE_BITS";
    InfoParamType[InfoParamType["ALPHA_BITS"] = 3413] = "ALPHA_BITS";
    InfoParamType[InfoParamType["DEPTH_BITS"] = 3414] = "DEPTH_BITS";
    InfoParamType[InfoParamType["STENCIL_BITS"] = 3415] = "STENCIL_BITS";
    InfoParamType[InfoParamType["POLYGON_OFFSET_UNITS"] = 10752] = "POLYGON_OFFSET_UNITS";
    InfoParamType[InfoParamType["POLYGON_OFFSET_FACTOR"] = 32824] = "POLYGON_OFFSET_FACTOR";
    InfoParamType[InfoParamType["TEXTURE_BINDING_2D"] = 32873] = "TEXTURE_BINDING_2D";
    InfoParamType[InfoParamType["SAMPLE_BUFFERS"] = 32936] = "SAMPLE_BUFFERS";
    InfoParamType[InfoParamType["SAMPLES"] = 32937] = "SAMPLES";
    InfoParamType[InfoParamType["SAMPLE_COVERAGE_VALUE"] = 32938] = "SAMPLE_COVERAGE_VALUE";
    InfoParamType[InfoParamType["SAMPLE_COVERAGE_INVERT"] = 32939] = "SAMPLE_COVERAGE_INVERT";
    InfoParamType[InfoParamType["COMPRESSED_TEXTURE_FORMATS"] = 34467] = "COMPRESSED_TEXTURE_FORMATS";
    InfoParamType[InfoParamType["VENDOR"] = 7936] = "VENDOR";
    InfoParamType[InfoParamType["RENDERER"] = 7937] = "RENDERER";
    InfoParamType[InfoParamType["VERSION"] = 7938] = "VERSION";
    InfoParamType[InfoParamType["IMPLEMENTATION_COLOR_READ_TYPE"] = 35738] = "IMPLEMENTATION_COLOR_READ_TYPE";
    InfoParamType[InfoParamType["IMPLEMENTATION_COLOR_READ_FORMAT"] = 35739] = "IMPLEMENTATION_COLOR_READ_FORMAT";
    InfoParamType[InfoParamType["BROWSER_DEFAULT_WEBGL"] = 37444] = "BROWSER_DEFAULT_WEBGL";
    // WebGL2
    InfoParamType[InfoParamType["READ_BUFFER"] = 3074] = "READ_BUFFER";
    InfoParamType[InfoParamType["UNPACK_ROW_LENGTH"] = 3314] = "UNPACK_ROW_LENGTH";
    InfoParamType[InfoParamType["UNPACK_SKIP_ROWS"] = 3315] = "UNPACK_SKIP_ROWS";
    InfoParamType[InfoParamType["UNPACK_SKIP_PIXELS"] = 3316] = "UNPACK_SKIP_PIXELS";
    InfoParamType[InfoParamType["PACK_ROW_LENGTH"] = 3330] = "PACK_ROW_LENGTH";
    InfoParamType[InfoParamType["PACK_SKIP_ROWS"] = 3331] = "PACK_SKIP_ROWS";
    InfoParamType[InfoParamType["PACK_SKIP_PIXELS"] = 3332] = "PACK_SKIP_PIXELS";
    InfoParamType[InfoParamType["TEXTURE_BINDING_3D"] = 32874] = "TEXTURE_BINDING_3D";
    InfoParamType[InfoParamType["UNPACK_SKIP_IMAGES"] = 32877] = "UNPACK_SKIP_IMAGES";
    InfoParamType[InfoParamType["UNPACK_IMAGE_HEIGHT"] = 32878] = "UNPACK_IMAGE_HEIGHT";
    InfoParamType[InfoParamType["MAX_3D_TEXTURE_SIZE"] = 32883] = "MAX_3D_TEXTURE_SIZE";
    InfoParamType[InfoParamType["MAX_ELEMENTS_VERTICES"] = 33000] = "MAX_ELEMENTS_VERTICES";
    InfoParamType[InfoParamType["MAX_ELEMENTS_INDICES"] = 33001] = "MAX_ELEMENTS_INDICES";
    InfoParamType[InfoParamType["MAX_TEXTURE_LOD_BIAS"] = 34045] = "MAX_TEXTURE_LOD_BIAS";
    InfoParamType[InfoParamType["MAX_FRAGMENT_UNIFORM_COMPONENTS"] = 35657] = "MAX_FRAGMENT_UNIFORM_COMPONENTS";
    InfoParamType[InfoParamType["MAX_VERTEX_UNIFORM_COMPONENTS"] = 35658] = "MAX_VERTEX_UNIFORM_COMPONENTS";
    InfoParamType[InfoParamType["MAX_ARRAY_TEXTURE_LAYERS"] = 35071] = "MAX_ARRAY_TEXTURE_LAYERS";
    InfoParamType[InfoParamType["MIN_PROGRAM_TEXEL_OFFSET"] = 35076] = "MIN_PROGRAM_TEXEL_OFFSET";
    InfoParamType[InfoParamType["MAX_PROGRAM_TEXEL_OFFSET"] = 35077] = "MAX_PROGRAM_TEXEL_OFFSET";
    InfoParamType[InfoParamType["MAX_VARYING_COMPONENTS"] = 35659] = "MAX_VARYING_COMPONENTS";
    InfoParamType[InfoParamType["FRAGMENT_SHADER_DERIVATIVE_HINT"] = 35723] = "FRAGMENT_SHADER_DERIVATIVE_HINT";
    InfoParamType[InfoParamType["RASTERIZER_DISCARD"] = 35977] = "RASTERIZER_DISCARD";
    InfoParamType[InfoParamType["VERTEX_ARRAY_BINDING"] = 34229] = "VERTEX_ARRAY_BINDING";
    InfoParamType[InfoParamType["MAX_VERTEX_OUTPUT_COMPONENTS"] = 37154] = "MAX_VERTEX_OUTPUT_COMPONENTS";
    InfoParamType[InfoParamType["MAX_FRAGMENT_INPUT_COMPONENTS"] = 37157] = "MAX_FRAGMENT_INPUT_COMPONENTS";
    InfoParamType[InfoParamType["MAX_SERVER_WAIT_TIMEOUT"] = 37137] = "MAX_SERVER_WAIT_TIMEOUT";
    InfoParamType[InfoParamType["MAX_ELEMENT_INDEX"] = 36203] = "MAX_ELEMENT_INDEX";
})(InfoParamType || (InfoParamType = {}));
/**
 * Constants passed to WebGLRenderingContext.bufferData(), WebGLRenderingContext.bufferSubData(), WebGLRenderingContext.bindBuffer(), or WebGLRenderingContext.getBufferParameter().;
 **/
export var XBufferType;
(function (XBufferType) {
    XBufferType[XBufferType["ARRAY_BUFFER"] = 34962] = "ARRAY_BUFFER";
    XBufferType[XBufferType["ELEMENT_ARRAY_BUFFER"] = 34963] = "ELEMENT_ARRAY_BUFFER";
    // WebGL2
    XBufferType[XBufferType["PIXEL_PACK_BUFFER"] = 35051] = "PIXEL_PACK_BUFFER";
    XBufferType[XBufferType["PIXEL_UNPACK_BUFFER"] = 35052] = "PIXEL_UNPACK_BUFFER";
    XBufferType[XBufferType["PIXEL_PACK_BUFFER_BINDING"] = 35053] = "PIXEL_PACK_BUFFER_BINDING";
    XBufferType[XBufferType["PIXEL_UNPACK_BUFFER_BINDING"] = 35055] = "PIXEL_UNPACK_BUFFER_BINDING";
    XBufferType[XBufferType["COPY_READ_BUFFER"] = 36662] = "COPY_READ_BUFFER";
    XBufferType[XBufferType["COPY_WRITE_BUFFER"] = 36663] = "COPY_WRITE_BUFFER";
    XBufferType[XBufferType["COPY_READ_BUFFER_BINDING"] = 36662] = "COPY_READ_BUFFER_BINDING";
    XBufferType[XBufferType["COPY_WRITE_BUFFER_BINDING"] = 36663] = "COPY_WRITE_BUFFER_BINDING";
})(XBufferType || (XBufferType = {}));
export var BufferUsage;
(function (BufferUsage) {
    BufferUsage[BufferUsage["STATIC_DRAW"] = 35044] = "STATIC_DRAW";
    BufferUsage[BufferUsage["STREAM_DRAW"] = 35040] = "STREAM_DRAW";
    BufferUsage[BufferUsage["DYNAMIC_DRAW"] = 35048] = "DYNAMIC_DRAW";
    // WebGL2
    BufferUsage[BufferUsage["STREAM_READ"] = 35041] = "STREAM_READ";
    BufferUsage[BufferUsage["STREAM_COPY"] = 35042] = "STREAM_COPY";
    BufferUsage[BufferUsage["STATIC_READ"] = 35045] = "STATIC_READ";
    BufferUsage[BufferUsage["STATIC_COPY"] = 35046] = "STATIC_COPY";
    BufferUsage[BufferUsage["DYNAMIC_READ"] = 35049] = "DYNAMIC_READ";
    BufferUsage[BufferUsage["DYNAMIC_COPY"] = 35050] = "DYNAMIC_COPY";
})(BufferUsage || (BufferUsage = {}));
export var BufferTarget;
(function (BufferTarget) {
    BufferTarget[BufferTarget["ARRAY_BUFFER"] = 34962] = "ARRAY_BUFFER";
    BufferTarget[BufferTarget["ELEMENT_ARRAY_BUFFER"] = 34963] = "ELEMENT_ARRAY_BUFFER";
    // WebGL2
    BufferTarget[BufferTarget["PIXEL_PACK_BUFFER"] = 35051] = "PIXEL_PACK_BUFFER";
    BufferTarget[BufferTarget["PIXEL_UNPACK_BUFFER"] = 35052] = "PIXEL_UNPACK_BUFFER";
    BufferTarget[BufferTarget["TRANSFORM_FEEDBACK_BUFFER"] = 35982] = "TRANSFORM_FEEDBACK_BUFFER";
    BufferTarget[BufferTarget["UNIFORM_BUFFER"] = 35345] = "UNIFORM_BUFFER";
    BufferTarget[BufferTarget["COPY_READ_BUFFER"] = 36662] = "COPY_READ_BUFFER";
    BufferTarget[BufferTarget["COPY_WRITE_BUFFER"] = 36663] = "COPY_WRITE_BUFFER";
})(BufferTarget || (BufferTarget = {}));
export var BufferParam;
(function (BufferParam) {
    BufferParam[BufferParam["BUFFER_SIZE"] = 34660] = "BUFFER_SIZE";
    BufferParam[BufferParam["BUFFER_USAGE"] = 34661] = "BUFFER_USAGE"; //Passed to getBufferParameter to get the hint for the buffer passed in when it was created.;
})(BufferParam || (BufferParam = {}));
/**
 * Constants passed to WebGLRenderingContext.getVertexAttrib().;
 **/
export var VertexAttribType;
(function (VertexAttribType) {
    VertexAttribType[VertexAttribType["CURRENT_VERTEX_ATTRIB"] = 34342] = "CURRENT_VERTEX_ATTRIB";
    VertexAttribType[VertexAttribType["VERTEX_ATTRIB_ARRAY_ENABLED"] = 34338] = "VERTEX_ATTRIB_ARRAY_ENABLED";
    VertexAttribType[VertexAttribType["VERTEX_ATTRIB_ARRAY_SIZE"] = 34339] = "VERTEX_ATTRIB_ARRAY_SIZE";
    VertexAttribType[VertexAttribType["VERTEX_ATTRIB_ARRAY_STRIDE"] = 34340] = "VERTEX_ATTRIB_ARRAY_STRIDE";
    VertexAttribType[VertexAttribType["VERTEX_ATTRIB_ARRAY_TYPE"] = 34341] = "VERTEX_ATTRIB_ARRAY_TYPE";
    VertexAttribType[VertexAttribType["VERTEX_ATTRIB_ARRAY_NORMALIZED"] = 34922] = "VERTEX_ATTRIB_ARRAY_NORMALIZED";
    VertexAttribType[VertexAttribType["VERTEX_ATTRIB_ARRAY_POINTER"] = 34373] = "VERTEX_ATTRIB_ARRAY_POINTER";
    VertexAttribType[VertexAttribType["VERTEX_ATTRIB_ARRAY_BUFFER_BINDING"] = 34975] = "VERTEX_ATTRIB_ARRAY_BUFFER_BINDING";
    // WebGL2
    VertexAttribType[VertexAttribType["VERTEX_ATTRIB_ARRAY_INTEGER"] = 35069] = "VERTEX_ATTRIB_ARRAY_INTEGER";
    VertexAttribType[VertexAttribType["VERTEX_ATTRIB_ARRAY_DIVISOR"] = 35070] = "VERTEX_ATTRIB_ARRAY_DIVISOR";
})(VertexAttribType || (VertexAttribType = {}));
/**
 * Constants passed to WebGLRenderingContext.cullFace().;
 **/
export var CullingMode;
(function (CullingMode) {
    CullingMode[CullingMode["CULL_FACE"] = 2884] = "CULL_FACE";
    CullingMode[CullingMode["FRONT"] = 1028] = "FRONT";
    CullingMode[CullingMode["BACK"] = 1029] = "BACK";
    CullingMode[CullingMode["FRONT_AND_BACK"] = 1032] = "FRONT_AND_BACK";
})(CullingMode || (CullingMode = {}));
/**
 * Constants passed to WebGLRenderingContext.enable() or WebGLRenderingContext.disable().;
 **/
export var GLMode;
(function (GLMode) {
    GLMode[GLMode["BLEND"] = 3042] = "BLEND";
    GLMode[GLMode["DEPTH_TEST"] = 2929] = "DEPTH_TEST";
    GLMode[GLMode["DITHER"] = 3024] = "DITHER";
    GLMode[GLMode["POLYGON_OFFSET_FILL"] = 32823] = "POLYGON_OFFSET_FILL";
    GLMode[GLMode["SAMPLE_ALPHA_TO_COVERAGE"] = 32926] = "SAMPLE_ALPHA_TO_COVERAGE";
    GLMode[GLMode["SAMPLE_COVERAGE"] = 32928] = "SAMPLE_COVERAGE";
    GLMode[GLMode["SCISSOR_TEST"] = 3089] = "SCISSOR_TEST";
    GLMode[GLMode["STENCIL_TEST"] = 2960] = "STENCIL_TEST";
})(GLMode || (GLMode = {}));
/**
 * Constants returned from WebGLRenderingContext.getError().;
 **/
export var GLError;
(function (GLError) {
    GLError[GLError["NO_ERROR"] = 0] = "NO_ERROR";
    GLError[GLError["INVALID_ENUM"] = 1280] = "INVALID_ENUM";
    GLError[GLError["INVALID_VALUE"] = 1281] = "INVALID_VALUE";
    GLError[GLError["INVALID_OPERATION"] = 1282] = "INVALID_OPERATION";
    GLError[GLError["OUT_OF_MEMORY"] = 1285] = "OUT_OF_MEMORY";
    GLError[GLError["CONTEXT_LOST_WEBGL"] = 37442] = "CONTEXT_LOST_WEBGL";
})(GLError || (GLError = {}));
/**
 * Constants passed to WebGLRenderingContext.frontFace().;
 **/
export var FrontFaceDirection;
(function (FrontFaceDirection) {
    FrontFaceDirection[FrontFaceDirection["CW"] = 2304] = "CW";
    FrontFaceDirection[FrontFaceDirection["CCW"] = 2305] = "CCW";
})(FrontFaceDirection || (FrontFaceDirection = {}));
/**
 * Constants passed to WebGLRenderingContext.hint();
 **/
export var Hint;
(function (Hint) {
    Hint[Hint["DONT_CARE"] = 4352] = "DONT_CARE";
    Hint[Hint["FASTEST"] = 4353] = "FASTEST";
    Hint[Hint["NICEST"] = 4354] = "NICEST";
    Hint[Hint["GENERATE_MIPMAP_HINT"] = 33170] = "GENERATE_MIPMAP_HINT";
})(Hint || (Hint = {}));
export var DataType;
(function (DataType) {
    DataType[DataType["BYTE"] = 5120] = "BYTE";
    DataType[DataType["UNSIGNED_BYTE"] = 5121] = "UNSIGNED_BYTE";
    DataType[DataType["SHORT"] = 5122] = "SHORT";
    DataType[DataType["UNSIGNED_SHORT"] = 5123] = "UNSIGNED_SHORT";
    DataType[DataType["INT"] = 5124] = "INT";
    DataType[DataType["UNSIGNED_INT"] = 5125] = "UNSIGNED_INT";
    DataType[DataType["FLOAT"] = 5126] = "FLOAT";
    // WebGL2
    DataType[DataType["FLOAT_MAT2x3"] = 35685] = "FLOAT_MAT2x3";
    DataType[DataType["FLOAT_MAT2x4"] = 35686] = "FLOAT_MAT2x4";
    DataType[DataType["FLOAT_MAT3x2"] = 35687] = "FLOAT_MAT3x2";
    DataType[DataType["FLOAT_MAT3x4"] = 35688] = "FLOAT_MAT3x4";
    DataType[DataType["FLOAT_MAT4x2"] = 35689] = "FLOAT_MAT4x2";
    DataType[DataType["FLOAT_MAT4x3"] = 35690] = "FLOAT_MAT4x3";
    DataType[DataType["UNSIGNED_INT_VEC2"] = 36294] = "UNSIGNED_INT_VEC2";
    DataType[DataType["UNSIGNED_INT_VEC3"] = 36295] = "UNSIGNED_INT_VEC3";
    DataType[DataType["UNSIGNED_INT_VEC4"] = 36296] = "UNSIGNED_INT_VEC4";
    DataType[DataType["UNSIGNED_NORMALIZED"] = 35863] = "UNSIGNED_NORMALIZED";
    DataType[DataType["SIGNED_NORMALIZED"] = 36764] = "SIGNED_NORMALIZED";
})(DataType || (DataType = {}));
export var PixelFormat;
(function (PixelFormat) {
    PixelFormat[PixelFormat["DEPTH_COMPONENT"] = 6402] = "DEPTH_COMPONENT";
    PixelFormat[PixelFormat["ALPHA"] = 6406] = "ALPHA";
    PixelFormat[PixelFormat["RGB"] = 6407] = "RGB";
    PixelFormat[PixelFormat["RGBA"] = 6408] = "RGBA";
    PixelFormat[PixelFormat["LUMINANCE"] = 6409] = "LUMINANCE";
    PixelFormat[PixelFormat["LUMINANCE_ALPHA"] = 6410] = "LUMINANCE_ALPHA";
})(PixelFormat || (PixelFormat = {}));
export var PixelType;
(function (PixelType) {
    PixelType[PixelType["UNSIGNED_BYTE"] = 5121] = "UNSIGNED_BYTE";
    PixelType[PixelType["UNSIGNED_SHORT_4_4_4_4"] = 32819] = "UNSIGNED_SHORT_4_4_4_4";
    PixelType[PixelType["UNSIGNED_SHORT_5_5_5_1"] = 32820] = "UNSIGNED_SHORT_5_5_5_1";
    PixelType[PixelType["UNSIGNED_SHORT_5_6_5"] = 33635] = "UNSIGNED_SHORT_5_6_5";
    // WebGL2
    PixelType[PixelType["UNSIGNED_INT_2_10_10_10_REV"] = 33640] = "UNSIGNED_INT_2_10_10_10_REV";
    PixelType[PixelType["UNSIGNED_INT_10F_11F_11F_REV"] = 35899] = "UNSIGNED_INT_10F_11F_11F_REV";
    PixelType[PixelType["UNSIGNED_INT_5_9_9_9_REV"] = 35902] = "UNSIGNED_INT_5_9_9_9_REV";
    PixelType[PixelType["FLOAT_32_UNSIGNED_INT_24_8_REV"] = 36269] = "FLOAT_32_UNSIGNED_INT_24_8_REV";
    PixelType[PixelType["UNSIGNED_INT_24_8"] = 34042] = "UNSIGNED_INT_24_8";
    PixelType[PixelType["HALF_FLOAT"] = 5131] = "HALF_FLOAT";
    PixelType[PixelType["RG"] = 33319] = "RG";
    PixelType[PixelType["RG_INTEGER"] = 33320] = "RG_INTEGER";
    PixelType[PixelType["INT_2_10_10_10_REV"] = 36255] = "INT_2_10_10_10_REV";
})(PixelType || (PixelType = {}));
/**
 * Constants passed to WebGLRenderingContext.createShader() or WebGLRenderingContext.getShaderParameter();
 **/
export var ShaderType;
(function (ShaderType) {
    ShaderType[ShaderType["FRAGMENT_SHADER"] = 35632] = "FRAGMENT_SHADER";
    ShaderType[ShaderType["VERTEX_SHADER"] = 35633] = "VERTEX_SHADER";
})(ShaderType || (ShaderType = {}));
export var ShaderParamType;
(function (ShaderParamType) {
    ShaderParamType[ShaderParamType["COMPILE_STATUS"] = 35713] = "COMPILE_STATUS";
    ShaderParamType[ShaderParamType["DELETE_STATUS"] = 35712] = "DELETE_STATUS";
    ShaderParamType[ShaderParamType["SHADER_TYPE"] = 35663] = "SHADER_TYPE";
})(ShaderParamType || (ShaderParamType = {}));
export var ProgramParamType;
(function (ProgramParamType) {
    ProgramParamType[ProgramParamType["DELETE_STATUS"] = 35712] = "DELETE_STATUS";
    ProgramParamType[ProgramParamType["LINK_STATUS"] = 35714] = "LINK_STATUS";
    ProgramParamType[ProgramParamType["VALIDATE_STATUS"] = 35715] = "VALIDATE_STATUS";
    ProgramParamType[ProgramParamType["ATTACHED_SHADERS"] = 35717] = "ATTACHED_SHADERS";
    ProgramParamType[ProgramParamType["ACTIVE_ATTRIBUTES"] = 35721] = "ACTIVE_ATTRIBUTES";
    ProgramParamType[ProgramParamType["ACTIVE_UNIFORMS"] = 35718] = "ACTIVE_UNIFORMS";
    ProgramParamType[ProgramParamType["MAX_VERTEX_ATTRIBS"] = 34921] = "MAX_VERTEX_ATTRIBS";
    ProgramParamType[ProgramParamType["MAX_VERTEX_UNIFORM_VECTORS"] = 36347] = "MAX_VERTEX_UNIFORM_VECTORS";
    ProgramParamType[ProgramParamType["MAX_VARYING_VECTORS"] = 36348] = "MAX_VARYING_VECTORS";
    ProgramParamType[ProgramParamType["MAX_COMBINED_TEXTURE_IMAGE_UNITS"] = 35661] = "MAX_COMBINED_TEXTURE_IMAGE_UNITS";
    ProgramParamType[ProgramParamType["MAX_VERTEX_TEXTURE_IMAGE_UNITS"] = 35660] = "MAX_VERTEX_TEXTURE_IMAGE_UNITS";
    ProgramParamType[ProgramParamType["MAX_TEXTURE_IMAGE_UNITS"] = 34930] = "MAX_TEXTURE_IMAGE_UNITS";
    ProgramParamType[ProgramParamType["MAX_FRAGMENT_UNIFORM_VECTORS"] = 36349] = "MAX_FRAGMENT_UNIFORM_VECTORS";
    ProgramParamType[ProgramParamType["SHADER_TYPE"] = 35663] = "SHADER_TYPE";
    ProgramParamType[ProgramParamType["SHADING_LANGUAGE_VERSION"] = 35724] = "SHADING_LANGUAGE_VERSION";
    ProgramParamType[ProgramParamType["CURRENT_PROGRAM"] = 35725] = "CURRENT_PROGRAM";
})(ProgramParamType || (ProgramParamType = {}));
/**
 * Constants passed to WebGLRenderingContext.depthFunc() or WebGLRenderingContext.stencilFunc().;
 **/
export var DepthAndStencilTest;
(function (DepthAndStencilTest) {
    DepthAndStencilTest[DepthAndStencilTest["NEVER"] = 512] = "NEVER";
    DepthAndStencilTest[DepthAndStencilTest["LESS"] = 513] = "LESS";
    DepthAndStencilTest[DepthAndStencilTest["EQUAL"] = 514] = "EQUAL";
    DepthAndStencilTest[DepthAndStencilTest["LEQUAL"] = 515] = "LEQUAL";
    DepthAndStencilTest[DepthAndStencilTest["GREATER"] = 516] = "GREATER";
    DepthAndStencilTest[DepthAndStencilTest["NOTEQUAL"] = 517] = "NOTEQUAL";
    DepthAndStencilTest[DepthAndStencilTest["GEQUAL"] = 518] = "GEQUAL";
    DepthAndStencilTest[DepthAndStencilTest["ALWAYS"] = 519] = "ALWAYS";
})(DepthAndStencilTest || (DepthAndStencilTest = {}));
/**
 * Constants passed to WebGLRenderingContext.stencilOp().;
 **/
export var StencilAction;
(function (StencilAction) {
    StencilAction[StencilAction["KEEP"] = 7680] = "KEEP";
    StencilAction[StencilAction["REPLACE"] = 7681] = "REPLACE";
    StencilAction[StencilAction["INCR"] = 7682] = "INCR";
    StencilAction[StencilAction["DECR"] = 7683] = "DECR";
    StencilAction[StencilAction["INVERT"] = 5386] = "INVERT";
    StencilAction[StencilAction["INCR_WRAP"] = 34055] = "INCR_WRAP";
    StencilAction[StencilAction["DECR_WRAP"] = 34056] = "DECR_WRAP";
})(StencilAction || (StencilAction = {}));
/**
 * Constants passed to WebGLRenderingContext.texParameteri(), WebGLRenderingContext.texParameterf(), WebGLRenderingContext.bindTexture(), WebGLRenderingContext.texImage2D(), and others.;
 **/
export var TextureParam;
(function (TextureParam) {
    TextureParam[TextureParam["NEAREST"] = 9728] = "NEAREST";
    TextureParam[TextureParam["LINEAR"] = 9729] = "LINEAR";
    TextureParam[TextureParam["NEAREST_MIPMAP_NEAREST"] = 9984] = "NEAREST_MIPMAP_NEAREST";
    TextureParam[TextureParam["LINEAR_MIPMAP_NEAREST"] = 9985] = "LINEAR_MIPMAP_NEAREST";
    TextureParam[TextureParam["NEAREST_MIPMAP_LINEAR"] = 9986] = "NEAREST_MIPMAP_LINEAR";
    TextureParam[TextureParam["LINEAR_MIPMAP_LINEAR"] = 9987] = "LINEAR_MIPMAP_LINEAR";
    TextureParam[TextureParam["TEXTURE_MAG_FILTER"] = 10240] = "TEXTURE_MAG_FILTER";
    TextureParam[TextureParam["TEXTURE_MIN_FILTER"] = 10241] = "TEXTURE_MIN_FILTER";
    TextureParam[TextureParam["TEXTURE_WRAP_S"] = 10242] = "TEXTURE_WRAP_S";
    TextureParam[TextureParam["TEXTURE_WRAP_T"] = 10243] = "TEXTURE_WRAP_T";
    TextureParam[TextureParam["TEXTURE"] = 5890] = "TEXTURE";
    TextureParam[TextureParam["TEXTURE_BINDING_CUBE_MAP"] = 34068] = "TEXTURE_BINDING_CUBE_MAP";
    TextureParam[TextureParam["TEXTURE_CUBE_MAP_POSITIVE_X"] = 34069] = "TEXTURE_CUBE_MAP_POSITIVE_X";
    TextureParam[TextureParam["TEXTURE_CUBE_MAP_NEGATIVE_X"] = 34070] = "TEXTURE_CUBE_MAP_NEGATIVE_X";
    TextureParam[TextureParam["TEXTURE_CUBE_MAP_POSITIVE_Y"] = 34071] = "TEXTURE_CUBE_MAP_POSITIVE_Y";
    TextureParam[TextureParam["TEXTURE_CUBE_MAP_NEGATIVE_Y"] = 34072] = "TEXTURE_CUBE_MAP_NEGATIVE_Y";
    TextureParam[TextureParam["TEXTURE_CUBE_MAP_POSITIVE_Z"] = 34073] = "TEXTURE_CUBE_MAP_POSITIVE_Z";
    TextureParam[TextureParam["TEXTURE_CUBE_MAP_NEGATIVE_Z"] = 34074] = "TEXTURE_CUBE_MAP_NEGATIVE_Z";
    TextureParam[TextureParam["MAX_CUBE_MAP_TEXTURE_SIZE"] = 34076] = "MAX_CUBE_MAP_TEXTURE_SIZE";
    TextureParam[TextureParam["ACTIVE_TEXTURE"] = 34016] = "ACTIVE_TEXTURE";
    TextureParam[TextureParam["REPEAT"] = 10497] = "REPEAT";
    TextureParam[TextureParam["CLAMP_TO_EDGE"] = 33071] = "CLAMP_TO_EDGE";
    TextureParam[TextureParam["MIRRORED_REPEAT"] = 33648] = "MIRRORED_REPEAT";
    // WebGL2
    TextureParam[TextureParam["RED"] = 6403] = "RED";
    TextureParam[TextureParam["RGB8"] = 32849] = "RGB8";
    TextureParam[TextureParam["RGBA8"] = 32856] = "RGBA8";
    TextureParam[TextureParam["RGB10_A2"] = 32857] = "RGB10_A2";
    TextureParam[TextureParam["TEXTURE_WRAP_R"] = 32882] = "TEXTURE_WRAP_R";
    TextureParam[TextureParam["TEXTURE_MIN_LOD"] = 33082] = "TEXTURE_MIN_LOD";
    TextureParam[TextureParam["TEXTURE_MAX_LOD"] = 33083] = "TEXTURE_MAX_LOD";
    TextureParam[TextureParam["TEXTURE_BASE_LEVEL"] = 33084] = "TEXTURE_BASE_LEVEL";
    TextureParam[TextureParam["TEXTURE_MAX_LEVEL"] = 33085] = "TEXTURE_MAX_LEVEL";
    TextureParam[TextureParam["TEXTURE_COMPARE_MODE"] = 34892] = "TEXTURE_COMPARE_MODE";
    TextureParam[TextureParam["TEXTURE_COMPARE_FUNC"] = 34893] = "TEXTURE_COMPARE_FUNC";
    TextureParam[TextureParam["SRGB"] = 35904] = "SRGB";
    TextureParam[TextureParam["SRGB8"] = 35905] = "SRGB8";
    TextureParam[TextureParam["SRGB8_ALPHA8"] = 35907] = "SRGB8_ALPHA8";
    TextureParam[TextureParam["COMPARE_REF_TO_TEXTURE"] = 34894] = "COMPARE_REF_TO_TEXTURE";
    TextureParam[TextureParam["RGBA32F"] = 34836] = "RGBA32F";
    TextureParam[TextureParam["RGB32F"] = 34837] = "RGB32F";
    TextureParam[TextureParam["RGBA16F"] = 34842] = "RGBA16F";
    TextureParam[TextureParam["RGB16F"] = 34843] = "RGB16F";
    TextureParam[TextureParam["TEXTURE_BINDING_2D_ARRAY"] = 35869] = "TEXTURE_BINDING_2D_ARRAY";
    TextureParam[TextureParam["R11F_G11F_B10F"] = 35898] = "R11F_G11F_B10F";
    TextureParam[TextureParam["RGB9_E5"] = 35901] = "RGB9_E5";
    TextureParam[TextureParam["RGBA32UI"] = 36208] = "RGBA32UI";
    TextureParam[TextureParam["RGB32UI"] = 36209] = "RGB32UI";
    TextureParam[TextureParam["RGBA16UI"] = 36214] = "RGBA16UI";
    TextureParam[TextureParam["RGB16UI"] = 36215] = "RGB16UI";
    TextureParam[TextureParam["RGBA8UI"] = 36220] = "RGBA8UI";
    TextureParam[TextureParam["RGB8UI"] = 36221] = "RGB8UI";
    TextureParam[TextureParam["RGBA32I"] = 36226] = "RGBA32I";
    TextureParam[TextureParam["RGB32I"] = 36227] = "RGB32I";
    TextureParam[TextureParam["RGBA16I"] = 36232] = "RGBA16I";
    TextureParam[TextureParam["RGB16I"] = 36233] = "RGB16I";
    TextureParam[TextureParam["RGBA8I"] = 36238] = "RGBA8I";
    TextureParam[TextureParam["RGB8I"] = 36239] = "RGB8I";
    TextureParam[TextureParam["RED_INTEGER"] = 36244] = "RED_INTEGER";
    TextureParam[TextureParam["RGB_INTEGER"] = 36248] = "RGB_INTEGER";
    TextureParam[TextureParam["RGBA_INTEGER"] = 36249] = "RGBA_INTEGER";
    TextureParam[TextureParam["R8"] = 33321] = "R8";
    TextureParam[TextureParam["RG8"] = 33323] = "RG8";
    TextureParam[TextureParam["R16F"] = 33325] = "R16F";
    TextureParam[TextureParam["R32F"] = 33326] = "R32F";
    TextureParam[TextureParam["RG16F"] = 33327] = "RG16F";
    TextureParam[TextureParam["RG32F"] = 33328] = "RG32F";
    TextureParam[TextureParam["R8I"] = 33329] = "R8I";
    TextureParam[TextureParam["R8UI"] = 33330] = "R8UI";
    TextureParam[TextureParam["R16I"] = 33331] = "R16I";
    TextureParam[TextureParam["R16UI"] = 33332] = "R16UI";
    TextureParam[TextureParam["R32I"] = 33333] = "R32I";
    TextureParam[TextureParam["R32UI"] = 33334] = "R32UI";
    TextureParam[TextureParam["RG8I"] = 33335] = "RG8I";
    TextureParam[TextureParam["RG8UI"] = 33336] = "RG8UI";
    TextureParam[TextureParam["RG16I"] = 33337] = "RG16I";
    TextureParam[TextureParam["RG16UI"] = 33338] = "RG16UI";
    TextureParam[TextureParam["RG32I"] = 33339] = "RG32I";
    TextureParam[TextureParam["RG32UI"] = 33340] = "RG32UI";
    TextureParam[TextureParam["R8_SNORM"] = 36756] = "R8_SNORM";
    TextureParam[TextureParam["RG8_SNORM"] = 36757] = "RG8_SNORM";
    TextureParam[TextureParam["RGB8_SNORM"] = 36758] = "RGB8_SNORM";
    TextureParam[TextureParam["RGBA8_SNORM"] = 36759] = "RGBA8_SNORM";
    TextureParam[TextureParam["RGB10_A2UI"] = 36975] = "RGB10_A2UI";
    TextureParam[TextureParam["TEXTURE_IMMUTABLE_FORMAT"] = 37167] = "TEXTURE_IMMUTABLE_FORMAT";
    TextureParam[TextureParam["TEXTURE_IMMUTABLE_LEVELS"] = 33503] = "TEXTURE_IMMUTABLE_LEVELS";
})(TextureParam || (TextureParam = {}));
export var TextureTarget;
(function (TextureTarget) {
    TextureTarget[TextureTarget["TEXTURE_2D"] = 3553] = "TEXTURE_2D";
    TextureTarget[TextureTarget["TEXTURE_3D"] = 32879] = "TEXTURE_3D";
    TextureTarget[TextureTarget["TEXTURE_CUBE_MAP"] = 34067] = "TEXTURE_CUBE_MAP";
    TextureTarget[TextureTarget["TEXTURE_2D_ARRAY"] = 35866] = "TEXTURE_2D_ARRAY";
})(TextureTarget || (TextureTarget = {}));
export var TextureUnit;
(function (TextureUnit) {
    TextureUnit[TextureUnit["TEXTURE0"] = 33984] = "TEXTURE0";
    TextureUnit[TextureUnit["TEXTURE1"] = 33985] = "TEXTURE1";
    TextureUnit[TextureUnit["TEXTURE2"] = 33986] = "TEXTURE2";
    TextureUnit[TextureUnit["TEXTURE3"] = 33987] = "TEXTURE3";
    TextureUnit[TextureUnit["TEXTURE4"] = 33988] = "TEXTURE4";
    TextureUnit[TextureUnit["TEXTURE5"] = 33989] = "TEXTURE5";
    TextureUnit[TextureUnit["TEXTURE6"] = 33990] = "TEXTURE6";
    TextureUnit[TextureUnit["TEXTURE7"] = 33991] = "TEXTURE7";
    TextureUnit[TextureUnit["TEXTURE8"] = 33992] = "TEXTURE8";
    TextureUnit[TextureUnit["TEXTURE9"] = 33993] = "TEXTURE9";
    TextureUnit[TextureUnit["TEXTURE10"] = 33994] = "TEXTURE10";
    TextureUnit[TextureUnit["TEXTURE11"] = 33995] = "TEXTURE11";
    TextureUnit[TextureUnit["TEXTURE12"] = 33996] = "TEXTURE12";
    TextureUnit[TextureUnit["TEXTURE13"] = 33997] = "TEXTURE13";
    TextureUnit[TextureUnit["TEXTURE14"] = 33998] = "TEXTURE14";
    TextureUnit[TextureUnit["TEXTURE15"] = 33999] = "TEXTURE15";
    TextureUnit[TextureUnit["TEXTURE16"] = 34000] = "TEXTURE16";
    TextureUnit[TextureUnit["TEXTURE17"] = 34001] = "TEXTURE17";
    TextureUnit[TextureUnit["TEXTURE18"] = 34002] = "TEXTURE18";
    TextureUnit[TextureUnit["TEXTURE19"] = 34003] = "TEXTURE19";
    TextureUnit[TextureUnit["TEXTURE20"] = 34004] = "TEXTURE20";
    TextureUnit[TextureUnit["TEXTURE21"] = 34005] = "TEXTURE21";
    TextureUnit[TextureUnit["TEXTURE22"] = 34006] = "TEXTURE22";
    TextureUnit[TextureUnit["TEXTURE23"] = 34007] = "TEXTURE23";
    TextureUnit[TextureUnit["TEXTURE24"] = 34008] = "TEXTURE24";
    TextureUnit[TextureUnit["TEXTURE25"] = 34009] = "TEXTURE25";
    TextureUnit[TextureUnit["TEXTURE26"] = 34010] = "TEXTURE26";
    TextureUnit[TextureUnit["TEXTURE27"] = 34011] = "TEXTURE27";
    TextureUnit[TextureUnit["TEXTURE28"] = 34012] = "TEXTURE28";
    TextureUnit[TextureUnit["TEXTURE29"] = 34013] = "TEXTURE29";
    TextureUnit[TextureUnit["TEXTURE30"] = 34014] = "TEXTURE30";
    TextureUnit[TextureUnit["TEXTURE31"] = 34015] = "TEXTURE31";
})(TextureUnit || (TextureUnit = {}));
export var UniformType;
(function (UniformType) {
    UniformType[UniformType["FLOAT_VEC2"] = 35664] = "FLOAT_VEC2";
    UniformType[UniformType["FLOAT_VEC3"] = 35665] = "FLOAT_VEC3";
    UniformType[UniformType["FLOAT_VEC4"] = 35666] = "FLOAT_VEC4";
    UniformType[UniformType["INT_VEC2"] = 35667] = "INT_VEC2";
    UniformType[UniformType["INT_VEC3"] = 35668] = "INT_VEC3";
    UniformType[UniformType["INT_VEC4"] = 35669] = "INT_VEC4";
    UniformType[UniformType["BOOL"] = 35670] = "BOOL";
    UniformType[UniformType["BOOL_VEC2"] = 35671] = "BOOL_VEC2";
    UniformType[UniformType["BOOL_VEC3"] = 35672] = "BOOL_VEC3";
    UniformType[UniformType["BOOL_VEC4"] = 35673] = "BOOL_VEC4";
    UniformType[UniformType["FLOAT_MAT2"] = 35674] = "FLOAT_MAT2";
    UniformType[UniformType["FLOAT_MAT3"] = 35675] = "FLOAT_MAT3";
    UniformType[UniformType["FLOAT_MAT4"] = 35676] = "FLOAT_MAT4";
    UniformType[UniformType["SAMPLER_2D"] = 35678] = "SAMPLER_2D";
    UniformType[UniformType["SAMPLER_CUBE"] = 35680] = "SAMPLER_CUBE";
    // WebGL2
    UniformType[UniformType["UNIFORM_BUFFER"] = 35345] = "UNIFORM_BUFFER";
    UniformType[UniformType["UNIFORM_BUFFER_BINDING"] = 35368] = "UNIFORM_BUFFER_BINDING";
    UniformType[UniformType["UNIFORM_BUFFER_START"] = 35369] = "UNIFORM_BUFFER_START";
    UniformType[UniformType["UNIFORM_BUFFER_SIZE"] = 35370] = "UNIFORM_BUFFER_SIZE";
    UniformType[UniformType["MAX_VERTEX_UNIFORM_BLOCKS"] = 35371] = "MAX_VERTEX_UNIFORM_BLOCKS";
    UniformType[UniformType["MAX_FRAGMENT_UNIFORM_BLOCKS"] = 35373] = "MAX_FRAGMENT_UNIFORM_BLOCKS";
    UniformType[UniformType["MAX_COMBINED_UNIFORM_BLOCKS"] = 35374] = "MAX_COMBINED_UNIFORM_BLOCKS";
    UniformType[UniformType["MAX_UNIFORM_BUFFER_BINDINGS"] = 35375] = "MAX_UNIFORM_BUFFER_BINDINGS";
    UniformType[UniformType["MAX_UNIFORM_BLOCK_SIZE"] = 35376] = "MAX_UNIFORM_BLOCK_SIZE";
    UniformType[UniformType["MAX_COMBINED_VERTEX_UNIFORM_COMPONENTS"] = 35377] = "MAX_COMBINED_VERTEX_UNIFORM_COMPONENTS";
    UniformType[UniformType["MAX_COMBINED_FRAGMENT_UNIFORM_COMPONENTS"] = 35379] = "MAX_COMBINED_FRAGMENT_UNIFORM_COMPONENTS";
    UniformType[UniformType["UNIFORM_BUFFER_OFFSET_ALIGNMENT"] = 35380] = "UNIFORM_BUFFER_OFFSET_ALIGNMENT";
    UniformType[UniformType["ACTIVE_UNIFORM_BLOCKS"] = 35382] = "ACTIVE_UNIFORM_BLOCKS";
    UniformType[UniformType["UNIFORM_TYPE"] = 35383] = "UNIFORM_TYPE";
    UniformType[UniformType["UNIFORM_SIZE"] = 35384] = "UNIFORM_SIZE";
    UniformType[UniformType["UNIFORM_BLOCK_INDEX"] = 35386] = "UNIFORM_BLOCK_INDEX";
    UniformType[UniformType["UNIFORM_OFFSET"] = 35387] = "UNIFORM_OFFSET";
    UniformType[UniformType["UNIFORM_ARRAY_STRIDE"] = 35388] = "UNIFORM_ARRAY_STRIDE";
    UniformType[UniformType["UNIFORM_MATRIX_STRIDE"] = 35389] = "UNIFORM_MATRIX_STRIDE";
    UniformType[UniformType["UNIFORM_IS_ROW_MAJOR"] = 35390] = "UNIFORM_IS_ROW_MAJOR";
    UniformType[UniformType["UNIFORM_BLOCK_BINDING"] = 35391] = "UNIFORM_BLOCK_BINDING";
    UniformType[UniformType["UNIFORM_BLOCK_DATA_SIZE"] = 35392] = "UNIFORM_BLOCK_DATA_SIZE";
    UniformType[UniformType["UNIFORM_BLOCK_ACTIVE_UNIFORMS"] = 35394] = "UNIFORM_BLOCK_ACTIVE_UNIFORMS";
    UniformType[UniformType["UNIFORM_BLOCK_ACTIVE_UNIFORM_INDICES"] = 35395] = "UNIFORM_BLOCK_ACTIVE_UNIFORM_INDICES";
    UniformType[UniformType["UNIFORM_BLOCK_REFERENCED_BY_VERTEX_SHADER"] = 35396] = "UNIFORM_BLOCK_REFERENCED_BY_VERTEX_SHADER";
    UniformType[UniformType["UNIFORM_BLOCK_REFERENCED_BY_FRAGMENT_SHADER"] = 35398] = "UNIFORM_BLOCK_REFERENCED_BY_FRAGMENT_SHADER";
})(UniformType || (UniformType = {}));
export var ShaderPrecision;
(function (ShaderPrecision) {
    ShaderPrecision[ShaderPrecision["LOW_FLOAT"] = 36336] = "LOW_FLOAT";
    ShaderPrecision[ShaderPrecision["MEDIUM_FLOAT"] = 36337] = "MEDIUM_FLOAT";
    ShaderPrecision[ShaderPrecision["HIGH_FLOAT"] = 36338] = "HIGH_FLOAT";
    ShaderPrecision[ShaderPrecision["LOW_INT"] = 36339] = "LOW_INT";
    ShaderPrecision[ShaderPrecision["MEDIUM_INT"] = 36340] = "MEDIUM_INT";
    ShaderPrecision[ShaderPrecision["HIGH_INT"] = 36341] = "HIGH_INT";
})(ShaderPrecision || (ShaderPrecision = {}));
export var FrameAndRenderBuffers;
(function (FrameAndRenderBuffers) {
    FrameAndRenderBuffers[FrameAndRenderBuffers["BACK"] = 1029] = "BACK";
    FrameAndRenderBuffers[FrameAndRenderBuffers["RGBA4"] = 32854] = "RGBA4";
    FrameAndRenderBuffers[FrameAndRenderBuffers["RGB5_A1"] = 32855] = "RGB5_A1";
    FrameAndRenderBuffers[FrameAndRenderBuffers["RGB565"] = 36194] = "RGB565";
    FrameAndRenderBuffers[FrameAndRenderBuffers["RGBA8"] = 32856] = "RGBA8";
    FrameAndRenderBuffers[FrameAndRenderBuffers["DEPTH_COMPONENT16"] = 33189] = "DEPTH_COMPONENT16";
    FrameAndRenderBuffers[FrameAndRenderBuffers["STENCIL_INDEX8"] = 36168] = "STENCIL_INDEX8";
    FrameAndRenderBuffers[FrameAndRenderBuffers["DEPTH_STENCIL"] = 34041] = "DEPTH_STENCIL";
    FrameAndRenderBuffers[FrameAndRenderBuffers["RENDERBUFFER_WIDTH"] = 36162] = "RENDERBUFFER_WIDTH";
    FrameAndRenderBuffers[FrameAndRenderBuffers["RENDERBUFFER_HEIGHT"] = 36163] = "RENDERBUFFER_HEIGHT";
    FrameAndRenderBuffers[FrameAndRenderBuffers["RENDERBUFFER_INTERNAL_FORMAT"] = 36164] = "RENDERBUFFER_INTERNAL_FORMAT";
    FrameAndRenderBuffers[FrameAndRenderBuffers["RENDERBUFFER_RED_SIZE"] = 36176] = "RENDERBUFFER_RED_SIZE";
    FrameAndRenderBuffers[FrameAndRenderBuffers["RENDERBUFFER_GREEN_SIZE"] = 36177] = "RENDERBUFFER_GREEN_SIZE";
    FrameAndRenderBuffers[FrameAndRenderBuffers["RENDERBUFFER_BLUE_SIZE"] = 36178] = "RENDERBUFFER_BLUE_SIZE";
    FrameAndRenderBuffers[FrameAndRenderBuffers["RENDERBUFFER_ALPHA_SIZE"] = 36179] = "RENDERBUFFER_ALPHA_SIZE";
    FrameAndRenderBuffers[FrameAndRenderBuffers["RENDERBUFFER_DEPTH_SIZE"] = 36180] = "RENDERBUFFER_DEPTH_SIZE";
    FrameAndRenderBuffers[FrameAndRenderBuffers["RENDERBUFFER_STENCIL_SIZE"] = 36181] = "RENDERBUFFER_STENCIL_SIZE";
    FrameAndRenderBuffers[FrameAndRenderBuffers["FRAMEBUFFER_ATTACHMENT_OBJECT_TYPE"] = 36048] = "FRAMEBUFFER_ATTACHMENT_OBJECT_TYPE";
    FrameAndRenderBuffers[FrameAndRenderBuffers["FRAMEBUFFER_ATTACHMENT_OBJECT_NAME"] = 36049] = "FRAMEBUFFER_ATTACHMENT_OBJECT_NAME";
    FrameAndRenderBuffers[FrameAndRenderBuffers["FRAMEBUFFER_ATTACHMENT_TEXTURE_LEVEL"] = 36050] = "FRAMEBUFFER_ATTACHMENT_TEXTURE_LEVEL";
    FrameAndRenderBuffers[FrameAndRenderBuffers["FRAMEBUFFER_ATTACHMENT_TEXTURE_CUBE_MAP_FACE"] = 36051] = "FRAMEBUFFER_ATTACHMENT_TEXTURE_CUBE_MAP_FACE";
    FrameAndRenderBuffers[FrameAndRenderBuffers["COLOR_ATTACHMENT0"] = 36064] = "COLOR_ATTACHMENT0";
    FrameAndRenderBuffers[FrameAndRenderBuffers["COLOR_ATTACHMENT1"] = 36065] = "COLOR_ATTACHMENT1";
    FrameAndRenderBuffers[FrameAndRenderBuffers["COLOR_ATTACHMENT2"] = 36066] = "COLOR_ATTACHMENT2";
    FrameAndRenderBuffers[FrameAndRenderBuffers["COLOR_ATTACHMENT3"] = 36067] = "COLOR_ATTACHMENT3";
    FrameAndRenderBuffers[FrameAndRenderBuffers["COLOR_ATTACHMENT4"] = 36068] = "COLOR_ATTACHMENT4";
    FrameAndRenderBuffers[FrameAndRenderBuffers["COLOR_ATTACHMENT5"] = 36069] = "COLOR_ATTACHMENT5";
    FrameAndRenderBuffers[FrameAndRenderBuffers["COLOR_ATTACHMENT6"] = 36070] = "COLOR_ATTACHMENT6";
    FrameAndRenderBuffers[FrameAndRenderBuffers["COLOR_ATTACHMENT7"] = 36071] = "COLOR_ATTACHMENT7";
    FrameAndRenderBuffers[FrameAndRenderBuffers["COLOR_ATTACHMENT8"] = 36072] = "COLOR_ATTACHMENT8";
    FrameAndRenderBuffers[FrameAndRenderBuffers["COLOR_ATTACHMENT9"] = 36073] = "COLOR_ATTACHMENT9";
    FrameAndRenderBuffers[FrameAndRenderBuffers["COLOR_ATTACHMENT10"] = 36074] = "COLOR_ATTACHMENT10";
    FrameAndRenderBuffers[FrameAndRenderBuffers["COLOR_ATTACHMENT11"] = 36075] = "COLOR_ATTACHMENT11";
    FrameAndRenderBuffers[FrameAndRenderBuffers["COLOR_ATTACHMENT12"] = 36076] = "COLOR_ATTACHMENT12";
    FrameAndRenderBuffers[FrameAndRenderBuffers["COLOR_ATTACHMENT13"] = 36077] = "COLOR_ATTACHMENT13";
    FrameAndRenderBuffers[FrameAndRenderBuffers["COLOR_ATTACHMENT14"] = 36078] = "COLOR_ATTACHMENT14";
    FrameAndRenderBuffers[FrameAndRenderBuffers["COLOR_ATTACHMENT15"] = 36079] = "COLOR_ATTACHMENT15";
    FrameAndRenderBuffers[FrameAndRenderBuffers["DEPTH_ATTACHMENT"] = 36096] = "DEPTH_ATTACHMENT";
    FrameAndRenderBuffers[FrameAndRenderBuffers["STENCIL_ATTACHMENT"] = 36128] = "STENCIL_ATTACHMENT";
    FrameAndRenderBuffers[FrameAndRenderBuffers["DEPTH_STENCIL_ATTACHMENT"] = 33306] = "DEPTH_STENCIL_ATTACHMENT";
    FrameAndRenderBuffers[FrameAndRenderBuffers["NONE"] = 0] = "NONE";
    FrameAndRenderBuffers[FrameAndRenderBuffers["FRAMEBUFFER_COMPLETE"] = 36053] = "FRAMEBUFFER_COMPLETE";
    FrameAndRenderBuffers[FrameAndRenderBuffers["FRAMEBUFFER_INCOMPLETE_ATTACHMENT"] = 36054] = "FRAMEBUFFER_INCOMPLETE_ATTACHMENT";
    FrameAndRenderBuffers[FrameAndRenderBuffers["FRAMEBUFFER_INCOMPLETE_MISSING_ATTACHMENT"] = 36055] = "FRAMEBUFFER_INCOMPLETE_MISSING_ATTACHMENT";
    FrameAndRenderBuffers[FrameAndRenderBuffers["FRAMEBUFFER_INCOMPLETE_DIMENSIONS"] = 36057] = "FRAMEBUFFER_INCOMPLETE_DIMENSIONS";
    FrameAndRenderBuffers[FrameAndRenderBuffers["FRAMEBUFFER_UNSUPPORTED"] = 36061] = "FRAMEBUFFER_UNSUPPORTED";
    FrameAndRenderBuffers[FrameAndRenderBuffers["FRAMEBUFFER_BINDING"] = 36006] = "FRAMEBUFFER_BINDING";
    FrameAndRenderBuffers[FrameAndRenderBuffers["RENDERBUFFER_BINDING"] = 36007] = "RENDERBUFFER_BINDING";
    FrameAndRenderBuffers[FrameAndRenderBuffers["MAX_RENDERBUFFER_SIZE"] = 34024] = "MAX_RENDERBUFFER_SIZE";
    FrameAndRenderBuffers[FrameAndRenderBuffers["INVALID_FRAMEBUFFER_OPERATION"] = 1286] = "INVALID_FRAMEBUFFER_OPERATION";
    // WebGL2
    FrameAndRenderBuffers[FrameAndRenderBuffers["FRAMEBUFFER_ATTACHMENT_COLOR_ENCODING"] = 33296] = "FRAMEBUFFER_ATTACHMENT_COLOR_ENCODING";
    FrameAndRenderBuffers[FrameAndRenderBuffers["FRAMEBUFFER_ATTACHMENT_COMPONENT_TYPE"] = 33297] = "FRAMEBUFFER_ATTACHMENT_COMPONENT_TYPE";
    FrameAndRenderBuffers[FrameAndRenderBuffers["FRAMEBUFFER_ATTACHMENT_RED_SIZE"] = 33298] = "FRAMEBUFFER_ATTACHMENT_RED_SIZE";
    FrameAndRenderBuffers[FrameAndRenderBuffers["FRAMEBUFFER_ATTACHMENT_GREEN_SIZE"] = 33299] = "FRAMEBUFFER_ATTACHMENT_GREEN_SIZE";
    FrameAndRenderBuffers[FrameAndRenderBuffers["FRAMEBUFFER_ATTACHMENT_BLUE_SIZE"] = 33300] = "FRAMEBUFFER_ATTACHMENT_BLUE_SIZE";
    FrameAndRenderBuffers[FrameAndRenderBuffers["FRAMEBUFFER_ATTACHMENT_ALPHA_SIZE"] = 33301] = "FRAMEBUFFER_ATTACHMENT_ALPHA_SIZE";
    FrameAndRenderBuffers[FrameAndRenderBuffers["FRAMEBUFFER_ATTACHMENT_DEPTH_SIZE"] = 33302] = "FRAMEBUFFER_ATTACHMENT_DEPTH_SIZE";
    FrameAndRenderBuffers[FrameAndRenderBuffers["FRAMEBUFFER_ATTACHMENT_STENCIL_SIZE"] = 33303] = "FRAMEBUFFER_ATTACHMENT_STENCIL_SIZE";
    FrameAndRenderBuffers[FrameAndRenderBuffers["FRAMEBUFFER_DEFAULT"] = 33304] = "FRAMEBUFFER_DEFAULT";
    FrameAndRenderBuffers[FrameAndRenderBuffers["DEPTH24_STENCIL8"] = 35056] = "DEPTH24_STENCIL8";
    FrameAndRenderBuffers[FrameAndRenderBuffers["DRAW_FRAMEBUFFER_BINDING"] = 36006] = "DRAW_FRAMEBUFFER_BINDING";
    FrameAndRenderBuffers[FrameAndRenderBuffers["READ_FRAMEBUFFER_BINDING"] = 36010] = "READ_FRAMEBUFFER_BINDING";
    FrameAndRenderBuffers[FrameAndRenderBuffers["RENDERBUFFER_SAMPLES"] = 36011] = "RENDERBUFFER_SAMPLES";
    FrameAndRenderBuffers[FrameAndRenderBuffers["FRAMEBUFFER_ATTACHMENT_TEXTURE_LAYER"] = 36052] = "FRAMEBUFFER_ATTACHMENT_TEXTURE_LAYER";
    FrameAndRenderBuffers[FrameAndRenderBuffers["FRAMEBUFFER_INCOMPLETE_MULTISAMPLE"] = 36182] = "FRAMEBUFFER_INCOMPLETE_MULTISAMPLE";
})(FrameAndRenderBuffers || (FrameAndRenderBuffers = {}));
export var RenderbufferType;
(function (RenderbufferType) {
    RenderbufferType[RenderbufferType["RENDERBUFFER"] = 36161] = "RENDERBUFFER";
})(RenderbufferType || (RenderbufferType = {}));
export var FramebufferType;
(function (FramebufferType) {
    FramebufferType[FramebufferType["FRAMEBUFFER"] = 36160] = "FRAMEBUFFER";
    // WebGL2
    FramebufferType[FramebufferType["READ_FRAMEBUFFER"] = 36008] = "READ_FRAMEBUFFER";
    FramebufferType[FramebufferType["DRAW_FRAMEBUFFER"] = 36009] = "DRAW_FRAMEBUFFER";
})(FramebufferType || (FramebufferType = {}));
/**
 * Constants passed to WebGLRenderingContext.pixelStorei().;
 **/
export var PixelStorageMode;
(function (PixelStorageMode) {
    PixelStorageMode[PixelStorageMode["UNPACK_FLIP_Y_WEBGL"] = 37440] = "UNPACK_FLIP_Y_WEBGL";
    PixelStorageMode[PixelStorageMode["UNPACK_PREMULTIPLY_ALPHA_WEBGL"] = 37441] = "UNPACK_PREMULTIPLY_ALPHA_WEBGL";
    PixelStorageMode[PixelStorageMode["UNPACK_COLORSPACE_CONVERSION_WEBGL"] = 37443] = "UNPACK_COLORSPACE_CONVERSION_WEBGL";
})(PixelStorageMode || (PixelStorageMode = {}));
export var QueryType;
(function (QueryType) {
    QueryType[QueryType["CURRENT_QUERY"] = 34917] = "CURRENT_QUERY";
    QueryType[QueryType["QUERY_RESULT"] = 34918] = "QUERY_RESULT";
    QueryType[QueryType["QUERY_RESULT_AVAILABLE"] = 34919] = "QUERY_RESULT_AVAILABLE";
    QueryType[QueryType["ANY_SAMPLES_PASSED"] = 35887] = "ANY_SAMPLES_PASSED";
    QueryType[QueryType["ANY_SAMPLES_PASSED_CONSERVATIVE"] = 36202] = "ANY_SAMPLES_PASSED_CONSERVATIVE";
})(QueryType || (QueryType = {}));
export var DrawBufferType;
(function (DrawBufferType) {
    DrawBufferType[DrawBufferType["MAX_DRAW_BUFFERS"] = 34852] = "MAX_DRAW_BUFFERS";
    DrawBufferType[DrawBufferType["DRAW_BUFFER0"] = 34853] = "DRAW_BUFFER0";
    DrawBufferType[DrawBufferType["DRAW_BUFFER1"] = 34854] = "DRAW_BUFFER1";
    DrawBufferType[DrawBufferType["DRAW_BUFFER2"] = 34855] = "DRAW_BUFFER2";
    DrawBufferType[DrawBufferType["DRAW_BUFFER3"] = 34856] = "DRAW_BUFFER3";
    DrawBufferType[DrawBufferType["DRAW_BUFFER4"] = 34857] = "DRAW_BUFFER4";
    DrawBufferType[DrawBufferType["DRAW_BUFFER5"] = 34858] = "DRAW_BUFFER5";
    DrawBufferType[DrawBufferType["DRAW_BUFFER6"] = 34859] = "DRAW_BUFFER6";
    DrawBufferType[DrawBufferType["DRAW_BUFFER7"] = 34860] = "DRAW_BUFFER7";
    DrawBufferType[DrawBufferType["DRAW_BUFFER8"] = 34861] = "DRAW_BUFFER8";
    DrawBufferType[DrawBufferType["DRAW_BUFFER9"] = 34862] = "DRAW_BUFFER9";
    DrawBufferType[DrawBufferType["DRAW_BUFFER10"] = 34863] = "DRAW_BUFFER10";
    DrawBufferType[DrawBufferType["DRAW_BUFFER11"] = 34864] = "DRAW_BUFFER11";
    DrawBufferType[DrawBufferType["DRAW_BUFFER12"] = 34865] = "DRAW_BUFFER12";
    DrawBufferType[DrawBufferType["DRAW_BUFFER13"] = 34866] = "DRAW_BUFFER13";
    DrawBufferType[DrawBufferType["DRAW_BUFFER14"] = 34867] = "DRAW_BUFFER14";
    DrawBufferType[DrawBufferType["DRAW_BUFFER15"] = 34868] = "DRAW_BUFFER15";
    DrawBufferType[DrawBufferType["MAX_COLOR_ATTACHMENTS"] = 36063] = "MAX_COLOR_ATTACHMENTS";
    DrawBufferType[DrawBufferType["COLOR_ATTACHMENT1"] = 36065] = "COLOR_ATTACHMENT1";
    DrawBufferType[DrawBufferType["COLOR_ATTACHMENT2"] = 36066] = "COLOR_ATTACHMENT2";
    DrawBufferType[DrawBufferType["COLOR_ATTACHMENT3"] = 36067] = "COLOR_ATTACHMENT3";
    DrawBufferType[DrawBufferType["COLOR_ATTACHMENT4"] = 36068] = "COLOR_ATTACHMENT4";
    DrawBufferType[DrawBufferType["COLOR_ATTACHMENT5"] = 36069] = "COLOR_ATTACHMENT5";
    DrawBufferType[DrawBufferType["COLOR_ATTACHMENT6"] = 36070] = "COLOR_ATTACHMENT6";
    DrawBufferType[DrawBufferType["COLOR_ATTACHMENT7"] = 36071] = "COLOR_ATTACHMENT7";
    DrawBufferType[DrawBufferType["COLOR_ATTACHMENT8"] = 36072] = "COLOR_ATTACHMENT8";
    DrawBufferType[DrawBufferType["COLOR_ATTACHMENT9"] = 36073] = "COLOR_ATTACHMENT9";
    DrawBufferType[DrawBufferType["COLOR_ATTACHMENT10"] = 36074] = "COLOR_ATTACHMENT10";
    DrawBufferType[DrawBufferType["COLOR_ATTACHMENT11"] = 36075] = "COLOR_ATTACHMENT11";
    DrawBufferType[DrawBufferType["COLOR_ATTACHMENT12"] = 36076] = "COLOR_ATTACHMENT12";
    DrawBufferType[DrawBufferType["COLOR_ATTACHMENT13"] = 36077] = "COLOR_ATTACHMENT13";
    DrawBufferType[DrawBufferType["COLOR_ATTACHMENT14"] = 36078] = "COLOR_ATTACHMENT14";
    DrawBufferType[DrawBufferType["COLOR_ATTACHMENT15"] = 36079] = "COLOR_ATTACHMENT15";
})(DrawBufferType || (DrawBufferType = {}));
export var SamplerType;
(function (SamplerType) {
    SamplerType[SamplerType["SAMPLER_3D"] = 35679] = "SAMPLER_3D";
    SamplerType[SamplerType["SAMPLER_2D_SHADOW"] = 35682] = "SAMPLER_2D_SHADOW";
    SamplerType[SamplerType["SAMPLER_2D_ARRAY"] = 36289] = "SAMPLER_2D_ARRAY";
    SamplerType[SamplerType["SAMPLER_2D_ARRAY_SHADOW"] = 36292] = "SAMPLER_2D_ARRAY_SHADOW";
    SamplerType[SamplerType["SAMPLER_CUBE_SHADOW"] = 36293] = "SAMPLER_CUBE_SHADOW";
    SamplerType[SamplerType["INT_SAMPLER_2D"] = 36298] = "INT_SAMPLER_2D";
    SamplerType[SamplerType["INT_SAMPLER_3D"] = 36299] = "INT_SAMPLER_3D";
    SamplerType[SamplerType["INT_SAMPLER_CUBE"] = 36300] = "INT_SAMPLER_CUBE";
    SamplerType[SamplerType["INT_SAMPLER_2D_ARRAY"] = 36303] = "INT_SAMPLER_2D_ARRAY";
    SamplerType[SamplerType["UNSIGNED_INT_SAMPLER_2D"] = 36306] = "UNSIGNED_INT_SAMPLER_2D";
    SamplerType[SamplerType["UNSIGNED_INT_SAMPLER_3D"] = 36307] = "UNSIGNED_INT_SAMPLER_3D";
    SamplerType[SamplerType["UNSIGNED_INT_SAMPLER_CUBE"] = 36308] = "UNSIGNED_INT_SAMPLER_CUBE";
    SamplerType[SamplerType["UNSIGNED_INT_SAMPLER_2D_ARRAY"] = 36311] = "UNSIGNED_INT_SAMPLER_2D_ARRAY";
    SamplerType[SamplerType["MAX_SAMPLES"] = 36183] = "MAX_SAMPLES";
    SamplerType[SamplerType["SAMPLER_BINDING"] = 35097] = "SAMPLER_BINDING";
})(SamplerType || (SamplerType = {}));
export var TransformFeedbackType;
(function (TransformFeedbackType) {
    TransformFeedbackType[TransformFeedbackType["TRANSFORM_FEEDBACK_BUFFER_MODE"] = 35967] = "TRANSFORM_FEEDBACK_BUFFER_MODE";
    TransformFeedbackType[TransformFeedbackType["MAX_TRANSFORM_FEEDBACK_SEPARATE_COMPONENTS"] = 35968] = "MAX_TRANSFORM_FEEDBACK_SEPARATE_COMPONENTS";
    TransformFeedbackType[TransformFeedbackType["TRANSFORM_FEEDBACK_VARYINGS"] = 35971] = "TRANSFORM_FEEDBACK_VARYINGS";
    TransformFeedbackType[TransformFeedbackType["TRANSFORM_FEEDBACK_BUFFER_START"] = 35972] = "TRANSFORM_FEEDBACK_BUFFER_START";
    TransformFeedbackType[TransformFeedbackType["TRANSFORM_FEEDBACK_BUFFER_SIZE"] = 35973] = "TRANSFORM_FEEDBACK_BUFFER_SIZE";
    TransformFeedbackType[TransformFeedbackType["TRANSFORM_FEEDBACK_PRIMITIVES_WRITTEN"] = 35976] = "TRANSFORM_FEEDBACK_PRIMITIVES_WRITTEN";
    TransformFeedbackType[TransformFeedbackType["MAX_TRANSFORM_FEEDBACK_INTERLEAVED_COMPONENTS"] = 35978] = "MAX_TRANSFORM_FEEDBACK_INTERLEAVED_COMPONENTS";
    TransformFeedbackType[TransformFeedbackType["MAX_TRANSFORM_FEEDBACK_SEPARATE_ATTRIBS"] = 35979] = "MAX_TRANSFORM_FEEDBACK_SEPARATE_ATTRIBS";
    TransformFeedbackType[TransformFeedbackType["INTERLEAVED_ATTRIBS"] = 35980] = "INTERLEAVED_ATTRIBS";
    TransformFeedbackType[TransformFeedbackType["SEPARATE_ATTRIBS"] = 35981] = "SEPARATE_ATTRIBS";
    TransformFeedbackType[TransformFeedbackType["TRANSFORM_FEEDBACK_BUFFER"] = 35982] = "TRANSFORM_FEEDBACK_BUFFER";
    TransformFeedbackType[TransformFeedbackType["TRANSFORM_FEEDBACK_BUFFER_BINDING"] = 35983] = "TRANSFORM_FEEDBACK_BUFFER_BINDING";
    TransformFeedbackType[TransformFeedbackType["TRANSFORM_FEEDBACK"] = 36386] = "TRANSFORM_FEEDBACK";
    TransformFeedbackType[TransformFeedbackType["TRANSFORM_FEEDBACK_PAUSED"] = 36387] = "TRANSFORM_FEEDBACK_PAUSED";
    TransformFeedbackType[TransformFeedbackType["TRANSFORM_FEEDBACK_ACTIVE"] = 36388] = "TRANSFORM_FEEDBACK_ACTIVE";
    TransformFeedbackType[TransformFeedbackType["TRANSFORM_FEEDBACK_BINDING"] = 36389] = "TRANSFORM_FEEDBACK_BINDING";
})(TransformFeedbackType || (TransformFeedbackType = {}));
export var SyncObjectType;
(function (SyncObjectType) {
    SyncObjectType[SyncObjectType["OBJECT_TYPE"] = 37138] = "OBJECT_TYPE";
    SyncObjectType[SyncObjectType["SYNC_CONDITION"] = 37139] = "SYNC_CONDITION";
    SyncObjectType[SyncObjectType["SYNC_STATUS"] = 37140] = "SYNC_STATUS";
    SyncObjectType[SyncObjectType["SYNC_FLAGS"] = 37141] = "SYNC_FLAGS";
    SyncObjectType[SyncObjectType["SYNC_FENCE"] = 37142] = "SYNC_FENCE";
    SyncObjectType[SyncObjectType["SYNC_GPU_COMMANDS_COMPLETE"] = 37143] = "SYNC_GPU_COMMANDS_COMPLETE";
    SyncObjectType[SyncObjectType["UNSIGNALED"] = 37144] = "UNSIGNALED";
    SyncObjectType[SyncObjectType["SIGNALED"] = 37145] = "SIGNALED";
    SyncObjectType[SyncObjectType["ALREADY_SIGNALED"] = 37146] = "ALREADY_SIGNALED";
    SyncObjectType[SyncObjectType["TIMEOUT_EXPIRED"] = 37147] = "TIMEOUT_EXPIRED";
    SyncObjectType[SyncObjectType["CONDITION_SATISFIED"] = 37148] = "CONDITION_SATISFIED";
    SyncObjectType[SyncObjectType["WAIT_FAILED"] = 37149] = "WAIT_FAILED";
    SyncObjectType[SyncObjectType["SYNC_FLUSH_COMMANDS_BIT"] = 1] = "SYNC_FLUSH_COMMANDS_BIT";
})(SyncObjectType || (SyncObjectType = {}));
export var WebGL2Misc;
(function (WebGL2Misc) {
    WebGL2Misc[WebGL2Misc["COLOR"] = 6144] = "COLOR";
    WebGL2Misc[WebGL2Misc["DEPTH"] = 6145] = "DEPTH";
    WebGL2Misc[WebGL2Misc["STENCIL"] = 6146] = "STENCIL";
    WebGL2Misc[WebGL2Misc["MIN"] = 32775] = "MIN";
    WebGL2Misc[WebGL2Misc["MAX"] = 32776] = "MAX";
    WebGL2Misc[WebGL2Misc["DEPTH_COMPONENT24"] = 33190] = "DEPTH_COMPONENT24";
    WebGL2Misc[WebGL2Misc["DEPTH_COMPONENT32F"] = 36012] = "DEPTH_COMPONENT32F";
    WebGL2Misc[WebGL2Misc["DEPTH32F_STENCIL8"] = 36013] = "DEPTH32F_STENCIL8";
    WebGL2Misc[WebGL2Misc["INVALID_INDEX"] = 4294967295] = "INVALID_INDEX";
    WebGL2Misc[WebGL2Misc["TIMEOUT_IGNORED"] = -1] = "TIMEOUT_IGNORED";
    WebGL2Misc[WebGL2Misc["MAX_CLIENT_WAIT_TIMEOUT_WEBGL"] = 37447] = "MAX_CLIENT_WAIT_TIMEOUT_WEBGL";
})(WebGL2Misc || (WebGL2Misc = {}));
export var XANGLE_instanced_arrays;
(function (XANGLE_instanced_arrays) {
    XANGLE_instanced_arrays[XANGLE_instanced_arrays["VERTEX_ATTRIB_ARRAY_DIVISOR_ANGLE"] = 35070] = "VERTEX_ATTRIB_ARRAY_DIVISOR_ANGLE";
})(XANGLE_instanced_arrays || (XANGLE_instanced_arrays = {}));
export var XWEBGL_debug_renderer_info;
(function (XWEBGL_debug_renderer_info) {
    XWEBGL_debug_renderer_info[XWEBGL_debug_renderer_info["UNMASKED_VENDOR_WEBGL"] = 37445] = "UNMASKED_VENDOR_WEBGL";
    XWEBGL_debug_renderer_info[XWEBGL_debug_renderer_info["UNMASKED_RENDERER_WEBGL"] = 37446] = "UNMASKED_RENDERER_WEBGL"; //Passed to getParameter to get the renderer string of the graphics driver.
})(XWEBGL_debug_renderer_info || (XWEBGL_debug_renderer_info = {}));
export var XEXT_texture_filter_anisotropic;
(function (XEXT_texture_filter_anisotropic) {
    XEXT_texture_filter_anisotropic[XEXT_texture_filter_anisotropic["MAX_TEXTURE_MAX_ANISOTROPY_EXT"] = 34047] = "MAX_TEXTURE_MAX_ANISOTROPY_EXT";
    XEXT_texture_filter_anisotropic[XEXT_texture_filter_anisotropic["TEXTURE_MAX_ANISOTROPY_EXT"] = 34046] = "TEXTURE_MAX_ANISOTROPY_EXT";
})(XEXT_texture_filter_anisotropic || (XEXT_texture_filter_anisotropic = {}));
export var XWEBGL_compressed_texture_s3tc;
(function (XWEBGL_compressed_texture_s3tc) {
    XWEBGL_compressed_texture_s3tc[XWEBGL_compressed_texture_s3tc["COMPRESSED_RGB_S3TC_DXT1_EXT"] = 33776] = "COMPRESSED_RGB_S3TC_DXT1_EXT";
    XWEBGL_compressed_texture_s3tc[XWEBGL_compressed_texture_s3tc["COMPRESSED_RGBA_S3TC_DXT1_EXT"] = 33777] = "COMPRESSED_RGBA_S3TC_DXT1_EXT";
    XWEBGL_compressed_texture_s3tc[XWEBGL_compressed_texture_s3tc["COMPRESSED_RGBA_S3TC_DXT3_EXT"] = 33778] = "COMPRESSED_RGBA_S3TC_DXT3_EXT";
    XWEBGL_compressed_texture_s3tc[XWEBGL_compressed_texture_s3tc["COMPRESSED_RGBA_S3TC_DXT5_EXT"] = 33779] = "COMPRESSED_RGBA_S3TC_DXT5_EXT";
})(XWEBGL_compressed_texture_s3tc || (XWEBGL_compressed_texture_s3tc = {}));
export var XWEBGL_compressed_texture_etc;
(function (XWEBGL_compressed_texture_etc) {
    XWEBGL_compressed_texture_etc[XWEBGL_compressed_texture_etc["COMPRESSED_R11_EAC"] = 37488] = "COMPRESSED_R11_EAC";
    XWEBGL_compressed_texture_etc[XWEBGL_compressed_texture_etc["COMPRESSED_SIGNED_R11_EAC"] = 37489] = "COMPRESSED_SIGNED_R11_EAC";
    XWEBGL_compressed_texture_etc[XWEBGL_compressed_texture_etc["COMPRESSED_RG11_EAC"] = 37490] = "COMPRESSED_RG11_EAC";
    XWEBGL_compressed_texture_etc[XWEBGL_compressed_texture_etc["COMPRESSED_SIGNED_RG11_EAC"] = 37491] = "COMPRESSED_SIGNED_RG11_EAC";
    XWEBGL_compressed_texture_etc[XWEBGL_compressed_texture_etc["COMPRESSED_RGB8_ETC2"] = 37492] = "COMPRESSED_RGB8_ETC2";
    XWEBGL_compressed_texture_etc[XWEBGL_compressed_texture_etc["COMPRESSED_RGBA8_ETC2_EAC"] = 37493] = "COMPRESSED_RGBA8_ETC2_EAC";
    XWEBGL_compressed_texture_etc[XWEBGL_compressed_texture_etc["COMPRESSED_SRGB8_ETC2"] = 37494] = "COMPRESSED_SRGB8_ETC2";
    XWEBGL_compressed_texture_etc[XWEBGL_compressed_texture_etc["COMPRESSED_SRGB8_ALPHA8_ETC2_EAC"] = 37495] = "COMPRESSED_SRGB8_ALPHA8_ETC2_EAC";
    XWEBGL_compressed_texture_etc[XWEBGL_compressed_texture_etc["COMPRESSED_RGB8_PUNCHTHROUGH_ALPHA1_ETC2"] = 37496] = "COMPRESSED_RGB8_PUNCHTHROUGH_ALPHA1_ETC2";
    XWEBGL_compressed_texture_etc[XWEBGL_compressed_texture_etc["COMPRESSED_SRGB8_PUNCHTHROUGH_ALPHA1_ETC2"] = 37497] = "COMPRESSED_SRGB8_PUNCHTHROUGH_ALPHA1_ETC2";
})(XWEBGL_compressed_texture_etc || (XWEBGL_compressed_texture_etc = {}));
export var XWEBGL_compressed_texture_pvrtc;
(function (XWEBGL_compressed_texture_pvrtc) {
    XWEBGL_compressed_texture_pvrtc[XWEBGL_compressed_texture_pvrtc["COMPRESSED_RGB_PVRTC_4BPPV1_IMG"] = 35840] = "COMPRESSED_RGB_PVRTC_4BPPV1_IMG";
    XWEBGL_compressed_texture_pvrtc[XWEBGL_compressed_texture_pvrtc["COMPRESSED_RGBA_PVRTC_4BPPV1_IMG"] = 35842] = "COMPRESSED_RGBA_PVRTC_4BPPV1_IMG";
    XWEBGL_compressed_texture_pvrtc[XWEBGL_compressed_texture_pvrtc["COMPRESSED_RGB_PVRTC_2BPPV1_IMG"] = 35841] = "COMPRESSED_RGB_PVRTC_2BPPV1_IMG";
    XWEBGL_compressed_texture_pvrtc[XWEBGL_compressed_texture_pvrtc["COMPRESSED_RGBA_PVRTC_2BPPV1_IMG"] = 35843] = "COMPRESSED_RGBA_PVRTC_2BPPV1_IMG";
})(XWEBGL_compressed_texture_pvrtc || (XWEBGL_compressed_texture_pvrtc = {}));
export var XWEBGL_compressed_texture_etc1;
(function (XWEBGL_compressed_texture_etc1) {
    XWEBGL_compressed_texture_etc1[XWEBGL_compressed_texture_etc1["COMPRESSED_RGB_ETC1_WEBGL"] = 36196] = "COMPRESSED_RGB_ETC1_WEBGL";
})(XWEBGL_compressed_texture_etc1 || (XWEBGL_compressed_texture_etc1 = {}));
export var XWEBGL_compressed_texture_atc;
(function (XWEBGL_compressed_texture_atc) {
    XWEBGL_compressed_texture_atc[XWEBGL_compressed_texture_atc["COMPRESSED_RGB_ATC_WEBGL"] = 35986] = "COMPRESSED_RGB_ATC_WEBGL";
    XWEBGL_compressed_texture_atc[XWEBGL_compressed_texture_atc["COMPRESSED_RGBA_ATC_EXPLICIT_ALPHA_WEBGL"] = 35986] = "COMPRESSED_RGBA_ATC_EXPLICIT_ALPHA_WEBGL";
    XWEBGL_compressed_texture_atc[XWEBGL_compressed_texture_atc["COMPRESSED_RGBA_ATC_INTERPOLATED_ALPHA_WEBGL"] = 34798] = "COMPRESSED_RGBA_ATC_INTERPOLATED_ALPHA_WEBGL";
})(XWEBGL_compressed_texture_atc || (XWEBGL_compressed_texture_atc = {}));
export var XWEBGL_depth_texture;
(function (XWEBGL_depth_texture) {
    XWEBGL_depth_texture[XWEBGL_depth_texture["UNSIGNED_INT_24_8_WEBGL"] = 34042] = "UNSIGNED_INT_24_8_WEBGL";
})(XWEBGL_depth_texture || (XWEBGL_depth_texture = {}));
export var XOES_texture_half_float;
(function (XOES_texture_half_float) {
    XOES_texture_half_float[XOES_texture_half_float["HALF_FLOAT_OES"] = 36193] = "HALF_FLOAT_OES";
})(XOES_texture_half_float || (XOES_texture_half_float = {}));
export var XWEBGL_color_buffer_float;
(function (XWEBGL_color_buffer_float) {
    XWEBGL_color_buffer_float[XWEBGL_color_buffer_float["RGBA32F_EXT"] = 34836] = "RGBA32F_EXT";
    XWEBGL_color_buffer_float[XWEBGL_color_buffer_float["RGB32F_EXT"] = 34837] = "RGB32F_EXT";
    XWEBGL_color_buffer_float[XWEBGL_color_buffer_float["FRAMEBUFFER_ATTACHMENT_COMPONENT_TYPE_EXT"] = 33297] = "FRAMEBUFFER_ATTACHMENT_COMPONENT_TYPE_EXT";
    XWEBGL_color_buffer_float[XWEBGL_color_buffer_float["UNSIGNED_NORMALIZED_EXT"] = 35863] = "UNSIGNED_NORMALIZED_EXT";
})(XWEBGL_color_buffer_float || (XWEBGL_color_buffer_float = {}));
export var XEXT_blend_minmax;
(function (XEXT_blend_minmax) {
    XEXT_blend_minmax[XEXT_blend_minmax["MIN_EXT"] = 32775] = "MIN_EXT";
    XEXT_blend_minmax[XEXT_blend_minmax["MAX_EXT"] = 32776] = "MAX_EXT";
})(XEXT_blend_minmax || (XEXT_blend_minmax = {}));
export var XEXT_sRGB;
(function (XEXT_sRGB) {
    XEXT_sRGB[XEXT_sRGB["SRGB_EXT"] = 35904] = "SRGB_EXT";
    XEXT_sRGB[XEXT_sRGB["SRGB_ALPHA_EXT"] = 35906] = "SRGB_ALPHA_EXT";
    XEXT_sRGB[XEXT_sRGB["SRGB8_ALPHA8_EXT"] = 35907] = "SRGB8_ALPHA8_EXT";
    XEXT_sRGB[XEXT_sRGB["FRAMEBUFFER_ATTACHMENT_COLOR_ENCODING_EXT"] = 33296] = "FRAMEBUFFER_ATTACHMENT_COLOR_ENCODING_EXT";
})(XEXT_sRGB || (XEXT_sRGB = {}));
export var XOES_standard_derivatives;
(function (XOES_standard_derivatives) {
    XOES_standard_derivatives[XOES_standard_derivatives["FRAGMENT_SHADER_DERIVATIVE_HINT_OES"] = 35723] = "FRAGMENT_SHADER_DERIVATIVE_HINT_OES";
})(XOES_standard_derivatives || (XOES_standard_derivatives = {}));
export var XWEBGL_draw_buffers;
(function (XWEBGL_draw_buffers) {
    XWEBGL_draw_buffers[XWEBGL_draw_buffers["COLOR_ATTACHMENT0_WEBGL"] = 36064] = "COLOR_ATTACHMENT0_WEBGL";
    XWEBGL_draw_buffers[XWEBGL_draw_buffers["COLOR_ATTACHMENT1_WEBGL"] = 36065] = "COLOR_ATTACHMENT1_WEBGL";
    XWEBGL_draw_buffers[XWEBGL_draw_buffers["COLOR_ATTACHMENT2_WEBGL"] = 36066] = "COLOR_ATTACHMENT2_WEBGL";
    XWEBGL_draw_buffers[XWEBGL_draw_buffers["COLOR_ATTACHMENT3_WEBGL"] = 36067] = "COLOR_ATTACHMENT3_WEBGL";
    XWEBGL_draw_buffers[XWEBGL_draw_buffers["COLOR_ATTACHMENT4_WEBGL"] = 36068] = "COLOR_ATTACHMENT4_WEBGL";
    XWEBGL_draw_buffers[XWEBGL_draw_buffers["COLOR_ATTACHMENT5_WEBGL"] = 36069] = "COLOR_ATTACHMENT5_WEBGL";
    XWEBGL_draw_buffers[XWEBGL_draw_buffers["COLOR_ATTACHMENT6_WEBGL"] = 36070] = "COLOR_ATTACHMENT6_WEBGL";
    XWEBGL_draw_buffers[XWEBGL_draw_buffers["COLOR_ATTACHMENT7_WEBGL"] = 36071] = "COLOR_ATTACHMENT7_WEBGL";
    XWEBGL_draw_buffers[XWEBGL_draw_buffers["COLOR_ATTACHMENT8_WEBGL"] = 36072] = "COLOR_ATTACHMENT8_WEBGL";
    XWEBGL_draw_buffers[XWEBGL_draw_buffers["COLOR_ATTACHMENT9_WEBGL"] = 36073] = "COLOR_ATTACHMENT9_WEBGL";
    XWEBGL_draw_buffers[XWEBGL_draw_buffers["COLOR_ATTACHMENT10_WEBGL"] = 36074] = "COLOR_ATTACHMENT10_WEBGL";
    XWEBGL_draw_buffers[XWEBGL_draw_buffers["COLOR_ATTACHMENT11_WEBGL"] = 36075] = "COLOR_ATTACHMENT11_WEBGL";
    XWEBGL_draw_buffers[XWEBGL_draw_buffers["COLOR_ATTACHMENT12_WEBGL"] = 36076] = "COLOR_ATTACHMENT12_WEBGL";
    XWEBGL_draw_buffers[XWEBGL_draw_buffers["COLOR_ATTACHMENT13_WEBGL"] = 36077] = "COLOR_ATTACHMENT13_WEBGL";
    XWEBGL_draw_buffers[XWEBGL_draw_buffers["COLOR_ATTACHMENT14_WEBGL"] = 36078] = "COLOR_ATTACHMENT14_WEBGL";
    XWEBGL_draw_buffers[XWEBGL_draw_buffers["COLOR_ATTACHMENT15_WEBGL"] = 36079] = "COLOR_ATTACHMENT15_WEBGL";
    XWEBGL_draw_buffers[XWEBGL_draw_buffers["DRAW_BUFFER0_WEBGL"] = 34853] = "DRAW_BUFFER0_WEBGL";
    XWEBGL_draw_buffers[XWEBGL_draw_buffers["DRAW_BUFFER1_WEBGL"] = 34854] = "DRAW_BUFFER1_WEBGL";
    XWEBGL_draw_buffers[XWEBGL_draw_buffers["DRAW_BUFFER2_WEBGL"] = 34855] = "DRAW_BUFFER2_WEBGL";
    XWEBGL_draw_buffers[XWEBGL_draw_buffers["DRAW_BUFFER3_WEBGL"] = 34856] = "DRAW_BUFFER3_WEBGL";
    XWEBGL_draw_buffers[XWEBGL_draw_buffers["DRAW_BUFFER4_WEBGL"] = 34857] = "DRAW_BUFFER4_WEBGL";
    XWEBGL_draw_buffers[XWEBGL_draw_buffers["DRAW_BUFFER5_WEBGL"] = 34858] = "DRAW_BUFFER5_WEBGL";
    XWEBGL_draw_buffers[XWEBGL_draw_buffers["DRAW_BUFFER6_WEBGL"] = 34859] = "DRAW_BUFFER6_WEBGL";
    XWEBGL_draw_buffers[XWEBGL_draw_buffers["DRAW_BUFFER7_WEBGL"] = 34860] = "DRAW_BUFFER7_WEBGL";
    XWEBGL_draw_buffers[XWEBGL_draw_buffers["DRAW_BUFFER8_WEBGL"] = 34861] = "DRAW_BUFFER8_WEBGL";
    XWEBGL_draw_buffers[XWEBGL_draw_buffers["DRAW_BUFFER9_WEBGL"] = 34862] = "DRAW_BUFFER9_WEBGL";
    XWEBGL_draw_buffers[XWEBGL_draw_buffers["DRAW_BUFFER10_WEBGL"] = 34863] = "DRAW_BUFFER10_WEBGL";
    XWEBGL_draw_buffers[XWEBGL_draw_buffers["DRAW_BUFFER11_WEBGL"] = 34864] = "DRAW_BUFFER11_WEBGL";
    XWEBGL_draw_buffers[XWEBGL_draw_buffers["DRAW_BUFFER12_WEBGL"] = 34865] = "DRAW_BUFFER12_WEBGL";
    XWEBGL_draw_buffers[XWEBGL_draw_buffers["DRAW_BUFFER13_WEBGL"] = 34866] = "DRAW_BUFFER13_WEBGL";
    XWEBGL_draw_buffers[XWEBGL_draw_buffers["DRAW_BUFFER14_WEBGL"] = 34867] = "DRAW_BUFFER14_WEBGL";
    XWEBGL_draw_buffers[XWEBGL_draw_buffers["DRAW_BUFFER15_WEBGL"] = 34868] = "DRAW_BUFFER15_WEBGL";
    XWEBGL_draw_buffers[XWEBGL_draw_buffers["MAX_COLOR_ATTACHMENTS_WEBGL"] = 36063] = "MAX_COLOR_ATTACHMENTS_WEBGL";
    XWEBGL_draw_buffers[XWEBGL_draw_buffers["MAX_DRAW_BUFFERS_WEBGL"] = 34852] = "MAX_DRAW_BUFFERS_WEBGL";
})(XWEBGL_draw_buffers || (XWEBGL_draw_buffers = {}));
export var XOES_vertex_array_object;
(function (XOES_vertex_array_object) {
    XOES_vertex_array_object[XOES_vertex_array_object["VERTEX_ARRAY_BINDING_OES"] = 34229] = "VERTEX_ARRAY_BINDING_OES";
})(XOES_vertex_array_object || (XOES_vertex_array_object = {}));
export var XEXT_disjoint_timer_query;
(function (XEXT_disjoint_timer_query) {
    XEXT_disjoint_timer_query[XEXT_disjoint_timer_query["QUERY_COUNTER_BITS_EXT"] = 34916] = "QUERY_COUNTER_BITS_EXT";
    XEXT_disjoint_timer_query[XEXT_disjoint_timer_query["CURRENT_QUERY_EXT"] = 34917] = "CURRENT_QUERY_EXT";
    XEXT_disjoint_timer_query[XEXT_disjoint_timer_query["QUERY_RESULT_EXT"] = 34918] = "QUERY_RESULT_EXT";
    XEXT_disjoint_timer_query[XEXT_disjoint_timer_query["QUERY_RESULT_AVAILABLE_EXT"] = 34919] = "QUERY_RESULT_AVAILABLE_EXT";
    XEXT_disjoint_timer_query[XEXT_disjoint_timer_query["TIME_ELAPSED_EXT"] = 35007] = "TIME_ELAPSED_EXT";
    XEXT_disjoint_timer_query[XEXT_disjoint_timer_query["TIMESTAMP_EXT"] = 36392] = "TIMESTAMP_EXT";
    XEXT_disjoint_timer_query[XEXT_disjoint_timer_query["GPU_DISJOINT_EXT"] = 36795] = "GPU_DISJOINT_EXT";
})(XEXT_disjoint_timer_query || (XEXT_disjoint_timer_query = {}));
export var XOVR_multiview2;
(function (XOVR_multiview2) {
    XOVR_multiview2[XOVR_multiview2["FRAMEBUFFER_ATTACHMENT_TEXTURE_NUM_VIEWS_OVR"] = 38448] = "FRAMEBUFFER_ATTACHMENT_TEXTURE_NUM_VIEWS_OVR";
    XOVR_multiview2[XOVR_multiview2["FRAMEBUFFER_ATTACHMENT_TEXTURE_BASE_VIEW_INDEX_OVR"] = 38450] = "FRAMEBUFFER_ATTACHMENT_TEXTURE_BASE_VIEW_INDEX_OVR";
    XOVR_multiview2[XOVR_multiview2["MAX_VIEWS_OVR"] = 38449] = "MAX_VIEWS_OVR";
    XOVR_multiview2[XOVR_multiview2["FRAMEBUFFER_INCOMPLETE_VIEW_TARGETS_OVR"] = 38451] = "FRAMEBUFFER_INCOMPLETE_VIEW_TARGETS_OVR";
})(XOVR_multiview2 || (XOVR_multiview2 = {}));
//# sourceMappingURL=GLEnum.js.map