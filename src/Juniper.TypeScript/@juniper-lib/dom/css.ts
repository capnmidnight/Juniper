import { isDefined, isNumber, isString } from "@juniper-lib/tslib";
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

/**
 * Combine style properties.
 **/
export function styles(...rest: (CssProp | CssPropSet)[]) {
    return new CssPropSet(...rest);
}

type CSSImportant<T extends string | number> = T | `${T} !important`;

type CSSGlobalValues = "inherit"
    | "initial"
    | "revert"
    | "revert-layer"
    | "unset";

type CSSUrl = `url(${string})`;

type CSSPercentage = `${number}%`;
type CSSNumberPercentage = number | CSSPercentage;

type CSSAngle = number
    | `${number}deg`
    | `${number}rad`
    | `${number}grad`
    | `${number}turn`;

type CSSFontRelativeLength = `${number}cap`
    | `${number}ch`
    | `${number}em`
    | `${number}ex`
    | `${number}ic`
    | `${number}lh`
    | `${number}rem`
    | `${number}rlh`;

type CSSViewportPercentageLength = `${number}vh`
    | `${number}vw`
    | `${number}vi`
    | `${number}vb`
    | `${number}vmin`
    | `${number}vmax`;

type CSSAbsoluteLength = `${number}px`
    | `${number}cm`
    | `${number}mm`
    | `${number}Q`
    | `${number}in`
    | `${number}pc`
    | `${number}pt`;

type CSSLength = CSSFontRelativeLength
    | CSSViewportPercentageLength
    | CSSAbsoluteLength;

type CSSLengthPercentage = CSSLength
    | CSSPercentage
    | 0
    | "0";

type CSSCalcStatement = `calc(${string})`;

type CSSSizePropertyValue = CSSGlobalValues
    | 0
    | "0"
    | CSSLengthPercentage
    | "auto"
    | "max-content"
    | "min-content"
    | `fit-content(${CSSLengthPercentage})`
    | CSSCalcStatement;

type CSSAlignItemsValue = CSSGlobalValues
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
export function alignItems(v: CSSAlignItemsValue) { return new CssProp("alignItems", v); }

type CSSAlignContentValue = CSSAlignItemsValue
    | "space-between"
    | "space-around"
    | "space-evenly";
export function alignContent(v: CSSAlignContentValue) { return new CssProp("alignContent", v); }

type CSSAlignSelfValue = CSSAlignItemsValue
    | "auto"
    | "self-start"
    | "self-end";
export function alignSelf(v: CSSAlignSelfValue) { return new CssProp("alignSelf", v); }

export function all(v: CSSGlobalValues) { return new CssProp("all", v); }

export function alignmentBaseline(v: string) { return new CssProp("alignmentBaseline", v); }

export function animation(v: string) { return new CssProp("animation", v); }

type CSSTimeSecondsValue = `${number}s`;
type CSSTimeMillisecondsValue = `${number}ms`;
type CSSTimeValue = CSSTimeSecondsValue | CSSTimeMillisecondsValue;
export function animationDelay(v: CSSGlobalValues): CssProp;
export function animationDelay(...v: CSSTimeValue[]): CssProp;
export function animationDelay(...v: string[]) { return new CssProp("animationDelay", v.join(", ")); }

export function animationDuration(v: CSSGlobalValues): CssProp;
export function animationDuration(...v: CSSTimeValue[]): CssProp;
export function animationDuration(...v: string[]) { return new CssProp("animationDuration", v.join(", ")); }

type CSSAnimationDirectionValue = "normal"
    | "reverse"
    | "alternate"
    | "alternate-reverse";
export function animationDirection(v: CSSGlobalValues): CssProp;
export function animationDirection(...v: CSSAnimationDirectionValue[]): CssProp;
export function animationDirection(...v: string[]): CssProp { return new CssProp("animationDirection", v.join(", ")); }

type CSSFillModeValue = "none"
    | "forwards"
    | "backwards"
    | "both";
export function animationFillMode(v: CSSGlobalValues): CssProp;
export function animationFillMode(...v: CSSFillModeValue[]): CssProp;
export function animationFillMode(...v: string[]): CssProp { return new CssProp("animationFillMode", v.join(", ")); }

