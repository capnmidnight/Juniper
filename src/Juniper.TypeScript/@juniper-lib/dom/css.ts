import { isDefined, isNumber, isString } from "@juniper-lib/tslib/typeChecks";
import { IElementAppliable } from "./tags";

export class CssProp implements IElementAppliable {
    public readonly name: string;
    constructor(
        public readonly key: string,
        public readonly value: string | number) {
        this.name = key.replace(/[A-Z]/g, (m) => `-${m.toLocaleLowerCase()}`);
    }

    /**
     * Set the attribute value on an HTMLElement
     * @param elem - the element on which to set the attribute.
     */
    applyToElement(elem: HTMLElement) {
        (elem.style as any)[this.key] = this.value;
    }
}

export class CssPropSet implements IElementAppliable {
    private rest: (CssProp | CssPropSet)[];
    constructor(...rest: (CssProp | CssPropSet)[]) {
        this.rest = rest;
    }

    /**
     * Set the attribute value on an HTMLElement
     * @param style - the element on which to set the attribute.
     */
    applyToElement(elem: HTMLElement) {
        for (const prop of this.rest) {
            prop.applyToElement(elem);
        }
    }
}

function asInt(v: number | string): string {
    return isNumber(v) ? v.toFixed(0) : v;
}

/**
 * Combine style properties.
 **/
export function styles(...rest: (CssProp | CssPropSet)[]) {
    return new CssPropSet(...rest);
}

export type CSSImportant<T extends string | number> = T | `${T} !important`;

export type CSSGlobalValues =
    | "inherit"
    | "initial"
    | "revert"
    | "revert-layer"
    | "unset";

export type CSSFunction<F extends string, T extends string | number> = `${F}(${T})`;

export type CSSUrl = CSSFunction<"url", string>;

export type CSSPercentage = `${number}%`;

export type CSSNumberPercentage =
    | number
    | CSSPercentage;

export type CSSAngle =
    | number
    | `${number}deg`
    | `${number}rad`
    | `${number}grad`
    | `${number}turn`;

export type CSSFontRelativeLength =
    | `${number}cap`
    | `${number}ch`
    | `${number}em`
    | `${number}ex`
    | `${number}ic`
    | `${number}lh`
    | `${number}rem`
    | `${number}rlh`;

export type CSSViewportPercentageLength =
    | `${number}vh`
    | `${number}vw`
    | `${number}vi`
    | `${number}vb`
    | `${number}vmin`
    | `${number}vmax`;

export type CSSAbsoluteLength =
    | `${number}px`
    | `${number}cm`
    | `${number}mm`
    | `${number}Q`
    | `${number}in`
    | `${number}pc`
    | `${number}pt`;

export type CSSLength =
    | CSSFontRelativeLength
    | CSSViewportPercentageLength
    | CSSAbsoluteLength;

export type CSSLengthPercentage =
    | CSSLength
    | CSSPercentage
    | 0
    | "0";

export type CSSLengthPercentageAuto =
    | CSSLengthPercentage
    | "auto";

export type CSSCalcStatement = CSSFunction<"calc", string>;

export type CSSColorKeywordValue =
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


export type CSSColorHashValue = `#${string}`;
export type CSSColorRGBValue = CSSFunction<"rgb" | "rgba", string>;
export type CSSColorHSLValue = CSSFunction<"hsl", string>;
export type CSSColorHWBValue = CSSFunction<"hwb", string>;
export type CSSColorLCHValue = CSSFunction<"lch", string>;
export type CSSColorLabValue = CSSFunction<"lab", string>;

export type CSSColorSpaceName =
    | "srgb"
    | "srgb-linear"
    | "display-p3"
    | "a98-rgb"
    | "prophoto-rgb"
    | "rec2020"
    | "xyz"
    | "xyz-d50"
    | "xyz-d65";

export type CSSColorFunctionValue = CSSFunction<"color", `${CSSColorSpaceName} ${string}`>;

export type CSSColorValue =
    | CSSColorKeywordValue
    | CSSColorHashValue
    | CSSColorRGBValue
    | CSSColorHSLValue
    | CSSColorHWBValue
    | CSSColorLCHValue
    | CSSColorLabValue
    | CSSColorFunctionValue;

export type CSSSizePropertyValue =
    | 0
    | "0"
    | CSSLengthPercentage
    | "auto"
    | "max-content"
    | "min-content"
    | CSSFunction<"fit-content", CSSLengthPercentage>
    | CSSCalcStatement;

export type CSSAlignItemsValue =
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
export function alignItems(v: CSSGlobalValues | CSSAlignItemsValue) { return new CssProp("alignItems", v); }

export type CSSAlignContentValue =
    | CSSAlignItemsValue
    | "space-between"
    | "space-around"
    | "space-evenly";
export function alignContent(v: CSSGlobalValues | CSSAlignContentValue) { return new CssProp("alignContent", v); }

export type CSSAlignSelfValue =
    | CSSAlignItemsValue
    | "auto"
    | "self-start"
    | "self-end";
export function alignSelf(v: CSSGlobalValues | CSSAlignSelfValue) { return new CssProp("alignSelf", v); }

export function all(v: CSSGlobalValues) { return new CssProp("all", v); }

export type CSSAlignmentBaselineValues =
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
    | "bottom"
export function alignmentBaseline(v: CSSAlignmentBaselineValues) { return new CssProp("alignmentBaseline", v); }

export type CSSTimeSecondsValue = `${number}s`;
export type CSSTimeMillisecondsValue = `${number}ms`;
export type CSSTimeValue = CSSTimeSecondsValue | CSSTimeMillisecondsValue;
export function animationDelay(v: CSSGlobalValues): CssProp;
export function animationDelay(...v: CSSTimeValue[]): CssProp;
export function animationDelay(...v: string[]) { return new CssProp("animationDelay", v.join(", ")); }

export function animationDuration(v: CSSGlobalValues): CssProp;
export function animationDuration(...v: CSSTimeValue[]): CssProp;
export function animationDuration(...v: string[]) { return new CssProp("animationDuration", v.join(", ")); }

export type CSSAnimationDirectionValue =
    | "normal"
    | "reverse"
    | "alternate"
    | "alternate-reverse";
export function animationDirection(v: CSSGlobalValues): CssProp;
export function animationDirection(...v: CSSAnimationDirectionValue[]): CssProp;
export function animationDirection(...v: string[]): CssProp { return new CssProp("animationDirection", v.join(", ")); }

