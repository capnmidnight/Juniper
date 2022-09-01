type CSSGlobalValue =
    | "inherit"
    | "initial"
    | "revert"
    | "revert-layer"
    | "unset";

type CSSFunction<F extends string, T extends string | number> = `${F}(${T})`;

type CSSUrl = CSSFunction<"url", string>;

type CSSPercentage = `${number}%`;

type CSSAlpha =
    | number
    | CSSPercentage;

type CSSAngle =
    | number
    | `${number}deg`
    | `${number}rad`
    | `${number}grad`
    | `${number}turn`;

type CSSFontRelativeLength =
    | `${number}cap`
    | `${number}ch`
    | `${number}em`
    | `${number}ex`
    | `${number}ic`
    | `${number}lh`
    | `${number}rem`
    | `${number}rlh`;

type CSSViewportPercentageLength =
    | `${number}vh`
    | `${number}vw`
    | `${number}vi`
    | `${number}vb`
    | `${number}vmin`
    | `${number}vmax`;

type CSSAbsoluteLength =
    | `${number}px`
    | `${number}cm`
    | `${number}mm`
    | `${number}Q`
    | `${number}in`
    | `${number}pc`
    | `${number}pt`;

type CSSWidthKeyword =
    | "thin"
    | "medium"
    | "thick";

type CSSLength =
    | CSSFontRelativeLength
    | CSSViewportPercentageLength
    | CSSAbsoluteLength;

type CSSLengthPercentage =
    | CSSLength
    | CSSPercentage
    | 0
    | "0";

type CSSAtRuleRegular =
    | "charset"
    | "import"
    | "namespace"
    | "layer";

type CSSBorderCollapseValue =
    | "collapse"
    | "separate";

type CSSBorderRepeatValue =
    | "stretch"
    | "repeat"
    | "round"
    | "space";

type CSSBorderStyleValue =
    | "none"
    | "hidden"
    | "dotted"
    | "dashed"
    | "solid"
    | "double"
    | "groove"
    | "ridge"
    | "inset"
    | "outset";

type CSSCalcStatement = CSSFunction<"calc", string>;

type CSSColorKeywordValue =
    | "transparent"
    | "currentcolor"
    | "ActiveText"
    | "ButtonBorder"
    | "ButtonFace"
    | "ButtonText"
    | "Canvas"
    | "CanvasText"
    | "Field"
    | "FieldText"
    | "GrayText"
    | "Highlight"
    | "HighlightText"
    | "LinkText"
    | "Mark"
    | "MarkText"
    | "VisitedText"
    | "aqua"
    | "black"
    | "blue"
    | "fuchsia"
    | "gray"
    | "grey"
    | "green"
    | "lime"
    | "maroon"
    | "navy"
    | "olive"
    | "purple"
    | "red"
    | "silver"
    | "teal"
    | "white"
    | "yellow"
    | "aliceblue"
    | "antiquewhite"
    | "aqua"
    | "aquamarine"
    | "azure"
    | "beige"
    | "bisque"
    | "black"
    | "blanchedalmond"
    | "blue"
    | "blueviolet"
    | "brown"
    | "burlywood"
    | "cadetblue"
    | "chartreuse"
    | "chocolate"
    | "coral"
    | "cornflowerblue"
    | "cornsilk"
    | "crimson"
    | "cyan"
    | "darkblue"
    | "darkcyan"
    | "darkgoldenrod"
    | "darkgray"
    | "darkgreen"
    | "darkkhaki"
    | "darkmagenta"
    | "darkolivegreen"
    | "darkorange"
    | "darkorchid"
    | "darkred"
    | "darksalmon"
    | "darkseagreen"
    | "darkslateblue"
    | "darkslategray"
    | "darkturquoise"
    | "darkviolet"
    | "deeppink"
    | "deepskyblue"
    | "dimgray"
    | "dodgerblue"
    | "firebrick"
    | "floralwhite"
    | "forestgreen"
    | "fuchsia"
    | "gainsboro"
    | "ghostwhite"
    | "gold"
    | "goldenrod"
    | "gray"
    | "green"
    | "greenyellow"
    | "honeydew"
    | "hotpink"
    | "indianred"
    | "indigo"
    | "ivory"
    | "khaki"
    | "lavender"
    | "lavenderblush"
    | "lawngreen"
    | "lemonchiffon"
    | "lightblue"
    | "lightcoral"
    | "lightcyan"
    | "lightgoldenrodyellow"
    | "lightgreen"
    | "lightgrey"
    | "lightpink"
    | "lightsalmon"
    | "lightseagreen"
    | "lightskyblue"
    | "lightslategray"
    | "lightsteelblue"
    | "lightyellow"
    | "lime"
    | "limegreen"
    | "linen"
    | "magenta"
    | "maroon"
    | "mediumaquamarine"
    | "mediumblue"
    | "mediumorchid"
    | "mediumpurple"
    | "mediumseagreen"
    | "mediumslateblue"
    | "mediumspringgreen"
    | "mediumturquoise"
    | "mediumvioletred"
    | "midnightblue"
    | "mintcream"
    | "mistyrose"
    | "moccasin"
    | "navajowhite"
    | "navy"
    | "navyblue"
    | "oldlace"
    | "olive"
    | "olivedrab"
    | "orange"
    | "orangered"
    | "orchid"
    | "palegoldenrod"
    | "palegreen"
    | "paleturquoise"
    | "palevioletred"
    | "papayawhip"
    | "peachpuff"
    | "peru"
    | "pink"
    | "plum"
    | "powderblue"
    | "purple"
    | "red"
    | "rosybrown"
    | "royalblue"
    | "saddlebrown"
    | "salmon"
    | "sandybrown"
    | "seagreen"
    | "seashell"
    | "sienna"
    | "silver"
    | "skyblue"
    | "slateblue"
    | "slategray"
    | "snow"
    | "springgreen"
    | "steelblue"
    | "tan"
    | "teal"
    | "thistle"
    | "tomato"
    | "turquoise"
    | "violet"
    | "wheat"
    | "white"
    | "whitesmoke"
    | "yellow"
    | "yellowgreen";