type CSSIterationCountValue = number | "infinite"
export function animationIterationCount(v: CSSGlobalValues): CssProp;
export function animationIterationCount(...v: CSSIterationCountValue[]): CssProp;
export function animationIterationCount(...v: (number | string)[]): CssProp { return new CssProp("animationIterationCount", v.join(", ")); }

export function animationName(v: CSSGlobalValues): CssProp;
export function animationName(...v: string[]): CssProp;
export function animationName(...v: string[]) { return new CssProp("animationName", v.join(", ")); }

type CSSPlayStateValue = "running" | "paused";
export function animationPlayState(v: CSSGlobalValues): CssProp;
export function animationPlayState(...v: CSSPlayStateValue[]): CssProp;
export function animationPlayState(...v: string[]) { return new CssProp("animationPlayState", v.join(", ")); }

type CSSTimingFunctionKeyword = "ease"
    | "ease-in"
    | "ease-out"
    | "ease-in-out"
    | "linear"
    | "step-start"
    | "step-end";
type CSSTimingFunctionCubicBezier = `cubic-bezier(${number}, ${number}, ${number}, ${number})`;
type CSSTimingFunctionSteps = `steps(${number}, ${CSSTimingFunctionKeyword})`;
type CSSTimingFunctionValue = CSSTimingFunctionKeyword
    | CSSTimingFunctionCubicBezier
    | CSSTimingFunctionSteps;
export function animationTimingFunction(v: CSSGlobalValues): CssProp;
export function animationTimingFunction(...v: CSSTimingFunctionValue[]): CssProp;
export function animationTimingFunction(v: string) { return new CssProp("animationTimingFunction", v); }

type CSSAppearanceValue = CSSGlobalValues
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

export function appearance(v: CSSAppearanceValue) { return new CssProp("appearance", v); }

type CSSFilterFunction = `blur(${CSSLength})`
    | `brightness(${CSSNumberPercentage})`
    | `contrast(${CSSNumberPercentage})`
    | `drop-shadow(${CSSLength} ${CSSLength})`
    | `drop-shadow(${CSSLength} ${CSSLength} ${string})`
    | `drop-shadow(${CSSLength} ${CSSLength} ${CSSLength} ${string})`
    | `grayscale(${CSSNumberPercentage})`
    | `hue-rotate(${CSSAngle})`
    | `invert(${CSSNumberPercentage})`
    | `opacity(${CSSNumberPercentage})`
    | `saturate(${CSSNumberPercentage})`
    | `sepia(${CSSNumberPercentage})`
    | CSSUrl;
export function backdropFilter(v: "none" | CSSGlobalValues): CssProp;
export function backdropFilter(...v: CSSFilterFunction[]): CssProp;
export function backdropFilter(...v: string[]): CssProp { return new CssProp("backdropFilter", v.join(' ')); }

type CSSBackfaceVisibilityValue = CSSGlobalValues
    | "visible"
    | "hiden";
export function backfaceVisibility(v: CSSBackfaceVisibilityValue) { return new CssProp("backfaceVisibility", v); }

export function background(v: string) { return new CssProp("background", v); }

type CSSBackgroundAttachmentValue = CSSGlobalValues
    | "scroll"
    | "fixed"
    | "local";
export function backgroundAttachment(v: CSSBackgroundAttachmentValue) { return new CssProp("backgroundAttachment", v); }

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
export function backgroundBlendMode(v: CSSGlobalValues): CssProp;
export function backgroundBlendMode(...v: CSSBlendModeValue[]): CssProp;
export function backgroundBlendMode(...v: string[]): CssProp { return new CssProp("backgroundBlendMode", v.join(", ")); }

type CSSBoxType = CSSGlobalValues
    | "border-box"
    | "padding-box"
    | "content-box";

type CSSClipValue = CSSBoxType
    | "text";
export function backgroundClip(v: CSSClipValue): CssProp { return new CssProp("backgroundClip", v); }

export function backgroundColor(v: string) { return new CssProp("backgroundColor", v); }

type CSSGradient = `linear-gradient(${string})`
    | `radial-gradient(${string})`
    | `repeating-linear-gradient(${string})`
    | `repeating-radial-gradient(${string})`
    | `conic-gradient(${string})`;

type CSSImage = CSSUrl
    | CSSGradient
    | `element(${string})`
    | `cross-fade(${string})`
    | `image-set(${string})`;
export function backgroundImage(v: CSSGlobalValues): CssProp;
export function backgroundImage(...v: CSSImage[]): CssProp;
export function backgroundImage(...v: string[]): CssProp { return new CssProp("backgroundImage", v.join(", ")); }