export type CSSAnimationFillModeValue =
    | "none"
    | "forwards"
    | "backwards"
    | "both";
export function animationFillMode(v: CSSGlobalValues): CssProp;
export function animationFillMode(...v: CSSAnimationFillModeValue[]): CssProp;
export function animationFillMode(...v: string[]): CssProp { return new CssProp("animationFillMode", v.join(", ")); }

export type CSSAnimationIterationCountValue =
    | number
    | "infinite";
export function animationIterationCount(v: CSSGlobalValues): CssProp;
export function animationIterationCount(...v: CSSAnimationIterationCountValue[]): CssProp;
export function animationIterationCount(...v: (number | string)[]): CssProp { return new CssProp("animationIterationCount", v.join(", ")); }

export type CSSAnimationNameValue = ``;
export function animationName(v: CSSGlobalValues): CssProp;
export function animationName(...v: CSSAnimationNameValue[]): CssProp;
export function animationName(...v: string[]) { return new CssProp("animationName", v.join(", ")); }

export type CSSAnimationPlayStateValue =
    | "running"
    | "paused";
export function animationPlayState(v: CSSGlobalValues): CssProp;
export function animationPlayState(...v: CSSAnimationPlayStateValue[]): CssProp;
export function animationPlayState(...v: string[]) { return new CssProp("animationPlayState", v.join(", ")); }

export type CSSAnimationTimingFunctionNamed =
    | "ease"
    | "ease-in"
    | "ease-out"
    | "ease-in-out"
    | "linear"
    | "step-start"
    | "step-end";
export type CSSAnimationTimingFunctionCubicBezier = `cubic-bezier(${number}, ${number}, ${number}, ${number})`;
export type CSSAnimationTimingFunctionSteps = `steps(${number}, ${CSSAnimationTimingFunctionNamed})`;
export type CSSAnimationTimingFunctionValue =
    | CSSAnimationTimingFunctionNamed
    | CSSAnimationTimingFunctionCubicBezier
    | CSSAnimationTimingFunctionSteps;
export function animationTimingFunction(v: CSSGlobalValues): CssProp;
export function animationTimingFunction(...v: CSSAnimationTimingFunctionValue[]): CssProp;
export function animationTimingFunction(...v: string[]) { return new CssProp("animationTimingFunction", v.join(' ')); }

export type CSSAnimationValue =
    | CSSTimeValue
    | CSSAnimationDirectionValue
    | CSSAnimationFillModeValue
    | CSSAnimationIterationCountValue
    | CSSAnimationNameValue
    | CSSAnimationTimingFunctionValue
    | string;
export function animation(v: CSSGlobalValues): CssProp;
export function animation(...v: CSSAnimationValue[]): CssProp;
export function animation(...v: (number | string)[]): CssProp { return new CssProp("animation", v.join(" ")); }

export type CSSAppearanceValue =
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
export function appearance(v: CSSGlobalValues | CSSAppearanceValue) { return new CssProp("appearance", v); }

export type CSSDropShadowParams =
    | `${CSSLength} ${CSSLength}`
    | `${CSSLength} ${CSSLength} ${string}`
    | `${CSSLength} ${CSSLength} ${CSSLength} ${string}`;
    

export type CSSFilterFunction = CSSFunction<"blur", CSSLength>
    | CSSFunction<"brightness", CSSNumberPercentage>
    | CSSFunction<"contrast", CSSNumberPercentage>
    | CSSFunction<"drop-shadow", CSSDropShadowParams>
    | CSSFunction<"grayscale", CSSNumberPercentage>
    | CSSFunction<"hue-rotate", CSSAngle>
    | CSSFunction<"invert", CSSNumberPercentage>
    | CSSFunction<"opacity", CSSNumberPercentage>
    | CSSFunction<"saturate", CSSNumberPercentage>
    | CSSFunction<"sepia", CSSNumberPercentage>
    | CSSUrl;
export function backdropFilter(v: "none" | CSSGlobalValues): CssProp;
export function backdropFilter(...v: CSSFilterFunction[]): CssProp;
export function backdropFilter(...v: string[]): CssProp { return new CssProp("backdropFilter", v.join(' ')); }

export type CSSBackfaceVisibilityValue =
    | "visible"
    | "hiden";
export function backfaceVisibility(v: CSSGlobalValues | CSSBackfaceVisibilityValue) { return new CssProp("backfaceVisibility", v); }

export type CSSBackgroundAttachmentValue =
    | "scroll"
    | "fixed"
    | "local";
export function backgroundAttachment(v: CSSGlobalValues | CSSBackgroundAttachmentValue) { return new CssProp("backgroundAttachment", v); }

export type CSSBlendModeValue = "normal"
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
export function backgroundBlendMode(v: CSSGlobalValues): CssProp;
export function backgroundBlendMode(...v: CSSBlendModeValue[]): CssProp;
export function backgroundBlendMode(...v: string[]): CssProp { return new CssProp("backgroundBlendMode", v.join(", ")); }

export type CSSBoxType =
    | "border-box"
    | "padding-box"
    | "content-box";

export type CSSClipValue =
    | CSSBoxType
    | "text";
export function backgroundClip(v: CSSGlobalValues | CSSClipValue): CssProp { return new CssProp("backgroundClip", v); }

export function backgroundColor(v: CSSGlobalValues | CSSColorValue) { return new CssProp("backgroundColor", v); }

export type CSSGradient =
    | CSSFunction<"linear-gradient", string>
    | CSSFunction<"radial-gradient", string>
    | CSSFunction<"repeating-linear-gradient", string>
    | CSSFunction<"repeating-radial-gradient", string>
    | CSSFunction<"conic-gradient", string>;

export type CSSImage = CSSUrl
    | CSSGradient
    | CSSFunction<"element", string>
    | CSSFunction<"cross-fade", string>
    | CSSFunction<"image-set", string>;
export function backgroundImage(v: CSSGlobalValues): CssProp;
export function backgroundImage(...v: CSSImage[]): CssProp;
export function backgroundImage(...v: string[]): CssProp { return new CssProp("backgroundImage", v.join(", ")); }

export function backgroundOrigin(v: CSSGlobalValues | CSSBoxType): CssProp { return new CssProp("backgroundOrigin", v); }

export type CSSPositionKeyword =
    | "top"
    | "bottom"
    | "left"
    | "right"
    | "center";
export type CSSPosition =
    | CSSPositionKeyword
    | CSSLengthPercentage;