type HexDigit = "0" | "1" | "2" | "3" | "4" | "5" | "6" | "7" | "8" | "9" | "a" | "b" | "c" | "d" | "e" | "f" | "A" | "B" | "C" | "D" | "E" | "F";
type HexNumber = `${HexDigit}${HexDigit}`
type CSSColorHashValue = `#${string}`;

type CSSColorRGBValue = CSSFunction<"rgb" | "rgba", string>;
type CSSColorHSLValue = CSSFunction<"hsl" | "hsla", string>;
type CSSColorHWBValue = CSSFunction<"hwb", string>;
type CSSColorLCHValue = CSSFunction<"lch", string>;
type CSSColorLabValue = CSSFunction<"lab", string>;

type CSSColorSpaceName =
    | "srgb"
    | "srgb-linear"
    | "display-p3"
    | "a98-rgb"
    | "prophoto-rgb"
    | "rec2020"
    | "xyz"
    | "xyz-d50"
    | "xyz-d65";

type CSSColorFunctionValue = CSSFunction<"color", `${CSSColorSpaceName} ${string}`>;

type CSSColorValue =
    | CSSColorKeywordValue
    | CSSColorHashValue
    | CSSColorRGBValue
    | CSSColorHSLValue
    | CSSColorHWBValue
    | CSSColorLCHValue
    | CSSColorLabValue
    | CSSColorFunctionValue;

type CSSColorProfilePropName =
    | "src"
    | "rendering-intent"
    | "components";

type CSSCounterStylePropName =
    | "system"
    | "negative"
    | "prefix"
    | "suffix"
    | "range"
    | "pad"
    | "fallback"
    | "symbols"
    | "additive-symbols"
    | "speak-as";

type CSSCounterStyleSystemValue =
    | "cyclic"
    | "numeric"
    | "alphabetic"
    | "symbolic"
    | "additive"
    | "fixed"
    | `fixed ${number}`
    | `extends ${string}`;

type CSSCounterStyleRangeValue =
    | `${number} ${number}`
    | `${number} infinite`
    | `infinite ${number}`
    | `infinite infinite`;

type CSSSizePropertyValue =
    | 0
    | "0"
    | CSSLengthPercentage
    | "auto"
    | "max-content"
    | "min-content"
    | "fit-content"
    | CSSCalcStatement;

type CSSAlignItemsValue =
    | "center"
    | "start"
    | "end"
    | "flex-start"
    | "flex-end"
    | "normal"
    | "baseline"
    | "first baseline"
    | "last baseline"
    | "stretch"
    | "safe center"
    | "unsafe center";

type CSSAlignContentValue =
    | CSSAlignItemsValue
    | "space-between"
    | "space-around"
    | "space-evenly";

type CSSAlignSelfValue =
    | CSSAlignItemsValue
    | "auto"
    | "self-start"
    | "self-end";