export function backgroundOrigin(v: CSSBoxType): CssProp { return new CssProp("backgroundOrigin", v); }

type CSSPositionKeyword = "top"
    | "bottom"
    | "left"
    | "right"
    | "center";
type CSSPosition = CSSPositionKeyword
    | CSSLengthPercentage;

type CSSPositionValue = CSSPosition
    | `${CSSPosition} ${CSSPosition}`
    | `${CSSPositionKeyword} ${CSSPositionKeyword} ${CSSLengthPercentage}`
    | `${CSSPositionKeyword} ${CSSLengthPercentage} ${CSSPositionKeyword} ${CSSLengthPercentage}`;
export function backgroundPosition(v: CSSGlobalValues): CssProp;
export function backgroundPosition(...v: CSSPositionValue[]): CssProp;
export function backgroundPosition(...v: (string | number)[]): CssProp { return new CssProp("backgroundPosition", v.join(", ")); }

type CSSPositionXYValue = CSSPosition
    | `${CSSPositionKeyword} ${CSSLengthPercentage}`;
export function backgroundPositionX(v: CSSGlobalValues): CssProp;
export function backgroundPositionX(...v: CSSPositionXYValue[]): CssProp;
export function backgroundPositionX(...v: (string | number)[]): CssProp { return new CssProp("backgroundPositionX", v.join(", ")); }

export function backgroundPositionY(v: CSSGlobalValues): CssProp;
export function backgroundPositionY(...v: CSSPositionXYValue[]): CssProp;
export function backgroundPositionY(...v: (string | number)[]): CssProp { return new CssProp("backgroundPositionY", v.join(", ")); }

type CSSBasicRepeat = "repeat"
    | "space"
    | "round"
    | "no-repeat";

type CSSRepeatValue = CSSGlobalValues
    | "repeat-x"
    | "repeat-y"
    | CSSBasicRepeat
    | `${CSSBasicRepeat} ${CSSBasicRepeat}`;
export function backgroundRepeat(v: CSSRepeatValue) { return new CssProp("backgroundRepeat", v); }

export function backgroundRepeatX(v: CSSGlobalValues | CSSBasicRepeat) { return new CssProp("backgroundRepeatX", v); }
export function backgroundRepeatY(v: CSSGlobalValues | CSSBasicRepeat) { return new CssProp("backgroundRepeatY", v); }

type CSSBackgroundSizeValue = "contain"
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

type CSSElementPositionValue = CSSGlobalValues
    | "auto"
    | CSSLengthPercentage;
export function bottom(v: CSSElementPositionValue) { return new CssProp("bottom", v); }
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
export function color(v: string) { return new CssProp("color", v); }
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

export type CSSCursorValue = CSSGlobalValues
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
export function cursor(v: CSSCursorValue) { return new CssProp("cursor", v); }

export function cx(v: string) { return new CssProp("cx", v); }
export function cy(v: string) { return new CssProp("cy", v); }
export function d(v: string) { return new CssProp("d", v); }
export function direction(v: string) { return new CssProp("direction", v); }

export type CSSDisplayValues = CSSGlobalValues
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
export function display(v: CSSImportant<CSSDisplayValues>) { return new CssProp("display", v); }

export function dominantBaseline(v: string) { return new CssProp("dominantBaseline", v); }
export function emptyCells(v: string) { return new CssProp("emptyCells", v); }
export function fill(v: string) { return new CssProp("fill", v); }
export function fillOpacity(v: string) { return new CssProp("fillOpacity", v); }
export function fillRule(v: string) { return new CssProp("fillRule", v); }
export function filter(v: string) { return new CssProp("filter", v); }

export type CSSFlexBasisValues = CSSGlobalValues
    | CSSLengthPercentage
    | "auto"
    | "max-content"
    | "min-content"
    | "fit-content"
export function flexBasis(v: CSSFlexBasisValues) { return new CssProp("flexBasis", v); }

export type CSSFlexDirectionValues = CSSGlobalValues
    | "row"
    | "row-reverse"
    | "column"
    | "column-reverse";
export function flexDirection(v: CSSFlexDirectionValues) { return new CssProp("flexDirection", v); }

export type CSSFlexWrapValues = CSSGlobalValues
    | "nowrap"
    | "wrap"
    | "wrap-reverse";