export type CSSPositionValue =
    | CSSPosition
    | `${CSSPosition} ${CSSPosition}`
    | `${CSSPositionKeyword} ${CSSPositionKeyword} ${CSSLengthPercentage}`
    | `${CSSPositionKeyword} ${CSSLengthPercentage} ${CSSPositionKeyword} ${CSSLengthPercentage}`;
export function backgroundPosition(v: CSSGlobalValues): CssProp;
export function backgroundPosition(...v: CSSPositionValue[]): CssProp;
export function backgroundPosition(...v: (string | number)[]): CssProp { return new CssProp("backgroundPosition", v.join(", ")); }

export type CSSPositionXYValue =
    | CSSPosition
    | `${CSSPositionKeyword} ${CSSLengthPercentage}`;
export function backgroundPositionX(v: CSSGlobalValues): CssProp;
export function backgroundPositionX(...v: CSSPositionXYValue[]): CssProp;
export function backgroundPositionX(...v: (string | number)[]): CssProp { return new CssProp("backgroundPositionX", v.join(", ")); }

export function backgroundPositionY(v: CSSGlobalValues): CssProp;
export function backgroundPositionY(...v: CSSPositionXYValue[]): CssProp;
export function backgroundPositionY(...v: (string | number)[]): CssProp { return new CssProp("backgroundPositionY", v.join(", ")); }

export type CSSBackgroundRepeat =
    | "repeat"
    | "space"
    | "round"
    | "no-repeat";

export type CSSBackgroundRepeatValue =
    | "repeat-x"
    | "repeat-y"
    | CSSBackgroundRepeat;
export function backgroundRepeat(v: CSSGlobalValues): CssProp;
export function backgroundRepeat(v: CSSBackgroundRepeatValue): CssProp;
export function backgroundRepeat(x: CSSBackgroundRepeat, y: CSSBackgroundRepeat): CssProp;
export function backgroundRepeat(...v: string[]) { return new CssProp("backgroundRepeat", v.join(" ")); }

export function backgroundRepeatX(v: CSSGlobalValues | CSSBackgroundRepeat) { return new CssProp("backgroundRepeatX", v); }
export function backgroundRepeatY(v: CSSGlobalValues | CSSBackgroundRepeat) { return new CssProp("backgroundRepeatY", v); }

export type CSSBackgroundSizeValue =
    | "contain"
    | "cover"
    | "auto"
    | CSSLengthPercentage
    | "auto auto"
    | `${CSSLengthPercentage} auto`
    | `auto ${CSSLengthPercentage}`
    | `${CSSLengthPercentage} ${CSSLengthPercentage}`
export function backgroundSize(v: CSSGlobalValues): CssProp;
export function backgroundSize(...v: CSSBackgroundSizeValue[]): CssProp;
export function backgroundSize(...v: (string | number)[]): CssProp { return new CssProp("backgroundSize", v.join(", ")); }

export function background(v: CSSGlobalValues): CssProp;
export function background(...v: string[]): CssProp;
export function background(...v: string[]): CssProp { return new CssProp("background", v.join(" ")); }

export function baselineShift(v: string) { return new CssProp("baselineShift", v); }
export function blockSize(v: string) { return new CssProp("blockSize", v); }
export function border(v: string | 0) { return new CssProp("border", v); }
export function borderBlockEnd(v: string) { return new CssProp("borderBlockEnd", v); }
export function borderBlockEndColor(v: string) { return new CssProp("borderBlockEndColor", v); }
export function borderBlockEndStyle(v: string) { return new CssProp("borderBlockEndStyle", v); }
export function borderBlockEndWidth(v: string) { return new CssProp("borderBlockEndWidth", v); }
export function borderBlockStart(v: string) { return new CssProp("borderBlockStart", v); }
export function borderBlockStartColor(v: string) { return new CssProp("borderBlockStartColor", v); }
export function borderBlockStartStyle(v: string) { return new CssProp("borderBlockStartStyle", v); }
export function borderBlockStartWidth(v: string) { return new CssProp("borderBlockStartWidth", v); }
export function borderBottom(v: string) { return new CssProp("borderBottom", v); }
export function borderBottomColor(v: string) { return new CssProp("borderBottomColor", v); }
export function borderBottomLeftRadius(v: string) { return new CssProp("borderBottomLeftRadius", v); }
export function borderBottomRightRadius(v: string) { return new CssProp("borderBottomRightRadius", v); }
export function borderBottomStyle(v: string) { return new CssProp("borderBottomStyle", v); }
export function borderBottomWidth(v: string) { return new CssProp("borderBottomWidth", v); }
export function borderCollapse(v: string) { return new CssProp("borderCollapse", v); }
export function borderColor(v: string) { return new CssProp("borderColor", v); }
export function borderImage(v: string) { return new CssProp("borderImage", v); }
export function borderImageOutset(v: string) { return new CssProp("borderImageOutset", v); }
export function borderImageRepeat(v: string) { return new CssProp("borderImageRepeat", v); }
export function borderImageSlice(v: string) { return new CssProp("borderImageSlice", v); }
export function borderImageSource(v: string) { return new CssProp("borderImageSource", v); }
export function borderImageWidth(v: string) { return new CssProp("borderImageWidth", v); }
export function borderInlineEnd(v: string) { return new CssProp("borderInlineEnd", v); }
export function borderInlineEndColor(v: string) { return new CssProp("borderInlineEndColor", v); }
export function borderInlineEndStyle(v: string) { return new CssProp("borderInlineEndStyle", v); }
export function borderInlineEndWidth(v: string) { return new CssProp("borderInlineEndWidth", v); }
export function borderInlineStart(v: string) { return new CssProp("borderInlineStart", v); }
export function borderInlineStartColor(v: string) { return new CssProp("borderInlineStartColor", v); }
export function borderInlineStartStyle(v: string) { return new CssProp("borderInlineStartStyle", v); }
export function borderInlineStartWidth(v: string) { return new CssProp("borderInlineStartWidth", v); }
export function borderLeft(v: string) { return new CssProp("borderLeft", v); }
export function borderLeftColor(v: string) { return new CssProp("borderLeftColor", v); }
export function borderLeftStyle(v: string) { return new CssProp("borderLeftStyle", v); }
export function borderLeftWidth(v: string) { return new CssProp("borderLeftWidth", v); }
export function borderRadius(v: string | 0) { return new CssProp("borderRadius", v); }
export function borderRight(v: string) { return new CssProp("borderRight", v); }
export function borderRightColor(v: string) { return new CssProp("borderRightColor", v); }
export function borderRightStyle(v: string) { return new CssProp("borderRightStyle", v); }
export function borderRightWidth(v: string) { return new CssProp("borderRightWidth", v); }
export function borderSpacing(v: string) { return new CssProp("borderSpacing", v); }
export function borderStyle(v: string) { return new CssProp("borderStyle", v); }
export function borderTop(v: string) { return new CssProp("borderTop", v); }
export function borderTopColor(v: string) { return new CssProp("borderTopColor", v); }
export function borderTopLeftRadius(v: string) { return new CssProp("borderTopLeftRadius", v); }
export function borderTopRightRadius(v: string) { return new CssProp("borderTopRightRadius", v); }
export function borderTopStyle(v: string) { return new CssProp("borderTopStyle", v); }
export function borderTopWidth(v: string) { return new CssProp("borderTopWidth", v); }
export function borderWidth(v: string | 0) { return new CssProp("borderWidth", v); }