type CSSAlignmentBaselineValue =
    | "auto"
    | "baseline"
    | "before-edge"
    | "text-before-edge"
    | "middle"
    | "central"
    | "after-edge"
    | "text-after-edge"
    | "ideographic"
    | "alphabetic"
    | "hanging"
    | "mathematical"
    | "top"
    | "center"
    | "bottom";

type CSSTimeSecondsValue = `${number}s`;
type CSSTimeMillisecondsValue = `${number}ms`;
type CSSTimeValue = CSSTimeSecondsValue | CSSTimeMillisecondsValue;

type CSSAnimationDirectionValue =
    | "normal"
    | "reverse"
    | "alternate"
    | "alternate-reverse";

type CSSAnimationFillModeValue =
    | "none"
    | "forwards"
    | "backwards"
    | "both";

type CSSAnimationIterationCountValue =
    | number
    | "infinite";

type CSSAnimationPlayStateValue =
    | "running"
    | "paused";

type CSSAnimationTimingFunctionNamed =
    | "ease"
    | "ease-in"
    | "ease-out"
    | "ease-in-out"
    | "linear"
    | "step-start"
    | "step-end";
type CSSAnimationTimingFunctionCubicBezier = `cubic-bezier(${number}, ${number}, ${number}, ${number})`;
type CSSAnimationTimingFunctionSteps = `steps(${number}, ${CSSAnimationTimingFunctionNamed})`;
type CSSAnimationTimingFunctionValue =
    | CSSAnimationTimingFunctionNamed
    | CSSAnimationTimingFunctionCubicBezier
    | CSSAnimationTimingFunctionSteps;

type CSSAppearanceValue =
    | "none"
    | "auto"
    | "menulist-button"
    | "textfield"
    | "button"
    | "checkbox"
    | "listbox"
    | "menulist"
    | "meter"
    | "progress-bar"
    | "push-button"
    | "radio"
    | "searchfield"
    | "slider-horizontal"
    | "square-button"
    | "textarea";

type CSSDropShadowParams =
    | `${CSSLength} ${CSSLength}`
    | `${CSSLength} ${CSSLength} ${string}`
    | `${CSSLength} ${CSSLength} ${CSSLength} ${string}`;


type CSSFilterFunction = CSSFunction<"blur", CSSLength>
    | CSSFunction<"brightness", CSSAlpha>
    | CSSFunction<"contrast", CSSAlpha>
    | CSSFunction<"drop-shadow", CSSDropShadowParams>
    | CSSFunction<"grayscale", CSSAlpha>
    | CSSFunction<"hue-rotate", CSSAngle>
    | CSSFunction<"invert", CSSAlpha>
    | CSSFunction<"opacity", CSSAlpha>
    | CSSFunction<"saturate", CSSAlpha>
    | CSSFunction<"sepia", CSSAlpha>
    | CSSUrl;

type CSSBackfaceVisibilityValue =
    | "visible"
    | "hiden";

type CSSBackgroundAttachmentValue =
    | "scroll"
    | "fixed"
    | "local";

type CSSBlendModeValue = "normal"
    | "multiply"
    | "screen"
    | "overlay"
    | "darken"
    | "lighten"
    | "color-dodge"
    | "color-burn"
    | "hard-light"
    | "soft-light"
    | "difference"
    | "exclusion"
    | "hue"
    | "saturation"
    | "color"
    | "luminosity";

type CSSBoxType =
    | "border-box"
    | "padding-box"
    | "content-box";

type CSSClipValue =
    | CSSBoxType
    | "text";

type CSSGradient =
    | CSSFunction<"linear-gradient", string>
    | CSSFunction<"radial-gradient", string>
    | CSSFunction<"repeating-linear-gradient", string>
    | CSSFunction<"repeating-radial-gradient", string>
    | CSSFunction<"conic-gradient", string>;

type CSSImage = CSSUrl
    | CSSGradient
    | CSSFunction<"element", string>
    | CSSFunction<"cross-fade", string>
    | CSSFunction<"image-set", string>;

type CSSBackgroundPositionHorizontalKeyword =
    | "left"
    | "center"
    | "right";
type CSSBackgroundPositionVerticalKeyword =
    | "top"
    | "center"
    | "bottom";
type CSSBackgroundPositionKeyword =
    | CSSBackgroundPositionHorizontalKeyword
    | CSSBackgroundPositionVerticalKeyword;
type CSSBackgroundPositionHorizontalValue =
    | CSSBackgroundPositionKeyword
    | CSSLengthPercentage;
type CSSBackgroundPositionVerticalValue =
    | CSSBackgroundPositionKeyword
    | CSSLengthPercentage;