export function flexWrap(v: CSSFlexWrapValues) { return new CssProp("flexWrap", v); }

export type CSSFlexFlowValues = CSSFlexDirectionValues
    | CSSFlexWrapValues
    | `${CSSFlexDirectionValues} ${CSSFlexWrapValues}`;
export function flexFlow(v: CSSFlexFlowValues) { return new CssProp("flexFlow", v); }

export type CSSFlexValues = number
    | CSSFlexBasisValues
    | `${number} ${CSSFlexBasisValues}`
    | `${number} ${number}`
    | `${number} ${number} ${CSSFlexBasisValues}`;
export function flex(v: CSSFlexValues) { return new CssProp("flex", v); }

export function flexGrow(v: CSSGlobalValues | number) { return new CssProp("flexGrow", v); }

export function flexShrink(v: CSSGlobalValues | number) { return new CssProp("flexShrink", v); }

export type CSSFloatValues = CSSGlobalValues
    | "left"
    | "right"
    | "none"
    | "inline-start"
    | "inline-end";
export function float(v: CSSFloatValues) { return new CssProp("float", v); }
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

type CSSGridAutoFlowValues = CSSGlobalValues
    | "row"
    | "column"
    | "dense"
    | "row dense"
    | "column dense";
export function gridAutoFlow(v: CSSGridAutoFlowValues) { return new CssProp("gridAutoFlow", v); }

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
export function height(v: CSSImportant<CSSSizePropertyValue>) { return new CssProp("height", v); }
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
export function margin(v: string | 0) { return new CssProp("margin", v); }
export function marginBlockEnd(v: string) { return new CssProp("marginBlockEnd", v); }
export function marginBlockStart(v: string) { return new CssProp("marginBlockStart", v); }
export function marginBottom(v: string | 0) { return new CssProp("marginBottom", v); }
export function marginInlineEnd(v: string) { return new CssProp("marginInlineEnd", v); }
export function marginInlineStart(v: string) { return new CssProp("marginInlineStart", v); }
export function marginLeft(v: string | 0) { return new CssProp("marginLeft", v); }
export function marginRight(v: string | 0) { return new CssProp("marginRight", v); }
export function marginTop(v: string | 0) { return new CssProp("marginTop", v); }
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

export type CSSOverflowValues = CSSGlobalValues
    | "visible"
    | "hidden"
    | "clip"
    | "scroll"
    | "auto";

export function overflow(x: CSSOverflowValues, y: CSSOverflowValues): CssProp;
export function overflow(v: CSSOverflowValues | `${CSSOverflowValues} ${CSSOverflowValues}`): CssProp;
export function overflow(vOrX: CSSOverflowValues | `${CSSOverflowValues} ${CSSOverflowValues}`, y?: CSSOverflowValues): CssProp {
    if (isDefined(y)) {
        return new CssProp("overflow", vOrX + " " + y);
    }
    else {
        return new CssProp("overflow", vOrX);
    }
}
export function overflowAnchor(v: string) { return new CssProp("overflowAnchor", v); }
export function overflowWrap(v: string) { return new CssProp("overflowWrap", v); }
export function overflowX(v: CSSOverflowValues) { return new CssProp("overflowX", v); }
export function overflowY(v: CSSOverflowValues) { return new CssProp("overflowY", v); }
export function overscrollBehavior(v: string) { return new CssProp("overscrollBehavior", v); }
export function overscrollBehaviorBlock(v: string) { return new CssProp("overscrollBehaviorBlock", v); }
export function overscrollBehaviorInline(v: string) { return new CssProp("overscrollBehaviorInline", v); }
export function overscrollBehaviorX(v: string) { return new CssProp("overscrollBehaviorX", v); }
export function overscrollBehaviorY(v: string) { return new CssProp("overscrollBehaviorY", v); }
export function padding(...v: CSSLengthPercentage[]) { return new CssProp("padding", v.join(" ")); }
export function paddingBlockEnd(v: string) { return new CssProp("paddingBlockEnd", v); }
export function paddingBlockStart(v: string) { return new CssProp("paddingBlockStart", v); }
export function paddingBottom(v: CSSLengthPercentage) { return new CssProp("paddingBottom", v); }
export function paddingInlineEnd(v: string) { return new CssProp("paddingInlineEnd", v); }
export function paddingInlineStart(v: string) { return new CssProp("paddingInlineStart", v); }
export function paddingLeft(v: CSSLengthPercentage) { return new CssProp("paddingLeft", v); }
export function paddingRight(v: CSSLengthPercentage) { return new CssProp("paddingRight", v); }
export function paddingTop(v: CSSLengthPercentage) { return new CssProp("paddingTop", v); }
export function pageBreakAfter(v: string) { return new CssProp("pageBreakAfter", v); }
export function pageBreakBefore(v: string) { return new CssProp("pageBreakBefore", v); }
export function pageBreakInside(v: string) { return new CssProp("pageBreakInside", v); }
export function paintOrder(v: string) { return new CssProp("paintOrder", v); }
export function perspective(v: string) { return new CssProp("perspective", v); }
export function perspectiveOrigin(v: string) { return new CssProp("perspectiveOrigin", v); }
export function placeContent(v: string) { return new CssProp("placeContent", v); }
export function placeItems(v: string) { return new CssProp("placeItems", v); }
export function placeSelf(v: string) { return new CssProp("placeSelf", v); }