export type CSSElementPositionValue =
    | "auto"
    | CSSLengthPercentage;
export function bottom(v: CSSGlobalValues | CSSElementPositionValue) { return new CssProp("bottom", v); }
export function boxShadow(v: string) { return new CssProp("boxShadow", v); }
export function boxSizing(v: string) { return new CssProp("boxSizing", v); }
export function breakAfter(v: string) { return new CssProp("breakAfter", v); }
export function breakBefore(v: string) { return new CssProp("breakBefore", v); }
export function breakInside(v: string) { return new CssProp("breakInside", v); }
export function bufferedRendering(v: string) { return new CssProp("bufferedRendering", v); }
export function captionSide(v: string) { return new CssProp("captionSide", v); }
export function caretColor(v: string) { return new CssProp("caretColor", v); }
export function clear(v: string) { return new CssProp("clear", v); }
export function clip(v: string) { return new CssProp("clip", v); }
export function clipPath(v: string) { return new CssProp("clipPath", v); }
export function clipRule(v: string) { return new CssProp("clipRule", v); }

export function color(v: CSSGlobalValues): CssProp;
export function color(v: CSSColorValue): CssProp;
export function color(v: string): CssProp { return new CssProp("color", v); }

export function colorInterpolation(v: string) { return new CssProp("colorInterpolation", v); }
export function colorInterpolationFilters(v: string) { return new CssProp("colorInterpolationFilters", v); }
export function colorRendering(v: string) { return new CssProp("colorRendering", v); }
export function colorScheme(v: string) { return new CssProp("colorScheme", v); }
export function columnCount(v: string) { return new CssProp("columnCount", v); }
export function columnFill(v: string) { return new CssProp("columnFill", v); }
export function columnGap(v: string) { return new CssProp("columnGap", v); }
export function columnRule(v: string) { return new CssProp("columnRule", v); }
export function columnRuleColor(v: string) { return new CssProp("columnRuleColor", v); }
export function columnRuleStyle(v: string) { return new CssProp("columnRuleStyle", v); }
export function columnRuleWidth(v: string) { return new CssProp("columnRuleWidth", v); }
export function columnSpan(v: string) { return new CssProp("columnSpan", v); }
export function columnWidth(v: string) { return new CssProp("columnWidth", v); }
export function columns(v: string) { return new CssProp("columns", v); }
export function contain(v: string) { return new CssProp("contain", v); }
export function containIntrinsicSize(v: string) { return new CssProp("containIntrinsicSize", v); }
export function counterIncrement(v: string) { return new CssProp("counterIncrement", v); }
export function counterReset(v: string) { return new CssProp("counterReset", v); }

export type CSSCursorValue =
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
export function cursor(v: CSSGlobalValues | CSSCursorValue) { return new CssProp("cursor", v); }

export function cx(v: string) { return new CssProp("cx", v); }
export function cy(v: string) { return new CssProp("cy", v); }
export function d(v: string) { return new CssProp("d", v); }

export type CSSDirectionValues =
    | "ltr"
    | "rtl";
export function direction(v: CSSGlobalValues | CSSDirectionValues) { return new CssProp("direction", v); }

export type CSSDisplayValues =
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
export function display(v: CSSImportant<CSSGlobalValues | CSSDisplayValues>) { return new CssProp("display", v); }

export function dominantBaseline(v: string) { return new CssProp("dominantBaseline", v); }
export function emptyCells(v: string) { return new CssProp("emptyCells", v); }
export function fill(v: string) { return new CssProp("fill", v); }
export function fillOpacity(v: string) { return new CssProp("fillOpacity", v); }
export function fillRule(v: string) { return new CssProp("fillRule", v); }
export function filter(v: string) { return new CssProp("filter", v); }

export type CSSFlexBasisValues =
    | CSSLengthPercentage
    | "auto"
    | "max-content"
    | "min-content"
    | "fit-content"
export function flexBasis(v: CSSGlobalValues | CSSFlexBasisValues) { return new CssProp("flexBasis", v); }

export type CSSFlexDirectionValues =
    | "row"
    | "row-reverse"
    | "column"
    | "column-reverse";
export function flexDirection(v: CSSGlobalValues | CSSFlexDirectionValues) { return new CssProp("flexDirection", v); }

export type CSSFlexWrapValues =
    | "nowrap"
    | "wrap"
    | "wrap-reverse";
export function flexWrap(v: CSSGlobalValues | CSSFlexWrapValues) { return new CssProp("flexWrap", v); }

export type CSSFlexFlowValues =
    | CSSFlexDirectionValues
    | CSSFlexWrapValues;
export function flexFlow(v: CSSGlobalValues): CssProp;
export function flexFlow(v: CSSFlexFlowValues): CssProp;
export function flexFlow(x: CSSFlexFlowValues, y: CSSFlexFlowValues): CssProp;
export function flexFlow(...v: string[]): CssProp { return new CssProp("flexFlow", v.join(' ')); }


export function flex(v: CSSGlobalValues): CssProp;
export function flex(grow: number): CssProp;
export function flex(basis: CSSFlexBasisValues): CssProp;
export function flex(grow: number, shrink: number): CssProp;
export function flex(grow: number, basis: CSSFlexBasisValues): CssProp;
export function flex(grow: number, shrink: number, basis: CSSFlexBasisValues): CssProp;
export function flex(...v: (string | number)[]): CssProp { return new CssProp("flex", v.join(' ')); }

export function flexGrow(v: CSSGlobalValues | number) { return new CssProp("flexGrow", v); }