type CSSBackgroundPositionValue =
    | CSSBackgroundPositionKeyword
    | CSSLengthPercentage;

type CSSBackgroundRepeat =
    | "repeat"
    | "space"
    | "round"
    | "no-repeat";

type CSSBackgroundRepeatValue =
    | "repeat-x"
    | "repeat-y"
    | CSSBackgroundRepeat;

type CSSBackgroundSizeValue =
    | "auto"
    | CSSLengthPercentage;
type CSSBackgroundSizeSingleValue =
    | "contain"
    | "cover"
    | CSSBackgroundSizeValue;

type CSSElementPositionValue =
    | "auto"
    | CSSLengthPercentage;

type CSSCursorValue =
    | "auto"
    | "default"
    | "none"
    | "context-menu"
    | "help"
    | "pointer"
    | "progress"
    | "wait"
    | "cell"
    | "crosshair"
    | "text"
    | "vertical-text"
    | "alias"
    | "copy"
    | "move"
    | "no-drop"
    | "not-allowed"
    | "grab"
    | "grabbing"
    | "all-scroll"
    | "col-resize"
    | "row-resize"
    | "n-resize"
    | "e-resize"
    | "s-resize"
    | "w-resize"
    | "ne-resize"
    | "nw-resize"
    | "se-resize"
    | "sw-resize"
    | "ew-resize"
    | "ns-resize"
    | "nesw-resize"
    | "nwse-resize"
    | "zoom-in"
    | "zoom-out";

type CSSDashedName = `--${string}`;

type CSSDirectionValue =
    | "ltr"
    | "rtl";

type CSSDisplayValue =
    | "none"
    | "contents"
    | "block"
    | "inline"
    | "inline-block"
    | "flex"
    | "inline-flex"
    | "inline flex"
    | "block flex"
    | "grid"
    | "inline-grid"
    | "inline grid"
    | "block grid"
    | "flow-root"
    | "inline flow-root"
    | "block flow-root"
    | "block flow"
    | "inline flow"
    | "table"
    | "table-row"
    | "list-item";

type CSSFlexBasisValue =
    | CSSLengthPercentage
    | "auto"
    | "max-content"
    | "min-content"
    | "fit-content"

type CSSFlexDirectionValue =
    | "row"
    | "row-reverse"
    | "column"
    | "column-reverse";

type CSSFlexWrapValue =
    | "nowrap"
    | "wrap"
    | "wrap-reverse";

type CSSFlexFlowValue =
    | CSSFlexDirectionValue
    | CSSFlexWrapValue;

type CSSFloatValue =
    | "left"
    | "right"
    | "none"
    | "inline-start"
    | "inline-end";

type CSSGridAutoFlowValue =
    | "row"
    | "column"
    | "dense"
    | "row dense"
    | "column dense";

type CSSGridLineName = `[${string}]`;

type CSSGridFlexValue = `${number}fr`;

type CSSGridTemplateTrackSize =
    | CSSLengthPercentage
    | CSSGridFlexValue
    | "max-content"
    | "min-content"
    | "auto";

type CSSGridTemplateTrackValue =
    | "none"
    | CSSGridLineName
    | CSSGridTemplateTrackSize
    | CSSFunction<"minmax", `${CSSGridTemplateTrackSize}, ${CSSGridTemplateTrackSize}`>
    | CSSFunction<"fit-content", CSSLengthPercentage>
    | CSSFunction<"repeat", `${number | "auto-fill" | "auto-fit"}, ${string}`>
    | "subgrid";

type CSSMediaQueryTypeValue =
    | "all"
    | "print"
    | "screen";

type CSSMediaQueryFeatureValue =
    | "any-hover"
    | "any-pointer"
    | "aspect-ratio"
    | "color"
    | "color-gamut"
    | "color-index"
    | "display-mode"
    | "dynamic-range"
    | "forced-colors"
    | "grid"
    | "height"
    | "hover"
    | "inverted-colors"
    | "monochrome"
    | "orientation"
    | "overflow-block"
    | "overflow-inline"
    | "pointer"
    | "prefers-color-scheme"
    | "prefers-contrast"
    | "prefers-reduced-motion"
    | "resolution"
    | "script"
    | "update"
    | "video-dynamic-range"
    | "width";

type CSSMediaQueryOperatorValue =
    | "and"
    | "not"
    | "only"
    | ","
    | "<"
    | "<="
    | ">"
    | ">=";

type CSSOverflowValue =
    | "visible"
    | "hidden"
    | "clip"
    | "scroll"
    | "auto";

type CSSPointerEventsValue =
    | "auto"
    | "none";

