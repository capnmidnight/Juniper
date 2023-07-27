type CssGlobalValue =
    | ""
    | "inherit"
    | "initial"
    | "revert"
    | "revert-layer"
    | "unset";

type CssFunction<F extends string, T extends string | number> = `${F}(${T})`;

type CssUrl = CssFunction<"url", string>;

type CssPercentage = `${number}%`;

type CssAlpha =
    | number
    | CssPercentage;

type CssAngle =
    | number
    | `${number}deg`
    | `${number}rad`
    | `${number}grad`
    | `${number}turn`;

type CssFontRelativeLength =
    | `${number}cap`
    | `${number}ch`
    | `${number}em`
    | `${number}ex`
    | `${number}ic`
    | `${number}lh`
    | `${number}rem`
    | `${number}rlh`;

type CssViewportPercentageLength =
    | `${number}vh`
    | `${number}vw`
    | `${number}vi`
    | `${number}vb`
    | `${number}vmin`
    | `${number}vmax`;

type CssAbsoluteLength =
    | `${number}px`
    | `${number}cm`
    | `${number}mm`
    | `${number}Q`
    | `${number}in`
    | `${number}pc`
    | `${number}pt`;

type CssWidthKeyword =
    | "thin"
    | "medium"
    | "thick";

type CssLength =
    | CssFontRelativeLength
    | CssViewportPercentageLength
    | CssAbsoluteLength;

type CssLengthPercentage =
    | CssLength
    | CssPercentage
    | 0
    | "0";

type CssAtRuleRegular =
    | "charset"
    | "import"
    | "namespace"
    | "layer";

type CssBorderCollapseValue =
    | "collapse"
    | "separate";

type CssBorderRepeatValue =
    | "stretch"
    | "repeat"
    | "round"
    | "space";

type CssBorderStyleValue =
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

type CssCalcStatement = CssFunction<"calc", string>;

type CssColorKeywordValue =
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
type CssColorHashValue = `#${string}`;

type CssColorRGBValue = CssFunction<"rgb" | "rgba", string>;
type CssColorHSLValue = CssFunction<"hsl" | "hsla", string>;
type CssColorHWBValue = CssFunction<"hwb", string>;
type CssColorLCHValue = CssFunction<"lch", string>;
type CssColorLabValue = CssFunction<"lab", string>;

type CssColorSpaceName =
    | "srgb"
    | "srgb-linear"
    | "display-p3"
    | "a98-rgb"
    | "prophoto-rgb"
    | "rec2020"
    | "xyz"
    | "xyz-d50"
    | "xyz-d65";

type CssColorFunctionValue = CssFunction<"color", `${CssColorSpaceName} ${string}`>;

type CssColorValue =
    | CssColorKeywordValue
    | CssColorHashValue
    | CssColorRGBValue
    | CssColorHSLValue
    | CssColorHWBValue
    | CssColorLCHValue
    | CssColorLabValue
    | CssColorFunctionValue;

type CssColorProfilePropName =
    | "src"
    | "rendering-intent"
    | "components";

type CssCounterStylePropName =
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

type CssCounterStyleSystemValue =
    | "cyclic"
    | "numeric"
    | "alphabetic"
    | "symbolic"
    | "additive"
    | "fixed"
    | `fixed ${number}`
    | `extends ${string}`;

type CssCounterStyleRangeValue =
    | `${number} ${number}`
    | `${number} infinite`
    | `infinite ${number}`
    | "infinite infinite";

type CssSizePropertyValue =
    | 0
    | "0"
    | CssLengthPercentage
    | "auto"
    | "max-content"
    | "min-content"
    | "fit-content"
    | CssCalcStatement;

type CssAlignmentBaselineValue =
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

type CssTimeSecondsValue = `${number}s`;
type CssTimeMillisecondsValue = `${number}ms`;
type CssTimeValue = CssTimeSecondsValue | CssTimeMillisecondsValue;

type CssAnimationDirectionValue =
    | "normal"
    | "reverse"
    | "alternate"
    | "alternate-reverse";

type CssAnimationFillModeValue =
    | "none"
    | "forwards"
    | "backwards"
    | "both";

type CssAnimationIterationCountValue =
    | number
    | "infinite";

type CssAnimationPlayStateValue =
    | "running"
    | "paused";

type CssAnimationTimingFunctionNamed =
    | "ease"
    | "ease-in"
    | "ease-out"
    | "ease-in-out"
    | "linear"
    | "step-start"
    | "step-end";
type CssAnimationTimingFunctionCubicBezier = `cubic-bezier(${number}, ${number}, ${number}, ${number})`;
type CssAnimationTimingFunctionSteps = `steps(${number}, ${CssAnimationTimingFunctionNamed})`;
type CssAnimationTimingFunctionValue =
    | CssAnimationTimingFunctionNamed
    | CssAnimationTimingFunctionCubicBezier
    | CssAnimationTimingFunctionSteps;

type CssAppearanceValue =
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