export function flexShrink(v: CSSGlobalValues | number) { return new CssProp("flexShrink", v); }

export type CSSFloatValues =
    | "left"
    | "right"
    | "none"
    | "inline-start"
    | "inline-end";
export function float(v: CSSGlobalValues | CSSFloatValues) { return new CssProp("float", v); }

export function floodColor(v: string) { return new CssProp("floodColor", v); }
export function floodOpacity(v: string) { return new CssProp("floodOpacity", v); }
export function font(v: string) { return new CssProp("font", v); }
export function fontDisplay(v: string) { return new CssProp("fontDisplay", v); }
export function fontFamily(v: string) { return new CssProp("fontFamily", v); }
export function fontFeatureSettings(v: string) { return new CssProp("fontFeatureSettings", v); }
export function fontKerning(v: string) { return new CssProp("fontKerning", v); }
export function fontOpticalSizing(v: string) { return new CssProp("fontOpticalSizing", v); }
export function fontSize(v: string) { return new CssProp("fontSize", v); }
export function fontStretch(v: string) { return new CssProp("fontStretch", v); }
export function fontStyle(v: string) { return new CssProp("fontStyle", v); }
export function fontVariant(v: string) { return new CssProp("fontVariant", v); }
export function fontVariantCaps(v: string) { return new CssProp("fontVariantCaps", v); }
export function fontVariantEastAsian(v: string) { return new CssProp("fontVariantEastAsian", v); }
export function fontVariantLigatures(v: string) { return new CssProp("fontVariantLigatures", v); }
export function fontVariantNumeric(v: string) { return new CssProp("fontVariantNumeric", v); }
export function fontVariationSettings(v: string) { return new CssProp("fontVariationSettings", v); }
export function fontWeight(v: string) { return new CssProp("fontWeight", v); }
export function forcedColorAdjust(v: string) { return new CssProp("forcedColorAdjust", v); }
export function gap(v: string) { return new CssProp("gap", v); }
export function grid(v: string) { return new CssProp("grid", v); }

export function gridArea(v: string): CssProp;
export function gridArea(rowStart: number, colStart?: number, rowEnd?: number, colEnd?: number): CssProp;
export function gridArea(vOrRowStart: string | number, colStart?: number, rowEnd?: number, colEnd?: number): CssProp {
    if (!isString(vOrRowStart)) {
        vOrRowStart = [vOrRowStart, colStart, rowEnd, colEnd]
            .filter(isDefined)
            .join('/');
    }

    return new CssProp("gridArea", vOrRowStart);
}
export function gridAutoColumns(v: string) { return new CssProp("gridAutoColumns", v); }

export type CSSGridAutoFlowValues =
    | "row"
    | "column"
    | "dense"
    | "row dense"
    | "column dense";
export function gridAutoFlow(v: CSSGlobalValues | CSSGridAutoFlowValues) { return new CssProp("gridAutoFlow", v); }

export function gridAutoRows(v: string) { return new CssProp("gridAutoRows", v); }

export function gridColumn(v: string): CssProp
export function gridColumn(colStart: number, colEnd?: number): CssProp
export function gridColumn(vOrColStart: string | number, colEnd?: number): CssProp {
    if (!isString(vOrColStart)) {
        vOrColStart = [vOrColStart, colEnd]
            .filter(isDefined)
            .join('/');
    }
    return new CssProp("gridColumn", vOrColStart);
}

export function gridColumnEnd(v: string | number) { return new CssProp("gridColumnEnd", v); }
export function gridColumnGap(v: CSSLengthPercentage) { return new CssProp("gridColumnGap", v); }
export function gridColumnStart(v: string | number) { return new CssProp("gridColumnStart", v); }
export function gridGap(v: CSSLengthPercentage) { return new CssProp("gridGap", v); }

export function gridRow(v: string): CssProp
export function gridRow(rowStart: number, rowEnd?: number): CssProp
export function gridRow(vOrRowStart: string | number, rowEnd?: number): CssProp {
    if (!isString(vOrRowStart)) {
        vOrRowStart = [vOrRowStart, rowEnd]
            .filter(isDefined)
            .join('/');
    }
    return new CssProp("gridRow", vOrRowStart);
}

export function gridRowEnd(v: string | number) { return new CssProp("gridRowEnd", v); }
export function gridRowGap(v: CSSLengthPercentage) { return new CssProp("gridRowGap", v); }
export function gridRowStart(v: string | number) { return new CssProp("gridRowStart", v); }
export function gridTemplate(v: string) { return new CssProp("gridTemplate", v); }

export function gridTemplateAreas(...v: string[]) { return new CssProp("gridTemplateAreas", v.map((r) => '"' + r + '"').join('\n')); }

export function gridTemplateColumns(v: string) { return new CssProp("gridTemplateColumns", v); }
export function gridTemplateRows(v: string) { return new CssProp("gridTemplateRows", v); }
export function height(v: CSSImportant<CSSGlobalValues | CSSSizePropertyValue>) { return new CssProp("height", v); }
export function hyphens(v: string) { return new CssProp("hyphens", v); }
export function imageOrientation(v: string) { return new CssProp("imageOrientation", v); }
export function imageRendering(v: string) { return new CssProp("imageRendering", v); }
export function inlineSize(v: string) { return new CssProp("inlineSize", v); }
export function isolation(v: string) { return new CssProp("isolation", v); }
export function justifyContent(v: string) { return new CssProp("justifyContent", v); }
export function justifyItems(v: string) { return new CssProp("justifyItems", v); }
export function justifySelf(v: string) { return new CssProp("justifySelf", v); }
export function left(v: CSSElementPositionValue) { return new CssProp("left", v); }
export function letterSpacing(v: string) { return new CssProp("letterSpacing", v); }
export function lightingColor(v: string) { return new CssProp("lightingColor", v); }
export function lineBreak(v: string) { return new CssProp("lineBreak", v); }
export function lineHeight(v: string) { return new CssProp("lineHeight", v); }
export function listStyle(v: string) { return new CssProp("listStyle", v); }
export function listStyleImage(v: string) { return new CssProp("listStyleImage", v); }
export function listStylePosition(v: string) { return new CssProp("listStylePosition", v); }
export function listStyleType(v: string) { return new CssProp("listStyleType", v); }