type CSSPointerEventsValue = CSSGlobalValues
    | "auto"
    | "none";
export function pointerEvents(v: CSSPointerEventsValue) { return new CssProp("pointerEvents", v); }

type CSSPositionValues = CSSGlobalValues
    | "static"
    | "absolute"
    | "fixed"
    | "relative"
    | "sticky";
export function position(v: CSSImportant<CSSPositionValues>) { return new CssProp("position", v); }

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
export function textAlign(v: string) { return new CssProp("textAlign", v); }
export function textAlignLast(v: string) { return new CssProp("textAlignLast", v); }
export function textAnchor(v: string) { return new CssProp("textAnchor", v); }
export function textCombineUpright(v: string) { return new CssProp("textCombineUpright", v); }
export function textDecoration(v: string) { return new CssProp("textDecoration", v); }
export function textDecorationColor(v: string) { return new CssProp("textDecorationColor", v); }
export function textDecorationLine(v: string) { return new CssProp("textDecorationLine", v); }
export function textDecorationSkipInk(v: string) { return new CssProp("textDecorationSkipInk", v); }
export function textDecorationStyle(v: string) { return new CssProp("textDecorationStyle", v); }
export function textIndent(v: string) { return new CssProp("textIndent", v); }
export function textOrientation(v: string) { return new CssProp("textOrientation", v); }
export function textOverflow(v: string) { return new CssProp("textOverflow", v); }
export function textRendering(v: string) { return new CssProp("textRendering", v); }
export function textShadow(v: string) { return new CssProp("textShadow", v); }
export function textSizeAdjust(v: string) { return new CssProp("textSizeAdjust", v); }
export function textTransform(v: string) { return new CssProp("textTransform", v); }
export function textUnderlinePosition(v: string) { return new CssProp("textUnderlinePosition", v); }
export function top(v: CSSElementPositionValue) { return new CssProp("top", v); }

type CSSTouchActionValues = CSSGlobalValues
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
export function touchAction(v: CSSImportant<CSSTouchActionValues>) { return new CssProp("touchAction", v); }

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
export function verticalAlign(v: string) { return new CssProp("verticalAlign", v); }
export function visibility(v: string) { return new CssProp("visibility", v); }
export function whiteSpace(v: string) { return new CssProp("whiteSpace", v); }
export function widows(v: string) { return new CssProp("widows", v); }
export function width(v: CSSImportant<CSSSizePropertyValue>) { return new CssProp("width", v); }
export function willChange(v: string) { return new CssProp("willChange", v); }
export function wordBreak(v: string) { return new CssProp("wordBreak", v); }
export function wordSpacing(v: string) { return new CssProp("wordSpacing", v); }
export function wordWrap(v: string) { return new CssProp("wordWrap", v); }
export function writingMode(v: string) { return new CssProp("writingMode", v); }
export function x(v: CSSImportant<CSSSizePropertyValue>) { return new CssProp("x", v); }
export function y(v: CSSImportant<CSSSizePropertyValue>) { return new CssProp("y", v); }
export function zIndex(v: CSSImportant<number>) { return new CssProp("zIndex", isNumber(v) ? v.toFixed(0) : v); }
export function zoom(v: CSSImportant<number>) { return new CssProp("zoom", isNumber(v) ? v.toFixed(0) : v); }


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