type CssDropShadowParams =
    | `${CssLength} ${CssLength}`
    | `${CssLength} ${CssLength} ${string}`
    | `${CssLength} ${CssLength} ${CssLength} ${string}`;


type CssFilterFunction = CssFunction<"blur", CssLength>
    | CssFunction<"brightness", CssAlpha>
    | CssFunction<"contrast", CssAlpha>
    | CssFunction<"drop-shadow", CssDropShadowParams>
    | CssFunction<"grayscale", CssAlpha>
    | CssFunction<"hue-rotate", CssAngle>
    | CssFunction<"invert", CssAlpha>
    | CssFunction<"opacity", CssAlpha>
    | CssFunction<"saturate", CssAlpha>
    | CssFunction<"sepia", CssAlpha>
    | CssUrl;

type CssBackfaceVisibilityValue =
    | "visible"
    | "hiden";

type CssBackgroundAttachmentValue =
    | "scroll"
    | "fixed"
    | "local";

type CssBlendModeValue = "normal"
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

type CssBoxType =
    | "border-box"
    | "padding-box"
    | "content-box";

type CssClipValue =
    | CssBoxType
    | "text";

type CssGradient =
    | CssFunction<"linear-gradient", string>
    | CssFunction<"radial-gradient", string>
    | CssFunction<"repeating-linear-gradient", string>
    | CssFunction<"repeating-radial-gradient", string>
    | CssFunction<"conic-gradient", string>;

type CssImage = CssUrl
    | CssGradient
    | CssFunction<"element", string>
    | CssFunction<"cross-fade", string>
    | CssFunction<"image-set", string>;

type CssBackgroundPositionHorizontalKeyword =
    | "left"
    | "center"
    | "right";
type CssBackgroundPositionVerticalKeyword =
    | "top"
    | "center"
    | "bottom";
type CssBackgroundPositionKeyword =
    | CssBackgroundPositionHorizontalKeyword
    | CssBackgroundPositionVerticalKeyword;
type CssBackgroundPositionHorizontalValue =
    | CssBackgroundPositionKeyword
    | CssLengthPercentage;
type CssBackgroundPositionVerticalValue =
    | CssBackgroundPositionKeyword
    | CssLengthPercentage;
type CssBackgroundPositionValue =
    | CssBackgroundPositionKeyword
    | CssLengthPercentage;

type CssBackgroundRepeat =
    | "repeat"
    | "space"
    | "round"
    | "no-repeat";

type CssBackgroundRepeatValue =
    | "repeat-x"
    | "repeat-y"
    | CssBackgroundRepeat;

type CssBackgroundSizeValue =
    | "auto"
    | CssLengthPercentage;
type CssBackgroundSizeSingleValue =
    | "contain"
    | "cover"
    | CssBackgroundSizeValue;

type CssElementPositionValue =
    | "auto"
    | CssLengthPercentage;

type CssCursorValue =
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

type CssDashedName = `--${string}`;

type CssDirectionValue =
    | "ltr"
    | "rtl";

type CssDisplayValue =
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

type CssFlexBasisValue =
    | CssLengthPercentage
    | "auto"
    | "max-content"
    | "min-content"
    | "fit-content"

type CssFlexDirectionValue =
    | "row"
    | "row-reverse"
    | "column"
    | "column-reverse";

type CssFlexWrapValue =
    | "nowrap"
    | "wrap"
    | "wrap-reverse";

type CssFlexFlowValue =
    | CssFlexDirectionValue
    | CssFlexWrapValue;

type CssFloatValue =
    | "left"
    | "right"
    | "none"
    | "inline-start"
    | "inline-end";

type CssGridAutoFlowValue =
    | "row"
    | "column"
    | "dense"
    | "row dense"
    | "column dense";

type CssGridLineName = `[${string}]`;

type CssGridFlexValue = `${number}fr`;

type CssGridTemplateTrackSize =
    | CssLengthPercentage
    | CssGridFlexValue
    | "max-content"
    | "min-content"
    | "auto";

type CssGridTemplateTrackValue =
    | "none"
    | CssGridLineName
    | CssGridTemplateTrackSize
    | CssFunction<"minmax", `${CssGridTemplateTrackSize}, ${CssGridTemplateTrackSize}`>
    | CssFunction<"fit-content", CssLengthPercentage>
    | CssFunction<"repeat", `${number | "auto-fill" | "auto-fit"}, ${string}`>
    | "subgrid";

type CssJustifyAlignValue =
    | "center"
    | "start"
    | "end"
    | "flex-start"
    | "flex-end"
    | "left"
    | "right"
    | "normal"
    | "stretch"
    | "safe center"
    | "unsafe center";

type CssJustifyAlignContentValue =
    | CssJustifyAlignValue
    | "space-between"
    | "space-around"
    | "space-evenly";

type CssJustifyAlignItemsContentValue =
    | CssJustifyAlignValue
    | "self-start"
    | "self-end"
    | "baseline"
    | "first baseline"
    | "last baseline";