export function margin(v: CSSGlobalValues | CSSLengthPercentageAuto): CssProp;
export function margin(vert: CSSLengthPercentageAuto, horiz: CSSLengthPercentageAuto): CssProp;
export function margin(top: CSSLengthPercentageAuto, horiz: CSSLengthPercentageAuto, bot: CSSLengthPercentageAuto): CssProp;
export function margin(top: CSSLengthPercentageAuto, right: CSSLengthPercentageAuto, bot: CSSLengthPercentageAuto, left: CSSLengthPercentageAuto): CssProp;
export function margin(...v: (number | string)[]) { return new CssProp("margin", v.join(" ")); }

export function marginBlockEnd(v: CSSLengthPercentageAuto) { return new CssProp("marginBlockEnd", v); }
export function marginBlockStart(v: CSSLengthPercentageAuto) { return new CssProp("marginBlockStart", v); }
export function marginBottom(v: CSSLengthPercentageAuto) { return new CssProp("marginBottom", v); }
export function marginInlineEnd(v: CSSLengthPercentageAuto) { return new CssProp("marginInlineEnd", v); }
export function marginInlineStart(v: CSSLengthPercentageAuto) { return new CssProp("marginInlineStart", v); }
export function marginLeft(v: CSSLengthPercentageAuto) { return new CssProp("marginLeft", v); }
export function marginRight(v: CSSLengthPercentageAuto) { return new CssProp("marginRight", v); }
export function marginTop(v: CSSLengthPercentageAuto) { return new CssProp("marginTop", v); }

export function marker(v: string) { return new CssProp("marker", v); }
export function markerEnd(v: string) { return new CssProp("markerEnd", v); }
export function markerMid(v: string) { return new CssProp("markerMid", v); }
export function markerStart(v: string) { return new CssProp("markerStart", v); }
export function mask(v: string) { return new CssProp("mask", v); }
export function maskType(v: string) { return new CssProp("maskType", v); }
export function maxBlockSize(v: string) { return new CssProp("maxBlockSize", v); }
export function maxHeight(v: string | 0) { return new CssProp("maxHeight", v); }
export function maxInlineSize(v: string) { return new CssProp("maxInlineSize", v); }
export function maxWidth(v: string | 0) { return new CssProp("maxWidth", v); }
export function maxZoom(v: string) { return new CssProp("maxZoom", v); }
export function minBlockSize(v: string) { return new CssProp("minBlockSize", v); }
export function minHeight(v: string | 0) { return new CssProp("minHeight", v); }
export function minInlineSize(v: string) { return new CssProp("minInlineSize", v); }
export function minWidth(v: string | 0) { return new CssProp("minWidth", v); }
export function minZoom(v: string) { return new CssProp("minZoom", v); }
export function mixBlendMode(v: string) { return new CssProp("mixBlendMode", v); }
export function objectFit(v: string) { return new CssProp("objectFit", v); }
export function objectPosition(v: string) { return new CssProp("objectPosition", v); }
export function offset(v: string) { return new CssProp("offset", v); }
export function offsetDistance(v: string) { return new CssProp("offsetDistance", v); }
export function offsetPath(v: string) { return new CssProp("offsetPath", v); }
export function offsetRotate(v: string) { return new CssProp("offsetRotate", v); }

export function opacity(v: CSSGlobalValues | CSSNumberPercentage) { return new CssProp("opacity", v); }

export function order(v: string) { return new CssProp("order", v); }
export function orientation(v: string) { return new CssProp("orientation", v); }
export function orphans(v: string) { return new CssProp("orphans", v); }
export function outline(v: string) { return new CssProp("outline", v); }
export function outlineColor(v: string) { return new CssProp("outlineColor", v); }
export function outlineOffset(v: string) { return new CssProp("outlineOffset", v); }
export function outlineStyle(v: string) { return new CssProp("outlineStyle", v); }
export function outlineWidth(v: string | 0) { return new CssProp("outlineWidth", v); }

export type CSSOverflowValues =
    | "visible"
    | "hidden"
    | "clip"
    | "scroll"
    | "auto";

export function overflow(v: CSSGlobalValues | CSSOverflowValues): CssProp;
export function overflow(x: CSSOverflowValues, y: CSSOverflowValues): CssProp;
export function overflow(...v: string[]): CssProp { return new CssProp("overflow", v.join(" ")); }

export function overflowX(v: CSSGlobalValues | CSSOverflowValues) { return new CssProp("overflowX", v); }
export function overflowY(v: CSSGlobalValues | CSSOverflowValues) { return new CssProp("overflowY", v); }

export function overflowAnchor(v: string) { return new CssProp("overflowAnchor", v); }
export function overflowWrap(v: string) { return new CssProp("overflowWrap", v); }
export function overscrollBehavior(v: string) { return new CssProp("overscrollBehavior", v); }
export function overscrollBehaviorBlock(v: string) { return new CssProp("overscrollBehaviorBlock", v); }
export function overscrollBehaviorInline(v: string) { return new CssProp("overscrollBehaviorInline", v); }
export function overscrollBehaviorX(v: string) { return new CssProp("overscrollBehaviorX", v); }
export function overscrollBehaviorY(v: string) { return new CssProp("overscrollBehaviorY", v); }

export function padding(v: CSSGlobalValues | CSSLengthPercentage): CssProp;
export function padding(vert: CSSLengthPercentage, horiz: CSSLengthPercentage): CssProp;
export function padding(top: CSSLengthPercentage, horiz: CSSLengthPercentage, bot: CSSLengthPercentage): CssProp;
export function padding(top: CSSLengthPercentage, right: CSSLengthPercentage, bot: CSSLengthPercentage, left: CSSLengthPercentage): CssProp;
export function padding(...v: (CSSGlobalValues | CSSLengthPercentage)[]) { return new CssProp("padding", v.join(" ")); }

export function paddingBlockEnd(v: CSSGlobalValues | CSSLengthPercentage) { return new CssProp("paddingBlockEnd", v); }
export function paddingBlockStart(v: CSSGlobalValues | CSSLengthPercentage) { return new CssProp("paddingBlockStart", v); }
export function paddingBottom(v: CSSGlobalValues | CSSLengthPercentage) { return new CssProp("paddingBottom", v); }
export function paddingInlineEnd(v: CSSGlobalValues | CSSLengthPercentage) { return new CssProp("paddingInlineEnd", v); }
export function paddingInlineStart(v: CSSGlobalValues | CSSLengthPercentage) { return new CssProp("paddingInlineStart", v); }
export function paddingLeft(v: CSSGlobalValues | CSSLengthPercentage) { return new CssProp("paddingLeft", v); }
export function paddingRight(v: CSSGlobalValues | CSSLengthPercentage) { return new CssProp("paddingRight", v); }
export function paddingTop(v: CSSGlobalValues | CSSLengthPercentage) { return new CssProp("paddingTop", v); }