type CSSPositionValue =
    | "static"
    | "absolute"
    | "fixed"
    | "relative"
    | "sticky";

type CSSRenderingIntentValue =
    | "relative-colorimetric"
    | "absolute-colorimetric"
    | "perceptual"
    | "saturation";

type CSSTextAlignLastValue =
    | "auto"
    | "start"
    | "end"
    | "left"
    | "right"
    | "center"
    | "justify"
    | "match-parent";

type CSSTextAlignValue =
    | CSSTextAlignLastValue
    | "justify-all";

type CSSTextOverflowValue =
    | "clip"
    | "ellipsis";

type CSSTouchActionValue =
    | "auto"
    | "none"
    | "pan-x"
    | "pan-left"
    | "pan-right"
    | "pan-y"
    | "pan-up"
    | "pan-down"
    | "pinch-zoom"
    | "manipulation";


type CSSTransformMatrixValue = `matrix(${number}, ${number}, ${number}, ${number}, ${number}, ${number})`;
type CSSTransformMatrix3DValue = `matrix3d(${number}, ${number}, ${number}, ${number}, ${number}, ${number}, ${number}, ${number}, ${number}, ${number}, ${number}, ${number}, ${number}, ${number}, ${number}, ${number})`;
type CSSTransformPerspectiveValue = `perspective(${CSSLength | "none"})`;
type CSSTransformRotate = `rotate(${CSSAngle})`;
type CSSTransformRotate3D = `rotate3d(${number}, ${number}, ${number}, ${CSSAngle})`;
type CSSTransformRotateX = `rotateX(${CSSAngle})`;
type CSSTransformRotateY = `rotateY(${CSSAngle})`;
type CSSTransformRotateZ = `rotateZ(${CSSAngle})`;
type CSSTransformTranslate = `translate(${CSSLengthPercentage}, ${CSSLengthPercentage})`;
type CSSTransformTranslate3D = `translate3d(${CSSLengthPercentage}, ${CSSLengthPercentage}, ${CSSLengthPercentage})`;
type CSSTransformTranslateX = `translateX(${CSSLengthPercentage})`;
type CSSTransformTranslateY = `translateY(${CSSLengthPercentage})`;
type CSSTransformTranslateZ = `translateZ(${CSSLengthPercentage})`;
type CSSTransformScale = `scale(${number}, ${number})`;
type CSSTransformScale3D = `scale3d(${number}, ${number}, ${number})`;
type CSSTransformScaleX = `scaleX(${number})`;
type CSSTransformScaleY = `scaleY(${number})`;
type CSSTransformScaleZ = `scaleZ(${number})`;
type CSSTransformSkew = `skew(${CSSAngle}, ${CSSAngle})`;
type CSSTransformSkewX = `skewX(${CSSAngle})`;
type CSSTransformSkewY = `skewY(${CSSAngle})`;

type CSSTransformValue =
    | CSSTransformMatrixValue
    | CSSTransformMatrix3DValue
    | CSSTransformRotate
    | CSSTransformRotate3D
    | CSSTransformRotateX
    | CSSTransformRotateY
    | CSSTransformRotateZ
    | CSSTransformTranslate
    | CSSTransformTranslate3D
    | CSSTransformTranslateX
    | CSSTransformTranslateY
    | CSSTransformTranslateZ
    | CSSTransformScale
    | CSSTransformScale3D
    | CSSTransformScaleX
    | CSSTransformScaleY
    | CSSTransformScaleZ
    | CSSTransformSkew
    | CSSTransformSkewX
    | CSSTransformSkewY;

type CSSTransformBoxValue =
    | "content-box"
    | "border-box"
    | "fill-box"
    | "stroke-box"
    | "view-box";

type CSSTransformStyleValue =
    | "flat"
    | "preserve-3d";

type CSSVerticalAlignValue =
    | CSSLengthPercentage
    | "baseline"
    | "sub"
    | "super"
    | "text-top"
    | "text-bottom"
    | "middle"
    | "top"
    | "bottom";

type CSSVisiblityValue =
    | "visible"
    | "hidden"
    | "collapse";

type CSSWhiteSpaceValue =
    | "normal"
    | "nowrap"
    | "pre"
    | "pre-wrap"
    | "pre-line"
    | "break-spaces";

type CSSWordWrapValue =
    | "normal"
    | "break-word"
    | "anywhere";

type CSSWritingModeValue =
    | "horizontal-tb"
    | "vertical-rl"
    | "vertical-lr"
    | "sideways-rl"
    | "sideways-lr";