type CssJustifyItemsValue =
    | CssJustifyAlignItemsContentValue
    | "legacy right"
    | "legacy left"
    | "legacy center";

type CssJustifyAlignSelfValue =
    | CssJustifyAlignItemsContentValue
    | "auto";

type CssMediaQueryTypeValue =
    | "all"
    | "print"
    | "screen";

type CssMediaQueryFeatureValue =
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

type CssMediaQueryOperatorValue =
    | "and"
    | "not"
    | "only"
    | ","
    | "<"
    | "<="
    | ">"
    | ">=";

type CssOverflowValue =
    | "visible"
    | "hidden"
    | "clip"
    | "scroll"
    | "auto";

type CssPointerEventsValue =
    | "auto"
    | "none";

type CssPositionValue =
    | "static"
    | "absolute"
    | "fixed"
    | "relative"
    | "sticky";

type CssRenderingIntentValue =
    | "relative-colorimetric"
    | "absolute-colorimetric"
    | "perceptual"
    | "saturation";

type CssTextAlignLastValue =
    | "auto"
    | "start"
    | "end"
    | "left"
    | "right"
    | "center"
    | "justify"
    | "match-parent";

type CssTextAlignValue =
    | CssTextAlignLastValue
    | "justify-all";

type CssTextOverflowValue =
    | "clip"
    | "ellipsis";

type CssTouchActionValue =
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


type CssTransformMatrixValue = `matrix(${number}, ${number}, ${number}, ${number}, ${number}, ${number})`;
type CssTransformMatrix3DValue = `matrix3d(${number}, ${number}, ${number}, ${number}, ${number}, ${number}, ${number}, ${number}, ${number}, ${number}, ${number}, ${number}, ${number}, ${number}, ${number}, ${number})`;
type CssTransformPerspectiveValue = `perspective(${CssLength | "none"})`;
type CssTransformRotate = `rotate(${CssAngle})`;
type CSSTransformRotate3D = `rotate3d(${number}, ${number}, ${number}, ${CssAngle})`;
type CssTransformRotateX = `rotateX(${CssAngle})`;
type CssTransformRotateY = `rotateY(${CssAngle})`;
type CssTransformRotateZ = `rotateZ(${CssAngle})`;
type CssTransformTranslate = `translate(${CssLengthPercentage}, ${CssLengthPercentage})`;
type CSSTransformTranslate3D = `translate3d(${CssLengthPercentage}, ${CssLengthPercentage}, ${CssLengthPercentage})`;
type CssTransformTranslateX = `translateX(${CssLengthPercentage})`;
type CssTransformTranslateY = `translateY(${CssLengthPercentage})`;
type CssTransformTranslateZ = `translateZ(${CssLengthPercentage})`;
type CssTransformScale = `scale(${number}, ${number})`;
type CSSTransformScale3D = `scale3d(${number}, ${number}, ${number})`;
type CssTransformScaleX = `scaleX(${number})`;
type CssTransformScaleY = `scaleY(${number})`;
type CssTransformScaleZ = `scaleZ(${number})`;
type CssTransformSkew = `skew(${CssAngle}, ${CssAngle})`;
type CssTransformSkewX = `skewX(${CssAngle})`;
type CssTransformSkewY = `skewY(${CssAngle})`;

type CssTransformValue =
    | CssTransformMatrixValue
    | CssTransformMatrix3DValue
    | CssTransformRotate
    | CSSTransformRotate3D
    | CssTransformRotateX
    | CssTransformRotateY
    | CssTransformRotateZ
    | CssTransformTranslate
    | CSSTransformTranslate3D
    | CssTransformTranslateX
    | CssTransformTranslateY
    | CssTransformTranslateZ
    | CssTransformScale
    | CSSTransformScale3D
    | CssTransformScaleX
    | CssTransformScaleY
    | CssTransformScaleZ
    | CssTransformSkew
    | CssTransformSkewX
    | CssTransformSkewY;

type CssTransformBoxValue =
    | "content-box"
    | "border-box"
    | "fill-box"
    | "stroke-box"
    | "view-box";

type CssTransformStyleValue =
    | "flat"
    | "preserve-3d";

type CssVerticalAlignValue =
    | CssLengthPercentage
    | "baseline"
    | "sub"
    | "super"
    | "text-top"
    | "text-bottom"
    | "middle"
    | "top"
    | "bottom";

type CssVisiblityValue =
    | "visible"
    | "hidden"
    | "collapse";

type CssWhiteSpaceValue =
    | "normal"
    | "nowrap"
    | "pre"
    | "pre-wrap"
    | "pre-line"
    | "break-spaces";

type CssWordWrapValue =
    | "normal"
    | "break-word"
    | "anywhere";

type CssWritingModeValue =
    | "horizontal-tb"
    | "vertical-rl"
    | "vertical-lr"
    | "sideways-rl"
    | "sideways-lr";