export function pageBreakAfter(v: string) { return new CssProp("pageBreakAfter", v); }
export function pageBreakBefore(v: string) { return new CssProp("pageBreakBefore", v); }
export function pageBreakInside(v: string) { return new CssProp("pageBreakInside", v); }
export function paintOrder(v: string) { return new CssProp("paintOrder", v); }
export function perspective(v: string) { return new CssProp("perspective", v); }
export function perspectiveOrigin(v: string) { return new CssProp("perspectiveOrigin", v); }
export function placeContent(v: string) { return new CssProp("placeContent", v); }
export function placeItems(v: string) { return new CssProp("placeItems", v); }
export function placeSelf(v: string) { return new CssProp("placeSelf", v); }

export type CSSPointerEventsValue =
    | "auto"
    | "none";
export function pointerEvents(v: CSSGlobalValues | CSSPointerEventsValue) { return new CssProp("pointerEvents", v); }

export type CSSPositionValues =
    | "static"
    | "absolute"
    | "fixed"
    | "relative"
    | "sticky";
export function position(v: CSSImportant<CSSGlobalValues | CSSPositionValues>) { return new CssProp("position", v); }

export function quotes(v: string) { return new CssProp("quotes", v); }
export function r(v: string) { return new CssProp("r", v); }
export function resize(v: string) { return new CssProp("resize", v); }
export function right(v: CSSElementPositionValue) { return new CssProp("right", v); }
export function rowGap(v: string | 0) { return new CssProp("rowGap", v); }
export function rubyPosition(v: string) { return new CssProp("rubyPosition", v); }
export function rx(v: string) { return new CssProp("rx", v); }
export function ry(v: string) { return new CssProp("ry", v); }
export function scrollBehavior(v: string) { return new CssProp("scrollBehavior", v); }
export function scrollMargin(v: string | 0) { return new CssProp("scrollMargin", v); }
export function scrollMarginBlock(v: string) { return new CssProp("scrollMarginBlock", v); }
export function scrollMarginBlockEnd(v: string) { return new CssProp("scrollMarginBlockEnd", v); }
export function scrollMarginBlockStart(v: string) { return new CssProp("scrollMarginBlockStart", v); }
export function scrollMarginBottom(v: string | 0) { return new CssProp("scrollMarginBottom", v); }
export function scrollMarginInline(v: string) { return new CssProp("scrollMarginInline", v); }
export function scrollMarginInlineEnd(v: string) { return new CssProp("scrollMarginInlineEnd", v); }
export function scrollMarginInlineStart(v: string) { return new CssProp("scrollMarginInlineStart", v); }
export function scrollMarginLeft(v: string | 0) { return new CssProp("scrollMarginLeft", v); }
export function scrollMarginRight(v: string | 0) { return new CssProp("scrollMarginRight", v); }
export function scrollMarginTop(v: string | 0) { return new CssProp("scrollMarginTop", v); }
export function scrollPadding(v: string | 0) { return new CssProp("scrollPadding", v); }
export function scrollPaddingBlock(v: string) { return new CssProp("scrollPaddingBlock", v); }
export function scrollPaddingBlockEnd(v: string) { return new CssProp("scrollPaddingBlockEnd", v); }
export function scrollPaddingBlockStart(v: string) { return new CssProp("scrollPaddingBlockStart", v); }
export function scrollPaddingBottom(v: string | 0) { return new CssProp("scrollPaddingBottom", v); }
export function scrollPaddingInline(v: string) { return new CssProp("scrollPaddingInline", v); }
export function scrollPaddingInlineEnd(v: string) { return new CssProp("scrollPaddingInlineEnd", v); }
export function scrollPaddingInlineStart(v: string) { return new CssProp("scrollPaddingInlineStart", v); }
export function scrollPaddingLeft(v: string | 0) { return new CssProp("scrollPaddingLeft", v); }
export function scrollPaddingRight(v: string | 0) { return new CssProp("scrollPaddingRight", v); }
export function scrollPaddingTop(v: string | 0) { return new CssProp("scrollPaddingTop", v); }
export function scrollSnapAlign(v: string) { return new CssProp("scrollSnapAlign", v); }
export function scrollSnapStop(v: string) { return new CssProp("scrollSnapStop", v); }
export function scrollSnapType(v: string) { return new CssProp("scrollSnapType", v); }
export function shapeImageThreshold(v: string) { return new CssProp("shapeImageThreshold", v); }
export function shapeMargin(v: string) { return new CssProp("shapeMargin", v); }
export function shapeOutside(v: string) { return new CssProp("shapeOutside", v); }
export function shapeRendering(v: string) { return new CssProp("shapeRendering", v); }
export function speak(v: string) { return new CssProp("speak", v); }
export function stopColor(v: string) { return new CssProp("stopColor", v); }
export function stopOpacity(v: string) { return new CssProp("stopOpacity", v); }
export function stroke(v: string) { return new CssProp("stroke", v); }
export function strokeDasharray(v: string) { return new CssProp("strokeDasharray", v); }
export function strokeDashoffset(v: string) { return new CssProp("strokeDashoffset", v); }
export function strokeLinecap(v: string) { return new CssProp("strokeLinecap", v); }
export function strokeLinejoin(v: string) { return new CssProp("strokeLinejoin", v); }
export function strokeMiterlimit(v: string) { return new CssProp("strokeMiterlimit", v); }
export function strokeOpacity(v: string) { return new CssProp("strokeOpacity", v); }
export function strokeWidth(v: string | 0) { return new CssProp("strokeWidth", v); }
export function tabSize(v: string) { return new CssProp("tabSize", v); }
export function tableLayout(v: string) { return new CssProp("tableLayout", v); }


export type CSSTextAlignLastValues =
    | "auto"
    | "start"
    | "end"
    | "left"
    | "right"
    | "center"
    | "justify"
    | "match-parent";

export type CSSTextAlignValues =
    | CSSTextAlignLastValues
    | "justify-all";

export function textAlign(v: CSSGlobalValues | CSSTextAlignValues) { return new CssProp("textAlign", v); }
export function textAlignLast(v: CSSGlobalValues | CSSTextAlignLastValues) { return new CssProp("textAlignLast", v); }

export function textAnchor(v: string) { return new CssProp("textAnchor", v); }
export function textCombineUpright(v: string) { return new CssProp("textCombineUpright", v); }
export function textDecoration(v: string) { return new CssProp("textDecoration", v); }
export function textDecorationColor(v: string) { return new CssProp("textDecorationColor", v); }
export function textDecorationLine(v: string) { return new CssProp("textDecorationLine", v); }
export function textDecorationSkipInk(v: string) { return new CssProp("textDecorationSkipInk", v); }
export function textDecorationStyle(v: string) { return new CssProp("textDecorationStyle", v); }
export function textIndent(v: string) { return new CssProp("textIndent", v); }
export function textOrientation(v: string) { return new CssProp("textOrientation", v); }

export type CSSTextOverflowValues =
    | "clip"
    | "ellipsis";
export function textOverflow(v: CSSGlobalValues | CSSTextOverflowValues) { return new CssProp("textOverflow", v); }

export function textRendering(v: string) { return new CssProp("textRendering", v); }
export function textShadow(v: string) { return new CssProp("textShadow", v); }
export function textSizeAdjust(v: string) { return new CssProp("textSizeAdjust", v); }
export function textTransform(v: string) { return new CssProp("textTransform", v); }
export function textUnderlinePosition(v: string) { return new CssProp("textUnderlinePosition", v); }
export function top(v: CSSElementPositionValue) { return new CssProp("top", v); }

export type CSSTouchActionValues =
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
export function touchAction(v: CSSImportant<CSSGlobalValues | CSSTouchActionValues>) { return new CssProp("touchAction", v); }

export function transform(v: string) { return new CssProp("transform", v); }
export function transformBox(v: string) { return new CssProp("transformBox", v); }
export function transformOrigin(v: string) { return new CssProp("transformOrigin", v); }
export function transformStyle(v: string) { return new CssProp("transformStyle", v); }
export function transition(v: string) { return new CssProp("transition", v); }
export function transitionDelay(v: string | 0) { return new CssProp("transitionDelay", v); }
export function transitionDuration(v: string | 0) { return new CssProp("transitionDuration", v); }
export function transitionProperty(v: string) { return new CssProp("transitionProperty", v); }
export function transitionTimingFunction(v: string) { return new CssProp("transitionTimingFunction", v); }
export function unicodeBidi(v: string) { return new CssProp("unicodeBidi", v); }
export function unicodeRange(v: string) { return new CssProp("unicodeRange", v); }
export function userSelect(v: string) { return new CssProp("userSelect", v); }
export function userZoom(v: string) { return new CssProp("userZoom", v); }
export function vectorEffect(v: string) { return new CssProp("vectorEffect", v); }

export type CSSVerticalAlignValues =
    | CSSLengthPercentage
    | "baseline"
    | "sub"
    | "super"
    | "text-top"
    | "text-bottom"
    | "middle"
    | "top"
    | "bottom";
export function verticalAlign(v: CSSGlobalValues | CSSVerticalAlignValues) { return new CssProp("verticalAlign", v); }

export type CSSVisiblityValues =
    | "visible"
    | "hidden"
    | "collapse";
export function visibility(v: CSSGlobalValues | CSSVisiblityValues) { return new CssProp("visibility", v); }

export type CSSWhiteSpaceValues =
    | "normal"
    | "nowrap"
    | "pre"
    | "pre-wrap"
    | "pre-line"
    | "break-spaces";
export function whiteSpace(v: CSSGlobalValues | CSSWhiteSpaceValues) { return new CssProp("whiteSpace", v); }

export function widows(v: CSSGlobalValues | number) { return new CssProp("widows", asInt(v)); }
export function width(v: CSSImportant<CSSGlobalValues | CSSSizePropertyValue>) { return new CssProp("width", v); }
export function willChange(v: string) { return new CssProp("willChange", v); }
export function wordBreak(v: string) { return new CssProp("wordBreak", v); }
export function wordSpacing(v: string) { return new CssProp("wordSpacing", v); }
export function wordWrap(v: string) { return new CssProp("wordWrap", v); }

export type CSSWritingModeValues =
    | "horizontal-tb"
    | "vertical-rl"
    | "vertical-lr"
    | "sideways-rl"
    | "sideways-lr";
export function writingMode(v: CSSGlobalValues | CSSWritingModeValues) { return new CssProp("writingMode", v); }

export function x(v: CSSImportant<CSSGlobalValues | CSSSizePropertyValue>) { return new CssProp("x", v); }
export function y(v: CSSImportant<CSSGlobalValues | CSSSizePropertyValue>) { return new CssProp("y", v); }
export function zIndex(v: CSSImportant<number>) { return new CssProp("zIndex", asInt(v)); }
export function zoom(v: CSSImportant<number>) { return new CssProp("zoom", asInt(v)); }


/**
 * A selection of fonts for preferred monospace rendering.
 **/
export function getMonospaceFonts() {
    return "ui-monospace, 'Droid Sans Mono', 'Cascadia Mono', 'Segoe UI Mono', 'Ubuntu Mono', 'Roboto Mono', Menlo, Monaco, Consolas, monospace";
}

/**
 * A selection of fonts for preferred monospace rendering.
 **/
export function getMonospaceFamily() {
    return fontFamily(getMonospaceFonts());
}

/**
 * A selection of fonts that should match whatever the user's operating system normally uses.
 **/
export function getSystemFonts() {
    return "system-ui, -apple-system, '.SFNSText-Regular', 'San Francisco', 'Segoe UI', 'Ubuntu', 'Roboto', 'Noto Sans' 'Droid Sans', sans-serif";
}

/**
 * A selection of fonts that should match whatever the user's operating system normally uses.
 **/
export function getSystemFamily() {
    return fontFamily(getSystemFonts());
}

/**
 * A selection of serif fonts.
 **/
export function getSerifFonts() {
    return "Georgia, Cambria, 'Times New Roman', Times, serif";
}

export function getSerifFamily() {
    return fontFamily(getSerifFonts());
}


export class CSSInJSRule {
    constructor(private selector: string, private props: CssProp[]) {
    }

    apply(sheet: CSSStyleSheet) {
        const style = this.props
            .map((prop) => `${prop.name}: ${prop.value};`)
            .join("");
        sheet.insertRule(
            `${this.selector} {${style}}`,
            sheet.cssRules.length);
    }
}

export function rule(selector: string, ...props: CssProp[]): CSSInJSRule {
    return new CSSInJSRule(selector, props);
}